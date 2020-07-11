// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using OpenToolkit.Mathematics;

namespace Aximo.Audio.Rack.JsonModel
{
    public class JsModule
    {
        public int Id { get; set; }
        public Vector2i Position { get; set; }
        public string Name { get; set; }
        public List<JsParameter> Parameters { get; set; } = new List<JsParameter>();
        public JsRack SubRack { get; set; }
    }
}
