// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.Lights;
using Aximo.Engine.Components.UI;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.Audio.Rack.Gui
{
    public class RackApplication : Application
    {
        public static Matrix4 BoardTranslationMatrix = Matrix4.CreateScale(1, -1, 1);

        protected override void SetupScene()
        {
            var camSize = new Vector2(9 * RenderContext.ScreenAspectRatio, 9);

            RenderContext.Camera = new OrthographicCamera(new Vector3(4.5f + ((camSize.X - camSize.Y) / 2f) - 0.5f, -4.5f + 0.5f, 25))
            {
                Size = camSize,
                NearPlane = 1f,
                FarPlane = 100.0f,
                Facing = (float)Math.PI / 2,
                Pitch = -((float)(Math.PI / 2) - 0.00001f),
            };

            SceneContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "Ground",
                RelativeScale = new Vector3(50, 50, 1),
                RelativeTranslation = new Vector3(0f, 0f, -0.5f),
            }));

            SceneContext.AddActor(new Actor(new GridPlaneComponent(10, true)
            {
                Name = "GridPlaneXY",
                RelativeTranslation = new Vector3(0f, 0f, 0.01f),
            }));
            SceneContext.AddActor(new Actor(new CrossLineComponent(10, true)
            {
                Name = "CenterCross",
                RelativeTranslation = new Vector3(0f, 0f, 0.02f),
                RelativeScale = new Vector3(2.0f),
            }));

            var flowContainer = new UIFlowContainer()
            {
                DefaultChildSizes = new Vector2(0, 50),
                ExtraChildMargin = new UIAnchors(10, 10, 10, 0),
            };
            SceneContext.AddActor(new Actor(flowContainer));

            SceneContext.AddActor(new Actor(new StatsComponent()
            {
                Name = "Stats",
                CustomOrder = 10,
            }));

            SceneContext.AddActor(new Actor(new DirectionalLightComponent()
            {
                RelativeTranslation = new Vector3(2f, 0.5f, 3.25f),
                Name = "StaticLight",
            }));

            SceneContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "GroundCursor",
                RelativeTranslation = new Vector3(0, 1, 0.05f),
                RelativeScale = new Vector3(1.0f, 1.0f, 0.1f),
            }));

            SceneContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box1",
                RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
                RelativeScale = new Vector3(1),
                RelativeTranslation = new Vector3(0, 0, 0.5f),
            }));

            ModuleComponent mod;
            SceneContext.AddActor(new Actor(mod = new ModuleComponent(new Vector2i(200, 200))
            {
                Name = "Mod1",
                RelativeTranslation = new Vector3(2, 2, 0.05f),
                RelativeScale = new Vector3(4.0f, 6.0f, 0.1f),
            }));
            mod.AddKnob();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (CurrentMouseWorldPositionIsValid)
            {
                var cursor = SceneContext.GetActor("GroundCursor")?.RootComponent;
                if (cursor != null)
                    cursor.RelativeTranslation = new Vector3(CurrentMouseWorldPosition.X, CurrentMouseWorldPosition.Y, cursor.RelativeTranslation.Z);
            }
        }

    }
}
