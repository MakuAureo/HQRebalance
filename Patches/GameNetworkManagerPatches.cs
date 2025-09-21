using System;
using HarmonyLib;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class GameNetworkManagerPatches
{
    [HarmonyPatch(nameof(GameNetworkManager.Start))]
    [HarmonyPostfix]
    private static void PostStart(GameNetworkManager __instance)
    {
        Networking.HQRNetworkManager.CreateAndRegisterPrefab();
    }

    [HarmonyPatch(nameof(GameNetworkManager.SaveGameValues))]
    [HarmonyPostfix]
    private static void PostSaveGameValues(GameNetworkManager __instance)
    {
        if (!__instance.isHostingGame || !StartOfRound.Instance.inShipPhase || StartOfRound.Instance.isChallengeFile)
            return;

        try
        {
            ES3.Save("Tier3Pass", Networking.HQRNetworkManager.Instance.tier3pass.Value, __instance.currentSaveFileName);
        }
        catch (Exception arg)
        {
            HQRebalance.Logger.LogError($"Error while trying to save game values when disconnecting as host: {arg}");
        }
    }

    [HarmonyPatch(nameof(GameNetworkManager.Disconnect))]
    [HarmonyPrefix]
    private static void PreDisconnect(GameNetworkManager __instance)
    {
        Networking.HQRNetworkManager.DespawnNetworkHandler();
    }
}
