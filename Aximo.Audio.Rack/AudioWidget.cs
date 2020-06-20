// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Aximo.Engine.Audio
{

    public abstract class AudioWidget
    {
        public AudioModule Module { get; private set; }
        protected IWidgetInterface WidgetInterface { get; private set; }

        public AudioWidget(AudioModule module)
        {
            Module = module;
        }

        public virtual void UpdateFrame() { }

        internal void SetInterface(IWidgetInterface widgetInterface)
        {
            WidgetInterface = widgetInterface;
        }

        public virtual void Init()
        {
        }
    }

}
