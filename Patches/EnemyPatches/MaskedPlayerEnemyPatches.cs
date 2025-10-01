using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(MaskedPlayerEnemy))]
internal class MaskedPlayerEnemyPatches
{
    [HarmonyPatch(nameof(MaskedPlayerEnemy.Start))]
    [HarmonyPostfix]
    private static void PostStart(MaskedPlayerEnemy __instance)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject maskPrefab = (__instance.maskTypeIndex == MaskedPlayerEnemyHelper.comedyMaskIndex) ? (MaskedPlayerEnemyHelper.comedyPrefab) : (MaskedPlayerEnemyHelper.tragedyPrefab);
            GameObject maskObj = Object.Instantiate(maskPrefab, __instance.transform.position, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);
            NetworkObject maskNetObj = maskObj.GetComponent<NetworkObject>();
            maskNetObj.Spawn();

            NetworkObject maskedNetObj = __instance.GetComponent<NetworkObject>();
            Networking.HQRNetworkManager.Instance.GrabMaskClientRpc(maskedNetObj, maskNetObj);
        }
    }
    
    [HarmonyPatch(nameof(MaskedPlayerEnemy.SetMaskGlow))]
    [HarmonyPostfix]
    private static void PostSetMaskGlow(MaskedPlayerEnemy __instance, bool enable)
    {
        if (!MaskedPlayerEnemyHelper.masks.TryGetValue(__instance, out HauntedMaskItem maskItem))
        {
            HQRebalance.Logger.LogError("Could not find mask to glow");
            return;
        }

        if (enable)
        {
            __instance.maskTypes[__instance.maskTypeIndex].SetActive(true);
            maskItem.EnableItemMeshes(false);
        }
        else
        {
            __instance.maskTypes[__instance.maskTypeIndex].SetActive(false);
            maskItem.EnableItemMeshes(true);
        }
    }

    [HarmonyPatch(nameof(MaskedPlayerEnemy.LateUpdate))]
    [HarmonyPostfix]
    private static void PostLateUpdate(MaskedPlayerEnemy __instance)
    {
        if (MaskedPlayerEnemyHelper.masks.TryGetValue(__instance, out HauntedMaskItem maskItem))
        {
            if (maskItem.hasBeenHeld == true)
            {
                maskItem.transform.localScale = new Vector3(0.1646f, 0.1646f, 0.1646f);
                return;
            }

            maskItem.transform.rotation = __instance.maskTypes[0].transform.GetChild(2).transform.rotation;
            maskItem.transform.position = __instance.maskTypes[0].transform.GetChild(2).transform.position;
        }
        else
            HQRebalance.Logger.LogWarning("Could not find mask to update");

    }

    [HarmonyPatch(nameof(MaskedPlayerEnemy.KillEnemy))]
    [HarmonyPostfix]
    private static void PostKillEnemy(MaskedPlayerEnemy __instance)
    {
        if (MaskedPlayerEnemyHelper.masks.TryGetValue(__instance, out HauntedMaskItem maskItem))
        {
            if (NetworkManager.Singleton.IsServer)
            {
                NetworkObject maskNetObj = maskItem.gameObject.GetComponent<NetworkObject>();
                NetworkObject maskedNetObj = __instance.GetComponent<NetworkObject>();
                Networking.HQRNetworkManager.Instance.DropMaskClientRpc(maskedNetObj, maskNetObj);
            }
        }
        else
            HQRebalance.Logger.LogWarning("Could not find mask to drop");
    }

    [HarmonyPatch(nameof(MaskedPlayerEnemy.killAnimation), MethodType.Enumerator)]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspilekillAnimation(IEnumerable<CodeInstruction> codes)
    {
        CodeMatcher matcher = new(codes);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Vector3), nameof(Vector3.zero))));
        if (matcher.IsInvalid)
        {
            HQRebalance.Logger.LogError($"Could not find pattern. Aborting {nameof(MaskedPlayerEnemyPatches.TranspilekillAnimation)} transpiler.");
            return codes;
        }
        matcher.Advance(1);
    
        matcher.SetOpcodeAndAdvance(OpCodes.Ldc_I4_1);

        return matcher.InstructionEnumeration();
    }
}

internal static class MaskedPlayerEnemyHelper
{
    public const int comedyMaskIndex = 0;
    public const int tragedyMaskIndex = 1;

    public static GameObject comedyPrefab = null!;
    public static GameObject tragedyPrefab = null!;
    public static readonly Dictionary<EnemyAI, HauntedMaskItem> masks = new();

    public static void PopulateMaskedPlayerEnemyHelperInfo()
    {
        comedyPrefab = StartOfRound.Instance.allItemsList.itemsList.Find(item => { return (item.name == "ComedyMask"); }).spawnPrefab;
        tragedyPrefab = StartOfRound.Instance.allItemsList.itemsList.Find(item => { return (item.name == "TragedyMask"); }).spawnPrefab;
    }
}
