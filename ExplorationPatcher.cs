using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace exploration
{
    [BepInPlugin("com.winter.ageofexploration", "Age of Exploration", "0.2.0")]
    [BepInIncompatibility("com.winter.leopard")]
    public class ExplorationPatcher : BaseUnityPlugin
    {
        public const string pluginGuid = "com.winter.ageofexploration";
        public const string pluginName = "Age of Exploration";
        public const string pluginVersion = "0.2.0";

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "com.winter.ageofexploration");
        }
    }
}
