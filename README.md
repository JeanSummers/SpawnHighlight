# Spawn Highlight

Allows to highlight blocks where drifters can spawn.  
Ever tired of drifter-proofing your base? This mod is for you!

Press `Ctrl+L` to show/hide highlights. Keybinding can be changed in the settings.

Default colors:  
Red - drifters can spawn on this block  
Green - drifters cannot spawn on this block because it is sufficiently lit  
Not highlighted - drifter will never spawn on this block. Examples are lower slabs, stones and chiseled blocks

Remember that drifters **will not** spawn under sunlight, and **will** spawn anywhere they want while temporal storm is in progress.

To be more specific, the mod checks this rules for each block in radius:

1. Block has solid upper side
2. Upper block is air or tall grass
3. Non-natural light level is greater than 7

This mod is quite similar to [Easy Light Levels](https://mods.vintagestory.at/show/mod/2414). You may check it too if you want wider configuration options.

## Commands

`.sphi` - Show/Hide highlights. Does exactly the same as `Ctrl+L`  
`.sphi radius <number>` - Change size of cube around you, where mod will scan and highlight blocks. Default is `20`  
`.sphi litcolor #RRGGBBAA` - Set color for sufficiently lit zones. Default is `#00FF0020`  
`.sphi spawncolor #RRGGBBAA` - Set color for zones where mobs can spawn. Default is `#FF000020`

## Config

If you prefer json instead of commands, you can change the values in the `spawnhighlight-config.json` which can be found in `VintagestoryData/ModConfig` folder

```json
{
  "Radius": 20,
  "SafeColor": "#00FF0020",
  "SpawnableColor": "#FF000020"
}
```
