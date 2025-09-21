using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HQRebalance.Utility;

internal class EnemyPool
{
    public static Dictionary<string, EnemyType> enemyList = Resources.FindObjectsOfTypeAll<EnemyType>().ToDictionary(enemy => { return enemy.name; });
    private readonly List<SpawnableEnemyWithRarity> enemyPool = null!;

    public EnemyPool(
            int blob,
            int butler,
            int maneater,
            int centipede,
            int barber,
            int thumper,
            int girl,
            int bracken,
            int lootbug,
            int jester,
            int masked,
            int nutcracker,
            int puffer,
            int spider,
            int coilhead)
    {
        if (enemyList == null)
        {
            HQRebalance.Logger.LogError("All enemies list is null");
            return;
        }

        enemyPool = new()
        {
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Blob"),
                rarity = blob
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Butler"),
                rarity = butler
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("CaveDweller"),
                rarity = maneater
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Centipede"),
                rarity = centipede
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("ClaySurgeon"),
                rarity = barber
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Crawler"),
                rarity = thumper
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("DressGirl"),
                rarity = girl
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Flowerman"),
                rarity = bracken
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("HoarderBug"),
                rarity = lootbug
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Jester"),
                rarity = jester
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("MaskedPlayerEnemy"),
                rarity = masked
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Nutcracker"),
                rarity = nutcracker
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Puffer"),
                rarity = puffer
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("SandSpider"),
                rarity = spider
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("SpringMan"),
                rarity = coilhead
            }
        };
        
        enemyPool.RemoveAll(i => i.rarity == 0);
    }

    public EnemyPool(
            int baboon,
            int fox,
            int giant,
            int dog,
            int radmech,
            int worm,
            int locust,
            int manticoil,
            int tulip,
            int bee)
    {
        if (enemyList == null)
        {
            HQRebalance.Logger.LogError("All enemies list is null");
            return;
        }

        enemyPool = new()
        {
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("BaboonHawk"),
                rarity = baboon
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("BushWolf"),
                rarity = fox
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("ForestGiant"),
                rarity = giant
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("MouthDog"),
                rarity = dog
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("RadMech"),
                rarity = radmech
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("SandWorm"),
                rarity = worm
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("DocileLocustBees"),
                rarity = locust
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("Doublewing"),
                rarity = manticoil
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("FlowerSnake"),
                rarity = tulip
            },
            new SpawnableEnemyWithRarity
            {
                enemyType = EnemyByName("RedLocustBees"),
                rarity = bee
            }
        };

        enemyPool.RemoveAll(i => i.rarity == 0);
    }

    private EnemyType EnemyByName(string name)
    {
        if (enemyList.TryGetValue(name, out EnemyType enemy)) return enemy;
        else throw new Exception($"Could not retrieve enemy of name: {name}");
    }

    public List<SpawnableEnemyWithRarity> GetEnemyPool()
    {
        return enemyPool;
    }
}
