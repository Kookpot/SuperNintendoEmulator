using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using KSNES.AudioProcessing;
using KSNES.PictureProcessing;

namespace KSNES.Rendering
{
    public class CustomContractResolver : DefaultContractResolver
    {
        private readonly List<Type> _mapper = new List<Type>
        {
            typeof(SNESSystem.SNESSystem), typeof(CPU.CPU), typeof(PPU), typeof(APU), typeof(SPC700), typeof(DSP)
        };

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            List<MemberInfo> result = base.GetSerializableMembers(objectType);
            if (_mapper.Contains(objectType))
            {
                IEnumerable<FieldInfo> memberInfo = objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => !x.Name.Contains("BackingField") && !x.GetCustomAttributes(typeof(JsonIgnoreAttribute)).Any() && 
                    x.FieldType != typeof(EventHandler));

                result.AddRange(memberInfo);
            }
            return result;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            if (_mapper.Contains(type))
            {
                foreach (JsonProperty prop in properties)
                {
                    prop.Readable = true;
                    prop.Writable = true;
                }
            }
            return properties;
        }
    }
}