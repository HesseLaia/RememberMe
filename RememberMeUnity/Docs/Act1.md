# Remember Me · Unity Act I

English | [简体中文](Act1.zh-CN.md)

The English Windows playtest includes the Act I funeral and the first scene of Act II. Grace finds Dad crying in the cemetery, then remembers an argument between Dad and Grandma by a doorway. Mom gives Grace the belongings left behind. A music box evokes a brief memory of Grandma's empty house. An illustration of the rainy journey home and the Act II title card lead into Grace's bedroom. The repository's RPG Maker MV version is in the root `game/` directory.

## Play and edit

- After building for Windows, double-click `RememberMe.exe` in `RememberMeUnity/Build/Windows/`. Copy the entire `Windows` folder when sharing the build. You can also [play the Unity web version](https://jeddakholmes-byte.github.io/RememberGame/unity/).
- Tap WASD or the arrow keys to turn, and hold to move. Grace turns toward the pressed direction even when an obstacle blocks movement. Shift runs and exits crouch. C toggles crouch, E or Enter interacts, I opens inventory, and Esc pauses. The pause menu supports saving and loading, and **Continue** on the title screen resumes a saved game. Dialogue and UI use Windows' Times New Roman.
- Use left/right for horizontal choices and up/down for vertical menus, or use the mouse. The selected option has a gold background, bold text, and an arrow. The opening illustration fades to reveal the loaded cemetery before the first dialogue appears.
- Open `RememberMeUnity/` in Unity Hub with editor **6000.6.1f1**. The main scene is `Assets/Scenes/RememberMeAct1.unity`. The project was created with the 2D URP template and uses 2D editor mode, an orthographic camera, and 2D collisions.
- Act I's main route is playable from start to finish. Follow the main path to Grandma's white flowers and gravestone. Gold corner markers appear near the gravestone. Explore the white flowers, old graves, register, relatives, and note freely. Near the puddle on the path, press C to crouch, then E or Enter to view the reflection illustration. Returning to the gate before reading Grandma's gravestone prompts guidance from Mom. After investigating all four distinctive gravestones, the camera briefly moves to Mom and returns to Grace.

## Art specifications

- Objects in a room use a front view from slightly above. Keep table edges horizontal. Books, teacups, baskets, and letters use separate layers positioned against the tabletop. Open and closed doors share the same threshold, with clean transparent outer edges. Act I uses 96 PPU, with characters anchored at their feet.
- The ordinary four-direction sprites for Mom, Dad, Grandma, and Grandpa are retained. Auntie's and Uncle James's low-resolution four-direction sprites were remade. After Grace reads Grandma's gravestone, Dad uses a four-direction pose wiping tears with a tissue. The original MV dialogue portraits appear with each speaker. Grace's idle, walking, running, and crouching poses come from one action sheet, with perspective and silhouette matched to the family art. Each direction uses its idle pose as the crop and scale reference.
- The cemetery contains Grace and five family members. The original mourner art remains in `Assets/Art/Actors`, and its patrol logic remains in `Act1Patrol.cs`.
- The cemetery measures 22 × 15 world units, with 48 grave plots in six rows. Wilted flowers, moss, a pointed headstone, and three pebbles identify four investigable old graves. Ordinary plots use plain headstones. The 3,168 × 2,160 fine-grass background, stone paths, fences, headstones, and characters render separately. The original title and box-sealing illustration are retained, and act cards use chapter names from the original script. Separate images show the full unboxing view, unlocking, a rainy night window, and the journey home.

## Course goals and story sources

The story draws on *RememberMeScript*, two November 18 meeting records, CSS5190 lessons 2 and 3, and the project report guidelines. Act I preserves the child's perspective, showing Dad's frustration under caregiving pressure and grief after losing a family member. Act II begins with a faint sound from the keepsake box in the bedroom. Grace opens it again and first reunites with Grandma in memory in the kitchen. Current acceptance checks cover design and technical behavior. The course's empathy and learning outcomes require a separate player evaluation.

Interaction prompts follow the practice of giving clear actions near an object. Grace receives prompts based on the actual edges of characters or objects, and facing direction helps rank nearby targets. Movement uses readable silhouettes from 2D action games, with walking and running transitions preserving gait phase. [Soul Knight official gameplay guide](https://soulknight.chillyroom.com/et/article/base/67) · [Hollow Knight official introduction](https://www.hollowknight.com/)

## Windows acceptance checks

- Unity 6000.6.1f1 produces the Windows x64 build. Map verification covers 22 reachable interaction or trigger positions in the cemetery, seven in the front room, and six in the back room.
- Windows regression checks cover the Act I fade, turning in place on a tap, turning while blocked at the table, keyboard and mouse focus, walking/running/crouching, Shift exiting crouch, the puddle's crouch prompt and reflection illustration, Mom's triggers from four sides, dialogue portraits, gravestone events, keepsakes, and loading saves.
- [`QA/Reference/Comparison.png`](../QA/Reference/Comparison.png) and [`QA/Reference/Act1Actors.png`](../QA/Reference/Act1Actors.png) preserve early Windows playtest screenshots and character art comparisons.
