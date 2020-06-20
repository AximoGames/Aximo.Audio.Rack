// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Aximo.Engine.Audio
{

    public class AudioAutoWidget<TAudioModule> : AudioWidget<TAudioModule>
        where TAudioModule : AudioModule
    {
        public AudioAutoWidget(TAudioModule module) : base(module)
        {
        }
    }

}
