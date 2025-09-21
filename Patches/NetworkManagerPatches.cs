using HarmonyLib;
using Unity.Netcode;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(NetworkManager))]
internal class NetworkManagerPatches
{
    [HarmonyPatch(nameof(NetworkManager.ShutdownInternal))]
    [HarmonyPrefix]
    private static void PreShutdownInternal(NetworkManager __instance)
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.CustomMessagingManager == null)
            return;

        ButlerEnemyAIPatches.knifeCount = 0;
    }
}
