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

    public class PortComponent : UIPanelComponent
    {

        public Port Port;
        public PortComponent(Port port)
        {
            Port = port;
            Border = new UIAnchors(0.25f);
            OuterSize = new Vector2(3, 3);
        }

        public override void OnMouseEnter(MouseMoveArgs e)
        {
            HighLightPorts(true);
            base.OnMouseEnter(e);
        }

        private void HighLightPorts(bool state)
        {
            SceneContext.Current.Visit<PortComponent>(c =>
            {
                if (Port.ConnectedPorts.Contains(c.Port) || c.Port == Port)
                {
                    c.HighLight = state;
                    c.Redraw();
                }
            });

            if (state)
            {
                SceneContext.Current.Find<CablesComponent>().First().SetCables(Port.Cables);
            }
            else
            {
                SceneContext.Current.Find<CablesComponent>().First().SetCables(null);
            }
        }

        public override void OnMouseLeave(MouseMoveArgs e)
        {
            HighLightPorts(false);
            Redraw();
            base.OnMouseLeave(e);
        }

        public override void OnMouseDown(MouseButtonArgs e)
        {
            (Application.Current as RackApplication).PortMouseDown(this);
            base.OnMouseDown(e);
        }

        public override void OnMouseUp(MouseButtonArgs e)
        {
            (Application.Current as RackApplication).PortMouseUp(this);
            base.OnMouseDown(e);
        }

        public bool HighLight = false;

        protected override void DrawControl()
        {
            Color bgColor = Color.DarkBlue.WithAlpha(0.5f);
            if (Port.IsConnected)
                bgColor = Color.Green.WithAlpha(0.9f);

            if (HighLight)
                bgColor = Color.Yellow.WithAlpha(0.9f);

            Color borderColor = Color.Red;

            ImageContext.Clear(bgColor);
            ImageContext.DrawButton(Border, borderColor);
        }
    }
}
