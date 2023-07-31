using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace HopCore.Shared.Serialisation {
    public static class Serializer {
        
        public static T Convert<T>(ExpandoObject prop, string variable) {
            var baseType = typeof(T);
            
            var dict = (IDictionary<string, object>)prop;
            return (T)Convert(dict[variable], baseType);
        }
        
        public static List<T> ConvertList<T>(ExpandoObject prop, string variable) {
            var dict = (IDictionary<string, object>)prop;
            return (List<T>)Convert(dict[variable], typeof(T));
        }
        
        public static T[] ConvertArray<T>(ExpandoObject prop, string variable) {
            return ConvertList<T>(prop, variable).ToArray();
        }
        
        private static object Convert(object prop, Type generic) {
            var type = prop.GetType();

            if (type == typeof(List<object>)) {
                var rawList = prop as IList ?? new List<object>();
                
                var listType = typeof(List<>).MakeGenericType(generic);
                var list = Activator.CreateInstance(listType) as IList ?? new List<object>();

                foreach (var element in rawList) {
                    list.Add(Convert(element, generic));
                }

                return list;
            }

            if (generic.IsAssignableFrom(typeof(IPacket))) {
                var packet = (IPacket)Activator.CreateInstance(generic);
                packet.LoadData(prop as ExpandoObject);
                return packet;
            }

            return System.Convert.ChangeType(prop, generic);
        }
        
    }
}