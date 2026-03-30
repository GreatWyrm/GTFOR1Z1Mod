using System.Linq;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GTFO.API;
using HarmonyLib;
using Player;
using SNetwork;

namespace GTFOR1Z1Mod;

[BepInPlugin("com.giginss.r1z1", "Giginss's R1Z1 Mod", "1.0.0")]
public class LevelPlugin : BasePlugin
{

    public static ManualLogSource PluginLogger;
    public static Harmony _harmony = new("com.giginss.r1z1");
    
    public override void Load()
    {
        PluginLogger = Log;
        // Plugin startup logic
        Log.LogInfo("R1Z1 Level plugin loading...");
        
        _harmony.PatchAll();
        Log.LogInfo($"Patching successful with {_harmony.GetPatchedMethods().Count()} total patches.");
        Log.LogInfo("R1Z1 Level plugin loaded.");

        LevelAPI.OnLevelCleanup += RemoveSpecialModidiers;
    }

    private static void RemoveSpecialModidiers()
    {
        if (SNet.IsMaster)
        {
            foreach (var playerAgent in PlayerManager.PlayerAgentsInLevel)
            {
                AgentModifierManager.ClearAllModifiersOnAgent(playerAgent);
            }
        }
    }
}
