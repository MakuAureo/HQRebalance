using HarmonyLib;
using UnityEngine;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(JesterAI))]
internal class JesterAIPatches
{
    [HarmonyPatch(nameof(JesterAI.Start))]
    [HarmonyPostfix]
    private static void PostStart(JesterAI __instance)
    {
        if (HQRebalance.ConfigOptions.presetToUse == Presets.Custom && !HQRebalance.ConfigOptions.jesterPatches.Value && !HQRebalance.ConfigOptions.disableSolidHitbox.Value)
            return;

        __instance.mainCollider.gameObject.GetComponent<Collider>().isTrigger = true;
        __instance.enemyType.pushPlayerForce = HQRebalance.ConfigOptions.presetToUse != Presets.Custom ? 6.5f : HQRebalance.ConfigOptions.pushForce.Value;
    }

    [HarmonyPatch(nameof(JesterAI.SetJesterInitialValues))]
    [HarmonyPostfix]
    private static void PostSetJesterInitialValues(JesterAI __instance)
    {
        if (HQRebalance.ConfigOptions.presetToUse != Presets.Custom || HQRebalance.ConfigOptions.jesterPatches.Value || HQRebalance.ConfigOptions.disableSolidHitbox.Value)
            __instance.mainCollider.isTrigger = true;

        if (HQRebalance.ConfigOptions.presetToUse != Presets.Custom || HQRebalance.ConfigOptions.jesterPatches.Value || HQRebalance.ConfigOptions.disableSolidHitbox.Value)
            __instance.beginCrankingTimer = HQRebalance.ConfigOptions.presetToUse != Presets.Custom ?
                1.25f * (__instance.beginCrankingTimer - 13f) + 30 * StartOfRound.Instance.currentLevel.factorySizeMultiplier - 10 :
                HQRebalance.ConfigOptions.followTimerScaling.Value * (1.25f * (__instance.beginCrankingTimer - 13f) + 30 * StartOfRound.Instance.currentLevel.factorySizeMultiplier - 10) + (1f - HQRebalance.ConfigOptions.followTimerScaling.Value) * __instance.beginCrankingTimer;
    }
}
