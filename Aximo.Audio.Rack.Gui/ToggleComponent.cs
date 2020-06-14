// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Aximo.Engine;
using Aximo.Engine.Audio;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.UI;
using Aximo.Engine.Windows;
using Aximo.Render.OpenGL;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Audio.Rack.Gui
{
    public class ToggleComponent : UIPanelComponent
    {

        public AudioParameter Param;
        public ToggleComponent(AudioParameter param)
        {
            Param = param;
            Border = new UIAnchors(0.25f);
            OuterSize = new Vector2(3, 3);
        }

        public override void OnMouseClick(MouseButtonArgs e)
        {
            Param.Toggle();
            base.OnMouseClick(e);
            Redraw();
        }

        protected override void DrawControl()
        {
            Color bgColor = Color.Pink.WithAlpha(0.9f);
            if (Param.IsToggleUp)
                bgColor = Color.Purple.WithAlpha(0.9f);

            Color borderColor = Color.GreenYellow;

            ImageContext.Clear(bgColor);
            ImageContext.DrawButton(Border, borderColor);
        }
    }
}
