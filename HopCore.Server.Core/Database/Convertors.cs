using System;
using System.Linq;
using CitizenFX.Core;
using HopCore.Server.Database;

namespace HopCore.Server.Core.Database {
    internal sealed class FloatArrayConvertor : TypeConvertor<float[]> {
        public override string ConvertForDatabase(float[] obj) {
            return string.Join(";", obj);
        }

        public override float[] ConvertFromDatabase(string raw) {
            var split = raw.Split(';');
            return split.Select(Convert.ToSingle).ToArray();
        }
    }
    
    internal sealed class Vector3Convertor : TypeConvertor<Vector3> {
        private readonly FloatArrayConvertor _convertor = new FloatArrayConvertor();

        public override string ConvertForDatabase(Vector3 obj) {
            return _convertor.ConvertForDatabase(obj.ToArray());
        }

        public override Vector3 ConvertFromDatabase(string raw) {
            return new Vector3(_convertor.ConvertFromDatabase(raw));
        }
    }
    
    internal sealed class Vector4Convertor : TypeConvertor<Vector4> {
        private readonly FloatArrayConvertor _convertor = new FloatArrayConvertor();

        public override string ConvertForDatabase(Vector4 obj) {
            return _convertor.ConvertForDatabase(obj.ToArray());
        }

        public override Vector4 ConvertFromDatabase(string raw) {
            return new Vector4(_convertor.ConvertFromDatabase(raw));
        }
    }
}