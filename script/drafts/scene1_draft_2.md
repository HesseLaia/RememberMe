**English** · [简体中文](scene1_draft_2.zh-CN.md)

Historical v0.4 working draft. Its pixel-art direction and open questions record the design at that stage. Shipped v10 uses illustrated assets. Compare this draft with `KitchenPrototype.cs` before planning changes. See the [current handoff](../../docs/HANDOFF.md).

# RememberMe — Act II, Scene 1

## Grandma's Kitchen | Integrated Script & Level Design v0.4

**Status:** Revised working draft for team review and Unity implementation<br>
**Engine:** Unity 2D<br>
**Visual Style:** 2D pixel art, with gradual surreal environmental changes<br>
**Gameplay:** Narrative exploration, environmental puzzles, object interaction, character cooperation<br>
**Estimated Playtime:** 8–12 minutes (to be validated through playtesting)

**Main Characters:** Grace (age 7), Grandma<br>
**Scene Scope:** Grace's bedroom, Grandma's kitchen, the changing hallway, and the living room entrance

---

# 1. Core Design

## 1.1 Scene Role

This is Grace's first level inside the Memory House.

Following the Act I cemetery funeral, Grace opens the box Grandma left her at home. An old photograph takes her into a dreamlike space of family memories.

She is surprised to find Grandma, who has died, back in the kitchen, inviting her to bake together as she always did.

Grace wants to bring Grandma home. Grandma says they must finish the cookies first and take them to Dad in the living room.

This gives the player a concrete, natural goal: **help Grandma finish the cookies and go to the living room together.**

As baking progresses, the kitchen becomes unstable. The player uses observation, reasoning, and cooperation to overcome difficulties and find the way to the living room.

## 1.2 Three Design Goals

**Narrative and Characters**

Baking reveals Grandma's earlier life, her closeness with Grace, and her love for her son.

The player also notices traces of Dad's care, setting up a later explanation of the family conflict in Act I.

**Gameplay and Interaction**

The player investigates the environment, gathers clues, restores the recipe, and bakes with Grandma.

They then use previously gathered spatial information to solve the hallway puzzle and return to the kitchen for Grandma.

**Educational Experience**

Familiar activities, visual prompts, shared tasks, and companionship show difficulties that people with cognitive impairment may face in daily life and support their families can offer.

Puzzles address the immediate situation. Grandma's illness continues independently of puzzle success.

## 1.3 Rules of the Memory House

For this level, the Memory House is provisionally a symbolic world formed from Grace's memories, imagination, and knowledge of her family's experiences.

- Spatial changes reflect the symbolic world. They develop independently of real-time changes in Grandma's condition.
    
- The player can temporarily stabilize local spaces. The whole Memory House will eventually fade.
    
- Interactions with Grandma affect the current scene. Past events remain fixed.
    
- The rules emerge gradually through the story.
    

---

# 2. Scene Flow

|Stage|Main Event|Player Action|Narrative Role|
|---|---|---|---|
|K00|Open the memory box|Examine the photograph|Enter the Memory House from reality|
|K01|Reunite with Grandma|Explore the kitchen and talk to Grandma|Establish motivation and relationships|
|K02-A|Find ingredients|Examine cabinets, recipe, and note|Discover traces of Dad's care|
|K02-B|Recipe becomes unclear|Collect picture cards and restore their order|Experience prompts and cognitive support|
|K02-C|Bake together|Share the baking steps|Show Grandma's retained abilities|
|K03|Kitchen begins to collapse|Try to leave|Experience the first clear spatial disturbance|
|K04|Three-door hallway|Observe clues and identify the living room|Reason from earlier information|
|K05|Return for Grandma|Go back to the kitchen and invite her along|Put cooperation and companionship into action|
|K06|Enter the living room|Bring in the cookies|Pay off earlier clues and lead into the next scene|

---

# 3. Scene Setup and Objects

## 3.1 Grace's Bedroom

Setting: Grace's bedroom on the evening of the funeral.

Main interactive objects:

- Memory Box: the box received in Act I.
    
- Old Photograph: Grace as a younger child baking with Grandma in the kitchen.
    
- Music Box: a slightly rusty old music box that currently plays only a short melody.
    

The music box foreshadows later events and is optional for completing this level.

## 3.2 Grandma's Kitchen

Use a small or medium kitchen map with these areas:

|Area|Interactive Objects|Purpose|
|---|---|---|
|Left cabinets|Flour, sugar jar, salt jar|Ingredient puzzle|
|Central counter|Recipe, mixing bowl, wooden spoon, baking tray|Baking and step ordering|
|Fridge|Eggs, butter, hand-drawn step card|Ingredients and clues|
|Wall|Family photograph, clock|Narrative clues and spatial changes|
|By the door|Blue serving tray|Carry cookies to the living room|
|Kitchen exit|Hallway to the living room|Spatial puzzle|

### Key Visual Clues

**Family Photograph**

The wall photograph shows Dad as a child sitting by the living room window, with blue curtains behind him.

The photograph has two purposes:

1. Show Grandma and Dad's earlier relationship.
    
2. Provide a clue for reasoning through the later three-door hallway.
    

**Blue Ribbon**

A blue ribbon distinguishes the sugar jar from the salt jar.

The player later learns that Dad added this marker to help Grandma recognize ingredients.

**Picture Cards**

Dad drew the recipe cards for Grandma to consult while baking.

**The Biggest Cookie**

Grandma habitually saves the biggest cookie for Dad.

This detail appears in the kitchen, returns in the living room, and can extend to the game's ending.

---

# 4. Event K00 — The Memory Box

**Location:** Grace's Bedroom<br>
**Trigger:** The player examines the memory box

### Scene Direction

Light rain continues outside the window.

Grace's bedroom is quiet.

The memory box sits beside the bed.

The player approaches and opens it.

An old photograph and a small music box are inside.

Grace picks up the photograph.

It shows a younger Grace standing with Grandma in the kitchen. Freshly baked cookies sit on the table.

### Dialogue

**GRACE**

That's me.

And Grandma.

...

I was so little.

She examines the photograph.

**GRACE**

We used to make cookies.

Grace touches the photograph.

A faint sound of a spoon against a bowl can be heard.

The rain gradually fades.

**FADE OUT**

**FADE IN — GRANDMA'S KITCHEN**

### Implementation

Examining `MemoryBox` triggers an interaction sequence.

- Display the photograph UI.
    
- Play a brief ambient sound.
    
- Fade out the bedroom.
    
- Load or switch to the kitchen scene.
    
- Start the K01 cutscene.
    

---

# 5. Event K01 — A Familiar Voice

**Location:** Grandma's Kitchen<br>
**Trigger:** Plays automatically on entering the kitchen<br>
**Memory State:** S0 — Warm and Stable

### Scene Direction

The kitchen is bright with warm sunlight.

Grandma wears an apron and stands at the counter.

She is preparing to bake.

**GRANDMA**

Gracie?

There you are.

Come help me, sweetheart.

Grace stops in the kitchen doorway.

**GRACE**

Grandma?

Grandma turns around.

**GRANDMA**

What is it, dear?

**GRACE**

You're here!

Grandma smiles.

**GRANDMA**

Of course I am.

Come on.

We have cookies to make.

Grace approaches her.

**GRACE**

Can you come home with me?

Grandma looks at Grace.

**GRANDMA**

After the cookies, dear.

Your dad's waiting.

Grace pauses.

**GRACE**

Okay.

I'll help you.

**PLAYER CONTROL ENABLED**

**Objective:** Help Grandma make cookies.

## 5.1 Optional Interaction — Family Photograph

The player examines the photograph on the wall.

**GRACE**

That's Dad!

He's so little.

**GRANDMA**

He was.

Always hungry, too.

**GRACE**

Where was he sitting?

**GRANDMA**

By the living room window.

See the blue curtains?

**GRACE**

Oh, yeah.

**GRANDMA**

He always waited there for cookies.

Grace looks at the photograph again.

This interaction records the living room clue as discovered.

Players who skip it can return from the hallway to examine it later.

## 5.2 Optional Interaction — Music Box

The player examines the small music box by the door.

A short, incomplete melody plays.

**GRACE**

This sounds familiar.

**GRANDMA**

Your dad liked that song.

I used to play it at bedtime.

**GRACE**

Can you play it again?

**GRANDMA**

Maybe later, sweetheart.

The cookies first.

This interaction foreshadows later events and is optional for completing the level.

---

# 6. Puzzle K1 — The Old Recipe

**Type:** Environmental Puzzle + Sequence Puzzle + Cooperative Interaction

**Overall Goal:** Make cookies with Grandma.

The puzzle has three consecutive stages within one everyday activity.

## 6.1 Step A — Finding the Ingredients

### Trigger

The player talks to Grandma to start making cookies.

**GRANDMA**

Could you get the flour, butter, eggs, and sugar?

I put them away somewhere.

**GRACE**

Okay!

### Interactive Objects

**Cabinet A**

Contains flour in intact packaging.

**Fridge**

Contains eggs and butter.

**Cabinet B**

Two white jars of similar size.

One has a blue ribbon. The other is unmarked.

The original written labels on both jars have worn away.

**Recipe Book**

A note is attached beside the recipe.

### Note — Written by Dad

Mom,

Sugar has the blue ribbon.

Salt has no ribbon.

Take your time.

Love, your son.

**GRACE**

Dad wrote this?

### Puzzle Logic

The player collects flour, butter, and eggs, then uses the note to identify the sugar jar.

The correct sugar jar has a blue ribbon.

Placing all ingredients on the counter unlocks the next stage.

### Incorrect Feedback

If the player chooses the wrong jar:

**GRACE**

Wait.

This might be salt.

The player can put the jar back and choose again.

### Completion

When the ingredients are ready:

**GRANDMA**

There it is!

Good job, Gracie.

Grandma takes the sugar jar.

**GRANDMA**

Your dad used to steal the sugar.

**GRACE**

Dad?

**GRANDMA**

Just a little.

He thought I never noticed.

Grandma smiles.

### Narrative Purpose

The blue ribbon is both a puzzle clue and evidence of Dad's care for Grandma.

This stage introduces Dad's thoughtfulness through the marker. His wider emotions unfold later, without a full caregiving diary here.

---

## 6.2 Step B — The Missing Steps

### Trigger

With all ingredients ready, the player examines the recipe on the counter.

### Scene Direction

Grandma opens the recipe book.

She looks at the familiar page but briefly cannot remember what to do next.

**GRANDMA**

Now, let's see...

What comes next?

**GRACE**

The recipe?

**GRANDMA**

Yes, dear.

But this part is hard to read.

### Environmental Change — S0 to S1

- The kitchen lighting dims slightly.
    
- The wall clock stops.
    
- The writing in the middle of the recipe gradually blurs.
    
- The ambient music changes slightly.
    

**GRACE**

Grandma?

Did the lights just...

**GRANDMA**

Hmm?

Grace looks at the ceiling.

**GRACE**

Never mind.

### Puzzle Setup

The recipe retains its first and last steps:

1. Cream the butter and sugar.
    
2. [Missing]
    
3. [Missing]
    
4. [Missing]
    
5. Bake the cookies.
    

Three simple picture cards drawn by Dad are in the kitchen.

**Card A — Add eggs and flour**

Location: beside the mixing bowl.

**Card B — Mix into dough**

Location: near the counter.

**Card C — Shape the cookies**

Location: on the fridge door.

### Player Task

The player finds all three picture cards.

They then open the ordering interface at the counter and place the cards in the recipe's three empty slots.

**Correct Order**

1. Cream butter and sugar.
    
2. Add eggs and flour.
    
3. Mix into dough.
    
4. Shape the cookies.
    
5. Bake the cookies.
    

### Wrong Order Feedback

If the order is wrong:

**GRACE**

Hmm.

That doesn't look right.

Try again?

The player can reorder the cards repeatedly, with no additional penalty.

After repeated failures, offer a clearer hint, such as mixing the ingredients before dividing the dough into small portions.

### Completion Dialogue

When the player puts the cards in the correct order:

**GRACE**

Look, Grandma.

Dad drew these.

Grandma examines the cards.

**GRANDMA**

Oh.

These are nice.

**GRACE**

The pictures tell us what to do.

**GRANDMA**

Then let's try.

---

## 6.3 Step C — Making Cookies Together

### Trigger

The recipe order is restored.

Grandma picks up the wooden spoon.

She looks at the ingredients and pauses.

**GRANDMA**

Oh.

What was I doing?

**GRACE**

We were mixing.

Grace points to the picture card.

**GRACE**

Look.

Like this.

Grandma watches.

**GRANDMA**

Right.

Shall we do it together?

**GRACE**

Yeah!

### Cooperative Gameplay

Use three short stages that require player input.

**Interaction 1 — Mixing**

The player performs a simple mixing action at the counter.

Use brief directional input, mouse interaction, or button presses.

Grandma joins in. Complex independent NPC control is unnecessary.

**Interaction 2 — Shaping**

The player divides the dough into portions and puts them on the baking tray.

The player can place portions in designated spots while Grandma helps arrange them.

**Interaction 3 — The Biggest Cookie**

Grandma pauses as the last portion goes onto the tray.

**GRANDMA**

Wait.

Let's make this one bigger.

**GRACE**

Why?

**GRANDMA**

Your dad always likes the biggest one.

Grace looks at the dough.

**GRACE**

Even now?

Grandma smiles.

**GRANDMA**

Oh, yes.

The player finishes placing the last portion.

### Completion

**GRANDMA**

There.

That should do it.

Brief fade out.

Play an oven sound.

Fade back in with baked cookies on the table.

**GRACE**

They smell good!

**GRANDMA**

They do, don't they?

We made them together.

### Gameplay Notes

Keep cooperative inputs simple. Input mistakes must not trigger deterioration in Grandma's condition, shame, or similar penalties.

The aim is to show Grandma participating in a familiar activity while Grace supports her through prompts and shared tasks.

---

# 7. Event K03 — The First Collapse

**Trigger:** Cookie making is complete<br>
**Memory State:** S1 → S2

### Scene Direction

Grandma puts the cookies on the blue serving tray.

**GRANDMA**

All done.

Let's bring these to your dad.

**GRACE**

Okay!

The player receives the tray of cookies.

Grace walks toward the kitchen exit.

Suddenly, the room visibly changes.

- The walls gradually lose their color.
    
- The previously clear furniture edges blur.
    
- The wall clock loses its minute hand.
    
- The living room door briefly disappears and reappears.
    

**GRACE**

Grandma?

Something's wrong.

Grandma looks toward the kitchen exit.

**GRANDMA**

Where's the living room?

**GRACE**

It's right there.

The player walks to the doorway.

Fade out.

When the scene returns, Grace is back on the other side of the kitchen.

**GRACE**

Huh?

Wasn't I just here?

The player tries to leave again.

This time, an unfamiliar short hallway appears beyond the door.

**Objective Updated:** Find the way to the living room.

---

# 8. Puzzle K2 — The Changing Hallway

**Type:** Spatial Puzzle + Environmental Clue Recognition

## 8.1 Setup

Three similar-looking doors appear outside the kitchen.

The player can examine them freely.

### Door A — The Clock

Examining it reveals a ticking clock.

**GRACE**

I hear a clock.

Opening it sends Grace back to the kitchen.

The player can return to the hallway and keep trying.

### Door B — The Blue Curtains

A corner of a blue curtain is visible through the gap.

The light is slightly warmer.

**GRACE**

Something blue...

It looks like curtains.

If the player examined the family photograph earlier:

**GRACE**

Like the ones in Dad's picture!

This is the correct living room entrance.

### Door C — The Water

Examining it reveals a faint dripping sound.

**GRACE**

Is that water?

Opening it leads briefly into a blurred space, then returns the player to the hallway.

A full additional room is unnecessary.

## 8.2 Core Puzzle Logic

The player uses the kitchen photograph to deduce that the real living room has blue curtains.

Door B is therefore the correct entrance.

Players who skipped the photograph can return to the kitchen or gain hints by repeatedly examining the doors.

### Correct Door

The player opens Door B.

A warm, bright living room appears beyond it.

Grace takes a step forward and stops.

She turns back.

Grandma has not followed.

**GRACE**

Grandma?

...

Where is she?

### New Objective

Go back for Grandma.

---

# 9. Event K05 — Don't Leave Grandma Behind

**Location:** Grandma's Kitchen<br>
**Trigger:** The player returns to the kitchen after finding the correct door

### Scene Direction

The kitchen is darker now.

Grandma is still near the counter.

She looks toward the exit, appearing uneasy.

**GRANDMA**

Grace?

Where did you go?

**GRACE**

I'm here!

Grace approaches her.

**GRANDMA**

I can't find the door.

Grace looks toward the hallway.

**GRACE**

I found it.

Come with me.

Grandma hesitates.

**GRANDMA**

Will you stay with me?

**GRACE**

Yes.

Grace holds out her hand.

Grandma takes it.

**GRANDMA**

All right, sweetheart.

Let's go.

**PLAYER CONTROL ENABLED**

The player leads Grandma back to the hallway and through Door B.

### Narrative Purpose

This is one of the level's central actions.

Having found the way out, the player returns for Grandma before completing the level.

The return echoes baking together: Grace and Grandma now face the unfamiliar space together as well as sharing a task.

Returning and holding hands carry the emotion of this sequence. Leading Grandma requires no additional multistep treatment minigame.

---

# 10. Event K06 — By the Blue Curtains

**Location:** Living Room<br>
**Memory State:** S3 — Temporarily Stable

### Scene Direction

Grace and Grandma finally reach the living room.

The light gradually grows warm again.

Blue curtains hang by the window.

The room closely resembles the old photograph on the kitchen wall.

Grandma sets the blue tray on the coffee table.

She takes the biggest cookie and places it by the window.

**GRANDMA**

There.

Your dad always sits right here.

He likes the biggest one.

Grace looks at the empty chair.

**GRACE**

Where is he now?

Grandma looks toward the hallway.

**GRANDMA**

He'll be home from school soon.

Grace looks confused.

**GRACE**

From school?

**GRANDMA**

Oh.

I should get his coat ready.

It's cold outside.

Grandma slowly walks toward the entryway.

The living room lights begin to dim.

A key slides into a lock in the distance.

**GRACE**

Grandma?

Wait!

**SCENE END**

**Next Scene:** Act II, Scene 2 — The Coat by the Door

---

# 11. Unity Implementation Specification

This section defines gameplay logic. The implementation can follow the project's existing code architecture.

Unity's Collider2D, interaction components, dialogue system, triggers, animation, and scene-state controls can implement it.

## 11.1 Suggested Level States

|State|Description|
|---|---|
|`BedroomIntro`|Memory box transition|
|`KitchenIntro`|Reunion with Grandma|
|`IngredientPuzzle`|Ingredient puzzle|
|`RecipePuzzle`|Picture-card ordering|
|`CooperativeBaking`|Baking together|
|`FirstCollapse`|First visible kitchen collapse|
|`HallwayPuzzle`|Three-door hallway|
|`ReturnForGrandma`|Return to the kitchen for Grandma|
|`LivingRoomEnding`|Living room ending|

Advance through the states in sequence after completing key events.

## 11.2 Suggested Event Flags

|Flag|Description|
|---|---|
|`photoExamined`|Family photograph examined|
|`sugarClueFound`|Blue ribbon clue found|
|`ingredientsReady`|Correct ingredients ready|
|`recipeSolved`|Recipe order restored|
|`bakingCompleted`|Cooperative baking completed|
|`cookiesCollected`|Cookie tray collected|
|`hallwayActivated`|Changing hallway activated|
|`correctDoorFound`|Door B found|
|`grandmaFollowing`|Grandma is accompanying Grace|
|`kitchenCompleted`|First level completed|

These names describe suggested logic. Adapt them to the existing Unity variables.

## 11.3 Key Interaction Requirements

**Object Interaction**

Show a simple examine prompt when the player approaches an object.

Support:

- Object descriptions or close-ups.
    
- Picking up or selecting items.
    
- Using items at the corresponding locations.
    
- Playing dialogue or changing event state.
    

**Recipe Puzzle UI**

After collecting three picture cards, the player arranges them in one interface.

Use drag-and-drop or buttons to swap their order.

Allow retries.

**Cooperative Baking**

Use a simplified input sequence or interaction prompts.

Complex physics simulation is unnecessary.

**Memory House States**

Use preset stage transitions for the environmental changes.

- S0: complete, brightly lit kitchen.
    
- S1: slight fading and a stopped clock.
    
- S2: clear fading and a looping exit.
    
- S3: hallway changes and local spaces reconnect.
    

Switch Tilemaps, SpriteRenderer states, lights, overlays, and sound effects to convey these changes.

**Hallway Door System**

The three doors have preset outcomes:

- Door A → return to the kitchen.
    
- Door B → unlock the living room entrance.
    
- Door C → brief wrong space, then return to the hallway.
    

**Grandma Following**

After finding the correct door, dialogue triggers Grandma's accompanying Grace.

Use a following component or limited preset movement paths.

Full NPC pathfinding AI is unnecessary.

## 11.4 Preventing Progression Blocks

All required puzzle clues remain available for repeated examination.

Wrong ingredients, recipe orders, and doors allow another attempt without permanent failure.

Players who miss the blue-curtain clue can return to the kitchen photograph.

After repeated failures in the hallway, gradually add Grace's hints:

**GRACE**

The living room...

I've seen it before.

Then:

**GRACE**

Dad was sitting by the blue curtains.

After the correct door is found, Grace's dialogue directs the player to return for Grandma.

---

# 12. Narrative and Educational Notes

## 12.1 Following Up Act I

In Act I, Grace cannot understand why Dad once lost his temper with Grandma and is now so sad.

The kitchen adds another part of their relationship: Dad used notes, color markers, and picture cards to help Grandma maintain familiar routines.

This lays the foundation for Scene 2's treatment of caregiving pressure.

## 12.2 Grandma's Character

Give Grandma a life and personality alongside her memory difficulties and support needs.

In this scene she also:

- Remembers Dad stealing sugar as a child.
    
- Bakes with Grace.
    
- Jokes and cares for Grace.
    
- Habitually saves the biggest cookie for her son.
    

Her life, personality, and family relationships remain central to her character.

## 12.3 Depicting Care

Visual prompts, smaller steps, and cooperation show ways families can support people with cognitive difficulties in familiar activities.

Their usefulness varies between individuals. Completing an activity does not restore cognitive ability.

## 12.4 Symbolism of the Collapse

The collapsing Memory House is an artistic device, not a medical simulation or an accurate account of the subjective experience of Alzheimer's disease.

The player can temporarily find a path forward while the space continues to fade.

---

# 13. Production Priorities

**Priority A — Required**

- Bedroom memory-box transition to the kitchen.
    
- Reunion with Grandma and basic dialogue.
    
- Ingredient examination and selection.
    
- Recipe picture-card ordering.
    
- Simplified cooperative baking.
    
- Kitchen-state transitions.
    
- Three-door hallway puzzle.
    
- Return for Grandma.
    
- Living room ending.
    

**Priority B — Recommended**

- Optional family-photograph dialogue.
    
- Dad's blue-ribbon note.
    
- Visual detail of the biggest cookie.
    
- Optional music-box interaction.
    
- Progressive hints after wrong choices.
    

**Priority C — Add If Time Allows**

- Richer character animation.
    
- Jigsaw minigame.
    
- Additional environmental interactions.
    
- Detailed spatial-fading effects.
    
- Complex two-character cooperative inputs.
    
- Full spatial-stability UI.
    

---

# 14. Current Open Questions

1. Does the Unity project already support ingredient pickup, inventory, and item use? If these are absent, choose simplified interactions or a lightweight inventory.
    
2. Should recipe ordering use drag-and-drop, swap buttons, or dialogue choices?
    
3. How can cooperative baking provide hands-on interaction within a small, focused sequence?
    
4. How should the three-door hallway fit the existing scene while keeping new maps and assets manageable?
    
5. Should Grandma use actual following or preset movement paths?
    
6. Will the old photograph and music box be used consistently in the later full script?
    
7. Validate the first level's actual playtime and difficulty against the targets through Unity prototype playtesting.
    

**Version Summary:** v0.4 retains the kitchen puzzle's core structure, incorporates relationship and foreshadowing details from the newer outline, and keeps the first level's staging and interaction scope focused. Later levels continue exploring the family relationships introduced in Act I.
