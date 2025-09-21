using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HQRebalance.Utility;

internal class LootPool
{
    public static Dictionary<string, Item> itemList = Resources.FindObjectsOfTypeAll<Item>().ToDictionary(item => { return item.name; });
    private readonly List<SpawnableItemWithRarity> lootPoll = null!;

    public LootPool(
            int airhorn,
            int bell,
            int big_bolt,
            int bottles,
            int brush,
            int candy,
            int cash_register,
            int chemical_jug,
            int clock,
            int clown_horn,
            int comedy,
            int control_pad,
            int cookie_mold,
            int dust_pan,
            int easter_egg,
            int egg_beater,
            int fancy_lamp,
            int flask,
            int garbage_lid,
            int gift_box,
            int gold_bar,
            int golden_cup,
            int hairdryer,
            int flashbang,
            int pickles,
            int large_axle,
            int laser_pointer,
            int magic_ball,
            int mag_glass,
            int metal_sheet,
            int mug,
            int old_phone,
            int painting,
            int perfume,
            int pill_bottle,
            int plastic_cup,
            int plastic_fish,
            int red_soda,
            int remote,
            int ring,
            int rubber_duck,
            int soccer_ball,
            int wheel,
            int stop_sign,
            int tea_kettle,
            int teeth,
            int toiler_paper,
            int toothpaste,
            int toy_cube,
            int toy_robot,
            int toy_train,
            int tragedy,
            int engine,
            int whoopie,
            int yield_sign,
            int zed_dog)
    {
        if (itemList == null)
        {
            HQRebalance.Logger.LogError("All items list is null");
            return;
        }

        lootPoll = new()
        {
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Airhorn"),
                rarity = airhorn
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Bell"),
                rarity = bell
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("BigBolt"),
                rarity = big_bolt
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("BottleBin"),
                rarity = bottles
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Brush"),
                rarity = brush
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Candy"),
                rarity = candy
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("CashRegister"),
                rarity = cash_register
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("ChemicalJug"),
                rarity = chemical_jug
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Clock"),
                rarity = clock
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("ClownHorn"),
                rarity = clown_horn
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("ComedyMask"),
                rarity = comedy
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("ControlPad"),
                rarity = control_pad
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("MoldPan"),
                rarity = cookie_mold
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("DustPan"),
                rarity = dust_pan
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("EasterEgg"),
                rarity = easter_egg
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("EggBeater"),
                rarity = egg_beater
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("FancyLamp"),
                rarity = fancy_lamp
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Flask"),
                rarity = flask
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("GarbageLid"),
                rarity = garbage_lid
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("GiftBox"),
                rarity = gift_box
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("GoldBar"),
                rarity = gold_bar
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("FancyCup"),
                rarity = golden_cup
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Hairdryer"),
                rarity = hairdryer
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("DiyFlashbang"),
                rarity = flashbang
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("PickleJar"),
                rarity = pickles
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Cog1"),
                rarity = large_axle
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("FlashLaserPointer"),
                rarity = laser_pointer
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("7Ball"),
                rarity = magic_ball
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("MagnifyingGlass"),
                rarity = mag_glass
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("MetalSheet"),
                rarity = metal_sheet
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Mug"),
                rarity = mug
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Phone"),
                rarity = old_phone
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("FancyPainting"),
                rarity = painting
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("PerfumeBottle"),
                rarity = perfume
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("PillBottle"),
                rarity = pill_bottle
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("PlasticCup"),
                rarity = plastic_cup
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("FishTestProp"),
                rarity = plastic_fish
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("SodaCanRed"),
                rarity = red_soda
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Remote"),
                rarity = remote
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Ring"),
                rarity = ring
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("RubberDuck"),
                rarity = rubber_duck
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("SoccerBall"),
                rarity = soccer_ball
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("SteeringWheel"),
                rarity = wheel
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("StopSign"),
                rarity = stop_sign
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("TeaKettle"),
                rarity = tea_kettle
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Dentures"),
                rarity = teeth
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("ToiletPaperRolls"),
                rarity = toiler_paper
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Toothpaste"),
                rarity = toothpaste
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("ToyCube"),
                rarity = toy_cube
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("RobotToy"),
                rarity = toy_robot
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("ToyTrain"),
                rarity = toy_train
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("TragedyMask"),
                rarity = tragedy
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("EnginePart1"),
                rarity = engine
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("WhoopieCushion"),
                rarity = whoopie
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("YieldSign"),
                rarity = yield_sign
            },
            new SpawnableItemWithRarity
            {
                spawnableItem = ItemByName("Zeddog"),
                rarity = zed_dog
            }
        };

        lootPoll.RemoveAll(i => i.rarity == 0);
    }

    private Item ItemByName(string name)
    {
        if (itemList.TryGetValue(name, out Item item)) return item;
        else throw new Exception($"Could not retrieve item of name: {name}");
    }

    public List<SpawnableItemWithRarity> GetLootpool()
    {
        return lootPoll;
    }
}

