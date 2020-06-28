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
