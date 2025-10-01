using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
internal class TimeOfDayPatches
{
    [HarmonyPatch(nameof(TimeOfDay.CalculateLuckValue))]
    [HarmonyPostfix]
    private static void PostCalculateLuckValue(TimeOfDay __instance)
    {
        TimeOfDayHelper.luckPoints = 0;
        AutoParentToShip[] furniture = Object.FindObjectsByType<AutoParentToShip>(FindObjectsSortMode.None);
        foreach (AutoParentToShip f in furniture)
        {
            if (f.unlockableID != -1 && StartOfRound.Instance.unlockablesList.unlockables[f.unlockableID].spawnPrefab)
            {
                if (StartOfRound.Instance.unlockablesList.unlockables[f.unlockableID].luckValue >= 0)
                    TimeOfDayHelper.luckPoints++;
                else
                    TimeOfDayHelper.luckPoints--;
            }
        }
    }

    [HarmonyPatch(nameof(TimeOfDay.SetNewProfitQuota))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileSetNewProfitQuota(IEnumerable<CodeInstruction> codes)
    {
        CodeMatcher matcher = new(codes);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Stloc_1));
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the starting pattern. Aborting {nameof(TimeOfDayPatches.TranspileSetNewProfitQuota)} transpiler.");
            return codes;
        }
        int startIndex = matcher.Pos;

        CodeMatcher endMatcher = matcher.Clone();
        endMatcher.MatchForward(true, new CodeMatch(OpCodes.Stloc_0));
        endMatcher.MatchForward(true, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Debug), nameof(Debug.Log), new System.Type[] { typeof(void) })));
        if (endMatcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find the ending pattern. Aborting {nameof(TimeOfDayPatches.TranspileSetNewProfitQuota)} transpiler.");
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
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TimeOfDayHelper), nameof(TimeOfDayHelper.CalculateQuotaIncrease))),
                new CodeInstruction(OpCodes.Stloc_0)
            );

        return matcher.InstructionEnumeration();
    }

}

internal static class TimeOfDayHelper
{
    public static int luckPoints;

    public static float CalculateQuotaIncrease(float unitaryRandom, float startIncrease)
    {
        const double a = 13f/7700f;
        const double b = 64f/77007;

        TimeOfDay tod = TimeOfDay.Instance;

        double luckFactor = (luckPoints < 0) ? -0.1f : a * luckPoints * luckPoints + b * luckPoints;
        float inc = tod.quotaVariables.baseIncrease * startIncrease;
        float random = 1f + tod.quotaVariables.randomizerCurve.Evaluate(unitaryRandom) * tod.quotaVariables.randomizerMultiplier + (float)luckFactor;

        return inc * Mathf.Clamp(random, 0.497f, 1.503f);;
    }
}
