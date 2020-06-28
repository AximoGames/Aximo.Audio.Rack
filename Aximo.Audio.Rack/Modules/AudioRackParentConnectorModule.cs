// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace Aximo.Engine.Audio.Modules
{

    public class AudioRackParentConnectorModule : AudioModule
    {
        internal Port[] InputChannels;
        internal Port[] OutputChannels;

        public AudioRackParentConnectorModule()
        {
            Name = "RackParentConnector";

            ConfigureParameter(0, "SwitchToChild", AudioParameterType.Button, 0, 1, 0);

            ConfigureInput(0, "Input1");
            ConfigureInput(1, "Input2");
            ConfigureInput(2, "Input3");
            ConfigureInput(3, "Input4");
            ConfigureInput(4, "Input5");
            ConfigureInput(5, "Input6");
            ConfigureInput(6, "Input7");
            ConfigureInput(7, "Input8");

            ConfigureOutput(0, "Output1");
            ConfigureOutput(1, "Output2");
            ConfigureOutput(2, "Output3");
            ConfigureOutput(3, "Output4");

            InputChannels = new Port[] { Inputs[0], Inputs[1], Inputs[2], Inputs[3], Inputs[4], Inputs[5], Inputs[6], Inputs[7] };
            OutputChannels = new Port[] { Outputs[0], Outputs[1], Outputs[2], Outputs[3] };

            var childRack = new AudioRack();
            Child = new AudioRackChildConnectorModule();
            Child.Parent = this;
            childRack.AddModule(Child);
        }

        internal AudioRackChildConnectorModule Child;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process(AudioProcessArgs e)
        {
            for (var i = 0; i < InputChannels.Length; i++)
                Child.OutputChannels[i].SetVoltage(InputChannels[i].GetVoltage());

            Child.Rack.Process(e);

            for (var i = 0; i < OutputChannels.Length; i++)
                OutputChannels[i].SetVoltage(Child.InputChannels[i].GetVoltage());
        }

        public override AudioWidget CreateWidget() => new Widget(this);

        private class Widget : AudioAutoWidget<AudioRackParentConnectorModule>
        {
            public Widget(AudioRackParentConnectorModule module) : base(module)
            {
            }

            public override void Init()
            {
                WidgetInterface.SetParameterClickedHandler((p) =>
                {
                    if (p == Module.GetParameter(0))
                        WidgetInterface.Application.SwitchRack(Module.Child.Rack);
                });
                base.Init();
            }
        }
    }
}
