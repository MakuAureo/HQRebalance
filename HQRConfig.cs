using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using HarmonyLib;

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

    public HQRConfig(ConfigFile cfg)
    {
        if (lethalConfigLoaded == null)
            lethalConfigLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(HQRebalance.LethalConfigGUID);

        if (fairerFireExitsLoaded == null)
            fairerFireExitsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(HQRebalance.FairerFireExitsGUID);

        cfg.SaveOnConfigSet = false;

        selectableEnemies = new();

        preset = cfg.Bind(
                "General",
                "Preset",
                Presets.Custom,
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
                "Default Butler Patches",
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
                "Default Maneater Patches",
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
                "Default Jester Patches",
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
                "Default Masked Patches",
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
                "Use Mineshaft Patch",
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
                "Use custom quotas fulfilled difficulty scaling instead of vanilla's days completed in the current quota difficulty scaling"
                );

        quotaScalingFactor = cfg.Bind(
                "GameSystemPatch.DifficultyScaling",
                "Quota Scaling Factor",
                0.2f,
                new ConfigDescription("How many spawns each fulfilled quota should add", new AcceptableValueRange<float>(0f, 1f))
                );

        infestationPatch = cfg.Bind(
                "GameSystemPatch.Infestation",
                "Infestation Patch",
                true,
                "Enable custom infestation event"
                );

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
                "Use HQR's modified values for item count, enemy pool and loot pool\nWARNING: This is incompatible with other mods that change moons enemies, spawns or loot"
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
            AddLethalConfigItemsPassOne();
            ConfigLethalConfigModEntry();
        }
    }

    static void ClearOrphanedEntries(ConfigFile cfg)
    {
        PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
        orphanedEntries.Clear();
    }

    public void AddAllEnemiesToConfig(ConfigFile cfg, EnemyType[] allEnemies)
    {
        cfg.SaveOnConfigSet = false;

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

        ClearOrphanedEntries(cfg);
        cfg.Save();
        cfg.SaveOnConfigSet = true;

        if (lethalConfigLoaded == null || !lethalConfigLoaded.Value)
            return;

        AddLethalConfigItemsPassTwo();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void AddLethalConfigItemsPassOne()
    {
        LethalConfig.ConfigItems.EnumDropDownConfigItem<Presets> presetConfig = new(preset, new LethalConfig.ConfigItems.Options.EnumDropDownOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
        LethalConfig.LethalConfigManager.AddConfigItem(presetConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem fireExitPatchConfig = new(fireExitPatch, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
		LethalConfig.LethalConfigManager.AddConfigItem(fireExitPatchConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem butlerPatchesConfig = new(butlerPatches, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem addKnifeIconConfig = new(addKnifeIcon, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem disableStealthStabConfig = new(disableStealthStab, false);
		LethalConfig.LethalConfigManager.AddConfigItem(butlerPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(addKnifeIconConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(disableStealthStabConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem maneaterPatchesConfig = new(maneaterPatches, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem applyNomalDamageConfig = new(applyNomalDamage, false);
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem cannotCryOrEatBeforeSeeingPlayerConfig = new(cannotCryOrEatBeforeSeeingPlayer, false);
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem disableIncreasedSpawnChanceConfig = new(disableIncreasedSpawnChance, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(maneaterPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(applyNomalDamageConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(cannotCryOrEatBeforeSeeingPlayerConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(disableIncreasedSpawnChanceConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem jesterPatchesConfig = new(jesterPatches, false);
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem disableSolidHitboxConfig = new(disableSolidHitbox, false);
        LethalConfig.ConfigItems.FloatSliderConfigItem pushForceConfig = new(pushForce, new LethalConfig.ConfigItems.Options.FloatSliderOptions { Min = 0f, Max = 7f , RequiresRestart = false });
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem scaleFollowTimerWithInteriorSizeConfig = new(scaleFollowTimerWithInteriorSize, false);
        LethalConfig.ConfigItems.FloatSliderConfigItem followTimerScalingConfig = new(followTimerScaling, new LethalConfig.ConfigItems.Options.FloatSliderOptions { Min = 0f, Max = 1f, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(jesterPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(disableSolidHitboxConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(pushForceConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(scaleFollowTimerWithInteriorSizeConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(followTimerScalingConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem maskedPatchesConfig = new(maskedPatches, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem useMaskItemConfig = new(useMaskItem, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        LethalConfig.ConfigItems.IntSliderConfigItem maskValueConfig = new(maskValue, new LethalConfig.ConfigItems.Options.IntSliderOptions { Min = 28, Max = 51, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
		LethalConfig.LethalConfigManager.AddConfigItem(maskedPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(useMaskItemConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(maskValueConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem mineshaftPatchConfig = new(mineshaftPatch, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        LethalConfig.ConfigItems.FloatSliderConfigItem caveSizeConfig = new(caveSize, new LethalConfig.ConfigItems.Options.FloatSliderOptions { Min = 0.05f, Max = 1f, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
        LethalConfig.ConfigItems.FloatSliderConfigItem facilityDeltaConfig = new(facilityDelta, new LethalConfig.ConfigItems.Options.FloatSliderOptions { Min = 0f, Max = 0.1f, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
        LethalConfig.ConfigItems.FloatSliderConfigItem mapTileSizeConfig = new(mapTileSize, new LethalConfig.ConfigItems.Options.FloatSliderOptions { Min = 0.8f, Max = 1.2f, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
		LethalConfig.LethalConfigManager.AddConfigItem(mineshaftPatchConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(caveSizeConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(facilityDeltaConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(mapTileSizeConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem disableSingleItemDayConfig = new(disableSingleItemDay, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
		LethalConfig.LethalConfigManager.AddConfigItem(disableSingleItemDayConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem difficultyScalingPatchConfig = new(difficultyScalingPatch, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
        LethalConfig.ConfigItems.FloatSliderConfigItem quotaScalingFactorConfig = new(quotaScalingFactor, new LethalConfig.ConfigItems.Options.FloatSliderOptions { Min = 0f, Max = 1f, RequiresRestart = false, CanModifyCallback = DontAllowSettingChangeMidGameCallback });
		LethalConfig.LethalConfigManager.AddConfigItem(difficultyScalingPatchConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(quotaScalingFactorConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem infestationPatchConfig = new(infestationPatch, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
		LethalConfig.LethalConfigManager.AddConfigItem(infestationPatchConfig);
        LethalConfig.ConfigItems.IntSliderConfigItem baseChanceConfig = new(baseChance, new LethalConfig.ConfigItems.Options.IntSliderOptions { Min = 0, Max = 100, RequiresRestart = false });
        LethalConfig.ConfigItems.IntSliderConfigItem boostedChanceConfig = new(boostedChance, new LethalConfig.ConfigItems.Options.IntSliderOptions { Min = 0, Max = 100, RequiresRestart = false });
        LethalConfig.ConfigItems.IntSliderConfigItem daysLootedInARowConfig = new(daysLootedInARow, new LethalConfig.ConfigItems.Options.IntSliderOptions { Min = 1, Max = 9, RequiresRestart = false });
        LethalConfig.ConfigItems.IntSliderConfigItem lootThresholdConfig = new(lootThreshold, new LethalConfig.ConfigItems.Options.IntSliderOptions { Min = 0, Max = 100, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(baseChanceConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(boostedChanceConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(daysLootedInARowConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(lootThresholdConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem moonPatchesConfig = new(moonPatches, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(moonPatchesConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem tier3passPatchConfig = new(tier3passPatch, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        LethalConfig.ConfigItems.IntInputFieldConfigItem tier3passPriceConfig = new(tier3passPrice, new LethalConfig.ConfigItems.Options.IntInputFieldOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
        LethalConfig.ConfigItems.IntInputFieldConfigItem artPriceConfig = new(artPrice, new LethalConfig.ConfigItems.Options.IntInputFieldOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(tier3passPatchConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(tier3passPriceConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(artPriceConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem luckPatchConfig = new(luckPatch, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = true });
        LethalConfig.ConfigItems.EnumDropDownConfigItem<LuckType> luckSystemConfig = new(luckSystem, false);
		LethalConfig.LethalConfigManager.AddConfigItem(luckPatchConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(luckSystemConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem DisableCavesSignalPatchConfig = new(disableCavesSignalPatch, false);
		LethalConfig.LethalConfigManager.AddConfigItem(DisableCavesSignalPatchConfig);

        LethalConfig.ConfigItems.BoolCheckBoxConfigItem playerMovementPatchesConfig = new(playerMovementPatches, false);
        LethalConfig.ConfigItems.BoolCheckBoxConfigItem usePreV64GroundColisionConfig = new(usePreV64GroundColision, false);
        LethalConfig.ConfigItems.FloatSliderConfigItem speedLostToWaterCavesConfig = new(speedLostToWaterCaves, new LethalConfig.ConfigItems.Options.FloatSliderOptions { Min = 0f, Max = 1f, RequiresRestart = false });
		LethalConfig.LethalConfigManager.AddConfigItem(playerMovementPatchesConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(usePreV64GroundColisionConfig);
		LethalConfig.LethalConfigManager.AddConfigItem(speedLostToWaterCavesConfig);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void AddLethalConfigItemsPassTwo()
    {
        foreach (KeyValuePair<EnemyType, ConfigEntry<bool>> enemyEntryPair in selectableEnemies)
        {
            LethalConfig.ConfigItems.BoolCheckBoxConfigItem enemyEntryConfig = new(enemyEntryPair.Value, new LethalConfig.ConfigItems.Options.BoolCheckBoxOptions { CanModifyCallback = DontAllowSettingChangeMidGameCallback, RequiresRestart = false });
            LethalConfig.LethalConfigManager.AddConfigItem(enemyEntryConfig);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void ConfigLethalConfigModEntry()
    {
        LethalConfig.LethalConfigManager.SetModDescription("HQR Configs");
    }


    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private LethalConfig.ConfigItems.Options.CanModifyResult DontAllowSettingChangeMidGameCallback()
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
