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
            new Keyframe(0.3f, 0f, 0f, 0.04168295f),
            new Keyframe(0.45f, 0.1f, 0f, 0.04168295f),
            new Keyframe(0.75f, 1f, 0.02613646f, 0.02613646f),
            new Keyframe(1f, 1f, 0.02613646f, 0.02613646f)
        };

        public readonly static Keyframe[] mansion =
        {
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.1f, 0f, 0f, 0.04168295f),
            new Keyframe(0.3f, 0.1f, 0f, 0.04168295f),
            new Keyframe(0.95f, 1f, 0.02613646f, 0.02613646f),
            new Keyframe(1f, 1f, 0.02613646f, 0.02613646f)
        };

        public readonly static Keyframe[] mineshaft =
        {
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.5f, 0f, 0f, 0.04168295f),
            new Keyframe(0.7f, 0.1f, 0f, 0.04168295f),
            new Keyframe(0.85f, 1f, 0.02613646f, 0.02613646f),
            new Keyframe(1f, 1f, 0.02613646f, 0.02613646f)
        };
    }

    [HarmonyPatch(nameof(DungeonGenerator.ProcessGlobalProps))]
    [HarmonyPrefix]
    private static void PreProcessGlobalProps(DungeonGenerator __instance)
    {
        if (HQRebalance.ConfigOptions.fairerFireExitsLoaded != null && HQRebalance.ConfigOptions.fairerFireExitsLoaded.Value)
            return;

        if (HQRebalance.ConfigOptions.presetToUse == Presets.Custom && !HQRebalance.ConfigOptions.fireExitPatch.Value)
            return;

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
    public static IEnumerable<CodeInstruction> TranspileProcessGlobalProps(IEnumerable<CodeInstruction> codes)
    {
        if (HQRebalance.ConfigOptions.fairerFireExitsLoaded != null && HQRebalance.ConfigOptions.fairerFireExitsLoaded.Value)
            return codes;

        if (HQRebalance.ConfigOptions.preset.Value != Presets.Default && !HQRebalance.ConfigOptions.fireExitPatch.Value)
            return codes;

        CodeInstruction[] callGetNormalizedPathDepthForFireExit =
        {
            new CodeInstruction(OpCodes.Ldloc_S, 6),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DungeonGeneratorHelper), nameof(DungeonGeneratorHelper.GetNormalizedPathDepthForFireExit)))
        };

        return new CodeMatcher(codes)
            .MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(TilePlacementData), nameof(TilePlacementData.NormalizedDepth))))
            .Advance(-1)
            .RemoveInstructions(2)
            .Insert(callGetNormalizedPathDepthForFireExit)
            .InstructionEnumeration();
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
