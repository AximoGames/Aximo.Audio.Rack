// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace Aximo.Engine.Audio.Modules
{

    public class AudioADSRModule : AudioModule
    {
        private Port SinOut;

        private AudioParameter AttackParam;
        private AudioParameter DecayParam;
        private AudioParameter SubstainParam;
        private AudioParameter ReleaseParam;
        private AudioParameter TriggerParam;

        private Port Trigger;
        private Port Volume;

        private EnvelopeGenerator Env;
        private const float MinTime = 0.001f;
        private const float MaxTime = 10f;
        private const float LamdaBase = MaxTime / MinTime;

        public AudioADSRModule()
        {
            Name = "ADSR";

            AttackParam = ConfigureParameter(0, "Attack", AudioParameterType.Slider, 0, 1, 0.5f).SetDisplayRangeExp(LamdaBase, 1000);
            DecayParam = ConfigureParameter(1, "Decay", AudioParameterType.Slider, 0, 1, 0.5f).SetDisplayRangeExp(LamdaBase, 1000);
            SubstainParam = ConfigureParameter(2, "Substain", AudioParameterType.Slider, 0, 1, 0.5f).SetDisplayRangeLinear(0, 100);
            ReleaseParam = ConfigureParameter(3, "Release", AudioParameterType.Slider, 0, 1, 0.5f).SetDisplayRangeExp(LamdaBase, 1000);
            TriggerParam = ConfigureParameter(4, "Trigger", AudioParameterType.Button, 0, 1, 0f);

            Trigger = ConfigureInput(0, "Gate");
            Volume = ConfigureOutput(0, "Out");

            Env = new EnvelopeGenerator();

            SinOut = Outputs[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process(AudioProcessArgs e)
        {
            Env.SustainLevel = SubstainParam.GetValue();
            Env.AttackRate = 1f / (44100f / (1000f / MathF.Pow(AttackParam.DisplayBase, AttackParam.GetValue())));
            Env.DecayRate = -((1f - Env.SustainLevel) / (44100f / (1000f / MathF.Pow(DecayParam.DisplayBase, DecayParam.GetValue()))));
            Env.ReleaseTime = 44100f / (1000f / MathF.Pow(ReleaseParam.DisplayBase, ReleaseParam.GetValue()));

            Env.Gate(Trigger.GetVoltage() >= 0.9f || TriggerParam.IsToggleUp);

            var volume = -Env.Process(out _);
            Volume.SetVoltage(volume * 10f);
        }
    }
}
