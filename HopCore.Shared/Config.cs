using System.Dynamic;
using HopCore.Shared.Serialisation;

namespace HopCore.Shared {
    public sealed class Config : IPacket {
        public float[] Spawn { get; set; }
        public string DefaultChar { get; set; }
        public bool Debug { get; set; }

        public void LoadData(ExpandoObject data) {
            Spawn = Serializer.ConvertArray<float>(data, nameof(Spawn));
            DefaultChar = Serializer.Convert<string>(data, nameof(DefaultChar));
            Debug = Serializer.Convert<bool>(data, nameof(Debug));
        }
    }
}