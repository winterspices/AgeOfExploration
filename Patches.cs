using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace exploration
{
    internal class Patches
    {
        public static GameObject ship;
        public static GameObject santa;
        public static GameObject embarkSanta;

        public static bool explorationInstalled;

        public static float maxPop;
        public static List<float> pops = new List<float>();

        [HarmonyPatch(typeof(FloatingOriginManager))]
        public static class FloatingOriginManagerPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("Start")]
            public static void StartPatch(FloatingOriginManager __instance)
            {
                if (__instance.name == "_shifting world")
                {
                    LoadAssets();

                    if (santa)
                    {
                        try
                        {
                            ship = UnityEngine.Object.Instantiate<GameObject>(santa, __instance.transform);
                            embarkSanta = GameObject.Find("WALK boat santa maria");

                            Transform transform = GameObject.Find("walk cols").transform;

                            MatLib.RegisterMaterials();

                            embarkSanta.transform.SetParent(transform);
                            embarkSanta.transform.localPosition = new Vector3(-450f, 0f, -200f);

                            ship.transform.Find("boat santa maria/mask water").GetComponent<MeshRenderer>().material = MatLib.convexHull;
                            ship.transform.Find("WaterObjectInteractionSphereBack").GetComponent<MeshRenderer>().material = MatLib.objectInteraction;
                            ship.transform.Find("WaterObjectInteractionSphereFront").GetComponent<MeshRenderer>().material = MatLib.objectInteraction;
                            ship.transform.Find("water foam").GetComponent<MeshRenderer>().material = MatLib.foam;
                            ship.transform.Find("boat santa maria/damage_water").GetComponent<MeshRenderer>().material = MatLib.water4;
                            //ship.transform.Find("boat preussen/mask splash").GetComponent<MeshRenderer>().material = MatLib.splash;
                            //ship.transform.Find("boat preussen/mask splash rear").GetComponent<MeshRenderer>().material = MatLib.splash;

                            ship.transform.localRotation = Quaternion.Euler(0f, 326.8f, 0f);

                            // refresh all the shaders
                            UpdateShaders(ship.transform);

                            // rotate it at siren song
                            ship.transform.localRotation = Quaternion.Euler(0f, -104f, 0f);

                            // moor the ship up
                            Transform frontMoor = GameObject.Find("island 21 M (Siren Song)/dock_mooring M (3)").transform;
                            Transform backMoor = GameObject.Find("island 21 M (Siren Song)/dock_mooring M (2)").transform;

                            ship.GetComponent<BoatMooringRopes>().mooringFront = frontMoor;
                            ship.GetComponent<BoatMooringRopes>().mooringBack = backMoor;

                            Debug.LogWarning("[Age of Exploration] Santa Maria loaded");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[Age of Exploration] Could not load Santa Maria due to exception:\n{e}");
                        }
                    }
                }
            }
        }

        private static void LoadAssets()
        {
            string path = Paths.PluginPath + "\\AgeOfExploration";

            // load .dll file `PreussenBridge.dll`
            //if (File.Exists(path + "\\PreussenBridge.dll"))
            //{
            //    Assembly.LoadFrom(path + "\\PreussenBridge.dll");
            //}

            // load assetbundle
            if (!File.Exists(path + "\\exploration"))
            {
                Debug.LogError("Age of Exploration not installed correctly");
                explorationInstalled = false;
            }
            else
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(path + "\\exploration");
                string prefab1 = "Assets/Exploration/BOAT SANTA MARIA (222).prefab";
                santa = (bundle.LoadAsset(prefab1) as GameObject);
                explorationInstalled = true;
            }
        }

        private static void UpdateShaders(Transform root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    if (material != null && material.shader != null)
                    {
                        if (material.name != "glass (Instance)")
                        {
                            material.shader = Shader.Find(material.shader.name);
                        }
                    }
                }
            }
        }
    }
}
