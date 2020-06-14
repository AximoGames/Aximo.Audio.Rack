// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using OpenToolkit.Windowing.Common;

namespace Aximo.Engine.Audio
{

    public enum AudioParameterType
    {
        Slider,
        Knob,
        Toggle,
    }

    public class AudioParameter
    {
        public AudioModule Module;
        public string Name;
        public float Min;
        public float Max;
        public float Value;
        public AudioParameterType Type;


        public bool IsToggleUp => Value >= Max;
        public bool IsToggleDown => Value <= Min;

        public void Toggle()
        {
            if (IsToggleUp)
                Value = Min;
            else
                Value = Max;
        }

        public AudioParameter(AudioModule module, string name, AudioParameterType type, float min, float max)
        {
            Module = module;
            Name = name;
            Type = type;
            Min = min;
            Max = max;
            Value = min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void SetValue(float value)
        {
            Value = MathF.Max(MathF.Min(value, Max), Min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float GetValue() => Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float GetMappedValue(float toMin, float toMax) => AxMath.Map(Value, Min, Max, toMin, toMax);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float GetMappedValue(float value, float toMin, float toMax) => AxMath.Map(value, Min, Max, toMin, toMax);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float SetMappedValue(float value, float toMin, float toMax) => AxMath.Map(Value, toMin, toMax, Min, Max);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float GetScaledMappedValue(float scale, float toMin, float toMax) => GetMappedValue(AxMath.Map(AxMath.Map(Value, Min, Max, 0, 1) * scale, 0, 1, Min, Max), toMin, toMax);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float GetBipolarMappedValue(Port input, float toMin, float toMax)
        {
            if (input.IsConnected)
                return GetScaledMappedValue(input.GetVoltage() / 5f, toMin, toMax);
            else
                return GetMappedValue(toMin, toMax);
        }
    }
}
