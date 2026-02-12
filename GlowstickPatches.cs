using HarmonyLib;
using LevelGeneration;
using UnityEngine;

namespace GTFOR1Z1Mod;

[HarmonyPatch]
public class GlowstickPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GlowstickInstance), nameof(GlowstickInstance.Setup))]
    static void GlowstickSetup_Postfix(GlowstickInstance __instance)
    {
        if (__instance.ItemDataBlock.persistentID == 200)
        {
            Color color = new Color(1, 0.9f, 0, 1);
            __instance.m_LightColorTarget = color;
            __instance.s_lightRange = 80;
            __instance.s_lightLifeTime = 15f;
        }
    }
    
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GlowstickInstance), nameof(GlowstickInstance.Update))]
    static void GlowstickUpdate_Postfix(GlowstickInstance __instance)
    {
        if (__instance.m_state != eFadeState.One) return;
        if (__instance.ItemDataBlock.persistentID == 200)
        {
            if (Dimension.TryGetCourseNodeFromPos(__instance.transform.position, out var courseNode))
            {
                if (courseNode.m_enemiesInNode._size == 0)
                {
                    return;
                }
                var enemies = courseNode.m_enemiesInNode.ToArray();
                foreach (var enemy in enemies)
                {
                    if (enemy.EnemyData.persistentID == 47)
                    {
                        // Pablo is special, and instant dead does not work on him :(
                        enemy.Damage.MeleeDamage(float.MaxValue, null, enemy.transform.position, Vector3.one);
                    }
                    else
                    {
                        enemy.Damage.InstantDead();
                    }
                }
            }
        }
    }
}