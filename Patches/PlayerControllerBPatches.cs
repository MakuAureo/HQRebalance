using UnityEngine;
using GameNetcodeStuff;
using HarmonyLib;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatches
{
    [HarmonyPatch(nameof(PlayerControllerB.CalculateGroundNormal))]
    [HarmonyPrefix]
    private static bool OverwriteCalculateGroundNormal(PlayerControllerB __instance)
    {
        //Pre v60 logic for this method
        if (Physics.Raycast(__instance.transform.position + Vector3.up * 0.2f, -Vector3.up, out RaycastHit hit, 6f, 268438273, QueryTriggerInteraction.Ignore))
        {
            __instance.playerGroundNormal = hit.normal;
        }
        else
        {
            __instance.playerGroundNormal = Vector3.up;
        }

        __instance.hit = hit;
        return false;
    }

    [HarmonyPatch(nameof(PlayerControllerB.Update))]
    [HarmonyPostfix]
    private static void PostUpdate(PlayerControllerB __instance)
    {
        if (__instance.transform.position.y <= -80f && __instance.isUnderwater && RoundManager.Instance.currentDungeonType == 4)
        {
            __instance.hinderedMultiplier = 0.55f;
        }
    }
}
