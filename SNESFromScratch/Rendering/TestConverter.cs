using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SNESFromScratch.AudioProcessing;
using SNESFromScratch.CentralMemory;
using SNESFromScratch.CPU;
using SNESFromScratch.IO;
using SNESFromScratch.PictureProcessing;
using SNESFromScratch.ROM;
using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.Rendering
{
    public class TestConverter : JsonConverter
    {
        private readonly Dictionary<Type, Type> _mapper = new Dictionary<Type, Type>
        {
            {typeof(ISNESSystem), typeof(SNESSystem.SNESSystem)},
            {typeof(ICPU), typeof(C65816)},
            {typeof(IPPU), typeof(PPU)},
            {typeof(IROM), typeof(ROM.ROM)},
            {typeof(IIO), typeof(IO.IO)},
            {typeof(IDMA), typeof(DMA)},
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
