using UnityEngine;
using HarmonyLib;
using DunGen;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(DungeonGenerator))]
internal class DungeonGeneratorPatches
{
    private static class FireExitAnimCurveKeyframes
    {
        public readonly static Keyframe[] facility =
        {
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.3f, 0f, 0.000007952512f, 0.04168295f),
            new Keyframe(0.45f, 0.1f, 0.000007952512f, 0.04168295f),
            new Keyframe(0.75f, 1f, 0.02613646f, 0.02613646f),
            new Keyframe(1f, 1f, 0.02613646f, 0.02613646f)
        };

        public readonly static Keyframe[] mansion =
        {
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.1f, 0f, 0.000007952512f, 0.04168295f),
            new Keyframe(0.3f, 0.1f, 0.000007952512f, 0.04168295f),
            new Keyframe(0.95f, 1f, 0.02613646f, 0.02613646f),
            new Keyframe(1f, 1f, 0.02613646f, 0.02613646f)
        };

        public readonly static Keyframe[] mineshaft =
        {
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.4f, 0f, 0.000007952512f, 0.04168295f),
            new Keyframe(0.7f, 0.1f, 0.000007952512f, 0.04168295f),
            new Keyframe(0.85f, 1f, 0.02613646f, 0.02613646f),
            new Keyframe(1f, 1f, 0.02613646f, 0.02613646f)
        };
    }

    [HarmonyPatch(nameof(DungeonGenerator.ProcessGlobalProps))]
    [HarmonyPrefix]
    private static void PreProcessGlobalProps(DungeonGenerator __instance)
    {
        Keyframe[]? currDungeonKeyframes = null;

        if (__instance.DungeonFlow.name == "Level1Flow")
            currDungeonKeyframes = FireExitAnimCurveKeyframes.facility;
        else if (__instance.DungeonFlow.name == "Level2Flow")
            currDungeonKeyframes = FireExitAnimCurveKeyframes.mansion;
        else if (__instance.DungeonFlow.name == "Level3Flow")
            currDungeonKeyframes = FireExitAnimCurveKeyframes.mineshaft;

        if (currDungeonKeyframes == null)
            return;

        foreach (Tile tile in __instance.CurrentDungeon.AllTiles)
        {
            GlobalProp[] allProps = tile.GetComponentsInChildren<GlobalProp>();
            foreach (GlobalProp prop in allProps)
            {
                if (prop.PropGroupID == DungeonGeneratorHelper.FireExitGroupID)
                {
                    prop.DepthWeightScale = new AnimationCurve(currDungeonKeyframes);
                }
            }
        }
    }

    [HarmonyPatch(nameof(DungeonGenerator.ProcessGlobalProps))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileProcessGlobalProps(IEnumerable<CodeInstruction> codes)
    {
        CodeMatcher matcher = new(codes);

        matcher.MatchForward(true, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(TilePlacementData), nameof(TilePlacementData.NormalizedDepth))));
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not the ending pattern. Aborting {nameof(DungeonGeneratorPatches.TranspileProcessGlobalProps)} transpiler.");
            return codes;
        }
        int endIndex = matcher.Pos;

        matcher.MatchBack(false, new CodeMatch(OpCodes.Ldloc_3));
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not the starting pattern. Aborting {nameof(DungeonGeneratorPatches.TranspileProcessGlobalProps)} transpiler.");
            return codes;
        }
        matcher.Advance(1);
        int startIndex = matcher.Pos;

        matcher.RemoveInstructionsInRange(startIndex, endIndex);
        matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_S, 6),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DungeonGeneratorHelper), nameof(DungeonGeneratorHelper.GetNormalizedPathDepthForFireExit)))
        );

        return matcher.InstructionEnumeration();
    }
}

internal static class DungeonGeneratorHelper
{
    public const int FireExitGroupID = 1231;

    public static float GetNormalizedPathDepthForFireExit(Tile currTile, GlobalProp currProp)
    {
        return (currProp.PropGroupID == FireExitGroupID) ? (currTile.Placement.NormalizedPathDepth) : (currTile.Placement.NormalizedDepth);
    }
}
