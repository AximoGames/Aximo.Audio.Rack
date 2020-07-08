// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Audio
{
    public abstract class AudioModule
    {
        public Vector2i Position;
        internal AudioRack Rack;
        public Port[] Outputs = Array.Empty<Port>();
        public Port[] Inputs = Array.Empty<Port>();
        public AudioParameter[] Parameters = Array.Empty<AudioParameter>();

        public Port GetOutput(string name) => Outputs.FirstOrDefault(p => p.Name == name);
        public Port GetInput(string name) => Inputs.FirstOrDefault(p => p.Name == name);
        public Port GetOutput(int index) => Outputs.TryGet(index);
        public Port GetInput(int index) => Inputs.TryGet(index);

        public AudioParameter GetParameter(string name) => Parameters.FirstOrDefault(p => p.Name == name);
        public AudioParameter GetParameter(int index) => Parameters[index];

        public string Name;

        internal void ProcessPort(AudioProcessArgs e)
        {
            var ports = Inputs;
            var len = ports.Length;
            for (var i = 0; i < len; i++)
                ports[i].Process(e);

            ports = Outputs;
            len = ports.Length;
            for (var i = 0; i < len; i++)
                ports[i].Process(e);
        }

        public abstract void Process(AudioProcessArgs e);

        public const int SamplesPerSecond = 44100;
        public const float SampleTime = 1.0f / SamplesPerSecond;

        protected Port ConfigureOutput(int i, string name)
        {
            Outputs = Outputs.EnsureSize(i + 1);
            if (Outputs[i] == null)
                Outputs[i] = new Port(this, PortDirection.Output, name, i);

            var port = Outputs[i];
            port.Name = name;
            return port;
        }

        protected Port ConfigureInput(int i, string name)
        {
            Inputs = Inputs.EnsureSize(i + 1);
            if (Inputs[i] == null)
                Inputs[i] = new Port(this, PortDirection.Input, name, i);

            var port = Inputs[i];
            port.Name = name;
            return port;
        }

        protected AudioParameter ConfigureParameter(int i, string name, AudioParameterType type, float min, float max, float? value)
        {
            Parameters = Parameters.EnsureSize(i + 1);
            if (Parameters[i] == null)
                Parameters[i] = new AudioParameter(this, name, type, min, max);
            var parameter = Parameters[i];
            if (value != null)
                parameter.Value = (float)value;
            return parameter;
        }

        public virtual AudioWidget CreateWidget() => new AudioAutoWidget<AudioModule>(this);

        /// <summary>
        /// Custom Property for the Developer.
        /// </summary>
        public object Tag { get; set; }
    }

}
