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
        Network.HQRNetworkManager.SpawnNetworkHandler();
    }

    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    private static void PostStart(StartOfRound __instance)
    {
        daysClearedInARow = 0;

        if (HQRebalance.ConfigOptions.preset.Value == Presets.Default || HQRebalance.ConfigOptions.moonPatches.Value)
            HQRebalance.Instance.SetupMoons(__instance);

        if (HQRebalance.ConfigOptions.preset.Value == Presets.Default || HQRebalance.ConfigOptions.maneaterPatches.Value || HQRebalance.ConfigOptions.disableIncreasedSpawnChance.Value)
            UnityEngine.Resources.FindObjectsOfTypeAll<CaveDwellerAI>()[0].enemyType.increasedChanceInterior = -1;

        MaskedPlayerEnemyHelper.PopulateMaskedPlayerEnemyHelperInfo();
    }

    [HarmonyPatch(nameof(StartOfRound.ShipHasLeft))]
    [HarmonyPrefix]
    private static void PreShipHasLeft(StartOfRound __instance)
    {
        if (__instance.IsServer || __instance.IsHost)
        {
            int bodies = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>().Length;
            Network.HQRNetworkManager.Instance.bottomLine.Value = __instance.GetValueOfAllScrap(onlyScrapCollected: false, onlyNewScrap: true) + 35 * ButlerEnemyAIPatches.knifeIcons.Count - 5 * bodies;
        }
    }


    [HarmonyPatch(nameof(StartOfRound.EndOfGameClientRpc))]
    [HarmonyPrefix]
    private static void PreEndOfGameClientRpc(StartOfRound __instance, int scrapCollectedOnServer)
    {
        RoundManager.Instance.totalScrapValueInLevel = Network.HQRNetworkManager.Instance.bottomLine.Value;

        if (__instance.currentLevel.spawnEnemiesAndScrap)
        {
            double clearThreshold = HQRebalance.ConfigOptions.preset.Value == Presets.Default ? 0.85f : (double)HQRebalance.ConfigOptions.lootThreshold.Value/100f;

            if ((double)(scrapCollectedOnServer - 5 * __instance.GetBodiesInShip()) / (double)Network.HQRNetworkManager.Instance.bottomLine.Value >= 0.85f)
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
            Network.HQRNetworkManager.Instance.tier3pass.Value = false;
    }

    [HarmonyPatch(nameof(StartOfRound.SetTimeAndPlanetToSavedSettings))]
    [HarmonyPostfix]
    private static void PostSetTimeAndPlanetToSavedSettings(StartOfRound __instance)
    {
        Network.HQRNetworkManager.Instance.tier3pass.Value = ES3.Load("Tier3Pass", GameNetworkManager.Instance.currentSaveFileName, false);
    }
}
