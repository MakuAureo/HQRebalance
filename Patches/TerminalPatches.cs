using HarmonyLib;
using UnityEngine;

namespace HQRebalance.Patches;

[HarmonyPatch(typeof(Terminal))]
internal class TerminalPatches
{
    [HarmonyPatch(nameof(Terminal.ParsePlayerSentence))]
    [HarmonyPrefix]
    static void ParsePlayerSentencePrePatch(Terminal __instance)
    {
        string str = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (char c in str)
        {
            if (!char.IsPunctuation(c))
                sb.Append(c);
        }
        str = sb.ToString().ToLower();

        TerminalHelper.terminalInput = str;
    }

    [HarmonyPatch(nameof(Terminal.Start))]
    [HarmonyPostfix]
    private static void PostStart(Terminal __instance)
    {
        TerminalHelper.terminal = Object.FindObjectOfType<Terminal>();

        TerminalNode artificeNode = __instance.terminalNodes.allKeywords[27].compatibleNouns[10].result;
        TerminalNode rendNode = __instance.terminalNodes.allKeywords[27].compatibleNouns[5].result;
        TerminalNode dineNode = __instance.terminalNodes.allKeywords[27].compatibleNouns[6].result;
        TerminalNode titanNode = __instance.terminalNodes.allKeywords[27].compatibleNouns[9].result;

        artificeNode.itemCost = 3000;
        artificeNode.terminalOptions[1].result.itemCost = 3000;

        rendNode.itemCost = 0;
        rendNode.terminalOptions[1].result.itemCost = 0;
        rendNode.displayText = rendNode.displayText.Replace("The cost to route to 85-Rend is [totalCost]", "No additional cost required to route to 85-Rend");

        dineNode.itemCost = 0;
        dineNode.terminalOptions[1].result.itemCost = 0;
        dineNode.displayText = dineNode.displayText.Replace("The cost to route to 7-Dine is [totalCost]", "No additional cost required to route to 7-Dine");

        titanNode.itemCost = 0;
        titanNode.terminalOptions[1].result.itemCost = 0;
        titanNode.displayText = titanNode.displayText.Replace("The cost to route to 8-Titan is [totalCost]", "No additional cost required to route to 8-Titan");
    }

    [HarmonyPatch(nameof(Terminal.LoadNewNode))]
    [HarmonyPrefix]
    private static void PreLoadNewNode(Terminal __instance, ref TerminalNode node)
    {
        if ((node.name == "85route" || node.name == "7route" || node.name == "8route") && !Networking.HQRNetworkManager.Instance.tier3pass.Value)
        {
            if (node.name == "85route")
                TerminalHelper.moon = "85-Rend";
            else if (node.name == "7route")
                TerminalHelper.moon = "7-Dine";
            else if (node.name == "8route")
                TerminalHelper.moon = "8-Titan";

            __instance.totalCostOfItems = TerminalHelper.TerminalNodes.buyPass.itemCost;
            node = TerminalHelper.TerminalNodes.buyPass;

        }
    }

    [HarmonyPatch(nameof(Terminal.TextPostProcess))]
    [HarmonyPrefix]
    private static void PreTextPostProcess(Terminal __instance, ref string modifiedDisplayText)
    {
        if (modifiedDisplayText.Contains("[scanForItems]"))
        {
            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 91);
            int objCount = 0;
            int objValue = 0;
            GrabbableObject[] array = Object.FindObjectsOfType<GrabbableObject>();
            foreach (GrabbableObject obj in array)
            {
                if (obj.itemProperties.isScrap && !obj.isInShipRoom && !obj.isInElevator)
                {
                    objValue += obj.scrapValue;
                    objCount++;
                }
            }
            objValue += 35 * ButlerEnemyAIPatches.knifeCount;
            objCount += ButlerEnemyAIPatches.knifeCount;
            int mult = (objCount > 5) ? 2 : 1;
            objValue = mult * (int)((double)objValue * (0.1f * random.NextDouble() + 0.95f));
            modifiedDisplayText = modifiedDisplayText.Replace("[scanForItems]", $"There are {objCount} objects outside the ship, totalling at an approximate value of ${objValue}.");
            return;
        }

        if (modifiedDisplayText.Contains(TerminalHelper.passConfirmString))
        {
            modifiedDisplayText = modifiedDisplayText.Replace(TerminalHelper.passConfirmString, TerminalHelper.PassConfirmDisplay());
        }
        if (modifiedDisplayText.Contains("[coldMoon]"))
        {
            modifiedDisplayText = modifiedDisplayText.Replace("[coldMoon]", TerminalHelper.moon);
        }
    }
}

internal static class TerminalHelper
{
    private const int passCost = 610;
    public const string passConfirmString = "[passConfirm]";

    public static Terminal terminal = null!;
    public static string terminalInput = null!;
    public static string moon = null!;

    public class TerminalNodes
    {
        public static TerminalNode passConfirm = new()
        {
            acceptAnything = false,
            buyItemIndex = -1,
            buyRerouteToMoon = -1,
            buyUnlockable = false,
            buyVehicleIndex = -1,
            clearPreviousText = true,
            creatureFileID = -1,
            creatureName = "",
            displayPlanetInfo = -1,
            displayText = passConfirmString,
            displayTexture = null,
            displayVideo = null,
            hideFlags = UnityEngine.HideFlags.None,
            isConfirmationNode = false,
            itemCost = passCost,
            loadImageSlowly = false,
            maxCharactersToType = 35,
            name = "passConfirm",
            overrideOptions = false,
            persistentImage = false,
            playClip = null,
            playSyncedClip = 0,
            returnFromStorage = false,
            shipUnlockableID = -1,
            storyLogFileID = -1,
            terminalEvent = "",
            terminalOptions = { }
        };

        public static TerminalNode passDeny = new()
        {
            acceptAnything = false,
            buyItemIndex = -1,
            buyRerouteToMoon = -1,
            buyUnlockable = false,
            buyVehicleIndex = -1,
            clearPreviousText = false,
            creatureFileID = -1,
            creatureName = "",
            displayPlanetInfo = -1,
            displayText = "\nCold Moon Pass purchase canceled.\n\n",
            displayTexture = null,
            displayVideo = null,
            hideFlags = UnityEngine.HideFlags.None,
            isConfirmationNode = false,
            itemCost = 0,
            loadImageSlowly = false,
            maxCharactersToType = 35,
            name = "passDeny",
            overrideOptions = false,
            persistentImage = false,
            playClip = null,
            playSyncedClip = -1,
            returnFromStorage = false,
            shipUnlockableID = -1,
            storyLogFileID = -1,
            terminalEvent = "",
            terminalOptions = { }
        };

        public static TerminalNode buyPass = new()
        {
            acceptAnything = false,
            buyItemIndex = -1,
            buyRerouteToMoon = -1,
            buyUnlockable = false,
            buyVehicleIndex = -1,
            clearPreviousText = true,
            creatureFileID = -1,
            creatureName = "",
            displayPlanetInfo = -1,
            displayText = "You have requested to route to [coldMoon] without the Cold Moon Pass.\nYou must purchase a Cold Moon Pass first.\n\nThe Cold Moon Pass is a one time fee required to travel to 85-Rend, 7-Dine and 8-Titan. Once purchased, the crew will be able to freely travel to any of the cold moons for the duration of the current quota, free of additional expenses!\nCost: [totalCost].\n\nPlease CONFIRM or DENY.\n\n",
            displayTexture = null,
            displayVideo = null,
            hideFlags = UnityEngine.HideFlags.None,
            isConfirmationNode = true,
            itemCost = passCost,
            loadImageSlowly = false,
            maxCharactersToType = 35,
            name = "buyPass",
            overrideOptions = true,
            persistentImage = false,
            playClip = null,
            playSyncedClip = -1,
            returnFromStorage = false,
            shipUnlockableID = -1,
            storyLogFileID = -1,
            terminalEvent = "",
            terminalOptions = new CompatibleNoun[2]
            {
                new()
                {
                    noun = terminal.terminalNodes.allKeywords[3],
                    result = passConfirm
                },
                new()
                {
                    noun = terminal.terminalNodes.allKeywords[4],
                    result = passDeny
                }
            }
        };
    }

    public static string PassConfirmDisplay()
    {
        if (terminal.groupCredits < TerminalNodes.passConfirm.itemCost)
        {
            TerminalNodes.passConfirm.playSyncedClip = 1;
            return "Unable to purchase Cold Moon Pass.\n\n";
        }

        Networking.HQRNetworkManager.Instance.BuyTier3PassServerRpc();
        Networking.HQRNetworkManager.Instance.SyncTerminalCreditsServerRpc(terminal.groupCredits - TerminalNodes.passConfirm.itemCost);
        TerminalNodes.passConfirm.playSyncedClip = 0;
        return "Cold Moon Pass purchased.\n\n";
    }
}
