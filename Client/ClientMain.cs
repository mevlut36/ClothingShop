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

            RegisterCommand("menuskin", new Action<int, List<object>, string>((source, args, raw) => {
                var cam = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
                var coordsCam = GetOffsetFromEntityInWorldCoords(PlayerPedId(), (float)0.0, (float)0.5, (float)0.65);
                var coordsPly = GetEntityCoords(PlayerPedId(), true);
                SetCamCoord(cam, coordsCam.X, coordsCam.Y, coordsCam.Z);
                PointCamAtCoord(cam, coordsPly.X, coordsPly.Y, (float)(coordsPly.Z + 0.65));
                SetCamActive(cam, true);
                RenderScriptCams(true, true, 500, true, true);
                DisplayRadar(false);
                // RenderScriptCams(false, false, 0, true, true); Grosse erreur de mettre en true
                var menu = new NativeMenu("Vetements");
                var hautPedVariations = GetNumberOfPedTextureVariations(GetPlayerPed(-1), 11, 1);
                var basPedVariations = GetNumberOfPedTextureVariations(GetPlayerPed(-1), 4, 1);
                var shoesPedVariations = GetNumberOfPedTextureVariations(GetPlayerPed(-1), 6, 1);
                var hautList = new List<int>();
                var basList = new List<int>();
                var shoesList = new List<int>();
                foreach (int value in Enumerable.Range(1, hautPedVariations))
                {
                    hautList.Add(value);
                }
                foreach (int value in Enumerable.Range(1, hautPedVariations))
                {
                    basList.Add(value);
                }
                foreach (int value in Enumerable.Range(1, hautPedVariations))
                {
                    shoesList.Add(value);
                }
                var listHaut = new NativeListItem<int>("Haut", hautList.ToArray());
                listHaut.ItemChanged += (sender, e) =>
                {
                    SetPedComponentVariation(GetPlayerPed(-1), 11, e.Index - 1, 1, 2);
                };
                var listBas = new NativeListItem<int>("Bas", basList.ToArray());
                listBas.ItemChanged += (sender, e) =>
                {
                    SetPedComponentVariation(GetPlayerPed(-1), 4, e.Index - 1, 1, 2);
                };
                var listShoes = new NativeListItem<int>("Chaussures", shoesList.ToArray());
                listShoes.ItemChanged += (sender, e) =>
                {
                    SetPedComponentVariation(GetPlayerPed(-1), 6, e.Index - 1, 1, 2);
                };
                menu.Add(listHaut);
                menu.Add(listBas);
                menu.Add(listShoes);
                menu.UseMouse = false;
                Pool.Add(menu);
                menu.Visible = true;
                menu.Closing += (sender, e) =>
                {
                    RenderScriptCams(false, true, 500, true, true);
                    DisplayRadar(true);
                };
            }), false);
        }

        [Tick]
        public Task OnTick() {
            Pool.Process();
            return Task.FromResult(0);
        }
    }
}
