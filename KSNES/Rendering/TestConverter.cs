using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using KSNES.AudioProcessing;
using KSNES.CPU;
using KSNES.PictureProcessing;
using KSNES.SNESSystem;

namespace KSNES.Rendering
{
    public class TestConverter : JsonConverter
    {
        private readonly Dictionary<Type, Type> _mapper = new Dictionary<Type, Type>
        {
            {typeof(ISNESSystem), typeof(SNESSystem.SNESSystem)},
            {typeof(ICPU), typeof(CPU.CPU)},
            {typeof(IPPU), typeof(PPU)},
            {typeof(IAPU), typeof(APU)},
            {typeof(ISPC700), typeof(SPC700)},
            {typeof(IDSP), typeof(DSP)},
        };

        public override bool CanConvert(Type objectType)
        {
            return _mapper.ContainsKey(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, _mapper[objectType]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}