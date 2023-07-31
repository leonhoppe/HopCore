using System;
using System.Collections.Generic;
using System.Dynamic;

namespace HopCore.Shared.Serialisation {
    public static class SmartEvent {
        
        public static Action<ExpandoObject> Create<T1>(Action<T1> handler)
            where T1 : IPacket, new() {
            return ex1 => {
                handler.Invoke(ConvertToPacket<T1>(ex1));
            };
        }
        
        public static Action<List<object>> CreateList<T1>(Action<List<T1>> handler)
            where T1 : IPacket, new() {
            return ex1 => {
                handler.Invoke(ConvertToPacketList<T1>(ex1));
            };
        }

        public static Action<ExpandoObject, ExpandoObject> Create<T1, T2>(Action<T1, T2> handler)
            where T1 : IPacket, new() 
            where T2 : IPacket, new() {
            return (ex1, ex2) => {
                handler.Invoke(ConvertToPacket<T1>(ex1), ConvertToPacket<T2>(ex2));
            };
        }
        
        public static Action<ExpandoObject, ExpandoObject, ExpandoObject> Create<T1, T2, T3>(Action<T1, T2, T3> handler)
            where T1 : IPacket, new()
            where T2 : IPacket, new()
            where T3 : IPacket, new() {
            return (ex1, ex2, ex3) => {
                handler.Invoke(ConvertToPacket<T1>(ex1), ConvertToPacket<T2>(ex2), ConvertToPacket<T3>(ex3));
            };
        }
        
        private static T ConvertToPacket<T>(ExpandoObject data) where T : IPacket, new() {
            var packet = new T();
            packet.LoadData(data);
            return packet;
        }
        
        private static List<T> ConvertToPacketList<T>(List<object> data) where T : IPacket, new() {
            var packets = new List<T>();

            foreach (var element in data) {
                var packet = new T();
                packet.LoadData((ExpandoObject)element);
                packets.Add(packet);
            }

            return packets;
        }

    }
}