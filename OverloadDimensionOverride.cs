using HarmonyLib;
using LevelGeneration;

namespace GTFOR1Z1Mod;

[HarmonyPatch]
public class OverloadDimensionOverride
{

    private static LayerBuildData overloadData = null;
    private static bool overrideIsMainDimension = false;
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Dimension), nameof(Dimension.IsMainDimension), MethodType.Getter)]
    public static void MainDimPrefix(Dimension __instance, ref bool __result)
    {
        if (overrideIsMainDimension)
        {
            __result = true;
        }
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LG_Floor), nameof(LG_Floor.InjectJobs))]
    public static bool InjectJobsPrefix(LG_Floor __instance, eDimensionIndex dimensionIndex)
    {
        // Skipping overload
        LevelPlugin.PluginLogger.LogInfo("Inject Jobs called, attempting to remove overload from the list.");
        if (Builder.LayerBuildDatas.Count > 1)
        {
            for (int i = 0; i < Builder.LayerBuildDatas.Count; i++)
            {
                if (Builder.LayerBuildDatas[i].m_type == LG_LayerType.ThirdLayer)
                {
                    overloadData = Builder.LayerBuildDatas[i];
                    Builder.LayerBuildDatas.RemoveAt(i);
                    LevelPlugin.PluginLogger.LogInfo("Removed overload LayerBuildData.");
                }
            }
        }
        return true;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LG_Floor), nameof(LG_Floor.InjectJobs))]
    public static void InjectJobsPostfix(LG_Floor __instance, eDimensionIndex dimensionIndex)
    {
        // Add back overload
        if (overloadData != null && dimensionIndex == eDimensionIndex.Dimension_1 && Dimension.GetDimension(dimensionIndex, out var dimension))
        {
            LevelPlugin.PluginLogger.LogInfo($"Adding back overload and attempting to spawn it in {dimensionIndex}.");
            Builder.LayerBuildDatas.Add(overloadData);
            overloadData = null;
            overrideIsMainDimension = true;
            __instance.BuildLayer(Builder.LayerBuildDatas.Count - 1, dimension);
            overrideIsMainDimension = false;
        }
    }
}