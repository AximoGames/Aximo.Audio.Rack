// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
//using System.Media;

namespace Aximo.Engine.Audio
{
    /// <summary>
    /// Transfers voltages from one Port to another Port.
    /// Flow direction is from <see cref="ModuleOutput"/> to <see cref="ModuleInput"/>
    /// </summary>
    public class AudioCable
    {
        public Port ModuleOutput;
        public Port ModuleInput;
        public Port[] Ports;

        public AudioCable(Port port1, Port port2)
        {
            if (port1.Direction == PortDirection.Output)
            {
                ModuleOutput = port1;
                ModuleInput = port2;
            }
            else
            {
                ModuleOutput = port2;
                ModuleInput = port1;
            }
            Ports = new Port[] { ModuleOutput, ModuleInput };
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void Process()
        {
            ModuleInput.SetVoltage(ModuleOutput.GetVoltage());
        }
    }
}
