// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Reflection;
using Aximo.Engine;
using Aximo.Engine.Audio;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.UI;
using Aximo.Render.OpenGL;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Audio.Rack.Gui
{
    public class ModuleComponent : UIPanelComponent, IWidgetInterface
    {

        public const float ModuleHeight = 26.25f;
        private const float ModuleHP = 1f;
        private AudioModule Module;
        private AudioWidget Widget;

        public ModuleComponent(AudioModule module, int width)
        {
            Module = module;
            Margin = UIAnchors.Zero;
            Padding = UIAnchors.Zero;
            Border = new UIAnchors(1);
            OuterSize = new Vector2(width, ModuleHeight);
            ModuleSize = new Vector2(width, ModuleHeight);
            Init();
        }

        private void Init()
        {
            Widget = Module.CreateWidget();
            Widget.SetInterface(this);
            Widget.Init();

            AddComponent(new UILabelComponent(Module.Name)
            {
                Size = new Vector2(Size.X, 2),
                FontSize = 1.25f,
            });
            CurrentLocation.Y += 2;

            foreach (var port in Module.Inputs)
            {
                AddPort(port);
            }

            foreach (var param in Module.Parameters.Where(p => p.Type == AudioParameterType.Slider))
            {
                AddSlider(param);
            }

            foreach (var param in Module.Parameters.Where(p => p.Type == AudioParameterType.Toggle))
            {
                AddToggle(param);
            }

            foreach (var port in Module.Outputs)
            {
                AddPort(port);
            }
        }

        protected override void DrawControl()
        {
            Color bgColor = Color.LightCoral.WithAlpha(0.5f);
            Color borderColor = Color.DarkGray;

            ImageContext.Clear(bgColor);
            ImageContext.DrawButton(Border, borderColor);
        }

        public SceneComponent AddSlider(AudioParameter param)
        {
            var comp = new AudioSliderComponent(param);
            AddComponent(comp);
            comp.SliderThickness = 2f;
            comp.OuterSize = new Vector2(Size.X, 3);
            if (CurrentLocation.X > 0)
            {
                CurrentLocation.X = 0;
                CurrentLocation.Y += comp.OuterSize.Y;
            }
            comp.Location = CurrentLocation;
            CurrentLocation.Y += comp.OuterSize.Y;
            CurrentLocation.X = 0;
            return comp;
        }

        private Vector2 CurrentLocation;

        public Vector2 ModuleSize { get; private set; }

        public SceneComponent AddPort(Port port)
        {
            var comp = new PortComponent(port);
            AddComponent(comp);
            comp.Location = CurrentLocation;
            CurrentLocation.X += comp.OuterSize.X;
            if (CurrentLocation.X >= Size.X)
            {
                CurrentLocation.X = 0;
                CurrentLocation.Y += comp.OuterSize.Y;
            }
            return comp;
        }

        public SceneComponent AddToggle(AudioParameter param)
        {
            var comp = new ToggleComponent(param);
            AddComponent(comp);
            comp.Location = CurrentLocation;
            CurrentLocation.X += comp.OuterSize.X;
            if (CurrentLocation.X >= Size.X)
            {
                CurrentLocation.X = 0;
                CurrentLocation.Y += comp.OuterSize.Y;
            }
            return comp;
        }

        public UIComponent AddCanvas(Vector2 size)
        {
            var comp = new AudioCanvasComponent(Widget);
            AddComponent(comp);
            comp.OuterSize = size;
            if (CurrentLocation.X > 0)
            {
                CurrentLocation.X = 0;
                CurrentLocation.Y += comp.OuterSize.Y;
            }
            comp.Location = CurrentLocation;
            CurrentLocation.Y += comp.OuterSize.Y;
            CurrentLocation.X = 0;
            return comp;
        }

        public ImageContext RegisterCanvas(Vector2 size)
        {
            return AddCanvas(size).ImageContext;
        }
    }
}
