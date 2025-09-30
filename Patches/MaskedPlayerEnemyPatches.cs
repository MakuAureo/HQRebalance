using System.Collections.Generic;
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
            //UPDATE THIS LATER
            GameObject maskObj = Object.Instantiate(Patches.MaskedPlayerEnemyHelper.comedyPrefab, __instance.transform.position, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);
            NetworkObject maskNetObj = maskObj.GetComponent<NetworkObject>();
            maskNetObj.Spawn();

            NetworkObject maskedNetObj = __instance.GetComponent<NetworkObject>();
            Networking.HQRNetworkManager.Instance.GrabMaskClientRpc(maskedNetObj, maskNetObj);
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
            HQRebalance.Logger.LogError("Could not find mask to update");

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
            HQRebalance.Logger.LogError("Could not find mask to drop");
    }
}

internal static class MaskedPlayerEnemyHelper
{
    public static GameObject comedyPrefab = null!;
    public static GameObject tragedyPrefab = null!;
    public static readonly Dictionary<EnemyAI, HauntedMaskItem> masks = new();

    public static void PopulateMaskPrefabs()
    {
        comedyPrefab = StartOfRound.Instance.allItemsList.itemsList.Find(item => { return (item.name == "ComedyMask"); }).spawnPrefab;
        tragedyPrefab = StartOfRound.Instance.allItemsList.itemsList.Find(item => { return (item.name == "TragedyMask"); }).spawnPrefab;
    }
}
