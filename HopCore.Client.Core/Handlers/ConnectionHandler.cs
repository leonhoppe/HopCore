using CitizenFX.Core;
using HopCore.Client.Extensions;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;

namespace HopCore.Client.Core.Handlers {
    public class ConnectionHandler {

        public async void OnJoin() {
            var config = Dependency.Inject<Config>();
            var spawn = config.Spawn.ToVector4();

            var model = new Model(config.DefaultChar);
            await model.Load();
            await Game.Player.ChangeModel(model);

            var character = Game.PlayerPed;
            character.Position = spawn.ToVector3();
            character.Heading = spawn.W;
        }

    }
}