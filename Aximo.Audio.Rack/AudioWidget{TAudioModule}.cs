// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Aximo.Engine.Audio
{

    public abstract class AudioWidget<TAudioModule> : AudioWidget
        where TAudioModule : AudioModule
    {
        public new TAudioModule Module => (TAudioModule)base.Module;

        public AudioWidget(TAudioModule module) : base(module)
        {
        }
    }

}
