using System;
using CitizenFX.Core;

namespace HopCore.Server.Extensions {
    public static class PlayerExtensions {
        
        public static int ServerId(this Player player) => Convert.ToInt32(player.Handle);
        
    }
}