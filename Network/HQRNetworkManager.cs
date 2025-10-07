using Unity.Netcode;
using UnityEngine;
using static Unity.Netcode.XXHash;

namespace HQRebalance.Network;

internal class HQRNetworkManager : NetworkBehaviour
{
    private static GameObject prefab = null!;
    public static HQRNetworkManager Instance { get; private set; } = null!;

    private const bool default_tier3pass = false;
    public NetworkVariable<bool> tier3pass = new(default_tier3pass);

    private const int default_bottomLine = -1;
    public NetworkVariable<int> bottomLine = new(default_bottomLine); 

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

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void BuyTier3PassServerRpc(int groupCredits)
    {
        tier3pass.Value = true;
        Patches.TerminalHelper.terminal.groupCredits = groupCredits;
        Patches.TerminalHelper.terminal.SyncGroupCreditsClientRpc(groupCredits, Patches.TerminalHelper.terminal.numberOfItemsInDropship);
    }

    [Rpc(SendTo.Everyone)]
    public void GrabMaskEveryoneRpc(NetworkObjectReference maskedPlayerEnemyNetObjRef, NetworkObjectReference maskItemNetObjRef, int maskValue = 40)
    {
        if (!maskedPlayerEnemyNetObjRef.TryGet(out NetworkObject maskedPlayerEnemy))
        {
            HQRebalance.Logger.LogError("TryGet maskedPlayerEnemy from NetObjRef failed");
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
        MaskedPlayerEnemy masked = maskedPlayerEnemy.GetComponent<MaskedPlayerEnemy>();
        if (masked == null)
        {
            HQRebalance.Logger.LogError("Masked in GrabMask function did not have MaskedPlayerEnemy component.");
            return;
        }

        if (Patches.MaskedPlayerEnemyHelper.masks.TryGetValue(masked, out _))
        {
            HQRebalance.Logger.LogWarning("Duplicate Masked entry... skipping");
            return;
        }

        masked.maskTypes[0].SetActive(value: false);
        masked.maskTypes[1].SetActive(value: false);

        mask.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
        mask.SetScrapValue(maskValue);
        mask.isHeldByEnemy = true;
        mask.grabbableToEnemies = false;
        mask.grabbable = false;

        Patches.HauntedMaskItemInfo maskInfo = new()
        {
            mask = mask,
            hasBeenHeld = false
        };
        Patches.MaskedPlayerEnemyHelper.masks[masked] = maskInfo;
    }
}
