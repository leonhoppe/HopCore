using System;
using System.Data;
using HopCore.Server.Core.Models;
using HopCore.Server.Database;
using HopCore.Server.Models;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;
using MySql.Data.MySqlClient;

namespace HopCore.Server.Core.Database {
    public sealed class DatabaseContext : IDbContext, IDisposable {
        private readonly IDbConnection _connection;
        private readonly ILogger _logger;

        public DatabaseContext(string connectionString) {
            _logger = Dependency.Inject<ILogger>();
            
            _connection = new MySqlConnection(connectionString);

            _logger.Database("Try database connection...");
            try {
                _connection.Open();
                _logger.Database("Connected.");
                
                Dependency.Inject<IDbContextPopulator>().PopulateTableStores(this);
                _logger.Database("Database initialized successfully.");
            }
            catch (Exception e) {
                _logger.Error(e);
                _logger.Fatal("Could not initialize database!");
            }
            finally {
                _connection.Close();
            }
        }

        public IDbConnection GetConnection() {
            return _connection;
        }

        public IDbTable<PlayerData> Users { get; set; }
        public IDbTable<DbLog> Logs { get; set; }

        public void Dispose() {
            _logger.Database("Disposing database connection...");
            _connection?.Dispose();
            _logger.Database("All connections disposed.");
        }
    }
}