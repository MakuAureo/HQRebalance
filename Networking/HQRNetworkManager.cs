using Unity.Netcode;
using UnityEngine;
using static Unity.Netcode.XXHash;

namespace HQRebalance.Networking;

internal class HQRNetworkManager : NetworkBehaviour
{
    private static GameObject prefab = null!;
    public static HQRNetworkManager Instance { get; private set; } = null!;

    private const bool default_tier3pass = false;
    public NetworkVariable<bool> tier3pass = new(default_tier3pass);

    private const int default_bottomLine = -1;
    public NetworkVariable<int> bottomLine = new(default_bottomLine); 

    private const int default_itemCount = 0;
    public NetworkVariable<int> itemCount = new(default_itemCount);

    public static void CreateAndRegisterPrefab()
    {
        if (prefab != null)
            return;

        prefab = new GameObject(MyPluginInfo.PLUGIN_GUID + " Prefab");
        prefab.hideFlags |= HideFlags.HideAndDontSave;
        NetworkObject networkObject = prefab.AddComponent<NetworkObject>();
        networkObject.GlobalObjectIdHash = prefab.name.Hash32();
        prefab.AddComponent<HQRNetworkManager>();
        NetworkManager.Singleton.AddNetworkPrefab(prefab);

        HQRebalance.Logger.LogInfo("Network prefab created and registered");
    }

    public static void SpawnNetworkHandler()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            Object.Instantiate(prefab).GetComponent<NetworkObject>().Spawn();
            HQRebalance.Logger.LogInfo("Network handler spawned");
        }
    }

    public static void DespawnNetworkHandler()
    {
        if (Instance != null && Instance.gameObject.GetComponent<NetworkObject>().IsSpawned && (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
        {
            Instance.gameObject.GetComponent<NetworkObject>().Despawn();
            HQRebalance.Logger.LogInfo("Network handler despawned");
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void BuyTier3PassServerRpc()
    {
        tier3pass.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncTerminalCreditsServerRpc(int groupCredits)
    {
        Patches.TerminalHelper.terminal.groupCredits = groupCredits;
        Patches.TerminalHelper.terminal.SyncGroupCreditsClientRpc(groupCredits, Patches.TerminalHelper.terminal.numberOfItemsInDropship);
    }

    [ClientRpc]
    public void GrabMaskClientRpc(NetworkObjectReference maskedEnemyAINetObjRef, NetworkObjectReference maskItemNetObjRef)
    {
        if (!maskedEnemyAINetObjRef.TryGet(out NetworkObject maskedEnemyAI))
        {
            HQRebalance.Logger.LogError("TryGet maskedEnemyAI from NetObjRef failed");
            return;
        }
        if (!maskItemNetObjRef.TryGet(out NetworkObject maskItem))
        {
            HQRebalance.Logger.LogError("TryGet maskItem from NetObjRef failed");
            return;
        }

        HauntedMaskItem mask = maskItem.GetComponent<HauntedMaskItem>();
        if (mask == null)
        {
            HQRebalance.Logger.LogError("Mask in GrabMask function did not have HauntedMaskItem component.");
            return;
        }
        MaskedPlayerEnemy masked = maskedEnemyAI.GetComponent<MaskedPlayerEnemy>();
        if (masked == null)
        {
            HQRebalance.Logger.LogError("Masked in GrabMask function did not have MaskedPLayerEnemy component.");
            return;
        }

        Patches.MaskedPlayerEnemyHelper.masks[masked] = mask;
        masked.maskTypes[0].SetActive(value: false);
        mask.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);

        mask.SetScrapValue(45);
        mask.isHeldByEnemy = true;
        mask.grabbableToEnemies = false;
        mask.grabbable = false;
    }

    [ClientRpc]
    public void DropMaskClientRpc(NetworkObjectReference maskedEnemyAINetObjRef, NetworkObjectReference maskItemNetObjRef)
    {
        if (!maskedEnemyAINetObjRef.TryGet(out NetworkObject maskedEnemyAI))
        {
            HQRebalance.Logger.LogError("TryGet maskedEnemyAI from NetObjRef failed");
            return;
        }
        if (!maskItemNetObjRef.TryGet(out NetworkObject maskItem))
        {
            HQRebalance.Logger.LogError("TryGet maskItem from NetObjRef failed");
            return;
        }

        HauntedMaskItem mask = maskItem.GetComponent<HauntedMaskItem>();
        if (mask == null)
        {
            HQRebalance.Logger.LogError("Mask in GrabMask function did not have HauntedMaskItem component.");
            return;
        }
        MaskedPlayerEnemy masked = maskedEnemyAI.GetComponent<MaskedPlayerEnemy>();
        if (masked == null)
        {
            HQRebalance.Logger.LogError("Masked in GrabMask function did not have MaskedPLayerEnemy component.");
            return;
        }

        mask.isHeldByEnemy = false;
        mask.grabbableToEnemies = true;
        mask.grabbable = true;
    }
}
