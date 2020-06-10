// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Windows;
using OpenToolkit.Mathematics;

namespace Aximo.Audio.Rack.Gui
{
    public class KnobComponent : StaticMeshComponent
    {
        public KnobComponent() : base()
        {
            TranslationMatrix = RackApplication.BoardTranslationMatrix;

            Material = new Material
            {
                Color = new Vector4(1, 1, 0, 1),
            };
            var mesh = Mesh.CreateCylinder();
            var marker = Mesh.CreateCube();
            marker.Scale(new Vector3(0.1f, 0.5f, 1.1f));
            marker.TranslateY(0.3f);
            mesh.AddMesh(marker, 1);

            SetMesh(mesh);

            IsAbsoluteScale = true;
            IsAbsoluteRotation = true;
            IsAbsoluteTranslation = true;

            RelativeTranslation = new Vector3(2, 2, 0);
            RelativeRotation = new Quaternion(0, 0, 0.2f);
        }
    }
}
