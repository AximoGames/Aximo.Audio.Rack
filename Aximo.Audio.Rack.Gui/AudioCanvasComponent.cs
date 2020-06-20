using System;
using System.Collections.Generic;
using System.Text;
using Aximo.Engine.Audio;
using Aximo.Engine.Components.UI;

namespace Aximo.Audio.Rack.Gui
{
    class AudioCanvasComponent : UIPanelComponent
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
