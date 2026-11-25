**English** · [简体中文](README.zh-CN.md)

# New asset submission area incoming/

Submit art for the historical RPG Maker MV project here for review before integration into `game/`. This keeps submissions visible and the working project ready to play. For the current Unity asset workflow, see [Contributing](../../CONTRIBUTING.md).

## Naming

| Asset | Naming | Example |
|---|---|---|
| Face set | `RM_Face_<character name>.png` | `RM_Face_Grace.png` |
| Single-character sprite | `$RM_<character name>.png` | `$RM_Grace.png` |
| Scene / item | `RM_<thing name>.png` | `RM_Gravestone.png` |
| Music / sound effect | `RM_<purpose>`, both `.ogg` and `.m4a` | `RM_DoorCreak.ogg`, `RM_DoorCreak.m4a` |

Use the exact dimensions and grids in the [art asset guide](../README.md) so MV can slice and display the images correctly.

## When you are replacing a version

For a replacement asset, such as a new drawing of `$RM_Grace.png`, add the date to the submission filename and retain the previous version:

```
$RM_Grace_20261005.png
```

That way the old version is still there, so you can compare against it and roll back to it.

## After you hand it in

Say in the group chat which asset you handed in and which line or scene it replaces or adds. Whoever owns the project copies it into `game/`, then plays through to confirm.

Share new face-set filenames with the writer so they can connect the images to message events.
