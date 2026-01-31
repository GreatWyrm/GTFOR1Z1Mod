using CellMenu;
using HarmonyLib;
using Player;
using SNetwork;
using UnityEngine;

namespace GTFOR1Z1Mod;

[HarmonyPatch]
public class PageMapPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CM_PageMap), nameof(CM_PageMap.UpdatePlayerData))]
    public static void SetMapVisualsPostfix(CM_PageMap __instance)
    {
        var agent = PlayerManager.Current.m_localPlayerAgentInLevel;
        if (agent != null && agent.DimensionIndex != eDimensionIndex.Reality)
        {
            __instance.SetMapDisconnetedTextIsActive(false);
            __instance.SetMapVisualsIsActive(true);
            // Manually re-add player nav markers
            foreach (var snetPlayer in SNet.Slots.SlottedPlayers)
            {
                if (snetPlayer.HasPlayerAgent)
                {
                    __instance.UpdateNavMarker(snetPlayer);
                    __instance.UpdateMarker(snetPlayer, snetPlayer.PlayerSlotIndex());
                }
            }
        }
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.Update))]
    public static void EditVisibilityConePostfix(PlayerAgent __instance)
    {
        if (__instance.m_isSetup && __instance.m_dimensionIndex != eDimensionIndex.Reality)
        {
            GameObject modifiedCone = new GameObject();
            modifiedCone.transform.rotation = __instance.m_mapVisibilityTrans.rotation;
            Vector3 position = __instance.m_mapVisibilityTrans.position;
            modifiedCone.transform.localScale = __instance.m_mapVisibilityTrans.localScale;
            // Shift to Reality
            position.y -= __instance.Dimension.GroundY;
            modifiedCone.transform.position = position;
        
            MapDetails.AddVisiblityCone(modifiedCone.transform, __instance.IsLocallyOwned ? MapDetails.VisibilityLayer.LocalPlayer : MapDetails.VisibilityLayer.OtherPlayer);
        }
    }
}