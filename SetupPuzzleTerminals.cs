using System;
using System.Collections.Generic;
using System.Linq;
using GameData;
using LevelGeneration;

namespace GTFOR1Z1Mod;

public class SetupPuzzleTerminals
{

    private static readonly eDimensionIndex TargetDimension = eDimensionIndex.Dimension_2;
    private static readonly eLocalZoneIndex TargetZoneIndex = eLocalZoneIndex.Zone_5;
    
    private static readonly Dictionary<string, string> HintToSolutionDict = new()
    {
        { "Two power cells. Two goals. Two great enemies guard the final gate.", "DOWNWARDS" },
        { "The same layout, again and again. The hunt for the cube. The enemies vary.", "CRYPTOMNESIA" },
        { "Your weapons empty. A world of glue. An inadvertent lockdown.", "RECKLESS" },
        { "A forced reactor. The hardest challenge. The timer ticks down." , "RELEASE" },
        { "A long trek. Enemies in pursuit. All in vain.", "VALIANT" }, // Weebneet
        { "The effort of a dead man. A microdrive gifted. Magnetism awaits.", "UNPLUGGED" },
        { "Back and forth for an unstable Reactor. Chased to the end by 3 titans.", "ERROR" }, // Weebneet
        { "A cluster in fog. An arena for battle. The reactor warms up.", "ACCESS" },
        { "An unkillable foe. The dimensions twist. No time for passwords.", "CHAOS" },
        { "A sudden stop. An endless assault. A trojan deployed.", "AWOL" },
        { "The objective corrupts. The 7 ids. Dive into the fog.", "???" },
        { "A surge of enemies. A cradle reached. Carry it to the end.", "CRIB" },
        { "Collect and collect. Making a long trek. The sphere in the darkness.", "PABULUM" },
        { "An endless alarm, repeated again. A cell ferried, and a command to end.", "KDS DEEP" },
        { "The lights unstable. A password to fix it. The room of an ally.", "FLUX" },
        { "A reactor on, a reactor off. Two endless terrible titans. The fog steadily rises.", "POWER HUNGRY"},
        { "Connection with your others. The coordinates obtained. Progress towards a better world.", "LINK" },
    };

    public static void SetupPuzzleTerminalPasswords()
    {
        var list = GetPuzzleTerminals();
        if (list.Count == 0)
        {
            LevelPlugin.PluginLogger.LogError($"No puzzle terminals found.");
            return;
        }
        
        // Remove first and last terminal
        list.RemoveAt(0);
        list.RemoveAt(list.Count - 1);
        
        Dictionary<string, string> dict = new Dictionary<string, string>(HintToSolutionDict);
        Random random = new(Builder.SessionSeedRandom.Seed);
        foreach (var terminal in list)
        {
            if (dict.Count == 0)
            {
                break;
            }
            var entry = dict.ElementAt(random.Next(0, dict.Count));
            dict.Remove(entry.Key);
            terminal.LockWithPassword(entry.Value, "<color=orange>" + entry.Key + "</color>");
        }
    }

    private static List<LG_ComputerTerminal> GetPuzzleTerminals()
    {
        if (!Dimension.GetDimension(TargetDimension, out var dimension))
        {
            LevelPlugin.PluginLogger.LogError($"No dimension with index {TargetDimension} found.");
            return new List<LG_ComputerTerminal>();
        }
        var zone = dimension.MainLayer.m_zonesByLocalIndex[TargetZoneIndex];
        if (zone == null)
        {
            LevelPlugin.PluginLogger.LogError($"Dimension {TargetDimension} did not have a zone with index {TargetZoneIndex}.");
            return new List<LG_ComputerTerminal>();
        }

        List<LG_ComputerTerminal> list = new List<LG_ComputerTerminal>();
        foreach (var item in zone.TerminalsSpawnedInZone)
        {
            list.Add(item);
        }
        return list;
    }
}