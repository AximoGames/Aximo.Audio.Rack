// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Aximo.Engine.Audio
{
    // C# ADSR based on work by Nigel Redmon, EarLevel Engineering: earlevel.com
    // http://www.earlevel.com/main/2013/06/03/envelope-generators-adsr-code/

    /// <summary>
    /// Envelope generator (ADSR)
    /// </summary>
    public class EnvelopeGenerator
    {

        /// <summary>
        /// Envelope State
        /// </summary>
        public enum EnvelopeState
        {
            /// <summary>
            /// Idle
            /// </summary>
            Idle = 0,

            /// <summary>
            /// Attack
            /// </summary>
            Attack,

            /// <summary>
            /// Decay
            /// </summary>
            Decay,

            /// <summary>
            /// Sustain
            /// </summary>
            Sustain,

            /// <summary>
            /// Release
            /// </summary>
            Release,
        }

        private float level;
        private EnvelopeState state;
        private bool isReleased;
        private float releaseRate;
        private float interval = 1000f / 44100f;

        public void Attack()
        {
            state = EnvelopeState.Attack;
            level = BaseLevel;
            isReleased = false;
        }

        public void Release()
        {
            isReleased = true;
        }

        /// <summary>
        /// Creates and Initializes an Envelope Generator
        /// </summary>
        public EnvelopeGenerator()
        {
        }

        public float BaseLevel = 0;
        public float AttackRate;   // in level/msec
        public float PeakLevel = 1;
        public float DecayRate;    // in level/msec
        public float SustainLevel;
        public float ReleaseTime;  // in msec

        public void Gate(bool trigger)
        {
            if (trigger)
            {
                if (isReleased)
                    Attack();
            }
            else
            {
                if (!isReleased)
                    Release();
            }
        }

        /// <summary>
        /// Attack Rate (seconds * SamplesPerSecond)
        /// </summary>
        public float Process(out bool completed)
        {
            completed = false;
            // If note is released, go directly to Release state,
            // except if still attacking
            if (isReleased &&
              (state == EnvelopeState.Decay || state == EnvelopeState.Sustain))
            {
                state = EnvelopeState.Release;
                releaseRate = (BaseLevel - level) / ReleaseTime;
            }
            switch (state)
            {
                case EnvelopeState.Idle:
                    level = BaseLevel;
                    completed = true;
                    break;
                case EnvelopeState.Attack:
                    level += AttackRate;
                    if ((AttackRate > 0 && level >= PeakLevel) ||
                      (AttackRate < 0 && level <= PeakLevel))
                    {
                        level = PeakLevel;
                        state = EnvelopeState.Decay;
                    }
                    break;
                case EnvelopeState.Decay:
                    level += DecayRate;
                    if ((DecayRate > 0 && level >= SustainLevel) ||
                      (DecayRate < 0 && level <= SustainLevel))
                    {
                        level = SustainLevel;
                        state = EnvelopeState.Sustain;
                    }
                    break;
                case EnvelopeState.Sustain:
                    break;
                case EnvelopeState.Release:
                    level += releaseRate;
                    if ((releaseRate > 0 && level >= BaseLevel) ||
                      (releaseRate < 0 && level <= BaseLevel))
                    {
                        level = BaseLevel;
                        state = EnvelopeState.Idle;
                        completed = true;
                    }
                    break;
            }
            return level;
        }

    }
}
