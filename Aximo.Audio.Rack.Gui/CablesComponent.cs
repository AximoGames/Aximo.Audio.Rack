// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aximo.Audio.Rack.Gui;
using Aximo.Engine.Audio;
using Aximo.Render.OpenGL;
using Aximo.VertexData;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Components.Geometry
{
    public class CablesComponent : StaticMeshComponent
    {

        public CablesComponent()
        {
            Material = MaterialManager.CreateDefaultScreenLineMaterial();
            Transform = TransformUtil.TransformScreenUnitsToScreenSpace(RackApplication.AudioUnits);
        }

        private PortComponent GetPortComponent(Port port) => SceneContext.Current.Find<PortComponent>().Where(c => c.Port == port).FirstOrDefault();

        public void SetCables(IEnumerable<AudioCable> cables)
        {
            if (cables == null || !cables.Any())
            {
                // SetMesh(null);
                return; // TODO: Hide
            }

            var vertices = new List<VertexDataPosColor>();

            foreach (var cable in cables)
            {
                foreach (var port in cable.Ports)
                {
                    var p = GetPortComponent(port);
                    if (p != null)
                        vertices.Add(p.AbsoluteCenter.ToVector3(), new Vector4(1, 1, 0, 1));
                }
            }

            SetMesh(Mesh.CreateFromVertices(vertices.ToArray(), null, MeshFaceType.Line));
        }

    }
}
