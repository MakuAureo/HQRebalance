# HighQuotaRebalanced Beta
Due to how changes are implemented this mod is not compatible with mods that add new items, new enemies or change the vanilla moons in some way.
There will be a config for full release allowing you toggle different patches. 

## Description
HQR is meant to keep the vanilla feeling of the game with changes that make high quota more varied and more enjoyable to play by adding more emphasis on player choice and scalling difficulty.

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
- The `scan` command is now more accurate to the real value

### Jester changes:
- Jester's follow timer scales with map size: `followTimer = 30*mapSize - Random(0.0, 10.0)`
- Jester's hitbox is no longer solid and can be ran through

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

### Masked changes:
- Masked now wears a mask item instead of a mesh, that can be grabbed upon death, similar to Nutcracker's shotgun
- Masked will drop the player's items when converting them
- Converted maskeds will have the correct suit and also be tracked correct on the monitor by non-host players

### Bee changes:
- Hives are included in the total value displayed at the end of the day

### Kiwi bird changes:
- Eggs are included in the total value displayed at the end of the day

### Ship radar changes:
- Removed "No singal" effect when players are in caves

### Infestation changes:
- Infestation has a base 4% chance of happening any day
- Infestation no longer changes the total power available in the moon
- Infestation can only chose Nutcrackers, Masked or Butlers with an equal chance for all of them, if all can spawn on the moon
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
Every Tier 2 or higher moon has been changed to have more scrap or more valuable scrap and a lot more spawns
- Reworking this section for a wiki instead of using the README (only for the full release)

## Planned changes
There are no more planed big changes

HQR is in it's playtesting phase to give the best experience before it's first full release (this might change as I iterate on more ideas)

## Feedback
If you played this mod and want to give feedback or ideas on how it could be improved message me on discord (makuaureo) or use the proper channel on my [discord server](https://discord.gg/KQPQFGjCeX)
