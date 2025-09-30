using HarmonyLib;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRoundPatches
{
    public static int daysClearedInARow;

    [HarmonyPatch(nameof(StartOfRound.Awake))]
    [HarmonyPrefix]
    private static void PreAwake(StartOfRound __instance)
    {
        Networking.HQRNetworkManager.SpawnNetworkHandler();
    }

    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    private static void PostStart(StartOfRound __instance)
    {
        daysClearedInARow = 0;
        HQRebalance.Instance.SetupMoons(__instance);
        UnityEngine.Resources.FindObjectsOfTypeAll<CaveDwellerAI>()[0].enemyType.increasedChanceInterior = -1;
        MaskedPlayerEnemyHelper.PopulateMaskPrefabs();
    }

    [HarmonyPatch(nameof(StartOfRound.ShipHasLeft))]
    [HarmonyPrefix]
    private static void PreShipHasLeft(StartOfRound __instance)
    {
        if (__instance.IsServer || __instance.IsHost)
        {
            int bodies = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>().Length;
            Networking.HQRNetworkManager.Instance.bottomLine.Value = __instance.GetValueOfAllScrap(onlyScrapCollected: false, onlyNewScrap: true) + 35 * ButlerEnemyAIPatches.knifeCount - 5 * bodies;
        }
    }


    [HarmonyPatch(nameof(StartOfRound.EndOfGameClientRpc))]
    [HarmonyPrefix]
    private static void PreEndOfGameClientRpc(StartOfRound __instance, int scrapCollectedOnServer)
    {
        ButlerEnemyAIPatches.knifeCount = 0;
        RoundManager.Instance.totalScrapValueInLevel = Networking.HQRNetworkManager.Instance.bottomLine.Value;

        if (__instance.currentLevel.spawnEnemiesAndScrap)
        {
            if ((double)(scrapCollectedOnServer - 5 * __instance.GetBodiesInShip()) / (double)Networking.HQRNetworkManager.Instance.bottomLine.Value > 0.85f)
                daysClearedInARow++;
            else
                daysClearedInARow = 0;
        }

        if (RoundManager.Instance.enemyRushIndex != -1)
        {
            __instance.currentLevel.Enemies[RoundManager.Instance.enemyRushIndex].enemyType.MaxCount = RoundManagerHelper.saveMaxEnemyCount;
        }
    }

    [HarmonyPatch(nameof(StartOfRound.PassTimeToNextDay))]
    [HarmonyPostfix]
    private static void PostPassTimeToNextDay(StartOfRound __instance)
    {
        if ((__instance.IsServer || __instance.IsHost) && TimeOfDay.Instance.daysUntilDeadline == 0)
            Networking.HQRNetworkManager.Instance.tier3pass.Value = false;
    }

    [HarmonyPatch(nameof(StartOfRound.SetTimeAndPlanetToSavedSettings))]
    [HarmonyPostfix]
    private static void PostSetTimeAndPlanetToSavedSettings(StartOfRound __instance)
    {
        Networking.HQRNetworkManager.Instance.tier3pass.Value = ES3.Load("Tier3Pass", GameNetworkManager.Instance.currentSaveFileName, false);
    }
}
