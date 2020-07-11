// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace Aximo.Engine.Audio.Modules
{

    public class AudioVCOModule : AudioModule
    {
        private Port SinOut;

        private AudioParameter FreqParam;
        private AudioParameter LfoParam;

        private float vcoMin = 16;
        private float vcoMax = 2000;
        private float vcoDefault = Keys.Default;

        private float lfoMin = 0.01f;
        private float lfoMax = 20;
        private float lfoDefault = 1;

        public AudioVCOModule()
        {
            Name = "VCO";

            FreqParam = ConfigureParameter(0, "Freq", AudioParameterType.Slider, 0, 1, AxMath.Map(vcoDefault, vcoMin, vcoMax, 0, 1));
            LfoParam = ConfigureParameter(1, "LFO", AudioParameterType.Toggle, 0, 1, 0);

            ConfigureInput(0, "FreqVC");
            ConfigureOutput(0, "Out");

            SinOut = Outputs[0];
        }

        private float CurrentPhase;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process(AudioProcessArgs e)
        {
            var freqVC = Inputs[0];
            var lfo = LfoParam.IsToggleUp;
            float freq;
            if (lfo)
                freq = FreqParam.GetBipolarMappedValue(freqVC, lfoMin, lfoMax);
            else
                freq = FreqParam.GetBipolarMappedValue(freqVC, vcoMin, vcoMax);

            var phaseStep = 1 / (float)44100f * freq;
            CurrentPhase += phaseStep;
            CurrentPhase = AxMath.Digits(CurrentPhase);

            var v = MathF.Sin(MathF.PI * 2 * CurrentPhase) * 5;
            SinOut.SetVoltage(v);
        }
    }
}
