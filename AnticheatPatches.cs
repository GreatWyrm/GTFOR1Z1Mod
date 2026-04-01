namespace GTFOR1Z1Mod;

public class AnticheatPatches
{
    public static void FreecamPostfix(FreeflightCamera __instance)
    {
        __instance.m_moveSpeedCurr = 0f;
    }
}