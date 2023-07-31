using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace HopCore.Client {
    public static class Util {
        public static IEnumerable<T> GetEnumValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();
    }
}