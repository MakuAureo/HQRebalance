using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal class RoundManagerPatches
{
    [HarmonyPatch(nameof(RoundManager.Start))]
    [HarmonyPostfix]
    private static void PostStart(RoundManager __instance)
    {
        __instance.enemyRushIndex = -1;

        const double minesSize = 0.65f; //65% is mines
        const double facilityDelta = 0.05f; //How much bigger 2nd facility is than the 1st

        const double facilitySize = 1 - minesSize;
        const double facility1Size = (facilitySize - facilityDelta) * 0.5f;
        const double facility2Size = (facilitySize + facilityDelta) * 0.5f;

        __instance.dungeonFlowTypes[4].MapTileSize = 1.1f;

        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[0].Length = (float)facility1Size;

        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[1].Length = (float)minesSize;
        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[1].Position = (float)facility1Size;

        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[2].Length = (float)facility2Size;
        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[2].Position = (float)(facility1Size + minesSize);
    }

    [HarmonyPatch(nameof(RoundManager.OnDestroy))]
    [HarmonyPrefix]
    private static void PreOnDestroy(RoundManager __instance)
    {
        if (__instance.enemyRushIndex > -1)
        {
            StartOfRound.Instance.currentLevel.Enemies[__instance.enemyRushIndex].enemyType.MaxCount = RoundManagerHelper.saveMaxEnemyCount;
        }
    }

    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    [HarmonyPostfix]
    private static void PostDespawnPropsAtEndOfRound(RoundManager __instance)
    {
        ButlerEnemyAIPatches.knifeIcons.Clear();
        MaskedPlayerEnemyHelper.masks.Clear();
    }

    [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileSpawnScrapInLevel(IEnumerable<CodeInstruction> codes)
    {
        return new CodeMatcher(codes)
            .MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_6))
            .SetOpcodeAndAdvance(OpCodes.Ldc_I4_0)
            .MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)20))
            .SetOperandAndAdvance(-1)
            .InstructionEnumeration();
    }

    [HarmonyPatch(nameof(RoundManager.PlotOutEnemiesForNextHour))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspilePlotOutEnemiesForNextHour(IEnumerable<CodeInstruction> codes)
    {
        CodeInstruction[] callSteadSpawnIncrease =
        {
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundManagerHelper), nameof(RoundManagerHelper.SteadSpawnIncrease))),
            new CodeInstruction(OpCodes.Add),
            new CodeInstruction(OpCodes.Stloc_2)
        };

        return new CodeMatcher(codes)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(TimeOfDay), nameof(TimeOfDay.Instance))))
            .Advance(1)
            .RemoveInstructions(10)
            .InsertAndAdvance(callSteadSpawnIncrease)
            .MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_2))
            .SetOpcodeAndAdvance(OpCodes.Ldc_I4_0)
            .InstructionEnumeration();
    }

    [HarmonyPatch(nameof(RoundManager.RefreshEnemiesList))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileRefreshEnemyList(IEnumerable<CodeInstruction> codes)
    {
        CodeInstruction[] callOverwriteRushCode =
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundManagerHelper), nameof(RoundManagerHelper.OverwriteRushCode)))
        };

        return new CodeMatcher(codes)
            .MatchForward(false, new CodeMatch(OpCodes.Ldloca_S))
            .RemoveInstructions(152)
            .Insert(callOverwriteRushCode)
            .InstructionEnumeration();
    }

    [HarmonyPatch(nameof(RoundManager.AssignRandomEnemyToVent))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileAssignRandomEnemyToVent(IEnumerable<CodeInstruction> codes)
    {
        CodeInstruction[] callSpawnProbabilityCodeAndStore =
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Ldloc_3),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundManagerHelper), nameof(RoundManagerHelper.SpawnProbabilityCode))),
            new CodeInstruction(OpCodes.Stloc_S, (sbyte)4)
        };

        CodeInstruction[] getModifiedRandomWeightedIndex =
        {
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.SpawnProbabilities))),
            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<int>), nameof(List<int>.ToArray))),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.EnemySpawnRandom))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundManagerHelper), nameof(RoundManagerHelper.GetModifiedRandomWeightedIndex))),
            new CodeInstruction(OpCodes.Stloc_2)
        };

        return new CodeMatcher(codes)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.enemyRushIndex))),
                new CodeMatch(OpCodes.Ldc_I4_M1))
            .RemoveInstructions(83)
            .InsertAndAdvance(callSpawnProbabilityCodeAndStore)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldarg_0))
            .Advance(2)
            .RemoveInstructions(6)
            .Insert(getModifiedRandomWeightedIndex)
            .InstructionEnumeration();
    }
}

internal static class RoundManagerHelper
{
    public static int saveMaxEnemyCount;

    public static float SteadSpawnIncrease()
    {
        return (float)TimeOfDay.Instance.timesFulfilledQuota / 5f;
    }

    public static void OverwriteRushCode(RoundManager instance)
    {
        System.Random rng = new(StartOfRound.Instance.randomMapSeed + 2145);
        int chance = (StartOfRoundPatches.daysClearedInARow >= 3) ? 20 : 4;

        if (rng.Next(0, 100) < chance)
        {
            int index = -1;
            bool found = false;
            List<int> enem = new();

            for (int i = 0; i < instance.currentLevel.Enemies.Count; i++)
            {
                if (instance.currentLevel.Enemies[i].enemyType.enemyName == "Nutcracker" ||
                    instance.currentLevel.Enemies[i].enemyType.enemyName == "Butler" ||
                    instance.currentLevel.Enemies[i].enemyType.enemyName == "Masked")
                {
                    enem.Add(i);
                    found = true; ;
                }
            }

            if (!found)
                return;

            index = enem[rng.Next(0, enem.Count)];

            saveMaxEnemyCount = instance.currentLevel.Enemies[index].enemyType.MaxCount;
            instance.currentLevel.Enemies[index].enemyType.MaxCount = 999;
            instance.enemyRushIndex = index;
        }
    }

    public static int SpawnProbabilityCode(RoundManager instance, EnemyType enemy, int index)
    {
        int prob = 0;

        if (instance.increasedInsideEnemySpawnRateIndex == index)
            prob = 100;
        else
        {
            if (!enemy.useNumberSpawnedFalloff)
                prob = (int)((double)instance.currentLevel.Enemies[index].rarity * enemy.probabilityCurve.Evaluate(instance.timeScript.normalizedTimeOfDay));
            else
                prob = (int)((double)instance.currentLevel.Enemies[index].rarity * enemy.probabilityCurve.Evaluate(instance.timeScript.normalizedTimeOfDay) * enemy.numberSpawnedFalloff.Evaluate((float)enemy.numberSpawned / 10f));
        }

        return prob;
    }

    public static int GetModifiedRandomWeightedIndex(RoundManager instance, int[] SpawnProbabilities, System.Random EnemySpawnRandom)
    {
        if (instance.enemyRushIndex != -1 && EnemySpawnRandom.Next(0, 100) < 60)
            return instance.enemyRushIndex;
        else
            return instance.GetRandomWeightedIndex(SpawnProbabilities, EnemySpawnRandom);
    }
}

