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
        public static GameObject santa;
        public static GameObject santaAsset;
        public static GameObject embarkSanta;

        public static GameObject asturias;
        public static GameObject asturiasAsset;
        public static GameObject embarkAsturias;

        public static bool aoeInstalled;

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

                    if (santaAsset)
                    {
                        try
                        {
                            santa = UnityEngine.Object.Instantiate<GameObject>(santaAsset, __instance.transform);
                            embarkSanta = GameObject.Find("WALK boat santa maria");

                            Transform cols = GameObject.Find("walk cols").transform;

                            embarkSanta.transform.SetParent(cols);
                            embarkSanta.transform.localPosition = new Vector3(-450f, 0f, -200f);

                            // refresh all the shaders
                            UpdateShaders(santa.transform);

                            // rotate it at siren song
                            santa.transform.localRotation = Quaternion.Euler(0f, -104f, 0f);

                            // moor the ship up
                            Transform frontMoor = GameObject.Find("island 21 M (Siren Song)/dock_mooring M (3)").transform;
                            Transform backMoor = GameObject.Find("island 21 M (Siren Song)/dock_mooring M (2)").transform;

                            santa.GetComponent<BoatMooringRopes>().mooringFront = frontMoor;
                            santa.GetComponent<BoatMooringRopes>().mooringBack = backMoor;

                            Debug.LogWarning("[Age of Exploration] Santa Maria loaded");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[Age of Exploration] Could not load Santa Maria due to exception:\n{e}");
                        }
                    }

                    if (asturiasAsset)
                    {
                        try
                        {
                            asturias = UnityEngine.Object.Instantiate<GameObject>(asturiasAsset, __instance.transform);
                            embarkAsturias = asturias.transform.Find("WALK boat asturias").gameObject;

                            Transform cols = GameObject.Find("walk cols").transform;

                            embarkAsturias.transform.SetParent(cols);
                            embarkAsturias.transform.localPosition = new Vector3(-550f, 0f, -200f);

                            // refresh all shaders
                            UpdateShaders(asturias.transform);

                            // rotate it at eastwind
                            asturias.transform.localRotation = Quaternion.Euler(0f, 85f, 0f);

                            // attach custom doors
                            Transform ab = asturias.GetComponent<BoatRefs>().boatModel;
                            BoatEmbarkCollider em = ab.transform.Find("embark (player)").gameObject.GetComponent<BoatEmbarkCollider>();
                            
                            GPButtonCustomDoor td = asturias.transform.Find("boat asturias/structure_container/TRIM/gunport (1)").gameObject.AddComponent<GPButtonCustomDoor>();
                            td.angle = 135f;
                            td.importedActualBoat = ab;
                            td.embarkCol = em;
                            td.Setup();
                            
                            GPButtonCustomDoor td2 = asturias.transform.Find("boat asturias/structure_container/TRIM/gunport (2)").gameObject.AddComponent<GPButtonCustomDoor>();
                            td2.angle = 135f;
                            td2.importedActualBoat = ab;
                            td2.embarkCol = em;
                            td2.Setup();
                            
                            GPButtonCustomDoor door1 = asturias.transform.Find("boat asturias/structure_container/TRIM/door (1)").gameObject.AddComponent<GPButtonCustomDoor>();
                            door1.angle = 120f;
                            door1.importedActualBoat = ab;
                            door1.embarkCol = em;
                            door1.Setup();
                            
                            GPButtonCustomDoor door2 = asturias.transform.Find("boat asturias/structure_container/TRIM/door (2)").gameObject.AddComponent<GPButtonCustomDoor>();
                            door2.angle = 120f;
                            door2.importedActualBoat = ab;
                            door2.embarkCol = em;
                            door2.Setup();

                            // moor the ship up
                            Transform frontMoor = GameObject.Find("island 19 M (Eastwind)/dock_mooring M (1)").transform;
                            Transform backMoor = GameObject.Find("island 19 M (Eastwind)/dock_mooring M").transform;

                            asturias.GetComponent<BoatMooringRopes>().mooringFront = frontMoor;
                            asturias.GetComponent<BoatMooringRopes>().mooringBack = backMoor;

                            Debug.LogWarning("[Age of Exploration] Asturias loaded");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[Age of Exploration] Could not load Asturias due to exception:\n{e}");
                        }
                    }
                }
            }
        }

        private static void LoadAssets()
        {
            string dloc = Assembly.GetExecutingAssembly().Location;
            string path = Path.GetDirectoryName(dloc);

            // load .dll file `PreussenBridge.dll`
            //if (File.Exists(path + "\\PreussenBridge.dll"))
            //{
            //    Assembly.LoadFrom(path + "\\PreussenBridge.dll");
            //}

            // load assetbundle
            if (!File.Exists(Path.Combine(path, "exploration")))
            {
                Debug.LogError("Age of Exploration not installed correctly");
                aoeInstalled = false;
            }
            else
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(path, "exploration"));

                string prefab1 = "Assets/Exploration/BOAT SANTA MARIA (222).prefab";
                santaAsset = (bundle.LoadAsset(prefab1) as GameObject);
                

                string prefab2 = "Assets/Exploration/BOAT ASTURIAS (227).prefab";
                asturiasAsset = (bundle.LoadAsset(prefab2) as GameObject);

                aoeInstalled = true;
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

        [HarmonyPatch(typeof(Mast))]
        public static class SaveLoadManagerPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("AttachSailToMast")]
            public static void LoadGamePatch(Mast __instance)
            {
                if (__instance.shipRigidbody.name == "BOAT SANTA MARIA (222)(Clone)" || __instance.shipRigidbody.name == "BOAT ASTURIAS (227)(Clone)")
                {
                    // reset the shaders for the initial sails
                    UpdateShaders(__instance.transform);
                }
            }
        }
    }
}
