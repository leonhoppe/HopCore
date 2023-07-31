#pragma warning disable CS1998

using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using HopCore.Client.Core.Handlers;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;
using HopCore.Shared.Serialisation;

namespace HopCore.Client.Core {
    public sealed class Client : BaseScript {

        private Config _config;

        public Client() {
            EventHandlers["core:client:config"] += SmartEvent.Create<Config>(LoadConfig);
        }

        private void LoadConfig(Config config) {
            _config = Dependency.Provide(config);

            var conHandler = new ConnectionHandler();
            conHandler.OnJoin();

            Tick += OnTick;
        }

        private async Task OnTick() {
            API.RestorePlayerStamina(Game.Player.Handle, 1.0f);
            API.DisableIdleCamera(true);
            Game.Player.WantedLevel = 0;
        }
        
    }
}