using Unity.Netcode;
using UnityEngine;
using static Unity.Netcode.XXHash;

namespace HQRebalance.Networking;

internal class HQRNetworkManager : NetworkBehaviour
{
    private static GameObject prefab = null!;
    public static HQRNetworkManager Instance { get; private set; } = null!;

    private const bool default_tier3pass = false;
    private const int default_bottomLine = -1;
    public NetworkVariable<bool> tier3pass = new(default_tier3pass);
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
}
