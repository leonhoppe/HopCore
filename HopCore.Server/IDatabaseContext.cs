using HopCore.Server.Database;
using HopCore.Server.Models;

namespace HopCore.Server {
    public interface IDatabaseContext {
        public IDbTable<PlayerData> Users { get; set; }
    }
}