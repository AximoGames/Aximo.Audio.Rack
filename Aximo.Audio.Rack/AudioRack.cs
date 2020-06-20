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
                Cables = Cables.AppendElement(cable);
                cable.CableInput.AddCable(cable);
                cable.CableOutput.AddCable(cable);
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
            cable.CableInput.RemoveCable(cable);
            cable.CableOutput.RemoveCable(cable);
        }

        public long Tick;

        public void LoadFromFile(string filePath)
        {
            var file = JsFile.LoadFile(filePath);
        }

        public void Process(AudioProcessArgs e)
        {
            var tasks = Tasks;
            lock (tasks)
            {
                while (tasks.Count > 0)
                {
                    var task = tasks.Dequeue();
                    task();
                }
            }

            var modules = Modules;
            var len = modules.Length;
            for (var i = 0; i < len; i++)
                modules[i].Process(e);

            var cables = Cables;
            len = cables.Length;
            for (var i = 0; i < len; i++)
                cables[i].Process();

            Tick++;
        }

        //private EventCounter Counter = new EventCounter();

        private bool Running;
        public void MainLoop()
        {
            Running = true;
            var e = new AudioProcessArgs();
            while (Running)
            {
                e.Time += 1f / 44100f;
                Process(e);
            }
        }

        private Thread Thread;
        public void StartThread()
        {
            Thread = new Thread(MainLoop);
            Thread.Priority = ThreadPriority.Highest;
            Thread.IsBackground = false;
            Thread.Start();
            Thread.Sleep(1000); // TODO: use waiter!
        }

        private Queue<Action> Tasks = new Queue<Action>();
        public void Dispatch(Action task)
        {
            lock (Tasks)
                Tasks.Enqueue(task);
        }

        public void Stop()
        {
            Running = false;
        }

        public AudioModule GetModule(string name)
        {
            return Modules.FirstOrDefault(m => m.Name == name);
        }
    }
}
