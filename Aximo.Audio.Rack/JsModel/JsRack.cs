using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenToolkit.Mathematics;

namespace Aximo.Audio.Rack.JsonModel
{

    public class JsRack
    {
        public string Version { get; set; }
        public List<JsModule> Modules { get; set; } = new List<JsModule>();
        public List<JsCable> Cables { get; set; } = new List<JsCable>();

        public void SaveToFile(string filePath)
        {
            File.WriteAllText(filePath, ToString());
        }

        public class CustomContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                if (IsIgnored(member))
                {
                    property.ShouldSerialize = i => false;
                    property.Ignored = true;
                }

                return property;
            }

            private bool IsIgnored(MemberInfo member)
            {
                if (member.DeclaringType == typeof(Vector2i))
                    return member.Name != "X" && member.Name != "Y";

                return false;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new CustomContractResolver(),
            });
        }

        public static JsRack Parse(string json)
        {
            return JsonConvert.DeserializeObject<JsRack>(json);
        }

        public static JsRack LoadFile(string filePath)
        {
            return Parse(File.ReadAllText(filePath));
        }
    }
}
