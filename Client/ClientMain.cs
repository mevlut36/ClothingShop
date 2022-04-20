using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
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

        private void onClientStart(string resourceName)
        {
            var pedVariation = 0;
            var pedTexture = 0;
            if (GetCurrentResourceName() != resourceName) return;

            Dictionary<string, int> categories = new Dictionary<string, int>() {
                { "Chapeaux / Casquettes", 0 }, // affiche ca
                { "Lunettes", 1 },
                { "Masques", 2 },
                { "Sac", 5 },
                { "Vestes / Tee-shirt", 8 }, // affiche ca
                { "Bas (Jeans, Regular coupe skinny)", 4 }, // affiche ca
                { "Grôles", 6 }
            };

            RegisterCommand("shop", new Action<int, List<object>, string>((source, args, raw) => {
                Debug.WriteLine("ClothingUI called");
                var menu = new NativeMenu("Clothing Shop", "Acheter vos vêtements");

                foreach (KeyValuePair<string, int> entry in categories) {
                    List<int> drawingVariations = new List<int>();
                    for (int i = 0; i < GetNumberOfPedDrawableVariations(GetPlayerPed(-1), entry.Value); i++)
                        drawingVariations.Add(i);
                    List<int> textureVariations = new List<int>();
                    for (int i = 0; i < GetNumberOfPedTextureVariations(GetPlayerPed(-1), entry.Value, pedVariation); i++)
                        textureVariations.Add(i);
                    NativeListItem<int> title = new NativeListItem<int>(entry.Key, drawingVariations.ToArray());
                    title.ItemChanged += (sender, e) =>
                     {
                         // Pool.Remove(menu);
                         SetPedComponentVariation(GetPlayerPed(-1), entry.Value, pedVariation, pedTexture, 0);
                     };
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
