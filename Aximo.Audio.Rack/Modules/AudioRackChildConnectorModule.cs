// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace Aximo.Engine.Audio.Modules
{

    public class AudioRackChildConnectorModule : AudioModule
    {
        internal Port[] InputChannels;
        internal Port[] OutputChannels;

        internal AudioRackParentConnectorModule? Parent;

        public AudioRackChildConnectorModule()
        {
            Name = "RackChildConnector";

            ConfigureParameter(0, "SwitchToParent", AudioParameterType.Button, 0, 1, 0);

            ConfigureInput(0, "Input1");
            ConfigureInput(1, "Input2");
            ConfigureInput(2, "Input3");
            ConfigureInput(3, "Input4");

            ConfigureOutput(0, "Output1");
            ConfigureOutput(1, "Output2");
            ConfigureOutput(2, "Output3");
            ConfigureOutput(3, "Output4");
            ConfigureOutput(4, "Output5");
            ConfigureOutput(5, "Output6");
            ConfigureOutput(6, "Output7");
            ConfigureOutput(7, "Output8");

            InputChannels = new Port[] { Inputs[0], Inputs[1], Inputs[2], Inputs[3] };
            OutputChannels = new Port[] { Outputs[0], Outputs[1], Outputs[2], Outputs[3], Outputs[4], Outputs[5], Outputs[6], Outputs[7] };
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process(AudioProcessArgs e)
        {
        }

        public override AudioWidget CreateWidget() => new Widget(this);

        private class Widget : AudioAutoWidget<AudioRackChildConnectorModule>
        {
            public Widget(AudioRackChildConnectorModule module) : base(module)
            {
            }

            public override void Init()
            {
                WidgetInterface.SetParameterClickedHandler((p) =>
                {
                    if (p == Module.GetParameter(0) && Module.Parent != null)
                        WidgetInterface.Application.SwitchRack(Module.Parent.Rack);
                });
                base.Init();
            }
        }

    }
}
