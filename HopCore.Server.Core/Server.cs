using System;
using System.IO;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using HopCore.Server.Core.Database;
using HopCore.Server.Core.Handlers;
using HopCore.Server.Database;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;

namespace HopCore.Server.Core {
    public sealed class Server : BaseScript {
        public static string CurrentResource { get; private set; }
        
        private readonly Config _config;
        private readonly ILogger _logger;
        private readonly PlayerDataHandler _playerDataHandler;

        public Server() {
            _config = Dependency.Provide(new Config {
                Spawn = new[] { 258.7932f, -942.7487f, 29.3940f, 0.0f },
                DefaultChar = "a_m_m_bevhills_02",
                Debug = true
            });
            var logger = new Logger();
            
            try {
                _logger = Dependency.Provide<ILogger, Logger>(logger);
                _logger.Debug("initializing HopCore...");
            
                CurrentResource = API.GetCurrentResourceName();
                Environment.CurrentDirectory = Path.GetFullPath(API.GetResourcePath(CurrentResource));

                #region Database INIT
                    var conString = API.GetResourceMetadata(CurrentResource, "database_connection", 0);
                    Dependency.Provide<IDbContextPopulator, DbContextPopulator>();
                    
                    var context = new DatabaseContext(conString);
                    logger.Context = context;
                    
                    Dependency.Provide<IDbContext, DatabaseContext>(context);
                #endregion
                
                _logger.Debug("All dependencies provided.");

                #region EventHandling
                    _playerDataHandler = new PlayerDataHandler();
                    EventHandlers["onResourceStart"] += new Action<string>(OnStart);
                    EventHandlers["playerJoining"] += new Action<Player>(_playerDataHandler.OnPlayerJoin);
                    EventHandlers["playerDropped"] += new Action<Player>(_playerDataHandler.OnPlayerQuit);
                #endregion
            
            }
            catch (Exception e) {
                logger.Error(e);
            }
            
            if (logger.Errors == 0)
                logger.Debug("HopCore started successfully.");
            else logger.Fatal("HopCore started with some errors!");
        }

        ~Server() {
            _logger.Debug("Shutting down HopCore...");
            Dependency.DisposeDependencies();
            _logger.Debug("Shutdown successfull.");
        }
        
        private async void OnStart(string name) {
            if (CurrentResource != name) return;
            if (!Players.Any()) return;
            await Delay(2000);
            
            foreach (var player in Players) {
                _playerDataHandler.OnPlayerJoin(player);
            }
            
            _logger.Debug("Initialized all users");
        }
        
    }
}