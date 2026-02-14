using GameData;
using LevelGeneration;
using Player;
using UnityEngine;
using GTFO.API.Utilities;
using SNetwork;

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
        public Event71Data(eDimensionIndex startDim, LG_LayerType initialLayer, eLocalZoneIndex startIndex, eDimensionIndex targetDim, LG_LayerType endLayer, eLocalZoneIndex zone)
        {
            startDimIndex = startDim;
            startLayer = initialLayer;
            startZoneIndex = startIndex;
            targetDimIndex = targetDim;
            targetLayer = endLayer;
            targetZoneIndex = zone;
        }
        
        public eDimensionIndex startDimIndex;
        public LG_LayerType startLayer;
        public eLocalZoneIndex startZoneIndex;
        public eDimensionIndex targetDimIndex;
        public LG_LayerType targetLayer;
        public eLocalZoneIndex targetZoneIndex;
    }
    
    public static void Event71(WardenObjectiveEventData data)
    {
        var playerAgent = PlayerManager.Current.m_localPlayerAgentInLevel;
        CoroutineDispatcher.StartCoroutine(
            Event71Coroutine(new Event71Data(playerAgent.DimensionIndex, playerAgent.CourseNode.LayerType, playerAgent.CourseNode.m_zone.LocalIndex, data.DimensionIndex, data.Layer, data.LocalIndex)));
    }
    
    private static System.Collections.IEnumerator Event71Coroutine(Event71Data data)
    {
        GlobalZoneIndex start = new GlobalZoneIndex(data.startDimIndex, data.startLayer, data.startZoneIndex);
        GlobalZoneIndex target = new GlobalZoneIndex(data.targetDimIndex, data.targetLayer, data.targetZoneIndex);
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

    public static void Event72(WardenObjectiveEventData data)
    {
        if (SNet.IsMaster)
        {
            foreach (var playerAgent in PlayerManager.PlayerAgentsInLevel)
            {
                AgentModifierManager.AddSyncedModifierValue(playerAgent, AgentModifier.MeleeResistance, 0.75f);
                AgentModifierManager.AddSyncedModifierValue(playerAgent, AgentModifier.ProjectileResistance, 0.75f);
                AgentModifierManager.AddSyncedModifierValue(playerAgent, AgentModifier.SpecialWeaponDamage, 2.0f);
                AgentModifierManager.AddSyncedModifierValue(playerAgent, AgentModifier.StandardWeaponDamage, 2.0f);
            }
        }
    }
    
    public static void Event73(WardenObjectiveEventData data)
    {
        if (SNet.IsMaster)
        {
            pInfection infection = new pInfection();
            infection.effect = pInfectionEffect.None;
            infection.mode = pInfectionMode.Set;
            infection.amount = 0f;
            foreach (var playerAgent in PlayerManager.PlayerAgentsInLevel)
            {
                PlayerBackpackManager.GiveAmmoToPlayer(playerAgent.Owner, 1f, 1f, 1f);
                playerAgent.Damage.ModifyInfection(infection, true, true);
                playerAgent.Damage.AddHealth(playerAgent.Damage.HealthMax, playerAgent);
            }
        }
    }
}