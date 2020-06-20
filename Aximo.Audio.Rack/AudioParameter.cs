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

    public enum AudioParameterScale
    {
        Linear,
        Exp,
        Log,
    }

    public class AudioParameter
    {
        public AudioModule Module;
        public string Name;
        public float Min;
        public float Max;
        public float Value;
        public AudioParameterType Type;

        public AudioParameterScale ScaleType;
        public float DisplayMultiplier = 1;
        public float DisplayOffset;
        public float DisplayBase;

        public AudioParameter SetDisplayRangeLinear(float displayMultiplier, float displayOffset = 0)
        {
            ScaleType = AudioParameterScale.Linear;
            DisplayMultiplier = displayMultiplier;
            DisplayOffset = displayOffset;
            return this;
        }

        public AudioParameter SetDisplayRangeExp(float displayBase, float displayMultiplier, float displayOffset = 0)
        {
            ScaleType = AudioParameterScale.Exp;
            DisplayBase = displayBase;
            DisplayMultiplier = displayMultiplier;
            DisplayOffset = displayOffset;
            return this;
        }

        public AudioParameter SetDisplayRangeLog(float displayBase, float displayMultiplier, float displayOffset = 0)
        {
            ScaleType = AudioParameterScale.Log;
            DisplayBase = displayBase;
            DisplayMultiplier = displayMultiplier;
            DisplayOffset = displayOffset;
            return this;
        }

        public float GetDisplayValue()
        {
            float v = GetValue();

            if (ScaleType == AudioParameterScale.Log)
                v = MathF.Log(v) / MathF.Log(DisplayBase);
            else if (ScaleType == AudioParameterScale.Exp)
                v = MathF.Pow(DisplayBase, v);

            return (v * DisplayMultiplier) + DisplayOffset;
        }

        public void SetDisplayValue(float displayValue)
        {
            float v = displayValue - DisplayOffset;

            if (DisplayMultiplier == 0f)
                v = 0f;
            else
                v /= DisplayMultiplier;

            if (DisplayBase != 0f)
            {
                if (ScaleType == AudioParameterScale.Log)
                    v = MathF.Pow(DisplayBase, v);
                else if (ScaleType == AudioParameterScale.Exp)
                    v = MathF.Log(v) / MathF.Log(DisplayBase);
            }

            SetValue(v);
        }

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

        public float GetUnipolarMappedValue(Port input, float toMin, float toMax)
        {
            if (input.IsConnected)
                return GetScaledMappedValue(input.GetVoltage() / 10f, toMin, toMax);
            else
                return GetMappedValue(toMin, toMax);
        }

        public float GetUnipolarValue(float scale)
        {
            return Value * scale;
        }

        public float GetUnipolarValue(Port input)
        {
            if (input.IsConnected)
                return Value * (input.GetVoltage() / 10f);
            return
                Value;
        }
    }
}
