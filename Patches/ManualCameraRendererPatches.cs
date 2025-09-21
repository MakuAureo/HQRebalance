using HarmonyLib;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(ManualCameraRenderer))]
internal class ManualCameraRendererPatches
{
    [HarmonyPatch(nameof(ManualCameraRenderer.CheckIfPlayerIsInCaves))]
    [HarmonyPrefix]
    private static bool PreCheckIfPlayerIsInCaves(ManualCameraRenderer __instance)
    {
        return false;
    }
}
