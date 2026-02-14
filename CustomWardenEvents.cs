using System;
using System.Collections.Generic;
using GameData;
using GTFO.API.Utilities;
using HarmonyLib;
using UnityEngine;

namespace GTFOR1Z1Mod;

[HarmonyPatch]
public class CustomWardenEvents
{
    private static Dictionary<int, Action<WardenObjectiveEventData>> customWardenEvents = new()
    {
        { 70, WardenEventProcessor.Event70 },
        { 71, WardenEventProcessor.Event71 },
        { 72, WardenEventProcessor.Event72 },
        { 73, WardenEventProcessor.Event73 },
    };
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldEventManager), nameof(WorldEventManager.ExecuteEvent), typeof(WardenObjectiveEventData), typeof(float))]
    static bool ExecuteEventPrefix(ref WardenObjectiveEventData eData, float currentDuration)
    {
        if (customWardenEvents.TryGetValue((int)eData.Type, out Action<WardenObjectiveEventData> eventAction))
        {
            bool allowed = CheckWardenEventCondition(eData);
            if (!allowed)
            {
                return false;
            }
            float delay = eData.Delay;
            if (delay > 0)
            {
                CoroutineDispatcher.StartCoroutine(ExecuteEventCoroutine(eventAction, eData, delay));
            }
            else
            {
                ExecuteEvent(wardenAction: eventAction, eData: eData);
            }
            // Don't pass to original handler
            return false;
        }
        // Otherwise proceed
        return true;
    }

    private static bool CheckWardenEventCondition(WardenObjectiveEventData eventData)
    {
        int conditionIndex = eventData.Condition.ConditionIndex;
        if (conditionIndex == -1)
        {
            // No condition set, return true
            return true;
        }
        bool match = eventData.Condition.IsTrue;
        bool currentState = WorldEventManager.GetCondition(conditionIndex);
        return match == currentState;
    }
    
    private static System.Collections.IEnumerator ExecuteEventCoroutine(Action<WardenObjectiveEventData> wardenAction, WardenObjectiveEventData eData, float delay)
    {
        yield return new WaitForSeconds(delay);

        ExecuteEvent(wardenAction, eData);
    }
    
    private static void ExecuteEvent(Action<WardenObjectiveEventData> wardenAction, WardenObjectiveEventData eData)
    {
        wardenAction.Invoke(eData);
        WardenObjectiveManager.DisplayWardenIntel(eData.Layer, eData.WardenIntel);
    }
}