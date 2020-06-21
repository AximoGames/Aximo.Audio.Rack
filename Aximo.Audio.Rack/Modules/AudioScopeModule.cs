// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using OpenToolkit.Graphics.ES20;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Aximo.Engine.Audio
{

    public class AudioScopeModule : AudioModule
    {
        private AudioParameter TimeParam;
        private AudioParameter ScaleY1Param;
        private Port Input1;

        public AudioScopeModule()
        {
            Name = "Scope";

            const float timeBase = (float)BUFFER_SIZE / 6;
            TimeParam = ConfigureParameter(0, "Time", AudioParameterType.Slider, 6, 16, 14).SetDisplayRangeExp(0.5f, 1000 * timeBase);
            ScaleY1Param = ConfigureParameter(1, "ScaleY1", AudioParameterType.Slider, 0.5f, 10, 1);

            Input1 = ConfigureInput(0, "Input1");
        }

        private const int BUFFER_SIZE = 512;
        private float[] Buffer = new float[BUFFER_SIZE];
        private int BufferIndex;
        private int FrameIndex;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process(AudioProcessArgs e)
        {
            float deltaTime = MathF.Pow(2f, -TimeParam.GetValue());
            int frameCount = (int)MathF.Ceiling(deltaTime * 44100);

            if (BufferIndex < BUFFER_SIZE)
            {
                if (++FrameIndex > frameCount)
                {
                    FrameIndex = 0;
                    Buffer[BufferIndex] = Input1.GetVoltage();
                    BufferIndex++;
                }
            }

            if (BufferIndex < BUFFER_SIZE)
                return;

            FrameIndex++;

            const float holdTime = 0.5f;
            if (FrameIndex * SampleTime >= holdTime)
            {
                Trigger();
                return;
            }
        }

        public void Trigger()
        {
            BufferIndex = 0;
            FrameIndex = 0;
        }

        public override AudioWidget CreateWidget() => new Widget(this);

        private class Widget : AudioAutoWidget<AudioScopeModule>
        {
            public Widget(AudioScopeModule module) : base(module)
            {
            }

            public override void Init()
            {
                ImageContext = WidgetInterface.RegisterCanvas(new Vector2(WidgetInterface.ModuleSize.X, 9));
                ImageContext.Clear(Color.BlueViolet);
                ImageContext.Flush();
                base.Init();
            }

            private ImageContext ImageContext;

            private long UpdateSkip = 0;
            public override void UpdateFrame()
            {
                if (UpdateSkip-- > 0)
                    return;
                UpdateSkip = 30;

                var buffer = Module.Buffer;
                ImageContext.Clear(Color.Black);
                DrawRanges();

                ImageContext.FillStyle(Color.Red);
                ImageContext.BeginPath();
                for (var i = 0; i < BUFFER_SIZE; i++)
                {
                    var p = new Vector2(i, buffer[i]);
                    p.Y *= Module.ScaleY1Param.GetDisplayValue();
                    p.Y = AxMath.Map(p.Y, -10, 10, 0, ImageContext.Image.Size().Height);
                    if (i == 0)
                        ImageContext.MoveTo(p);
                    else
                        ImageContext.LineTo(p);
                }
                ImageContext.Stroke();
                ImageContext.Flush();
            }

            private void DrawRanges()
            {
                DrawRange(5, Color.Green);
                DrawRange(10, Color.Blue);
                DrawRange(1, Color.DarkGray);
            }

            private void DrawRange(float num, Color color)
            {
                ImageContext.FillStyle(color);
                ImageContext.BeginPath();
                var y = AxMath.Map(num * Module.ScaleY1Param.GetDisplayValue(), -10, 10, 0, ImageContext.Image.Size().Height);
                ImageContext.MoveTo(new Vector2(0, y));
                ImageContext.LineTo(new Vector2(ImageContext.Image.Size().Width, y));
                y = AxMath.Map(-num * Module.ScaleY1Param.GetDisplayValue(), -10, 10, 0, ImageContext.Image.Size().Height);
                ImageContext.MoveTo(new Vector2(0, y));
                ImageContext.LineTo(new Vector2(ImageContext.Image.Size().Width, y));
                ImageContext.Stroke();
            }
        }
    }
}
