using System.Data;
using HopCore.Server.Models;

namespace HopCore.Server.Database {
    public interface IDbContext {
        IDbConnection GetConnection();
        
        IDbTable<PlayerData> Users { get; set; }
    }
}