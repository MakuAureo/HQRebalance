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
        __instance.mainCollider.gameObject.GetComponent<Collider>().isTrigger = true;
        __instance.enemyType.pushPlayerForce = 6.5f;
    }

    [HarmonyPatch(nameof(JesterAI.SetJesterInitialValues))]
    [HarmonyPostfix]
    private static void PostSetJesterInitialValues(JesterAI __instance)
    {
        __instance.mainCollider.isTrigger = true;

        __instance.beginCrankingTimer = 1.25f * (__instance.beginCrankingTimer - 13f) + 30 * StartOfRound.Instance.currentLevel.factorySizeMultiplier - 10;
    }
}
