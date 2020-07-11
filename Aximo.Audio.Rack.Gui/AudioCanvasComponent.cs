// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using Aximo.Engine.Audio;
using Aximo.Engine.Components.UI;

namespace Aximo.Audio.Rack.Gui
{
    internal class AudioCanvasComponent : UIPanelComponent
    {
        private AudioWidget Widget;

        public AudioCanvasComponent(AudioWidget widget)
        {
            Widget = widget;
        }

        public override void UpdateFrame()
        {
            Widget.UpdateFrame();

            base.UpdateFrame();
        }
    }
}
