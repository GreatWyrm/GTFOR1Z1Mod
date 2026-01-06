using GameData;
using LevelGeneration;
using Player;
using UnityEngine;

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
}