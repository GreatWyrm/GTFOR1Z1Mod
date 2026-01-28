using GameData;
using LevelGeneration;
using Player;
using UnityEngine;
using GTFO.API.Utilities;

namespace GTFOR1Z1Mod;

public class WardenEventProcessor
{
    public static void Event70(WardenObjectiveEventData data)
    {
        // Validate Dimensions
        if (!Dimension.GetDimension(data.DimensionIndex, out var targetDimension))
        {
            return;
        }


        PlayerAgent player = PlayerManager.GetLocalPlayerAgent();
        if (!Dimension.GetDimension(player.DimensionIndex, out var playerDimension))
        {
            return;
        }

        LocalPlayerAgent localPlayer = player as LocalPlayerAgent;
        Vector3 forwardLook = localPlayer == null ? player.TargetLookDir : localPlayer.FPSCamera.Forward;
        float yDifference = targetDimension.GroundY - playerDimension.GroundY;
        Vector3 vec = new Vector3();
        vec.x = player.transform.position.x;
        vec.y = player.transform.position.y + yDifference;
        vec.z = player.transform.position.z;
        player.RequestWarpToSync(data.DimensionIndex, vec, forwardLook, PlayerAgent.WarpOptions.None);
    }

    struct Event71Data
    {
        public Event71Data(eDimensionIndex startDim, eDimensionIndex targetDim, eLocalZoneIndex zone)
        {
            startDimIndex = startDim;
            targetDimIndex = targetDim;
            zoneIndex = zone;
        }
        
        public eDimensionIndex startDimIndex;
        public eDimensionIndex targetDimIndex;
        public eLocalZoneIndex zoneIndex;
    }
    
    public static void Event71(WardenObjectiveEventData data)
    {
        var playerAgent = PlayerManager.Current.m_localPlayerAgentInLevel;
        CoroutineDispatcher.StartCoroutine(
            Event71Coroutine(new Event71Data(playerAgent.DimensionIndex, data.DimensionIndex, data.LocalIndex)));
    }
    
    private static System.Collections.IEnumerator Event71Coroutine(Event71Data data)
    {
        GlobalZoneIndex start = new GlobalZoneIndex(data.startDimIndex, LG_LayerType.MainLayer, data.zoneIndex);
        GlobalZoneIndex target = new GlobalZoneIndex(data.targetDimIndex, LG_LayerType.MainLayer, data.zoneIndex);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(target, false);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(start, false);
        yield return new WaitForSeconds(0.4f);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(start, true);
        yield return new WaitForSeconds(0.3f);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(start, false);
        yield return new WaitForSeconds(0.3f);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(start, true);
        yield return new WaitForSeconds(0.2f);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(start, false);
        yield return new WaitForSeconds(0.2f);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(start, true);
        yield return new WaitForSeconds(0.4f);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(start, false);
        yield return new WaitForSeconds(2.0f);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(target, true);
        EnvironmentStateManager.AttemptSetExpeditionLightModeInZone(start, true);
    }
}