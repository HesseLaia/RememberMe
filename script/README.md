**English** · [简体中文](README.zh-CN.md)

> This guide records the original RPG Maker MV format and voice rules. Its counts and speaking roles describe an earlier writing snapshot. The later [Act I export](ACT1_SCRIPT.md) includes Aunt and Grandpa dialogue. Shipped v10 uses Unity: Act I dialogue is in `Act1Game.cs` / `Act1Story.cs`, and the kitchen is in `KitchenPrototype.cs`. See the [Unity project guide](../RememberMeUnity/README.md).

# Script guide

This folder contains historical dialogue references and working drafts. The running game reads its dialogue from the engine project. Use this guide when working with the original MV format. Use the Unity project conventions for current implementation.

- [`ACT1_SCRIPT.md`](ACT1_SCRIPT.md) — the historical Act I reading copy, exported automatically from the project data. Editing the reading copy leaves game dialogue unchanged.
- [`drafts/`](drafts/README.md) — scene drafts, including the historical [kitchen v0.4 draft](drafts/scene1_draft_2.md).

Each document has an English default and a `.zh-CN.md` companion. Language links switch between them. The English-only requirements below apply to text imported into the historical MV game.

---

## 1. What one line of dialogue looks like in the game

```
┌──────────────────────────────────────┐
│ ┌────────┐  \C[4]GRACE\C[0]          │  ← name line (takes one row)
│ │        │  Everyone wore black today.│  ← body text
│ │  face  │  Mom wore black. Dad wore  │
│ │144×144 │  black.                    │
│ └────────┘                            │
└──────────────────────────────────────┘
```

A message box holds at most **4 rows**, and the name line takes one of them, so **the body text is at most 3 rows**. In the existing Act I: 57 message boxes have 1 row of body text, 23 have 2, and only 1 fills all 3. **Default to 1–2 rows.**

**Keep every row under 40 characters** (spaces and punctuation included). The longest line right now is 41 characters, which is already on the edge. Past 40 it wraps or runs outside the box.

---

## 2. MV format

**MV game text is English.** Its interface and subtitles use English. The historical font setup renders Chinese as boxes. Chinese document translations are reading copies.

One line of dialogue is three things:

| Part | How to write it |
|---|---|
| Face | which sheet and which index — see section 4 |
| Name line | `\C[n]NAME\C[0]`, all capitals. **Narration has no name line.** |
| Body | plain English, one sentence per row, do not write long paragraphs |

The colour code in the name line goes with the face set. See the next section.

---

## 3. Colour codes

`\C[n]` switches the text colour and `\C[0]` switches it back to the default. Use these colour controls for MV text layout. Keep spaces and asterisks out of alignment formatting.

| Speaker | Colour code | Name line | Face set |
|---|---|---|---|
| Narration (third-person observation) | `\C[7]` | **no name** | a face set is optional |
| Grace | `\C[4]` | `\C[4]GRACE\C[0]` | `RM_Face_Grace` |
| Mom | `\C[6]` | `\C[6]MOM\C[0]` | `RM_Face_Mom` |
| Dad | `\C[1]` | `\C[1]DAD\C[0]` | `RM_Face_Dad` |
| Grandma | `\C[5]` | `\C[5]GRANDMA\C[0]` | `RM_Face_Grandma` |
| Uncle James | `\C[3]` | `\C[3]UNCLE JAMES\C[0]` | `RM_Face_UncleJames` |
| Aunt / Grandpa | `\C[7]` | **narration, no name line** | but give them a face set so the player knows they are present |

> In this earlier voice guide, Aunt and Grandpa **have no lines**, only a one-sentence observation from someone else's point of view ("She holds a tissue in both hands."). This is deliberate. See section 5.

---

## 4. Expression indexes

Each character's face set has 8 cells, numbered 0–7 (top row 0–3, bottom row 4–7). **To find out which number is which expression for each character, look at the art team's reference sheets**:

- Grace → [`../art/specs/faces_grace.png`](../art/specs/faces_grace.png)
- Other characters work the same way and are in [`../art/`](../art/)

**Specify the expression index, such as "index 3".** The indexes actually in use:

| Character | In use | Notes |
|---|---|---|
| Grace | 2, 3, 5, 7 | 3 is the default expression, used 22 times |
| Mom | 1, 2, 3, 6 | |
| Dad | 1, 2, 4, 5, 6, 7 | used the most |
| Grandma | 2, 4 | |
| Grandpa | 6 | its only line |
| Aunt | 2 | its only line |
| Uncle James | 0 | its only line |

If an emotion has no suitable cell, ask the art team to add an expression at a given number first. **Do not borrow another cell as a stopgap** — that cell's expression may already be in use by other lines.

---

## 5. Character voice

These voice notes describe the earlier Act I writing baseline.

**Grace (seven years old, first-person observation)**
Short, concrete sentences that let you see the picture. She describes before she understands, often stopping at "I don't understand", and she does not explain the emotion.
> "Everyone wore black today. Mom wore black. Dad wore black."
> "Dad's wiping his face."
> "I don't understand."

**Mom (practical, evasive, protective)**
One short sentence moves the subject away. She never answers directly.
> "In a little while."
> "I don't know, sweetheart."
> "When we get home. Let's keep it dry."

**Dad (says the least)**
One word when one word will do. The only all-capitals line in the whole script is his — volume is his signal for breaking down, so do not overuse capitals.
> "..."
> "A little."
> "Because I miss Grandma."
> "I CAN'T DO THIS AGAIN."

**Grandma (repetitive, insistent, confused about who she is talking to)**
In the flashback she repeats the same thing and calls the wrong person by name. Her lines are short, repeated and imprecise.
> "I need to go home." → "No." → "Grace?" → "Let me go home!"

**Uncle James (one line, giving permission)**
> "Take your time, Grace."

**Aunt / Grandpa (observation only, no lines)**
In this baseline, Grace observes them without spoken dialogue. Keep that role when revising this version, and keep illness explanations and task instructions out of their observations.

**Narration** uses `\C[7]`. There is no second person. It is always Grace's eyes watching.

---

## 6. Three content rules

1. **Do not explain the illness and do not teach.** This is what a child sees, not an information sheet. Grandma's Alzheimer's is never named, from start to finish.
2. **Do not add tasks or hints.** There is no instructional text such as "go and talk to Dad" — only observation.
3. **Do not write anything other than who is speaking.** Emotion notes in brackets and stage directions never make it into the game, and if you write them they will not match. To express emotion, change the picture instead (movement, sound effect, expression index).

---

## 7. Template

Copy this when you write a new section:

```text
[Scene] Act I cemetery — in front of the main headstone
[Trigger] Examine the headstone
[Assets needed] face set RM_Face_Grace, indexes 3, 5

Box 1
  Face: RM_Face_Grace / index 3
  Name line: \C[4]GRACE\C[0]
  Body: That's Grandma's name.
        Mom taught me to read it.

Box 2
  Face: RM_Face_Grace / index 5
  Name line: \C[4]GRACE\C[0]
  Body: She knew my name then.

Box 3 (narration)
  Face: none
  Body: \C[7]Dad's wiping his face.
```

Mark [Scene] and [Trigger] so the event builder can place each line.

---

## 8. Where drafts go and how to name them

Drafts go in [`drafts/`](drafts/). Naming:

```
ACT2_<scene name>_script.md        e.g. ACT2_Attic_script.md
```

Use one file per scene and assign an editor for shared work. Once the draft is final, one person integrates it into the engine project.

**Changing existing lines:** record "original / changed to" in a draft, then update the engine source. Re-export the MV reading copy after editing MV events. Regeneration overwrites direct edits to `ACT1_SCRIPT.md`.

---

## 9. Self-check before handover

- [ ] text destined for the historical MV game is English
- [ ] every box has at most 3 rows of body text and at most 40 characters per row
- [ ] the name line is written correctly (all capitals + `\C[n]...\C[0]`), and narration has no name line
- [ ] face indexes were looked up in the reference sheets, not written from memory
- [ ] [Scene] and [Trigger] are marked, so the event builder knows when these lines appear
- [ ] no lines were given to other characters (Aunt and Grandpa stay observation-only)
- [ ] the illness is not explained and there are no instructional hints
- [ ] every expression index you used has already been drawn by the art team

---

## 10. References

- the historical Act I reading copy: [`ACT1_SCRIPT.md`](ACT1_SCRIPT.md)
- which line is in which event and how the switches run: [`../docs/GAME_REFERENCE.md`](../docs/GAME_REFERENCE.md)
- what the face sets look like: [`../art/`](../art/)
