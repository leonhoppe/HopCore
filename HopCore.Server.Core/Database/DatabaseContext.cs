using System;
using System.Data;
using CitizenFX.Core.Native;
using HopCore.Server.Database;
using HopCore.Server.Models;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;
using MySql.Data.MySqlClient;

namespace HopCore.Server.Core.Database {
    public sealed class DatabaseContext : IDatabaseContext {
        private readonly IDbConnection _connection;
        private readonly ILogger _logger;

        public DatabaseContext(string connectionString) {
            _logger = Dependency.Inject<ILogger>();
            _connection = new MySqlConnection(connectionString);
            var defaultQuery = API.LoadResourceFile(Server.CurrentResource, "default.sql");

            _logger.Database("Try database connection...");
            try {
                _connection.Open();
                _logger.Database("Connected.");
                
                /*_connection.Execute(defaultQuery);
                _logger.Database("Executed default query.");*/
            }
            catch (Exception e) {
                _logger.Fatal(e);
            }
            finally {
                _connection.Close();
            }
            
            PopulateTableStores();
            _logger.Database("Database initialized successfully.");
        }

        private void PopulateTableStores() {
            var props = GetType().GetProperties();

            foreach (var info in props) {
                var genericType = typeof(DbTable<>).MakeGenericType(info.PropertyType.GetGenericArguments());
                var instance = Activator.CreateInstance(genericType, _connection, info.Name, this);
                info.SetValue(this, instance);
                
                _logger.Database($"Initialized {info.Name} handler.");
            }
        }

        public IDbTable<PlayerData> Users { get; set; }
    }
}