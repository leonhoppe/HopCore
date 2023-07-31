using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using HopCore.Server.Core.Handlers;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;

namespace HopCore.Server.Core {
    public sealed class Server : BaseScript {

        private readonly Config _config;
        private readonly PlayerDataHandler _playerDataHandler;

        public Server() {
            _config = Dependency.Provide(new Config {
                Spawn = new[] { 258.7932f, -942.7487f, 29.3940f, 0.0f },
                DefaultChar = "a_m_m_bevhills_02"
            });

            _playerDataHandler = new PlayerDataHandler();
            EventHandlers["onResourceStart"] += new Action<string>(OnStart);
            EventHandlers["playerJoining"] += new Action<Player>(_playerDataHandler.OnPlayerJoin);
            EventHandlers["playerDropped"] += new Action<Player>(_playerDataHandler.OnPlayerQuit);
        }
        
        private async void OnStart(string name) {
            if (API.GetCurrentResourceName() != name) return;
            await Delay(2000);
            
            foreach (var player in Players) {
                _playerDataHandler.OnPlayerJoin(player);
            }
        }
        
    }
}