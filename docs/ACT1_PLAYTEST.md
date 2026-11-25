**English** · [简体中文](ACT1_PLAYTEST.zh-CN.md)

# Remember Me · Act I art and playtest

This guide covers the historical RPG Maker MV version. For the current Unity game and v10 verification evidence, see the [documentation index](README.md) and [handoff](HANDOFF.md).

**Scope:** The funeral and original indoor flashback lead to Mom handing over Grandma's belongings in the cemetery. The player opens and winds the music box, enters the memory home, and reunites with Grandma. The photograph and other inspections are optional. Exploration proceeds at the player's pace, with no countdown, chase, or failure penalty.

## Implementation and playtest

- Open `game/Game.rpgproject` in RPG Maker MV, press F12, and choose **New Game** on the original title screen. Move with WASD or the arrow keys, run with Shift, toggle crouching with C, inspect with E / Enter / Space, open inventory with I, and pause with Esc.
- Main route: follow the gravel path to the grave with white flowers → talk to Dad → watch the original indoor flashback → find Mom → open the music box by the bench and choose **Turn the key** → find Grandma in the memory home. **Close the lid** returns to the cemetery, where the box can be opened again. After the reunion, the photograph, blanket, and cabinet remain available to inspect.
- The cemetery and memory home use a 1.32× following camera. The flashback uses fixed framing at its original scale. Both exploration maps use native MV tiles and events for collision and retain the existing character and face sheets.
- The lower-left grave with withered flowers, mossy headstone, pointed slate marker, and low grave with pebbles each have inspection text. Crouch to collect the blue ribbon among the chamomile. The music box and the memory home's family photograph also enter the inventory. All three are key items in the MV database and can be checked directly by later events.

## Asset list

Paths below are relative to `game/`.

| Project path | PNG specification | Purpose and production |
| --- | --- | --- |
| `img/parallaxes/!RM_CemeteryOvercast.png` | 1152×864, 24×18 tiles | Built-in image generation: original cover brushwork, wet stone under an overcast sky, and five distinct graves. Rain is animated separately |
| `img/parallaxes/!RM_CemeteryCanopyStory.png` | 1152×864, transparent | Canopy extracted from the updated background and overlaid in front of characters at matching coordinates |
| `img/parallaxes/!RM_HouseOvercast.png` | 1152×864, 24×18 tiles | Built-in image generation: cool grey window light and old wooden floors for the oppressive flashback |
| `img/parallaxes/!RM_HomeStorybook.png` | 1152×864, 24×18 tiles | Built-in image generation: warm memory home with inspectable photograph, blanket, and teacup |
| `img/characters/$RM_Grace_Run.png`, `$RM_Grace_Crouch.png` | 144×192 each, transparent | Built-in image generation: 12 cells per character sheet, 48×48 each. Rows face down, left, right, up. All three frames share a foot anchor |
| `img/pictures/RM_MusicBoxStorybook.png`, `RM_MemoryChapter.png` | 816×624 each | Built-in image generation: music-box close-up and full-screen chapter illustration for entering the memory home |
| `img/titles1/RM_Title.png` | 816×624 | Original cover restored with its source retained. Title commands arranged along the bottom |
| `img/system/RM_StoryPaper.png` | 192×192, transparent | Paper tones, fine borders, and selection state drawn within MV's window-skin regions |

MV asset specifications follow the English help installed with the editor at `Help/page/01_11_01.html`. Maps use 48-pixel tiles. The `!` prefix preserves map-coordinate alignment. MV renders light animated rain over the cemetery background. New original title, cemetery, flashback, and music-box scores share a melodic motif. Rain, indoor ambience, wooden-lid, menu, dripping-water, and three ground-surface footstep sounds were also remade. All new audio is supplied as Ogg and M4A. Its production source is `tools/render-act1-audio.py`.

## Verification procedure

- Run `node tools/check-act1-maps.cjs` from the repository root to check image dimensions, map reachability, and event wiring.
- Run `node tools/check-act1-assets.cjs` to check the cover, running and crouching sheets, chapter image, dynamic weather, and key-item configuration.
- Load the bundled MV runtime in a browser, advance the main route automatically, and check pause and resume. Use a New Game while preserving existing saves.
