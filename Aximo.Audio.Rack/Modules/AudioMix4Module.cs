// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace Aximo.Engine.Audio
{

    public class AudioMix4Module : AudioModule
    {
        private Port[] InputChannels;
        private Port[] OutputChannels;

        const float DefaultVolume = 0.5f;

        public AudioMix4Module()
        {
            Name = "Mix4";

            ConfigureParameter(0, "Volume1", AudioParameterType.Slider, 0, 1, DefaultVolume);
            ConfigureParameter(1, "Volume2", AudioParameterType.Slider, 0, 1, DefaultVolume);
            ConfigureParameter(2, "Volume3", AudioParameterType.Slider, 0, 1, DefaultVolume);
            ConfigureParameter(3, "Volume4", AudioParameterType.Slider, 0, 1, DefaultVolume);

            ConfigureInput(0, "Left1");
            ConfigureInput(1, "Right1");
            ConfigureInput(2, "Left2");
            ConfigureInput(3, "Right2");
            ConfigureInput(4, "Left3");
            ConfigureInput(5, "Right3");
            ConfigureInput(6, "Left4");
            ConfigureInput(7, "Right4");

            ConfigureOutput(0, "Left");
            ConfigureOutput(1, "Right");

            InputChannels = new Port[] { Inputs[0], Inputs[1], Inputs[2], Inputs[3], Inputs[4], Inputs[5], Inputs[6], Inputs[7] };
            OutputChannels = new Port[] { Outputs[0], Outputs[1] };
        }

        private float[] TmpOut = new float[2];

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process(AudioProcessArgs e)
        {
            var inputChannels = InputChannels;
            var len = InputChannels.Length / 2;
            var outputChannels = OutputChannels;
            var tmpOut = TmpOut;
            tmpOut[0] = 0;
            tmpOut[1] = 0;
            var parameters = Parameters;

            for (var i = 0; i < len; i++)
            {
                var param = parameters[i];
                var volume = param.Value;

                var idx = i * 2;
                var input = inputChannels[idx];
                tmpOut[0] += input.GetVoltage() * volume;

                idx++;
                input = inputChannels[idx];
                tmpOut[1] += input.GetVoltage() * volume;
            }

            outputChannels[0].SetVoltage(tmpOut[0]);
            outputChannels[1].SetVoltage(tmpOut[1]);
        }
    }
}
