using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace HQRebalance;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class HQRebalance : BaseUnityPlugin
{
    public static HQRebalance Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private SelectableLevel[] patchedMoons = null!;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Patch();
        NetcodePatch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    private static void NetcodePatch()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length > 0)
                {
                    method.Invoke(null, null);
                }
            }
        }
    }


    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch()
    {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }

    internal void SetupMoons(StartOfRound instance)
    {
        if (Instance.patchedMoons != null)
        {
            instance.levels = Instance.patchedMoons;
            return;
        }

        /* Moon index
         * 0 : Exp
         * 1 : Ass
         * 2 : Vow
         * 4 : Mar
         * 5 : Ada
         * 6 : Ren
         * 7 : Din
         * 8 : Off
         * 9 : Tit
         * 10: Art
         * 12: Emb */

        Instance.patchedMoons = instance.levels;

        //Scrap range changes
        Instance.patchedMoons[6].minScrap = 21;

        Instance.patchedMoons[7].minScrap = 24;
        Instance.patchedMoons[7].maxScrap = 29;

        Instance.patchedMoons[8].minScrap = 18;
        Instance.patchedMoons[8].maxScrap = 22;

        Instance.patchedMoons[9].minScrap = 31;
        Instance.patchedMoons[9].maxScrap = 34;

        Instance.patchedMoons[10].minScrap = 32;
        Instance.patchedMoons[10].maxScrap = 37;

        //Size changes
        Instance.patchedMoons[2].factorySizeMultiplier = 1f;
        Instance.patchedMoons[10].factorySizeMultiplier = 2f;

        //Indoor chance changes
        Instance.patchedMoons[0].dungeonFlowTypes[0].rarity = 98;
        Instance.patchedMoons[0].dungeonFlowTypes[1].rarity = 1;
        Instance.patchedMoons[0].dungeonFlowTypes[2].rarity = 1;

        Instance.patchedMoons[1].dungeonFlowTypes[0].rarity = 49;
        Instance.patchedMoons[1].dungeonFlowTypes[1].rarity = 2;
        Instance.patchedMoons[1].dungeonFlowTypes[2].rarity = 49;

        Instance.patchedMoons[2].dungeonFlowTypes[0].rarity = 7;
        Instance.patchedMoons[2].dungeonFlowTypes[1].rarity = 1;
        Instance.patchedMoons[2].dungeonFlowTypes[2].rarity = 92;

        Instance.patchedMoons[5].dungeonFlowTypes[0].rarity = 13;
        Instance.patchedMoons[5].dungeonFlowTypes[1].rarity = 74;
        Instance.patchedMoons[5].dungeonFlowTypes[2].rarity = 13;

        Instance.patchedMoons[6].dungeonFlowTypes[1].rarity = 88;
        Instance.patchedMoons[6].dungeonFlowTypes[0].rarity = 2;
        Instance.patchedMoons[6].dungeonFlowTypes[2].rarity = 10;

        Instance.patchedMoons[7].dungeonFlowTypes[1].rarity = 10;
        Instance.patchedMoons[7].dungeonFlowTypes[0].rarity = 88;
        Instance.patchedMoons[7].dungeonFlowTypes[2].rarity = 2;

        Instance.patchedMoons[8].dungeonFlowTypes[0].rarity = 46;
        Instance.patchedMoons[8].dungeonFlowTypes[1].rarity = 8;
        Instance.patchedMoons[8].dungeonFlowTypes[2].rarity = 46;

        Instance.patchedMoons[9].dungeonFlowTypes[0].rarity = 2;
        Instance.patchedMoons[9].dungeonFlowTypes[1].rarity = 10;
        Instance.patchedMoons[9].dungeonFlowTypes[2].rarity = 88;

        Instance.patchedMoons[10].dungeonFlowTypes[0].rarity = 100;
        Instance.patchedMoons[10].dungeonFlowTypes[1].rarity = 100;
        Instance.patchedMoons[10].dungeonFlowTypes[2].rarity = 100;

        Instance.patchedMoons[12].dungeonFlowTypes[0].rarity = 76;
        Instance.patchedMoons[12].dungeonFlowTypes[1].rarity = 1;
        Instance.patchedMoons[12].dungeonFlowTypes[2].rarity = 23;

        //Change spawns
        Keyframe[] offDayKeys = new Keyframe[]
        {
                new Keyframe(0f, 7.097602f, -12.1866f, -12.1866f, 0.3333f, 0.119f),
                new Keyframe(0.4764f, -1.126f, -27.2869f,  -27.2869f, 0.1039f, 0.125f),
                new Keyframe(1f, -15f, 0f, 0f, 0f, 0f)
        };
        Instance.patchedMoons[8].daytimeEnemySpawnChanceThroughDay = new AnimationCurve(offDayKeys);
        Instance.patchedMoons[8].daytimeEnemiesProbabilityRange = 1f;
        Instance.patchedMoons[8].spawnProbabilityRange = 8f;

        Keyframe[] renKeys = new Keyframe[]
        {
                new Keyframe(0f, -3f, 19.70508f, 19.70508f, 0f, 0.2589182f),
                new Keyframe(0.3162474f, 2.817775f, 15.63821f, 15.63821f, 0.5307108f, 0.6788604f),
                new Keyframe(1f, 15f, 35.60403f, 35.60403f, 0.1551032f, 0f)
        };
        Instance.patchedMoons[6].enemySpawnChanceThroughoutDay = new AnimationCurve(renKeys);

        Keyframe[] titKeys = new Keyframe[]
        {
            new Keyframe(0f, -3f, 59.19933f, 59.19933f, 0f, 0.1882948f),
            new Keyframe(0.2971778f, 2.103194f, 11.64772f, 11.64772f, 0.6591896f, 0.404842f),
            new Keyframe(0.7631003f, 11.27556f, 25.28926f, 25.28926f, 0.4166799f, 0.5360636f),
            new Keyframe(1f, 15f, 35.60403f, 35.60403f, 0.04912584f, 0f)
        };
        Instance.patchedMoons[9].enemySpawnChanceThroughoutDay = new AnimationCurve(titKeys);

        Keyframe[] artKeys = new Keyframe[]
        {
                new Keyframe(0f, -3f, 15.1757f, 15.1757f, 0f, 0.3430422f),
                new Keyframe(0.1625553f, 1.142029f, 9.178675f, 9.178675f, 0.6591896f, 0.6591896f),
                new Keyframe(0.4976344f, 5.041142f, 6.361235f, 6.361235f, 0.3227605f, 0.3704014f),
                new Keyframe(0.6644974f, 6.988362f, 2.044127f, 2.044127f, 0.4955147f, 0.4224021f),
                new Keyframe(1f, 15f, 14.53138f, 14.53138f, 0.3106435f, 0f)
        };
        Instance.patchedMoons[10].enemySpawnChanceThroughoutDay = new AnimationCurve(artKeys);

        //Change indoor hazards
        AnimationCurve lowSpawn = Instance.patchedMoons[5].spawnableMapObjects[0].numberToSpawn;

        AnimationCurve artTurrets = Instance.patchedMoons[10].spawnableMapObjects[0].numberToSpawn;
        AnimationCurve artMines = Instance.patchedMoons[10].spawnableMapObjects[1].numberToSpawn;
        AnimationCurve artSpikes = Instance.patchedMoons[10].spawnableMapObjects[2].numberToSpawn;

        Instance.patchedMoons[6].spawnableMapObjects[0].numberToSpawn = lowSpawn;
        Instance.patchedMoons[6].spawnableMapObjects[1].numberToSpawn = artSpikes;

        Instance.patchedMoons[7].spawnableMapObjects[0].numberToSpawn = artMines;
        Instance.patchedMoons[7].spawnableMapObjects[1].numberToSpawn = lowSpawn;
        Instance.patchedMoons[7].spawnableMapObjects[2].numberToSpawn = lowSpawn;

        Instance.patchedMoons[9].spawnableMapObjects[0].numberToSpawn = artTurrets;
        Instance.patchedMoons[9].spawnableMapObjects[1].numberToSpawn = lowSpawn;
        Instance.patchedMoons[9].spawnableMapObjects[2].numberToSpawn = lowSpawn;

        //Lootpool changes
        Utility.LootPool expLoot = new Utility.LootPool(13, 0, 80, 29, 0, 2, 6, 0, 0, 13, 0, 0, 5, 32, 9, 10, 0, 42, 0, 17, 1, 0, 0, 22, 25, 80, 11, 0, 0, 66, 0, 0, 0, 0, 4, 8, 12, 0, 0, 8, 0, 0, 32, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90, 0, 6, 1);
        Utility.LootPool assLoot = new Utility.LootPool(19, 18, 59, 100, 26, 0, 6, 0, 0, 19, 15, 12, 49, 0, 36, 34, 0, 10, 39, 18, 0, 0, 7, 14, 23, 35, 8, 0, 21, 23, 12, 7, 0, 0, 7, 19, 10, 12, 13, 0, 0, 23, 19, 34, 32, 0, 19, 0, 31, 6, 0, 3, 40, 11, 17, 1);
        Utility.LootPool vowLoot = new Utility.LootPool(35, 33, 31, 54, 46, 0, 8, 51, 0, 35, 0, 12, 29, 0, 28, 39, 0, 30, 27, 20, 0, 0, 0, 8, 27, 25, 8, 0, 29, 16, 24, 11, 0, 0, 5, 22, 30, 19, 12, 0, 24, 22, 17, 40, 40, 0, 34, 0, 13, 7, 0, 0, 25, 6, 16, 1);
        Utility.LootPool marLoot = new Utility.LootPool(43, 0, 72, 48, 0, 2, 4, 0, 0, 43, 0, 11, 24, 0, 65, 0, 0, 42, 20, 24, 5, 0, 0, 0, 0, 49, 6, 0, 19, 42, 0, 6, 0, 0, 21, 27, 28, 4, 13, 0, 0, 19, 0, 29, 24, 0, 32, 0, 18, 9, 0, 0, 60, 0, 8, 1);
        Utility.LootPool offLoot = new Utility.LootPool(18, 0, 89, 63, 0, 0, 0, 0, 6, 18, 61, 40, 16, 0, 19, 0, 0, 40, 31, 19, 0, 0, 3, 17, 0, 67, 10, 0, 20, 65, 0, 8, 0, 0, 23, 19, 28, 0, 15, 0, 0, 0, 0, 27, 24, 11, 18, 0, 19, 9, 6, 61, 80, 0, 28, 1);
        Utility.LootPool adaLoot = new Utility.LootPool(24, 57, 31, 68, 36, 0, 9, 52, 0, 24, 0, 31, 40, 0, 50, 29, 0, 30, 13, 23, 0, 0, 0, 21, 28, 27, 17, 0, 25, 16, 13, 9, 0, 0, 19, 24, 32, 17, 24, 0, 25, 0, 6, 40, 32, 0, 40, 0, 17, 9, 0, 0, 29, 14, 16, 1);
        Utility.LootPool renLoot = new Utility.LootPool(19, 44, 0, 6, 0, 3, 38, 0, 12, 19, 63, 5, 0, 0, 21, 0, 85, 0, 7, 82, 0, 14, 93, 0, 4, 0, 13, 23, 35, 0, 18, 17, 85, 89, 0, 0, 9, 17, 0, 19, 16, 28, 0, 0, 11, 91, 61, 0, 52, 73, 94, 63, 0, 0, 0, 1);
        Utility.LootPool dinLoot = new Utility.LootPool(40, 47, 0, 18, 0, 3, 41, 0, 0, 40, 27, 0, 0, 0, 44, 0, 80, 0, 8, 41, 0, 62, 73, 0, 12, 12, 55, 24, 14, 0, 48, 37, 80, 67, 0, 0, 5, 50, 0, 67, 25, 0, 0, 0, 11, 56, 7, 21, 33, 94, 0, 27, 19, 0, 0, 1);
        Utility.LootPool titLoot = new Utility.LootPool(39, 37, 47, 33, 0, 0, 0, 0, 0, 39, 43, 0, 0, 0, 0, 0, 31, 0, 0, 23, 0, 33, 45, 0, 8, 38, 10, 16, 35, 0, 21, 22, 51, 14, 0, 0, 16, 38, 0, 36, 24, 0, 0, 0, 30, 21, 8, 36, 20, 35, 0, 46, 28, 0, 0, 1);
        Utility.LootPool artLoot = new Utility.LootPool(39, 76, 0, 17, 0, 0, 32, 0, 69, 39, 57, 0, 0, 0, 19, 0, 63, 0, 14, 24, 32, 55, 75, 0, 33, 16, 53, 30, 42, 0, 35, 25, 72, 33, 0, 0, 16, 20, 0, 65, 60, 39, 0, 0, 30, 53, 42, 42, 22, 71, 35, 57, 30, 0, 0, 12);
        Utility.LootPool embLoot = new Utility.LootPool(23, 0, 66, 52, 0, 0, 0, 0, 0, 18, 0, 14, 52, 0, 52, 0, 0, 0, 0, 17, 0, 0, 0, 9, 0, 81, 6, 0, 14, 100, 0, 8, 0, 0, 0, 0, 28, 0, 28, 0, 0, 26, 0, 28, 23, 12, 45, 0, 25, 43, 0, 0, 80, 0, 14, 1);

        Instance.patchedMoons[0].spawnableScrap = expLoot.GetLootpool();
        Instance.patchedMoons[1].spawnableScrap = assLoot.GetLootpool();
        Instance.patchedMoons[2].spawnableScrap = vowLoot.GetLootpool();
        Instance.patchedMoons[4].spawnableScrap = marLoot.GetLootpool();
        Instance.patchedMoons[8].spawnableScrap = offLoot.GetLootpool();
        Instance.patchedMoons[5].spawnableScrap = adaLoot.GetLootpool();
        Instance.patchedMoons[6].spawnableScrap = renLoot.GetLootpool();
        Instance.patchedMoons[7].spawnableScrap = dinLoot.GetLootpool();
        Instance.patchedMoons[9].spawnableScrap = titLoot.GetLootpool();
        Instance.patchedMoons[10].spawnableScrap = artLoot.GetLootpool();
        Instance.patchedMoons[12].spawnableScrap = embLoot.GetLootpool();

        //Daytime changes
        //March
        Instance.patchedMoons[4].DaytimeEnemies[0].rarity = 20;
        Instance.patchedMoons[4].DaytimeEnemies[2].rarity = 100;

        //Offense
        SpawnableEnemyWithRarity kiwiBird = new SpawnableEnemyWithRarity
        {
            enemyType = Resources.FindObjectsOfTypeAll<GiantKiwiAI>()[0].enemyType,
            rarity = 100
        };
        Instance.patchedMoons[8].DaytimeEnemies.Add(kiwiBird);

        //Enemy indoor changes
        Utility.EnemyPool renIndoor = new Utility.EnemyPool(5, 20, 0, 15, 5, 0, 15, 40, 0, 30, 20, 100, 0, 30, 50);
        Utility.EnemyPool dinIndoor = new Utility.EnemyPool(8, 30, 8, 4, 20, 3, 3, 5, 7, 10, 0, 8, 0, 4, 5);
        Utility.EnemyPool titIndoor = new Utility.EnemyPool(0, 0, 0, 60, 0, 60, 20, 60, 80, 60, 30, 60, 0, 60, 60);
        Utility.EnemyPool artIndoor = new Utility.EnemyPool(35, 91, 42, 77, 45, 92, 35, 100, 95, 92, 89, 100, 0, 100, 86);

        Instance.patchedMoons[6].Enemies = renIndoor.GetEnemyPool();
        Instance.patchedMoons[7].Enemies = dinIndoor.GetEnemyPool();
        Instance.patchedMoons[9].Enemies = titIndoor.GetEnemyPool();
        Instance.patchedMoons[10].Enemies = artIndoor.GetEnemyPool();

        //Enemy outdoor changes
        Utility.EnemyPool dinOutdoor = new Utility.EnemyPool(0, 0, 0, 50, 35, 35, 0, 0, 0, 0);
        Utility.EnemyPool titOutdoor = new Utility.EnemyPool(0, 0, 32, 80, 25, 0, 0, 0, 0, 0);

        Instance.patchedMoons[7].OutsideEnemies = dinOutdoor.GetEnemyPool();
        Instance.patchedMoons[9].OutsideEnemies = titOutdoor.GetEnemyPool();

        //Swap the levels for the actual patched moons
        instance.levels = Instance.patchedMoons;
    }
}
