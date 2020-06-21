// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

    public class AudioSliderComponent : UISlider
    {
        private AudioParameter Param;
        public AudioSliderComponent(AudioParameter param) : base()
        {
            Param = param;
            Margin = new UIAnchors();
            Padding = new UIAnchors();
            Border = new UIAnchors(0.25f);
            BorderRadius = 0;

            Button.BorderRadius = 0;
            MinValue = param.Min;
            MaxValue = param.Max;
            Value = param.Value;

            //TranslationMatrix = RackApplication.BoardTranslationMatrix;

            //Material = new Material
            //{
            //    Color = new Vector4(1, 1, 0, 1),
            //};
            //var mesh = Mesh.CreateCylinder();
            //var marker = Mesh.CreateCube();
            //marker.Scale(new Vector3(0.1f, 0.5f, 1.1f));
            //marker.TranslateY(0.3f);
            //mesh.AddMesh(marker, 1);

            //SetMesh(mesh);

            //IsAbsoluteScale = true;
            //IsAbsoluteRotation = true;
            //IsAbsoluteTranslation = true;

            //RelativeTranslation = new Vector3(2, 2, 0);
            //RelativeRotation = new Quaternion(0, 0, 0.2f);
        }

        protected override void OnSliderValueChanged(SliderValueChangedArgs e)
        {
            Param.SetValueSmoothed(e.NewValue);
            base.OnSliderValueChanged(e);
        }

    }
}
