**English** · [简体中文](README.zh-CN.md)

# Script drafts

Store scene drafts here. Use the [script guide](../README.md) for the historical MV format and voice rules. Current Unity implementation follows the [Unity project guide](../../RememberMeUnity/README.md).

The [kitchen v0.4 draft](scene1_draft_2.md) preserves an earlier design, including pixel art and unresolved implementation questions. Shipped v10 uses illustrated assets. Compare the draft with `KitchenPrototype.cs` before changing the game. Each draft has an English default and a `.zh-CN.md` companion.

## Naming

```
ACT2_<scene name>_script.md        e.g. ACT2_Attic_script.md
```

Use one file per scene and designate an editor when several people contribute. Name the Chinese companion `ACT2_<scene name>_script.zh-CN.md` and add reciprocal language links.

## How to write a change to an existing line

Record the event or Unity trigger and the original and replacement lines in a draft. For the historical MV version, include map, event, and coordinates. Regenerate `../ACT1_SCRIPT.md` after updating engine events. Example:

```markdown
# Change: the typo in Act I OldGrave_4

## Event
Map001 → 017 OldGrave_4 (coordinates 18,4)

## Original
It seems to be abondoned a long time ago.

## Changed to
It looks like no one has been here
for a long time.

## Reason
Fix the `abondoned` spelling; also closer to a child's concrete observation.
```

Include the **event name and coordinates** for MV, or the **source file and trigger** for Unity, so the implementer can locate the line.

## Once it is final

One person integrates the approved draft into the engine project, then **plays from New Game** to verify dialogue placement and sequence. For MV changes, regenerate exported reading copies such as `../ACT1_SCRIPT.md`. Update both document languages together.
