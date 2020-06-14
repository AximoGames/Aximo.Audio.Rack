// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace Aximo.Engine.Audio
{

    public enum PortDirection
    {
        Input,
        Output,
    }

    public class Port
    {
        public Channel[] Channels;
        public const int MaxChannels = 16;
        public string Name;
        public AudioModule Module;
        public AudioCable[] Cables;
        public Port[] ConnectedPorts;
        public PortDirection Direction;

        public bool IsConnected
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return Cables.Length > 0;
            }
        }

        public Port(AudioModule module, PortDirection direction, string name)
        {
            Module = module;
            Name = name;
            Direction = direction;
            Channels = new Channel[MaxChannels];
            Cables = Array.Empty<AudioCable>();
            ConnectedPorts = Array.Empty<Port>();
            for (var i = 0; i < MaxChannels; i++)
                Channels[i] = new Channel(i, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void SetVoltage(float voltage, int channel = 0)
        {
            Channels[channel].Voltage = voltage;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float GetVoltage(int channel = 0)
        {
            return Channels[channel].Voltage;
        }

        public void AddCable(AudioCable cable)
        {
            if (Direction == PortDirection.Input && IsConnected)
                if (IsConnected)
                    throw new Exception("Input ports can have only a single cable");

            if (cable.CableInput.Direction == cable.CableOutput.Direction)
                throw new Exception("Cannot connect to ports with same direction");

            if (Direction == PortDirection.Input)
                ConnectedPorts = ConnectedPorts.AppendElement(cable.CableInput);
            else
                ConnectedPorts = ConnectedPorts.AppendElement(cable.CableOutput);

            Cables = Cables.AppendElement(cable);
            if (Direction == PortDirection.Input)
                cable.Process();
        }

        public void RemoveCable(AudioCable cable)
        {
            Cables = Cables.RemoveElement(cable);

            if (Direction == PortDirection.Input)
                ConnectedPorts = ConnectedPorts.RemoveElement(cable.CableInput);
            else
                ConnectedPorts = ConnectedPorts.RemoveElement(cable.CableOutput);

            if (Cables.Length == 0 && Direction == PortDirection.Input)
                SetVoltage(0);
        }
    }
}
