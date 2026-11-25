# Interaction Conventions

English | [简体中文](Interaction.zh-CN.md)

Act I and Act II share `StoryInput` and `Act1HUD`. Read this document before changing interactions.

| State | Keyboard | Mouse | Exit |
| --- | --- | --- | --- |
| Free exploration | WASD moves. E, Enter, numpad Enter, or Space activates the prompt at the bottom of the screen | Click the bottom interaction prompt to activate the same target | Esc pauses. I opens inventory |
| Dialogue | E, Enter, numpad Enter, or Space continues | Click the dialogue box to continue | Finish the current dialogue |
| Choice or pause menu | WASD or arrow keys selects. E, Enter, numpad Enter, or Space confirms | Hover to select, then click to confirm | Esc closes the menu |
| Recipe card sorting | Use menu controls. Backspace undoes the last card | Select **Undo last card**, or click a placed card to remove it and all later cards | Esc closes temporarily and preserves the unfinished order |
| Stirring | E, Enter, numpad Enter, or Space stirs once | Click **Stir** | Esc closes and preserves the count |
| Placing dough | E, Enter, numpad Enter, or Space fills the next available slot | Drag dough to a slot or click a slot | Esc closes and preserves progress |

## Input and display

- Consume confirmation input once per frame through `StoryInput.Consume`. Menus submit through `Act1HUD.HandleMenuInput`.
- `EventSystem.sendNavigationEvents` is disabled. The menu method above handles directional and confirmation input. Mouse input continues through `EventSystem`. This prevents a single Enter press from submitting through two paths.
- A menu's `Choose` calls `CloseChoices` before executing the callback. Undo checks inside the callback must use the chapter and placed cards because `sortingCards` has already been cleared.
- Scripted walking uses `GraceMotor.MoveScripted`, preserving gait and using the same collision checks. `Place` handles scene transitions and save restoration. `GraceMotor.LateUpdate` applies the final animation update so an idle pose cannot overwrite it.
- World interactions use the current `nearby` target's existing `Use`/`Trigger` method. Clicking a prompt uses that same entry point and obeys the same distance, collision, and story conditions.
- Dialogue clicks go through `Act1HUD.TakeContinue`. Do not also poll full-screen mouse clicks: a menu click could then skip the next dialogue line.
- Add story images, overlays, and clickable UI to the existing Canvas. A full-screen `OnGUI` overlay can cover Canvas dialogue. This previously caused the water portal to appear black.
- Story state determines which collected objects enter inventory. Display actual item art and keep collected objects detached from the character.

## Position and flow

- The cemetery and Act I interiors use 96 PPU, with the origin at the bottom left. Act II, including the kitchen, uses 134 PPU and logical coordinates starting at the top left. Convert these coordinates through `World`.
- The first flashback with Dad and the music box's empty house are separate rooms. After changing a fireplace, test front and side approaches in both rooms. If an old save places Grace inside a new collider, Enter restores her to walkable ground.
- The kitchen threshold remains interactive throughout the chapter. Before baking finishes, it explains why Grace stays. After she takes the tray, it advances the story loop. Prompt placement uses `doorwayPassage` and the character's foot radius. The kitchen and hallway entrances follow the same rule.
- Three recipe cards appear on their respective cabinet surfaces and refrigerator. Players can collect them in any order. Collection hides each card in the room while retaining its original art and progress in inventory. Viewing a card again does not add another copy.
- The pause menu includes resume, inventory, save, load, and return to title. Act I and Act II save their own progress. Kitchen tests use a separate QA save key to preserve player saves.

## Regression verification

After running `Act1Builder.Build`, launch the Windows build with `-remember-smoke -with-kitchen -kitchen-capture` followed by a screenshot directory. Check:

- Collisions in the first flashback and empty house.
- Pause-menu inventory and kitchen saving and loading.
- Collecting cards in any order, viewing them repeatedly, and retrying an incorrect recipe.
- Resuming stirring after closing, drag and drop, and threshold prompts.
- Visible water-portal dialogue and a clickable return action.
- Grandma's following behavior and the ending.
