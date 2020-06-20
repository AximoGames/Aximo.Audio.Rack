using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aximo.Audio.Rack.JsonModel
{
    public class JsFile
    {
        public string Version { get; set; }
        public List<JsModule> Modules { get; set; } = new List<JsModule>();
        public List<JsCable> Cables { get; set; } = new List<JsCable>();

        public void SaveToFile(string filePath)
        {
            File.WriteAllText(filePath, ToString());
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static JsFile Parse(string json)
        {
            return JsonConvert.DeserializeObject<JsFile>(json);
        }

        public static JsFile LoadFile(string filePath)
        {
            return Parse(File.ReadAllText(filePath));
        }
    }
}
