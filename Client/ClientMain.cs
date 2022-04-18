using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using LemonUI;
using LemonUI.Menus;
using static CitizenFX.Core.Native.API;

namespace ClothingUI.Client {
    public class ClientMain : BaseScript {
        public ObjectPool Pool = new ObjectPool();

        public ClientMain() {
            Debug.WriteLine("Hi from ClothingUI.Client!");
            EventHandlers["onClientResourceStart"] += new Action<string>(onClientStart);
        }

        private void onClientStart(string resourceName) {
            if (GetCurrentResourceName() != resourceName) return;

            RegisterCommand("shop", new Action<int, List<object>, string>((source, args, raw) => {
                Debug.WriteLine("ClothingUI called");
                var menu = new NativeMenu("Clothing Shop", "Acheter vos vêtements");
                var categories = new Dictionary<string, List<string>>()
                {
                    { "Chapeaux / Casquettes", new List<string>(){"id", "id", "id" } },
                    { "Masques", new List<string>(){ "id", "id", "id" } },
                    { "Lunettes", new List<string>(){ "id", "id", "id" } },
                    { "Vestes / Tee-shirt", new List<string>(){ "id", "id", "id" } },
                    { "Bas (Jeans, Regular coupe skinny)", new List<string>(){ "id", "id", "id" } },
                    { "Grôles", new List<string>(){ "id", "id", "id" } }
                };

                foreach (KeyValuePair<string, List<string>> entry in categories) {
                    var title = new NativeItem(entry.Key);
                    menu.Add(title);
                }
                Pool.Add(menu);
                menu.Visible = true;
            }), false);
        }

        [Tick]
        public Task OnTick() {
            Pool.Process();
            return Task.FromResult(0);
        }
    }
}