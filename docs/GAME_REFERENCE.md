**English** · [简体中文](GAME_REFERENCE.zh-CN.md)

> Historical RPG Maker MV maps and events. Current Unity navigation and implementation references are in the [documentation index](README.md).

# Act I game reference

RPG Maker MV 1.6.1 at 816 × 624. The source of truth is `game/data/` and the enabled `RM_Act1_Storybook` plugin.

## Walkthrough

1. Start a New Game, follow the path to Grandma's grave, and then talk to Dad.
2. Watch the cool-toned flashback. Back at the cemetery, speak to Mom to receive the music box.
3. Inspect the music box by the bench and choose **Turn the key**. Closing it returns to exploration and you can try again.
4. Explore the warm memory home and talk to Grandma. The reunion completes Act I, and free exploration remains available.

## Maps and events

### Map001 — `Act1_Cemetery`

24 × 18 · `!RM_CemeteryOvercast`

| ID | Event | Position |
|---|---|---|
| 01 | `OpeningController` | (0, 0) |
| 02 | `Mom` | (13, 12) |
| 03 | `Dad` | (14, 7) |
| 04 | `Gravestone_Main` | (11, 5) |
| 05 | `WhiteFlowers` | (10, 5) |
| 06 | `OldGrave_WitheredFlowers` | (5, 9) |
| 07 | `UncleJames` | (8, 9) |
| 08 | `Aunt` | (17, 10) |
| 09 | `Bench_RelicBox` | (6, 12) |
| 10 | `AfterFlashback_EndAct` | (1, 0) |
| 11 | `ExitLeft` | (11, 16) |
| 12 | `ExitRight` | (12, 16) |
| 13 | `Gravestone_Right` | (12, 5) |
| 14 | `Ambience` | (0, 0) |
| 15 | `Bench_RelicBox` | (5, 12) |
| 16 | `OldGrave_3` | (5, 4) |
| 17 | `OldGrave_4` | (18, 4) |
| 18 | `WhiteFlowers` | (13, 5) |
| 19 | `OldGrave_2` | (18, 9) |
| 20 | `Grandpa` | (16, 7) |
| 21 | `WindMusicBox` | (5, 12) |
| 22 | `RibbonAmongChamomile` | (8, 8) |
| 23 | `Birdbath` | (19, 12) |
| 24 | `GatePlaque` | (10, 15) |

### Map002 — `Act1_House_Flashback`

24 × 18 · `!RM_HouseOvercast`

| ID | Event | Position |
|---|---|---|
| 01 | `FlashbackController` | (0, 0) |
| 02 | `Grandma` | (12, 6) |
| 03 | `FlashbackDad` | (11, 7) |

### Map003 — `Act1_Memory_Home`

24 × 18 · `!RM_HomeStorybook`

| ID | Event | Position |
|---|---|---|
| 01 | `Grandma` | (12, 8) |
| 02 | `FamilyPhotograph` | (5, 7) |
| 03 | `GrandmasBlanket` | (5, 10) |
| 04 | `Cabinet` | (19, 7) |
| 05 | `Door` | (12, 4) |
| 06 | `Arrival` | (0, 0) |
| 07 | `Chair` | (18, 11) |
| 08 | `TeaCup` | (19, 11) |

## Switches and items

| ID | Switch |
|---|---|
| 0001 | `OpeningComplete` |
| 0002 | `VisitedGrave` |
| 0003 | `FlashbackActive` |
| 0004 | `FlashbackComplete` |
| 0005 | `Act1Complete` |
| 0006 | `MomCall` |
| 0007 | `MomHintDelivered` |
| 0008 | `FlowerSeen` |
| 0009 | `MusicBoxReceived` |
| 0010 | `MomReadyAfterFlashback` |
| 0011 | `MemoryHomeArrived` |
| 0012 | `RibbonCollected` |
| 0013 | `MemoryPhotoCollected` |

Key items:

- 5 — Music box
- 6 — Faded ribbon
- 7 — Family photograph

## Maintenance

- `Act1_TransparentCollision` separates passage from the parallax. Recheck accessible tiles and event coordinates whenever moving furniture, tree trunks, or grave bases.
- Inspectable events use `<RMInspect: label>`. The ribbon also uses `<RMCrouch>` so it can only be collected while crouching.
- MV animates the cemetery rain over the background. The flashback uses fixed framing while exploration maps use a following camera.
- Variable `0001` counts old grave visits. The ribbon and family photograph are optional collectibles.
