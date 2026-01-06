using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GameData;
using GTFO.API;
using HarmonyLib;
using SNetwork;
using UnityEngine;

namespace GTFOR1Z1Mod;

[BepInPlugin("com.giginss.r1z1", "Giginss's R1Z1 Mod", "1.0.0")]
public class LevelPlugin : BasePlugin
{

    public static ManualLogSource PluginLogger;
    private static Harmony _harmony = new Harmony("com.giginss.r1z1");
    
    public override void Load()
    {
        PluginLogger = Log;
        // Plugin startup logic
        Log.LogInfo("R1Z1 Level plugin loading...");
        
        _harmony.PatchAll();
        Log.LogInfo("R1Z1 Level plugin loaded.");
    }
}
