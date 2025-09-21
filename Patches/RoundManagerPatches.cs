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

        const double minSize = 0.65f; //65% is mines
        const double midSize = (1 - minSize) * 0.5f;

        const double delta = (0.05f) * 0.5f; //How much bigger 2nd facility is than the 1st
        const double fa1Size = midSize - delta;
        const double fa2Size = midSize + delta;

        __instance.dungeonFlowTypes[4].MapTileSize = 1.1f;

        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[0].Length = (float)fa1Size;

        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[1].Length = (float)minSize;
        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[1].Position = (float)fa1Size;

        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[2].Length = (float)fa2Size;
        __instance.dungeonFlowTypes[4].dungeonFlow.Lines[2].Position = (float)(fa1Size + minSize);
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

    [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileSpawnScrapInLevel(IEnumerable<CodeInstruction> codes)
    {
        CodeMatcher matcher = new(codes);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_6));
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the first pattern. Aborting {nameof(RoundManagerPatches.TranspileSpawnScrapInLevel)} transpiler.");
            return codes;
        }
        matcher.SetOpcodeAndAdvance(OpCodes.Ldc_I4_0);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)20));
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the second pattern. Aborting {nameof(RoundManagerPatches.TranspileSpawnScrapInLevel)} transpiler.");
            return codes;
        }
        matcher.SetOperandAndAdvance(-1);

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(RoundManager.PlotOutEnemiesForNextHour))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspilePlotOutEnemiesForNextHour(IEnumerable<CodeInstruction> codes)
    {
        CodeMatcher matcher = new(codes);

        matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(TimeOfDay), nameof(TimeOfDay.Instance)))
            );
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the starting pattern. Aborting {nameof(RoundManagerPatches.TranspilePlotOutEnemiesForNextHour)} transpiler.");
            return codes;
        }
        List<Label> labels = matcher.Instruction.labels;
        int startIndex = matcher.Pos;

        CodeMatcher endMatcher = matcher.Clone();
        endMatcher.MatchForward(false, new CodeMatch(OpCodes.Stloc_2));
        if (endMatcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the ending pattern. Aborting {nameof(RoundManagerPatches.TranspilePlotOutEnemiesForNextHour)} transpiler.");
            return codes;
        }
        int endIndex = endMatcher.Pos;

        matcher.RemoveInstructionsInRange(startIndex, endIndex);
        List<CodeInstruction> newIL = new()
        {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundManagerHelper), nameof(RoundManagerHelper.SteadSpawnIncrease))),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Stloc_2)
        };
        newIL[0].labels.AddRange(labels);
        matcher.InsertAndAdvance(newIL);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_2));
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the second pattern. Aborting {nameof(RoundManagerPatches.TranspilePlotOutEnemiesForNextHour)} transpiler.");
            return codes;
        }
        matcher.SetOpcodeAndAdvance(OpCodes.Ldc_I4_0);

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(RoundManager.RefreshEnemiesList))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileRefreshEnemyList(IEnumerable<CodeInstruction> codes)
    {
        CodeMatcher matcher = new(codes);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloca_S));
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the starting pattern. Aborting {nameof(RoundManagerPatches.TranspileRefreshEnemyList)} transpiler.");
            return codes;
        }
        int startIndex = matcher.Pos;

        CodeMatcher endMatcher = matcher.Clone();
        endMatcher.End().MatchBack(true, new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(UnityEngine.GameObject), nameof(UnityEngine.GameObject.SetActive))));
        if (endMatcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the ending pattern. Aborting {nameof(RoundManagerPatches.TranspileRefreshEnemyList)} transpiler.");
            return codes;
        }
        int endIndex = endMatcher.Pos;

        if (startIndex >= endIndex)
        {
            HQRebalance.Logger.LogWarning("Start and end patterns are the same or in the wrong order. Nothing to remove.");
            return codes;
        }

        matcher.RemoveInstructionsInRange(startIndex, endIndex);
        matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundManagerHelper), nameof(RoundManagerHelper.OverwriteRushCode)))
            );

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(RoundManager.AssignRandomEnemyToVent))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileAssignRandomEnemyToVent(IEnumerable<CodeInstruction> codes)
    {
        CodeMatcher matcher = new(codes);

        matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.enemyRushIndex))),
                new CodeMatch(OpCodes.Ldc_I4_M1)
            );
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the starting pattern. Aborting {nameof(RoundManagerPatches.TranspileAssignRandomEnemyToVent)} transpiler.");
            return codes;
        }
        int startIndex = matcher.Pos;

        CodeMatcher endMatcher = matcher.Clone();
        endMatcher.End().MatchBack(true, new CodeMatch(OpCodes.Stloc_S));
        if (endMatcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the ending pattern. Aborting {nameof(RoundManagerPatches.TranspileAssignRandomEnemyToVent)} transpiler.");
            return codes;
        }
        int endIndex = endMatcher.Pos;

        matcher.RemoveInstructionsInRange(startIndex, endIndex);
        matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundManagerHelper), nameof(RoundManagerHelper.SpawnProbabilityCode))),
                new CodeInstruction(OpCodes.Stloc_S, (sbyte)4)
            );

        matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldarg_0)
            );
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the starting pattern2. Aborting {nameof(RoundManagerPatches.TranspileAssignRandomEnemyToVent)} transpiler.");
            return codes;
        }
        startIndex = matcher.Pos;
        List<Label> labels = matcher.Instruction.labels;

        endMatcher = matcher.Clone();
        endMatcher.MatchForward(true, new CodeMatch(OpCodes.Stloc_2));
        if (endMatcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the ending pattern2. Aborting {nameof(RoundManagerPatches.TranspileAssignRandomEnemyToVent)} transpiler.");
            return codes;
        }
        endIndex = endMatcher.Pos;

        matcher.RemoveInstructionsInRange(startIndex, endIndex);
        List<CodeInstruction> newIL = new()
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.SpawnProbabilities))),
            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<int>), nameof(List<int>.ToArray))),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RoundManager), nameof(RoundManager.EnemySpawnRandom))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundManagerHelper), nameof(RoundManagerHelper.GetModifiedRandomWeightedIndex))),
            new CodeInstruction(OpCodes.Stloc_2)
        };
        newIL[0].labels.AddRange(labels);
        matcher.InsertAndAdvance(newIL);

        return matcher.InstructionEnumeration();
    }
}

internal class RoundManagerHelper
{
    public static int saveMaxEnemyCount;

    public static float SteadSpawnIncrease()
    {
        return (float)TimeOfDay.Instance.timesFulfilledQuota / 5f;
    }

    public static void OverwriteRushCode(RoundManager instance)
    {
        System.Random rng = new(StartOfRound.Instance.randomMapSeed + 2145);
        //int chance = 100;
        int chance = (StartOfRoundPatches.daysClearedInARow >= 3) ? 20 : 4;

        if (rng.Next(0, 100) < chance)
        {
            int index = -1;
            bool found = false;
            List<int> enem = new();

            for (int i = 0; i < instance.currentLevel.Enemies.Count; i++)
            {
                if (instance.currentLevel.Enemies[i].enemyType.enemyName == "Nutcracker")
                {
                    enem.Add(i);
                    found = true;;
                }
                if (instance.currentLevel.Enemies[i].enemyType.enemyName == "Butler")
                {
                    enem.Add(i);
                    found = true;;
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

