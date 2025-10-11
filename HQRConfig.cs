using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using HarmonyLib;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;

namespace HQRebalance;

internal class HQRConfig
{
    public readonly Presets presetToUse;
    public readonly ConfigEntry<Presets> preset;

    public readonly ConfigEntry<bool> fireExitPatch;

    public readonly ConfigEntry<bool> butlerPatches;
    public readonly ConfigEntry<bool> addKnifeIcon;
    public readonly ConfigEntry<bool> disableStealthStab;

    public readonly ConfigEntry<bool> maneaterPatches;
    public readonly ConfigEntry<bool> applyNomalDamage;
    public readonly ConfigEntry<bool> cannotCryOrEatBeforeSeeingPlayer;
    public readonly ConfigEntry<bool> disableIncreasedSpawnChance;

    public readonly ConfigEntry<bool> jesterPatches;
    public readonly ConfigEntry<bool> disableSolidHitbox;
    public readonly ConfigEntry<float> pushForce;
    public readonly ConfigEntry<bool> scaleFollowTimerWithInteriorSize;
    public readonly ConfigEntry<float> followTimerScaling;

    public readonly ConfigEntry<bool> maskedPatches;
    public readonly ConfigEntry<bool> useMaskItem;
    public readonly ConfigEntry<int> maskValue;

    public readonly ConfigEntry<bool> mineshaftPatch;
    public readonly ConfigEntry<float> caveSize;
    public readonly ConfigEntry<float> facilityDelta;
    public readonly ConfigEntry<float> mapTileSize;

    public readonly ConfigEntry<bool> disableSingleItemDay;

    public readonly ConfigEntry<bool> difficultyScalingPatch;
    public readonly ConfigEntry<float> quotaScalingFactor;

    public readonly ConfigEntry<bool> infestationPatch;
    public readonly Dictionary<EnemyType, ConfigEntry<bool>> selectableEnemies;
    public readonly ConfigEntry<int> baseChance;
    public readonly ConfigEntry<int> boostedChance;
    public readonly ConfigEntry<int> daysLootedInARow;
    public readonly ConfigEntry<int> lootThreshold;

    public readonly ConfigEntry<bool> moonPatches;

    public readonly ConfigEntry<bool> tier3passPatch;
    public readonly ConfigEntry<int> tier3passPrice;
    public readonly ConfigEntry<int> artPrice;

    public readonly ConfigEntry<bool> luckPatch;
    public readonly ConfigEntry<LuckType> luckSystem;

    public readonly ConfigEntry<bool> disableCavesSignalPatch;

    public readonly ConfigEntry<bool> playerMovementPatches;
    public readonly ConfigEntry<bool> usePreV64GroundColision;
    public readonly ConfigEntry<float> speedLostToWaterCaves;

    public bool? lethalConfigLoaded;
    public bool? fairerFireExitsLoaded;

    public HQRConfig(ConfigFile cfg, EnemyType[] allEnemies)
    {
        if (lethalConfigLoaded == null)
            lethalConfigLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(HQRebalance.LethalConfigGUID);

        if (fairerFireExitsLoaded == null)
            fairerFireExitsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(HQRebalance.FairerFireExitsGUID);

        cfg.SaveOnConfigSet = false;

        preset = cfg.Bind(
                "General",
                "Preset",
                Presets.Default,
                "General patches to apply takes priority over single patches\nDefault: intended experience for this mod, meant to be used in a mostly vanilla setting\nCustom: choose whatever patches you want to use"
                );

        fireExitPatch = cfg.Bind(
                "SystemPatches.FireExit",
                "Fire Exit Patch",
                true,
                "Use the custom fire exit spawning logic, this is automatically overwritten if you are using the FairerFireExits mod"
                );

        butlerPatches = cfg.Bind(
                "EnemyPatches.Butler",
                "Butler Patches",
                true,
                "Use all Butler patches with their default values, overwrites all other configs in this section"
                );

        addKnifeIcon = cfg.Bind(
                "EnemyPatches.Butler",
                "Add Knife Icon",
                true,
                "Live butlers will have a radar icon for their knives and contribute to the objects outside"
                );

        disableStealthStab = cfg.Bind(
                "EnemyPatches.Butler",
                "Disable Steath Stab",
                true,
                "Bring back butler's v50 behavior, they won't randomly attack players on contact"
                );

        maneaterPatches = cfg.Bind(
                "EnemyPatches.Maneater",
                "Maneater Patches",
                true,
                "Use all Maneater patches with their default values, overwrites all other configs in this section"
                );

        applyNomalDamage = cfg.Bind(
                "EnemyPatches.Maneater",
                "Apply Normal Damage",
                true,
                "Sources of damage that deal more than 1 point of damage will hit the maneater for more than 1 hp"
                );

        cannotCryOrEatBeforeSeeingPlayer = cfg.Bind(
                "EnemyPatches.Maneater",
                "Cannot Cry Or Eat Before Seeing Player",
                true,
                "Maneater won't start crying or eat scrap before it has spoted a player at least once"
                );

        disableIncreasedSpawnChance = cfg.Bind(
                "EnemyPatches.Maneater",
                "Disable Increased Spawn Chance",
                true,
                "Disable the extra chance of maneater spawns when interior is mineshaft"
                );

        jesterPatches = cfg.Bind(
                "EnemyPatches.Jester",
                "Jester Patches",
                true,
                "Use all Jester patches with their default values, overwrites all other configs in this section"
                );

        disableSolidHitbox = cfg.Bind(
                "EnemyPatches.Jester",
                "Disable Solid Hitbox",
                true,
                "Disables Jester's solid hitbox allowing you to walk through it"
                );

        pushForce = cfg.Bind(
                "EnemyPatches.Jester",
                "Push Force",
                6.5f,
                new ConfigDescription("Push force of the trigger hitbox, 0 is no push and 7.0 is a bit higher than Butler's push force", new AcceptableValueRange<float>(0f, 7f))
                );

        scaleFollowTimerWithInteriorSize = cfg.Bind(
                "EnemyPatches.Jester",
                "Scale Follow Timer With Interior Size",
                true,
                "Use interior size to scale the Jester's follow timer"
                );

        followTimerScaling = cfg.Bind(
                "EnemyPatches.Jester",
                "Follow Timer Scaling",
                1f,
                new ConfigDescription("How much the interior size should impact the Jester's follow timer", new AcceptableValueRange<float>(0f, 1f))
                );

        maskedPatches = cfg.Bind(
                "EnemyPatches.Masked",
                "Masked Patches",
                true,
                "Use all Masked patches with their default values, overwrites all other configs in this section"
                );

        useMaskItem = cfg.Bind(
                "EnemyPatches.Masked",
                "Use Mask Item",
                true,
                "Spawn a mask item that can be picked up after killing the masked instead of using a mesh"
                );

        maskValue = cfg.Bind(
                "EnemyPatches.Masked",
                "Mask Value",
                40,
                new ConfigDescription("Value of the mask item", new AcceptableValueRange<int>(28, 51))
                );

        mineshaftPatch = cfg.Bind(
                "DungenPatches.Mineshaft",
                "Mineshaft Patch",
                true,
                "Change how mineshaft generates"
                );

        caveSize = cfg.Bind(
                "DungenPatches.Mineshaft",
                "Cave Size",
                0.65f,
                new ConfigDescription("% of the map that spaws cave tiles\nWARNING: RASING THIS NUMBER ABOVE 80% MIGHT STOP FIRE EXIT FROM SPAWNING", new AcceptableValueRange<float>(0.05f, 1f))
                );

        facilityDelta = cfg.Bind(
                "DungenPatches.Mineshaft",
                "Facility Delta",
                0.05f,
                new ConfigDescription("How much bigger %-wise 2nd facility is than the first", new AcceptableValueRange<float>(0f, 0.1f))
                );

        mapTileSize = cfg.Bind(
                "DungenPatches.Mineshaft",
                "Map Tile Size",
                1.1f,
                new ConfigDescription("Shrink factor for Mineshaft\nVanilla is 0.9, meaning the interior expands from the actual size", new AcceptableValueRange<float>(0.8f, 1.2f))
                );

        disableSingleItemDay = cfg.Bind(
                "GameSystemPatch.SingleItemDays",
                "Disable Single Item Day",
                true,
                "Disable SIDs from happening"
                );

        difficultyScalingPatch = cfg.Bind(
                "GameSystemPatch.DifficultyScaling",
                "Difficulty Scaling Patch",
                true,
                "Use quotas fulfilled difficulty scaling instead of vanilla's days completed in the current quota difficulty scaling"
                );

        quotaScalingFactor = cfg.Bind(
                "GameSystemPatch.DifficultyScaling",
                "Quota Scaling Factor",
                0.2f,
                new ConfigDescription("How much quotas fulfilled should increase spawns", new AcceptableValueRange<float>(0f, 1f))
                );

        infestationPatch = cfg.Bind(
                "GameSystemPatch.Infestation",
                "Infestation Patch",
                true,
                "Enable custom infestation event"
                );

        selectableEnemies = new();
        foreach (EnemyType enemy in allEnemies)
        {
            if (enemy.isDaytimeEnemy || enemy.isOutsideEnemy)
                continue;

            if (selectableEnemies.TryGetValue(enemy, out _))
            {
                HQRebalance.Logger.LogWarning($"{enemy.enemyName} has duplicate entry... skipping");
                continue;
            }

            ConfigEntry<bool> enemyConfig = cfg.Bind(
                    "GameSystemPatch.Infestation.InfestationTargets",
                    $"Can Choose {enemy.enemyName}",
                    enemy.enemyName == "Nutcracker" || enemy.enemyName == "Butler" || enemy.enemyName == "Masked",
                    "Can the custom infestation code select the current enemy as the main enemy for an infestation event\nWARNING: Checking this for enemies that should only spawn once WILL make them spawn with virtually unlimited spawns"
                    );

            selectableEnemies[enemy] = enemyConfig;
        }

        baseChance = cfg.Bind(
                "GameSystemPatch.Infestation",
                "Base Chance",
                4,
                new ConfigDescription("Base % chance of an infestation any day", new AcceptableValueRange<int>(0, 100))
                );

        boostedChance = cfg.Bind(
                "GameSystemPatch.Infestation",
                "Boosted Chance",
                20,
                new ConfigDescription("Boosted % chance of an infestation any day after the clear threshold has been reached for couple days in a row", new AcceptableValueRange<int>(0, 100))
                );

        daysLootedInARow = cfg.Bind(
                "GameSystemPatch.Infestation",
                "Days Looted In a Row",
                3,
                new ConfigDescription("How many days that reached the clear threshold in row are needed to boost infestation chances", new AcceptableValueRange<int>(1, 9))
                );

        lootThreshold = cfg.Bind(
                "GameSystemPatch.Infestation",
                "Loot Threshold",
                85,
                new ConfigDescription("How much % of the total scrap available is needed to add one more day towards boosted infestation progress", new AcceptableValueRange<int>(0, 100))
                );

        moonPatches = cfg.Bind(
                "MoonPatches",
                "Moon Patches",
                true,
                "Use HQR's modified values for item count, enemy pool and loot pool"
                );

        tier3passPatch = cfg.Bind(
                "TerminalPatch.Routing",
                "Cold Moon Pass Patch",
                true,
                "Use the Cold Moon Pass system"
                );

        tier3passPrice = cfg.Bind(
                "TerminalPatch.Routing",
                "Cold Moon Pass Price",
                610,
                new ConfigDescription("Set the Cold Moon Pass price", new AcceptableValueRange<int>(500, 700))
                );

        artPrice = cfg.Bind(
                "TerminalPatch.Routing",
                "Artifice Price",
                3000,
                new ConfigDescription("Set Artifice's routing cost", new AcceptableValueRange<int>(1500, 4500))
                );

        luckPatch = cfg.Bind(
                "GameSystemPatch.Luck",
                "Luck Patch",
                true,
                "Use the revamped luck system"
                );

        luckSystem = cfg.Bind(
                "GameSystemPatch.Luck",
                "Luck System",
                LuckType.High,
                "If luck increases or decreases quota rolls"
                );

        disableCavesSignalPatch = cfg.Bind(
                "ShipPatch.ShipMonitor",
                "Disable Caves No Signal",
                true,
                "Disable the NO SIGNAL screen when the player being watched goes into caves"
                );

        playerMovementPatches = cfg.Bind(
                "PlayerPatch.PlayerMovement",
                "Player Movement Patches",
                true,
                "Use all Player Movement patches with their default values, overwrites all other configs in this section"
                );

        usePreV64GroundColision = cfg.Bind(
                "PlayerPatch.PlayerMovement",
                "Use Pre v64 Ground Colision",
                true,
                "Use the old (pre v64) ground colision code that allowed players to climb ledges"
                );

        speedLostToWaterCaves = cfg.Bind(
                "PlayerPatch.PlayerMovement",
                "Speed Lost To Water Caves",
                0f,
                new ConfigDescription("How much speed is lost when crouching in water caves, 0 is for no speed penalty and 1 is vanilla behavior", new AcceptableValueRange<float>(0f, 1f))
                );

        ClearOrphanedEntries(cfg);
        cfg.Save();
        cfg.SaveOnConfigSet = true;
        presetToUse = preset.Value;

        if (lethalConfigLoaded.Value)
        {
            AddLethalConfigItems();
            ConfigLethalConfigModEntry();
        }
    }

    static void ClearOrphanedEntries(ConfigFile cfg)
    {
        PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
        orphanedEntries.Clear();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void AddLethalConfigItems()
    {
        EnumDropDownConfigItem<Presets> presetConfig = new(preset, new EnumDropDownOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
        LethalConfig.LethalConfigManager.AddConfigItem(presetConfig);

        BoolCheckBoxConfigItem fireExitPatchConfig = new(fireExitPatch, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
		LethalConfig.LethalConfigManager.AddConfigItem(fireExitPatchConfig);

        BoolCheckBoxConfigItem butlerPatchesConfig = new(butlerPatches, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        BoolCheckBoxConfigItem addKnifeIconConfig = new(addKnifeIcon, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        BoolCheckBoxConfigItem disableStealthStabConfig = new(disableStealthStab, false);
		LethalConfig.LethalConfigManager.AddConfigItem(butlerPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(addKnifeIconConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(disableStealthStabConfig);

        BoolCheckBoxConfigItem maneaterPatchesConfig = new(maneaterPatches, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        BoolCheckBoxConfigItem applyNomalDamageConfig = new(applyNomalDamage, false);
        BoolCheckBoxConfigItem cannotCryOrEatBeforeSeeingPlayerConfig = new(cannotCryOrEatBeforeSeeingPlayer, false);
        BoolCheckBoxConfigItem disableIncreasedSpawnChanceConfig = new(disableIncreasedSpawnChance, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(maneaterPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(applyNomalDamageConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(cannotCryOrEatBeforeSeeingPlayerConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(disableIncreasedSpawnChanceConfig);

        BoolCheckBoxConfigItem jesterPatchesConfig = new(jesterPatches, false);
        BoolCheckBoxConfigItem disableSolidHitboxConfig = new(disableSolidHitbox, false);
        FloatSliderConfigItem pushForceConfig = new(pushForce, new FloatSliderOptions { Min = 0f, Max = 7f , RequiresRestart = false });
        BoolCheckBoxConfigItem scaleFollowTimerWithInteriorSizeConfig = new(scaleFollowTimerWithInteriorSize, false);
        FloatSliderConfigItem followTimerScalingConfig = new(followTimerScaling, new FloatSliderOptions { Min = 0f, Max = 1f, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(jesterPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(disableSolidHitboxConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(pushForceConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(scaleFollowTimerWithInteriorSizeConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(followTimerScalingConfig);

        BoolCheckBoxConfigItem maskedPatchesConfig = new(maskedPatches, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        BoolCheckBoxConfigItem useMaskItemConfig = new(useMaskItem, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        IntSliderConfigItem maskValueConfig = new(maskValue, new IntSliderOptions { Min = 28, Max = 51, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
		LethalConfig.LethalConfigManager.AddConfigItem(maskedPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(useMaskItemConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(maskValueConfig);

        BoolCheckBoxConfigItem mineshaftPatchConfig = new(mineshaftPatch, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        FloatSliderConfigItem caveSizeConfig = new(caveSize, new FloatSliderOptions { Min = 0.05f, Max = 1f, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
        FloatSliderConfigItem facilityDeltaConfig = new(facilityDelta, new FloatSliderOptions { Min = 0f, Max = 0.1f, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
        FloatSliderConfigItem mapTileSizeConfig = new(mapTileSize, new FloatSliderOptions { Min = 0.8f, Max = 1.2f, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
		LethalConfig.LethalConfigManager.AddConfigItem(mineshaftPatchConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(caveSizeConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(facilityDeltaConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(mapTileSizeConfig);

        BoolCheckBoxConfigItem disableSingleItemDayConfig = new(disableSingleItemDay, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
		LethalConfig.LethalConfigManager.AddConfigItem(disableSingleItemDayConfig);

        BoolCheckBoxConfigItem difficultyScalingPatchConfig = new(difficultyScalingPatch, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
        FloatSliderConfigItem quotaScalingFactorConfig = new(quotaScalingFactor, new FloatSliderOptions { Min = 0f, Max = 1f, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
		LethalConfig.LethalConfigManager.AddConfigItem(difficultyScalingPatchConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(quotaScalingFactorConfig);

        BoolCheckBoxConfigItem infestationPatchConfig = new(infestationPatch, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
		LethalConfig.LethalConfigManager.AddConfigItem(infestationPatchConfig);
        foreach (KeyValuePair<EnemyType, ConfigEntry<bool>> enemyEntryPair in selectableEnemies)
        {
            BoolCheckBoxConfigItem enemyEntryConfig = new(enemyEntryPair.Value, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
            LethalConfig.LethalConfigManager.AddConfigItem(enemyEntryConfig);
        }
        IntSliderConfigItem baseChanceConfig = new(baseChance, new IntSliderOptions { Min = 0, Max = 100, RequiresRestart = false });
        IntSliderConfigItem boostedChanceConfig = new(boostedChance, new IntSliderOptions { Min = 0, Max = 100, RequiresRestart = false });
        IntSliderConfigItem daysLootedInARowConfig = new(daysLootedInARow, new IntSliderOptions { Min = 1, Max = 9, RequiresRestart = false });
        IntSliderConfigItem lootThresholdConfig = new(lootThreshold, new IntSliderOptions { Min = 0, Max = 100, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(baseChanceConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(boostedChanceConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(daysLootedInARowConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(lootThresholdConfig);

        BoolCheckBoxConfigItem moonPatchesConfig = new(moonPatches, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(moonPatchesConfig);

        BoolCheckBoxConfigItem tier3passPatchConfig = new(tier3passPatch, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        IntInputFieldConfigItem tier3passPriceConfig = new(tier3passPrice, new IntInputFieldOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        IntInputFieldConfigItem artPriceConfig = new(artPrice, new IntInputFieldOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(tier3passPatchConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(tier3passPriceConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(artPriceConfig);

        BoolCheckBoxConfigItem luckPatchConfig = new(luckPatch, new BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
        EnumDropDownConfigItem<LuckType> luckSystemConfig = new(luckSystem, false);
		LethalConfig.LethalConfigManager.AddConfigItem(luckPatchConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(luckSystemConfig);

        BoolCheckBoxConfigItem DisableCavesSignalPatchConfig = new(disableCavesSignalPatch, false);
		LethalConfig.LethalConfigManager.AddConfigItem(DisableCavesSignalPatchConfig);

        BoolCheckBoxConfigItem playerMovementPatchesConfig = new(playerMovementPatches, false);
        BoolCheckBoxConfigItem usePreV64GroundColisionConfig = new(usePreV64GroundColision, false);
        FloatSliderConfigItem speedLostToWaterCavesConfig = new(speedLostToWaterCaves, new FloatSliderOptions { Min = 0f, Max = 1f, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(playerMovementPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(usePreV64GroundColisionConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(speedLostToWaterCavesConfig);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void ConfigLethalConfigModEntry()
    {
        LethalConfig.LethalConfigManager.SetModDescription("HQR Configs");
    }


    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private CanModifyResult DontAllowSettingChangeMidGameCallback()
    {
        return (StartOfRound.Instance == null, "Game already started");
    }
}

enum Presets
{
    Default,
    Custom
}

enum LuckType
{
    Low,
    High
}
