using System.Threading.Tasks;
using CitizenFX.Core;

namespace HopCore.Client.Extensions {
    public static class ModelExtensions {

        public static async Task Load(this Model model) {
            model.Request();

            if (!model.IsLoaded)
                await BaseScript.Delay(0);
        }
        
    }
}