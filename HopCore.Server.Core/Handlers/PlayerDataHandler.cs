using CitizenFX.Core;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;

namespace HopCore.Server.Core.Handlers {
    public sealed class PlayerDataHandler {

        private readonly Config _config;

        public PlayerDataHandler() {
            _config = Dependency.Inject<Config>();
        }

        public void OnPlayerJoin([FromSource] Player player) {
            player.TriggerEvent("core:client:config", _config);
        }

        public void OnPlayerQuit([FromSource] Player player) {
            //TODO: Save Player Data
        }

    }
}