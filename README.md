# HighQuotaRebalanced_Beta
 THIS MOD IS CURRENTLY IN BETA, MEANING IT IS **NOT** FEATURE COMPLETE AND STILL SOMEWHAT BUGGY
 It also is not compatible with mods that add new items or change the vanilla moons in some way. There will be a config for full release allowing you toggle different patches. For now, don't expect this mod to work with other mods that also apply changes to the vanilla moons.

## Description
HQR is meant to keep the vanilla feeling of the game with changes that make high quota more varied and more enjoyable to play by adding more emphasis on player choice and ways to counter enemies.

## Change list
### Player changes:
- Re-implement the ability to ledge climb
- Player's movement speed reduced while crouch through water caves is almost the same as regular crouching

### Generation changes:
- Fire exit is more likely to spawn (prevent small mineshaft interiors from having no fire exit, tho it is techinically possible)
- Fire exit spawning logic changed per interior:
    - Facility: fire exit will spawn on the other side of maze
    - Mansion: fire exit can still spawn close to main, but not in a dead end
    - Minesahft: fire exit will spawn on the otehr side of caves

### Terminal changes:
- The `scan` command is not more accurate to the real value

### Jester changes:
- Jester's follow timer scales with map size: `followTimer = 30*mapSize - Random(0.0, 10.0)`
- Jester's hitbox is no longer solid and can be walked through

### Maneater changes:
- Maneater takes multiple hit points of damage from sources that deal more than 1 damage (shotgun, landmines, etc...)
- Maneater can't cry or eat scrap until it has seen a player for the 1st time
- Maneater's increased spawns when interior is mineshaft has been removed

### Mineshaft changes:
- Removed the extra 6 items
- Less facility tiles, more cave tiles, overall smaller by ~10%

### Butler changes:
- Removed stealth stab
- Knives show up on the monitor and counts towards outside items when using the terminal's `scan` command while the butler is alive
- Knives from both alive and dead butlers are included in the total value displayed at the end of the day

### Bee changes:
- Hives are included in the total value displayed at the end of the day

### Kiwi bird changes:
- Eggs are included in the total value displayed at the end of the day

### Ship radar changes:
- Removed "No singal" effect when players are in caves

### Infestation changes:
- Infestation has a base 4% chance of happening any day
- Infestation no longer changes the total power available in the moon
- Infestation can only chose Nutcrackers or Butlers with an equal chance for both if both can spawn on the moon
- When choosen as the infestation enemy, they will have unlimited max spawn count
- Removed the extra +2 spawns per wave
- During an infestation, when attempting to spawn a new enemy, there's a 60% chance of spawning the current infestation enemy and 40% chance of spawning an enemy using the current moon's spawn distribution

### Luck changes:
- Luck now increases quota rolls, instead of decreasing
- Luck doesn't depend on what furniture has been bought, only how many
- Luck is much more effective in changing rolls

### Game changes:
- Indoor fog is permanently disabled
- Single Item Day is disabled
- Routing to Rend, Dine or Titan is a one time pay for a quota
- Removed spawn scaling based on how many days are left on the quota
- Removed extra spawn when suriving 5 days in a row
- Removed extra weather when surviving 3 days in a row
- Added spawn scaling based on quotas fulfilled
- Infestation chances increase to 20% if the crew collected at least 85% of the total value over the last 3 days

### Moon changes:
- Reworking this section for a wiki instead of using the README (only for the full release)

## Planned changes
There are no more planed big changes, now HQR will be play test and adjusted for the best experience before the first full release

## Feedback
If you played this mod and want to give feedback or ideas on how it could be improved message me on discord (makuaureo) or use the proper channel on my [discord server](https://discord.gg/KQPQFGjCeX)
