**English** · [简体中文](VERIFICATION.zh-CN.md)

# Historical MV verification record

This page records RPG Maker MV checks. Current Unity v10 verification evidence is in the [handoff](HANDOFF.md).

## MV build · 2026-11-19

- Two Node checks pass for MV asset dimensions, map reachability, event wiring, key items, and Ogg/M4A files.
- Tested the MV runtime over local HTTP in Chrome: run and crouch, animated rain, ribbon pickup and inventory, then the New Game route through the flashback, closing and reopening the music box, reuniting with Grandma, and pause/resume.
- Coverage limits: mobile, old saves, and subjective audio quality await manual testing. The original-release record and screenshots below describe the preceding MV build.

## Archive · original release


Before this repository's original release, the build in `game/` ran in a browser from **New Game** to the end of Act I. The following sections preserve that run's method, observations, and coverage limits.

Screenshots are in [`screenshots/`](screenshots/).

---

## Method

- Served `game/` over HTTP (`python3 -m http.server`) and loaded `index.html` in Chrome.
- Drove the browser through the DevTools protocol using the game's input layer (`Input._onKeyDown` / `Input._onKeyUp`). MV polls input once per frame, so presses were held long enough to register.
- Read the engine's live state between presses: current scene, `$gameMap._mapId`, player position, the eight switches, `$gameMessage.isBusy()`, `$gameMap.isEventRunning()`.
- The game ran at a steady 60 fps in the test browser.

---

## Result: the chapter completes

| Step | Expected | Observed |
|---|---|---|
| Boot | title screen with `GAME START / CONTINUE / SETTINGS` | ✓ |
| Start a new game | `Scene_Map`, Map 1, player at (12,12), opening message running | ✓ after 1 press |
| Play the cold open | ends with switch 1 (`OpeningComplete`) ON and the player free to move | ✓ after 12 presses |
| Enter the flashback | Map 2, player at (12,13), the flashback autorun playing | ✓ |
| Play the flashback | ends with switch 3 OFF, switch 4 (`FlashbackComplete`) ON, player back on Map 1 at (13,7) | ✓ after 14 presses |
| Play the ending | ends with switch 5 (`Act1Complete`) ON | ✓ after 24 presses |
| After the ending | returns to the title screen (command 354 = *Return to Title Screen*) | ✓ |

The run completed with zero JavaScript errors and zero frozen frames. All 229 image and audio files referenced by `data/*.json` were present on disk, and every browser request during the run returned HTTP 200. The final state was:

```
{"scene":"Scene_Title","map":1,"x":13,"y":7,"sw":"11111000","busy":false,"run":true}
```

`sw:"11111000"` is switches 1–8, so 1 through 5 are ON and 6–8 are OFF — exactly the state the ending is meant to leave behind.

---

## What the screenshots show

| File | What it is |
|---|---|
| `00_title_screen.png` | The opening event draws the title art and command window with `optDrawTitle` set to `false` in `System.json`. |
| `01_opening_over_black.png` | Grace's first line, mid-typewriter. The opening event fades to black before the first two lines and fades back in afterwards. |
| `02_cemetery_free_roam.png` | The cemetery once the player is free, in the rain. Grace, Mom, Dad, Uncle James, Aunt and Grandpa are all on screen with their own sprites. |
| `03_flashback.png` | The flashback interior with Grandma's portrait and English dialogue. |
| `04_back_in_cemetery.png` | Back in the cemetery after the flashback. |
| `05_back_at_title_after_the_ending.png` | The title screen again, after the ending returned to it. |

---

## Checks that were performed along the way

- **Custom image geometry** — all `RM_*` assets match the engine's grid rules: `$RM_*` walk sheets are 144 × 192 (3 × 4 of 48 × 48). Face sheets are 576 × 288 (4 × 2 of 144 × 144). The title is 816 × 624. The two parallaxes are 1152 × 864. The collision tileset is built on a 48 px grid. `Window.png` is 192 × 192.
- **Asset coverage** — every image and audio file referenced by `data/*.json` exists on disk, and every referenced audio track exists in both `.ogg` and `.m4a`.
- **Plaintext loading** — `System.json` omits `hasEncryptedImages` / `hasEncryptedAudio`, and the plaintext project files load directly from a static host.
- **The collision layer behaves** — tile 1536 in `Tilesets.json` has flag `1536` (low nibble zero → passable) and tile 1537 has flag `1551` (low nibble `0x0F` → impassable in all four directions), which matches the tileset's own note: *"A5 tile 1 = walkable, tile 2 = blocked."*
- **Event commands are internally consistent** — switch and variable reads and writes were traced across both maps. Every switch that is tested somewhere is set somewhere, and the eight switches are named.

---

## Coverage limits and observed issues

1. **Editor:** this run covered the runtime. Opening and re-saving in MV 1.6.1, plugin-parameter editing, and tileset assignment remained untested.
2. **Walking route:** the test set `VisitedGrave` directly and entered the flashback through the same state transition as Dad's event. Walking to Dad, inspecting the headstone, flowers and old graves, Mom's call, and interactions with the four side characters remained outside input coverage.
3. **Menu:** opening-phase menu locking and later menu access remained untested. The run used no `Esc` input.
4. **Observed issues in this original build:** the flashback played two screen shakes and two `RM_KnockHard` sounds. The system locale was `zh_CN`, with Chinese system text. These observations belong to the original-release build.
5. **Platforms:** desktop Chrome was tested. Safari, Firefox, and Safari's `.m4a` audio playback remained untested.
6. **Human review:** this was an automated run. Local human playtesting and audio verification remained pending.

---

## How to re-run it

```bash
cd game
python3 -m http.server 8080
```

Then open [the local game](http://127.0.0.1:8080/) and play from New Game. The later MV walkthrough is in [GAME_REFERENCE.md](GAME_REFERENCE.md#walkthrough). The original-release results above preserve their historical scope.
