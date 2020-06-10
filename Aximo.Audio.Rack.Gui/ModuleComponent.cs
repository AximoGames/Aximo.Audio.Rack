// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using Aximo.Render.OpenGL;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Audio.Rack.Gui
{

    public class ModuleComponent : StaticMeshComponent
    {
        public ModuleComponent(Vector2i size) : base(MeshDataBuilder.Cube(), Material.Default)
        {
            TranslationMatrix = RackApplication.BoardTranslationMatrix;
            Image = new Image<Rgba32>(size.X, size.Y);
            Texture = Texture.GetFromBitmap(Image, null);
            Material = new Material();
            Material.DiffuseTexture = Texture;
            UpdateTexture();
        }

        public SceneComponent AddKnob()
        {
            var comp = new KnobComponent();
            AddComponent(comp);
            return comp;
        }

        protected void ResizeImage(Vector2i size)
        {
            if (size.X == 0 || size.Y == 0)
                return;

            Image = new Image<Rgba32>(size.X, size.Y);
            UpdateTexture();
        }

        protected Image<Rgba32> Image { get; private set; }

        public Texture Texture { get; private set; }

        //public override bool ContainsScreenCoordinate(Vector2 pos)
        //{
        //    return base.ContainsScreenCoordinate(pos);
        //}

        public override void UpdateFrame()
        {
            Image.Mutate(ctx => ctx.Fill(Color.Red));
            base.UpdateFrame();
        }

        public void UpdateTexture()
        {
            Texture.SetData(Image);
            PropertyChanged();
        }

    }
}
