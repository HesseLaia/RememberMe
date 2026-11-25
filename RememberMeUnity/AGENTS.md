# Unity Interaction Instructions

English | [简体中文](AGENTS.zh-CN.md)

Read the [current handoff](../docs/HANDOFF.md) before continuing development. Check the version, verification scope, and next tasks.

Read the [interaction conventions](Docs/Interaction.md) before changing input, dialogue, pause menus, inventory, scene collisions, or chapter transitions.

Act I and Act II share `StoryInput` and `Act1HUD`. Keyboard and mouse input must call the same story action, with one action per frame. Preserve the existing painterly style and reuse the actual item art. After a change, verify the affected chapter, room, and sequence of actions.
