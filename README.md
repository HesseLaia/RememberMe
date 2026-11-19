**English** · [简体中文](README.zh-CN.md)

# Remember Me — Act I

A turn-based narrative game for the final project of **CSS5190 Game Your Psychology: AI, Creativity & Mental Health** (The Chinese University of Hong Kong, Shenzhen, Fall 2026), made with RPG Maker MV 1.6.1.

This repository is both a **playable game** and the **team's workbench**: art specs, script specs, file lists and the latest build all live here.

---

## Which file to look at

| You are | Look at this | What is in it |
|---|---|---|
| **Artist** | [`art/README.md`](art/README.md) | which folder each kind of asset goes in, file sizes and how the grid maths works, naming rules, the list of art that already exists, the handover process, the submission area |
| **Writer** | [`script/README.md`](script/README.md) | how one line of dialogue is written in the game, how many characters per row, colour codes and expression indexes, character voice, templates you can copy, where drafts go |
| Looking for what a file is for | [`docs/FILE_GUIDE.md`](docs/FILE_GUIDE.md) | what every folder and key file in the repository is for, and whether you can change it |
| Looking for a line or a switch | [`docs/GAME_REFERENCE.md`](docs/GAME_REFERENCE.md) | the three maps, switches, key items and walkthrough |
| Wanting to read the full script | [`script/ACT1_SCRIPT.md`](script/ACT1_SCRIPT.md) | every in-game line of Act I, in order of appearance |
| Adding files to the repository | [`docs/DEPLOY.md`](docs/DEPLOY.md) | where to put them, how to commit, how to publish |
| Checking the updated asset list | [Act I art and playtest notes (中文)](docs/ACT1_PLAYTEST.zh-CN.md) | new scenery, motion sheets, controls and audio formats |
| Just wanting to play | <https://jeddakholmes-byte.github.io/RememberGame/> | play in the browser, nothing to install |

---

## Play / open

| | |
|---|---|
| **Play online** | <https://jeddakholmes-byte.github.io/RememberGame/> |
| **Direct game link** | <https://jeddakholmes-byte.github.io/RememberGame/game/> |
| **Open it in MV to edit** | Open `game/Game.rpgproject` with RPG Maker MV 1.6.1 |
| **Run it on your own machine** | `cd game && python3 -m http.server 8080`, then open <http://127.0.0.1:8080/>. **Double-clicking `index.html` does not work** — it must be served over HTTP |

Controls: **WASD or arrows** to move, **Shift** to run, **C** to crouch, **E / Enter / Space** to interact, **I** for Inventory, and **Esc** to pause. The game runs at 816 × 624.

---

## What is in the repository

```
RememberGame/
├── README.md              ← you are here
├── index.html             ← landing page; English by default, with an EN / 中文 switch
├── art/                   ← for the artist
│   ├── README.md              asset specs, sizes, naming, inventory, handover process
│   ├── specs/                 auto-generated reference sheets (8 expressions and the sprite grid per character)
│   └── incoming/              put new assets here first — not straight into game/
├── script/                ← for the writer
│   ├── README.md              writing rules, format, voice, templates
│   ├── ACT1_SCRIPT.md         the full text of Act I, exported from the project (English only)
│   └── drafts/                new script drafts
├── docs/                  ← technical and operations
│   ├── FILE_GUIDE.md          what every file is for
│   ├── GAME_REFERENCE.md      maps / events / switches / flow
│   ├── DEPLOY.md              how to add files, how to publish
│   ├── VERIFICATION.md        the browser test record taken before going live
│   └── screenshots/           game screenshots
└── game/                  ← the game itself, an RPG Maker MV 1.6.1 project
```

**Languages.** Every document above except `script/ACT1_SCRIPT.md` also exists in Chinese at the same path with `.zh-CN.md` appended — `README.zh-CN.md`, `art/README.zh-CN.md`, and so on. Each page links to its other language in the first line. `ACT1_SCRIPT.md` has no translation because it is the English text the game actually plays.

---

**`game/` contains the current project.** It is two things at once:

- the project you open in RPG Maker MV, and
- the web build GitHub Pages is serving.

This is not a coincidence. An RPG Maker MV game is already HTML5 + JavaScript, with no "conversion" step — so the web build is a 1:1 copy of the original, not a rewrite.

---

## What the game is

Act I follows seven-year-old Grace at her grandmother's funeral. Find Grandma's grave, talk to Dad, and witness a flashback. Back in the cemetery, Mom gives Grace a music box. Wind it at the bench to enter Grandma's house as Grace remembers it, then find Grandma. The chapter ends with their reunion; the house stays open to explore.

| Map | Role |
|---|---|
| `Act1_Cemetery` (Map001) | Open exploration, rainy funeral, five distinct graves and optional keepsakes |
| `Act1_House_Flashback` (Map002) | Scripted scene in cool, overcast light |
| `Act1_Memory_Home` (Map003) | Warm memory, optional family photograph and reunion |

## Current build

- The original cover stays on the main menu. Full-screen illustrations mark story beats.
- Exploration uses a following camera, an objective card, and contextual interaction prompts.
- The music box, faded ribbon, and family photograph are native MV key items. The item menu is read-only in Act I.
- The title, cemetery, flashback, and keepsake share a musical motif. Background sound and footsteps use separate tracks with Ogg and M4A files.
- Start with **New Game** for this revision; old saves carry the original switch and event states. See [verification](docs/VERIFICATION.md).

---

## Before you add files

- **New assets go into `art/incoming/` first**, and new script goes into `script/drafts/` first. `game/` is always a clean, working build — do not drop half-finished work straight into it.
- **Do not rename or overwrite existing files in `game/img/` or `game/audio/`.** The game looks up images by filename string, so a rename throws no error — the image just silently fails to show.
- **Always play through after adding something, and start from New Game.** Loading an old save brings the old switch states in with it and makes a correct change look broken.
- For the exact commit and publish steps, see [`docs/DEPLOY.md`](docs/DEPLOY.md).

---

## Copyright and asset sources

Made with **RPG Maker MV 1.6.1** (Kadokawa / Degica). Most files in `game/img/` and `game/audio/` are Run Time Package assets that ship with RPG Maker MV, and distributing them with the project is what the RTP is for.

**Every asset made for this game starts with `RM_` in the filename** — the title image, the two parallax base images, the headstone and keepsake box close-ups, seven face sets, four sprite sheets, the collision tiles, the title music, and about thirty sound effects.
