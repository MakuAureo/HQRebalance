using HarmonyLib;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(CaveDwellerAI))]
internal class CaveDwellerAIPatches
{
    [HarmonyPatch(nameof(CaveDwellerAI.Start))]
    [HarmonyPostfix]
    private static void PostStart(CaveDwellerAI __instance)
    {
        __instance.enemyType.increasedChanceInterior = -1;
    }

    [HarmonyPatch(nameof(CaveDwellerAI.HitEnemy))]
    [HarmonyPrefix]
    private static void PreHitEnemy(CaveDwellerAI __instance, int force)
    {
        if (!__instance.inSpecialAnimation && __instance.currentBehaviourStateIndex != 0)
            __instance.enemyHP -= force - 1;
    }

    [HarmonyPatch(nameof(CaveDwellerAI.BabyUpdate))]
    [HarmonyPostfix]
    private static void PostBabyUpdate(CaveDwellerAI __instance)
    {
        //Patch to localy update hasPlayerFoundBaby to clients because ofc it is broken for client
        if (__instance.observingPlayer != null)
        {
            __instance.hasPlayerFoundBaby = true;
        }

        if (__instance.eatingScrap && !__instance.hasPlayerFoundBaby)
            __instance.eatingScrap = false;

        if (__instance.babyCrying && !__instance.hasPlayerFoundBaby)
        {
            __instance.SetCryingLocalClient(false);
            __instance.SetBabyCryingServerRpc(false);
        }
    }
}
