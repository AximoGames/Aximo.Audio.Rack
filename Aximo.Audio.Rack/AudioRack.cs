// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aximo.Audio.Rack.JsonModel;
//using System.Media;

namespace Aximo.Engine.Audio
{

    public class AudioRack
    {
        public AudioModule[] Modules = Array.Empty<AudioModule>();
        public AudioCable[] Cables = Array.Empty<AudioCable>();

        public IEnumerable<TModule> GetModules<TModule>()
            where TModule : AudioModule
        {
            // SLOW: To not call this method from processing

            var modules = Modules;
            var len = modules.Length;
            for (var i = 0; i < len; i++)
            {
                var mod = modules[i] as TModule;
                if (mod != null)
                    yield return mod;
            }
        }

        public void AddModule(AudioModule module)
        {
            Modules = Modules.AppendElement(module);
            module.Rack = this;
        }

        public void AddCable(AudioCable cable)
        {
            TryAddCable(cable);
        }

        public void AddCable(Port port1, Port port2)
        {
            TryAddCable(port1, port2);
        }

        public bool TryAddCable(AudioCable cable)
        {
            try
            {
                cable.ModuleOutput.AddCable(cable);
                cable.ModuleInput.AddCable(cable);
                Cables = Cables.AppendElement(cable);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool TryAddCable(Port port1, Port port2)
        {
            return TryAddCable(new AudioCable(port1, port2));
        }

        public void RemoveCable(AudioCable cable)
        {
            if (cable == null)
                return;

            Cables = Cables.RemoveElement(cable);
            cable.ModuleOutput.RemoveCable(cable);
            cable.ModuleInput.RemoveCable(cable);
        }

        public virtual void Process(AudioProcessArgs e)
        {
            ProcessSmoothing(e);

            var cables = Cables;
            var len = cables.Length;
            for (var i = 0; i < len; i++)
                cables[i].Process();

            var modules = Modules;
            len = modules.Length;
            for (var i = 0; i < len; i++)
                modules[i].Process(e);
        }

        public AudioModule GetModule(string name)
        {
            return Modules.FirstOrDefault(m => m.Name == name);
        }

        public void LoadFromFile(string filePath)
        {
            LoadFromJson(JsFile.LoadFile(filePath));
        }

        public void LoadFromJson(JsFile file)
        {
            // temporary list to support appending to existing rack
            var modules = new List<AudioModule>();
            foreach (var jsMod in file.Modules)
            {
                var mod = AudioModuleManager.Current.CreateInstance(jsMod.Name);
                if (mod == null)
                    continue;

                modules.Add(mod);
                foreach (var jsParam in jsMod.Parameters)
                {
                    var param = mod.GetParameter(jsParam.Id);
                    if (param == null)
                        continue;

                    param.SetDisplayValue(jsParam.Value);
                }
                AddModule(mod);
            }

            foreach (var jsCable in file.Cables)
            {
                var inputMod = modules[jsCable.InputModuleId];
                var outputMod = modules[jsCable.OutputModuleId];

                var inputPort = inputMod.GetInput(jsCable.InputPortId);
                if (inputPort == null)
                    continue;

                var outputPort = outputMod.GetOutput(jsCable.OutputPortId);
                if (outputPort == null)
                    continue;

                AddCable(inputPort, outputPort);
            }
        }

        public void SaveToFile(string filePath)
        {
            ToJsonModel().SaveToFile(filePath);
        }

        public JsFile ToJsonModel()
        {
            var file = new JsFile();
            var modId = 0;
            foreach (var mod in Modules)
            {
                var jsMod = new JsModule { Id = modId++, Name = mod.Name, Position = mod.Position };
                var paramId = 0;
                foreach (var param in mod.Parameters)
                    jsMod.Parameters.Add(new JsParameter { Id = paramId++, Value = param.GetDisplayValue() });

                file.Modules.Add(jsMod);
            }

            foreach (var cable in Cables)
            {
                var jsCable = new JsCable();

                jsCable.OutputModuleId = Array.IndexOf(Modules, cable.ModuleOutput.Module);
                jsCable.InputModuleId = Array.IndexOf(Modules, cable.ModuleInput.Module);

                jsCable.OutputPortId = cable.ModuleOutput.Index;
                jsCable.InputPortId = cable.ModuleInput.Index;

                file.Cables.Add(jsCable);
            }

            return file;
        }

        private object SmoothingLock = new object();
        private List<AudioParameter> SmoothingParameters = new List<AudioParameter>();
        internal void RegisterSmoothing(AudioParameter param)
        {
            lock (SmoothingLock)
            {
                if (param.InSmoothing)
                    return;

                param.InSmoothing = true;
                SmoothingParameters.Add(param);
            }
        }

        internal void UnregisterSmoothing(AudioParameter param)
        {
            lock (SmoothingLock)
            {
                if (!param.InSmoothing)
                    return;

                param.InSmoothing = false;
                SmoothingParameters.Remove(param);
            }
        }

        private void ProcessSmoothing(AudioProcessArgs e)
        {
            var parameters = SmoothingParameters;
            lock (SmoothingLock)
            {
                var len = parameters.Count;
                for (var i = len - 1; i >= 0; i--)
                    parameters[i].ProcessSmoothing(e);
            }
        }

    }
}
