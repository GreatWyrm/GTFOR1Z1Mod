using System.Linq;
using BepInEx;
using BepInEx.Configuration;
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
    private ConfigEntry<bool> anticheatEnabled;
    
    public override void Load()
    {
        PluginLogger = Log;
        // Plugin startup logic
        Log.LogInfo("R1Z1 Level plugin loading...");
        
        anticheatEnabled = Config.Bind("R1Z1", "Anticheat", true, "Basic anticheat to prevent people from peeking too much into the level.");

        if (anticheatEnabled.Value)
        {
            _harmony.Patch(original: typeof(FreeflightCamera).GetMethod(nameof(FreeflightCamera.Update)), postfix: new HarmonyMethod(typeof(AnticheatPatches), nameof(AnticheatPatches.FreecamPostfix)));
        }
        
        _harmony.PatchAll();
        Log.LogInfo($"Patching successful with {_harmony.GetPatchedMethods().Count()} total patches.");
        Log.LogInfo("R1Z1 Level plugin loaded.");

        LevelAPI.OnLevelCleanup += RemoveSpecialModidiers;
        LevelAPI.OnLevelCleanup += CustomFailScreen.RestoreFailText;
        LevelAPI.OnBuildDone += SetupPuzzleTerminals.SetupPuzzleTerminalPasswords;
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
