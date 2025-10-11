using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(ButlerEnemyAI))]
internal class ButlerEnemyAIPatches
{
    public static readonly Dictionary<EnemyAI, KnifeIconInfo> knifeIcons = new();

    [HarmonyPatch(nameof(ButlerEnemyAI.Start))]
    [HarmonyPostfix]
    private static void PostStart(ButlerEnemyAI __instance)
    {
        if (HQRebalance.ConfigOptions.presetToUse == Presets.Custom && !HQRebalance.ConfigOptions.butlerPatches.Value && !HQRebalance.ConfigOptions.addKnifeIcon.Value)
            return;

        knifeIcons.Add(__instance, new()
        {
            radarIcon = UnityEngine.Object.Instantiate(StartOfRound.Instance.itemRadarIconPrefab, RoundManager.Instance.mapPropsContainer.transform).transform,
            knifeTransform = __instance.gameObject.transform.GetChild(1).GetChild(4).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).transform
        });
    }

    [HarmonyPatch(nameof(ButlerEnemyAI.LateUpdate))]
    [HarmonyPostfix]
    private static void PostLateUpdate(ButlerEnemyAI __instance)
    {
        if (HQRebalance.ConfigOptions.presetToUse == Presets.Custom && __instance.isEnemyDead || (!HQRebalance.ConfigOptions.butlerPatches.Value && !HQRebalance.ConfigOptions.addKnifeIcon.Value))
            return;

        if (knifeIcons.TryGetValue(__instance, out KnifeIconInfo knifeIcon))
            knifeIcon.radarIcon.position = knifeIcon.knifeTransform.position;
        else
            HQRebalance.Logger.LogWarning("Could not find icon to update");
    }

    [HarmonyPatch(nameof(ButlerEnemyAI.KillEnemy))]
    [HarmonyPostfix]
    private static void PostKillEnemy(ButlerEnemyAI __instance)
    {
        if (HQRebalance.ConfigOptions.presetToUse == Presets.Custom && !HQRebalance.ConfigOptions.butlerPatches.Value && !HQRebalance.ConfigOptions.addKnifeIcon.Value)
            return;

        if (knifeIcons.TryGetValue(__instance, out KnifeIconInfo knifeIcon)) {
            Object.Destroy(knifeIcon.radarIcon.gameObject);
        }
        else
            HQRebalance.Logger.LogWarning("Could not find icon to destroy");
    }

    [HarmonyPatch(nameof(ButlerEnemyAI.OnCollideWithPlayer))]
    [HarmonyPrefix]
    private static void PreOnCollideWithPlayer(ButlerEnemyAI __instance)
    {
        if (HQRebalance.ConfigOptions.presetToUse == Presets.Custom && !HQRebalance.ConfigOptions.butlerPatches.Value && !HQRebalance.ConfigOptions.disableStealthStab.Value)
            return;

        __instance.timeSinceStealthStab = Time.realtimeSinceStartup;
    }
}

struct KnifeIconInfo
{
    public Transform radarIcon;
    public Transform knifeTransform;
}
