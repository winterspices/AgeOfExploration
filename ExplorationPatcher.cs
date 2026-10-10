using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace exploration
{
    [BepInPlugin("com.winter.ageofexploration", "Age of Exploration", "0.1.2")]
    public class ExplorationPatcher : BaseUnityPlugin
    {
        public const string pluginGuid = "com.winter.ageofexploration";
        public const string pluginName = "Age of Exploration";
        public const string pluginVersion = "0.1.2";

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "com.winter.ageofexploration");
        }
    }
}
