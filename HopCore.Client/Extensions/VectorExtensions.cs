using CitizenFX.Core;

namespace HopCore.Client.Extensions {
    public static class VectorExtensions {

        public static Vector3 ToVector3(this Vector4 vector4) => new Vector3(vector4.X, vector4.Y, vector4.Z);
        
        public static Vector3 ToVector3(this float[] data) => new Vector3(data);
        public static Vector4 ToVector4(this float[] data) => new Vector4(data);

    }
}