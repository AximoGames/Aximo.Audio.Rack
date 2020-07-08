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
        internal AudioParameter TriggerParam;
        internal Port Trigger;

        public AudioRackChildConnectorModule()
        {
            Name = "RackChildConnector";

            ConfigureParameter(0, "SwitchToParent", AudioParameterType.Button, 0, 1, 0);
            TriggerParam = ConfigureParameter(1, "Trigger", AudioParameterType.Button, 0, 1, 0);

            ConfigureInput(0, "Input1");
            ConfigureInput(1, "Input2");
            ConfigureInput(2, "Input3");
            ConfigureInput(3, "Input4");

            Trigger = ConfigureOutput(0, "Trigger");
            ConfigureOutput(1, "Velocity");
            ConfigureOutput(2, "Output1");
            ConfigureOutput(3, "Output2");
            ConfigureOutput(4, "Output3");
            ConfigureOutput(5, "Output4");
            ConfigureOutput(6, "Output5");
            ConfigureOutput(7, "Output6");

            InputChannels = new Port[] { Inputs[0], Inputs[1], Inputs[2], Inputs[3] };
            OutputChannels = new Port[] { Outputs[0], Outputs[1], Outputs[2], Outputs[3], Outputs[4], Outputs[5], Outputs[6], Outputs[7] };
        }

        private bool OldTriggerState;
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process(AudioProcessArgs e)
        {
            var triggerState = TriggerParam.IsToggleUp;
            if (triggerState)
            {
                Trigger.SetVoltage(TriggerParam.Max);
            }
            else
            {
                if (triggerState != OldTriggerState)
                    Trigger.SetVoltage(TriggerParam.Min);
            }
            OldTriggerState = triggerState;
            Trigger.SetVoltage(2);
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
