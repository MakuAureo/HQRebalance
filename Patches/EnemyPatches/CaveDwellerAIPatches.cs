using HarmonyLib;
using Unity.Netcode;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(CaveDwellerAI))]
internal class CaveDwellerAIPatches
{
    [HarmonyPatch(nameof(CaveDwellerAI.HitEnemy))]
    [HarmonyPrefix]
    private static void PreHitEnemy(CaveDwellerAI __instance, int force)
    {
        if (!__instance.inSpecialAnimation && __instance.currentBehaviourStateIndex != 0)
            __instance.enemyHP -= force - 1;
    }

    [HarmonyPatch(nameof(CaveDwellerAI.BabyUpdate))]
    [HarmonyPrefix]
    private static void PreBabyUpdate(CaveDwellerAI __instance)
    {
        if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            return;

        if (__instance.eatingScrap && !__instance.hasPlayerFoundBaby)
            __instance.eatingScrap = false;

        if (__instance.babyCrying && !__instance.hasPlayerFoundBaby)
        {
            __instance.SetCryingLocalClient(setCrying: false);
            __instance.SetBabyCryingServerRpc(setCry: false);
        }
    }

    [HarmonyPatch(nameof(CaveDwellerAI.ScareBaby))]
    [HarmonyPrefix]
    private static bool PreScareBaby(CaveDwellerAI __instance)
    {
        if (!__instance.hasPlayerFoundBaby)
            return false;

        return true;
    }
}
