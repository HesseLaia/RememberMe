# RememberMe — Act II, Scene 1

## Grandma's Kitchen | Integrated Script & Level Design v0.4

**Status:** Revised working draft for team review and Unity implementation  
**Engine:** Unity 2D  
**Visual Style:** 2D pixel art, with gradual surreal environmental changes  
**Gameplay:** Narrative exploration, environmental puzzles, object interaction, character cooperation  
**Estimated Playtime:** 8–12 minutes (to be validated through playtesting)

**主要角色：** Grace（7岁）、Grandma  
**场景范围：** Grace 的卧室、奶奶的厨房、变形走廊、客厅入口

---

# 1. Core Design — 核心设计

## 1.1 场景定位

本场景是 Grace 进入 Memory House 后的第一个关卡。

承接第一幕墓园葬礼，Grace 在家中打开奶奶留下的遗物箱，通过一张旧照片进入似真似幻的家庭记忆空间。

她惊讶地发现，已经去世的奶奶竟然重新出现在厨房里，像往常一样邀请她一起做点心。

Grace 希望带奶奶回家。奶奶却说，要先把点心做好，送到客厅给爸爸。

玩家因此获得一个具体、自然的行动目标：**帮助奶奶完成点心，并一起前往客厅。**

随着制作过程推进，厨房逐渐变得不稳定。玩家需要通过观察、推理与合作克服困难，并最终找到通往客厅的路。

## 1.2 本场景的三个设计目标

**Narrative — 剧情与人物**

通过做点心这一日常活动，呈现奶奶过去的生活、她与 Grace 的亲密关系，以及她对儿子的关爱。

同时，让玩家注意到爸爸曾经留下的照护痕迹，为后续解释第一幕中的家庭冲突埋下伏笔。

**Gameplay — 谜题与互动**

玩家需要主动调查环境、获取线索、整理食谱，并与奶奶共同完成制作。

随后，玩家需要利用此前获得的空间信息解决走廊谜题，并主动返回厨房接奶奶同行。

**Educational Experience — 教育体验**

通过熟悉的活动、视觉提示、合理分工与陪伴，表现认知障碍患者在日常生活中可能遇到的困难，以及家庭成员能够提供的支持。

玩家解决的是当前情境中的问题，而不是通过解谜逆转奶奶的病情。

## 1.3 记忆之屋的基本规则

本关卡暂定 Memory House 是由 Grace 的回忆、想象及她所了解的家庭经历构成的象征性世界。

- 空间的变化不等同于奶奶真实病情的即时变化。
    
- 玩家可以暂时稳定局部空间，但无法阻止整个记忆之家最终消散。
    
- 玩家与奶奶的互动影响当前场景，却不能改变过去已经发生的事实。
    
- 记忆空间的规则在剧情中逐渐呈现，不需要一开始向玩家完整解释。
    

---

# 2. Scene Flow — 完整流程

|阶段|主要事件|玩家行动|叙事功能|
|---|---|---|---|
|K00|打开遗物箱|调查照片|从现实进入 Memory House|
|K01|与奶奶重逢|探索厨房、与奶奶对话|建立行动动机和人物关系|
|K02-A|寻找食材|调查橱柜、食谱和便签|发现爸爸留下的照护线索|
|K02-B|食谱出现问题|收集图卡、恢复顺序|体验信息提示与认知支持|
|K02-C|合作烘焙|分工完成制作步骤|展现奶奶保留的能力|
|K03|厨房开始崩塌|尝试离开|第一次明显感受到空间异常|
|K04|三门走廊|观察线索、辨别客厅|利用先前信息进行推理|
|K05|返回接奶奶|回到厨房、邀请奶奶同行|将合作和陪伴转化为实际行动|
|K06|进入客厅|把点心送到客厅|回收伏笔并引出下一场景|

---

# 3. Scene Setup — 场景与物件

## 3.1 Grace's Bedroom

场景：葬礼当晚，Grace 的卧室。

主要交互物件：

- Memory Box：第一幕获得的遗物箱。
    
- Old Photograph：Grace 小时候与奶奶在厨房做点心的照片。
    
- Music Box：一枚有些生锈的旧八音盒，暂时只能奏出一小段旋律。
    

八音盒主要承担后续剧情伏笔，不作为第一关必需的谜题道具。

## 3.2 Grandma's Kitchen

建议采用一张中小型厨房地图，主要包含以下区域：

|区域|交互物件|作用|
|---|---|---|
|左侧橱柜|面粉、糖罐、盐罐|食材谜题|
|中央料理台|食谱、搅拌碗、木勺、烤盘|制作与步骤排序|
|冰箱|鸡蛋、黄油、手绘步骤卡|食材获取与线索收集|
|墙面|家庭照片、挂钟|叙事线索、空间变化|
|门边|蓝色托盘|送点心到客厅|
|厨房出口|通往客厅的走廊|空间解谜|

### 重要视觉线索

**Family Photograph**

墙上的照片中，年幼的爸爸坐在客厅窗边，身后是蓝色窗帘。

照片主要有两个用途：

1. 展现奶奶和爸爸过去的关系。
    
2. 为后续三门走廊提供可推理的线索。
    

**Blue Ribbon**

糖罐上系着一条蓝丝带，用于与盐罐区分。

玩家后来发现，这是爸爸为帮助奶奶辨认食材而做的标记。

**Picture Cards**

食谱步骤卡由爸爸绘制，帮助奶奶在做点心时参考。

**The Biggest Cookie**

奶奶习惯把最大的一块曲奇留给爸爸。

这一细节在厨房出现，客厅回收，并可以继续延伸至游戏结局。

---

# 4. Event K00 — The Memory Box

**Location:** Grace's Bedroom  
**Trigger:** 玩家调查遗物箱

### Scene Direction

窗外仍然下着小雨。

Grace 的卧室很安静。

遗物箱放在床边。

玩家走近并打开盒子。

盒子里放着一张旧照片和一个小八音盒。

Grace 拿起照片。

照片中，年龄更小的 Grace 与奶奶站在厨房里。桌上摆着刚做好的曲奇。

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

玩家调查 `MemoryBox` 后触发交互序列。

- 展示照片 UI。
    
- 播放简短环境音。
    
- 淡出卧室。
    
- 加载或切换至厨房场景。
    
- 启动 K01 自动剧情。
    

---

# 5. Event K01 — A Familiar Voice

**Location:** Grandma's Kitchen  
**Trigger:** 进入厨房时自动播放  
**Memory State:** S0 — Warm and Stable

### Scene Direction

厨房明亮，带有温暖的阳光。

奶奶穿着围裙，站在料理台前。

她正在准备做点心。

**GRANDMA**

Gracie?

There you are.

Come help me, sweetheart.

Grace 停在厨房门口。

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

玩家调查墙上的照片。

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

此交互记录玩家已发现的客厅线索。

如果玩家未调查照片，后续在走廊仍可返回查看。

## 5.2 Optional Interaction — Music Box

玩家调查门边的小八音盒。

播放一段短促、不完整的旋律。

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

此交互主要作为后续伏笔，不影响本关卡通关。

---

# 6. Puzzle K1 — The Old Recipe

**类型：** Environmental Puzzle + Sequence Puzzle + Cooperative Interaction

**整体目标：** 与奶奶合作制作曲奇。

谜题分为三个连续阶段，但统一为同一项日常活动。

## 6.1 Step A — Finding the Ingredients

### Trigger

玩家与奶奶对话，开始制作曲奇。

**GRANDMA**

Could you get the flour, butter, eggs, and sugar?

I put them away somewhere.

**GRACE**

Okay!

### Interactive Objects

**Cabinet A**

里面放着面粉，包装完整。

**Fridge**

里面放着鸡蛋和黄油。

**Cabinet B**

两个大小相似的白色罐子。

一个系着蓝色丝带，另一个没有标记。

两者原有的文字标签均已经磨损。

**Recipe Book**

食谱旁贴着一张便签。

### Note — Written by Dad

Mom,

Sugar has the blue ribbon.

Salt has no ribbon.

Take your time.

Love, your son.

**GRACE**

Dad wrote this?

### Puzzle Logic

玩家需要收集面粉、黄油、鸡蛋，并依据便签辨别糖罐。

正确的糖罐系着蓝色丝带。

玩家将所有食材放到料理台后，解锁下一阶段。

### Incorrect Feedback

如果玩家选错罐子：

**GRACE**

Wait.

This might be salt.

玩家可以放回罐子重新选择。

### Completion

当食材准备完成：

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

蓝丝带既是解谜线索，也是爸爸曾经照顾奶奶的证据。

这一阶段只暗示爸爸的体贴，不通过完整照护日记直接解释他的全部情绪。

---

## 6.2 Step B — The Missing Steps

### Trigger

食材全部准备完成，玩家调查料理台上的食谱。

### Scene Direction

奶奶打开食谱。

她看着熟悉的页面，却一时想不起来接下来该做什么。

**GRANDMA**

Now, let's see...

What comes next?

**GRACE**

The recipe?

**GRANDMA**

Yes, dear.

But this part is hard to read.

### Environmental Change — S0 to S1

- 厨房灯光轻微变暗。
    
- 墙上的时钟停止。
    
- 食谱中间部分字迹逐渐模糊。
    
- 环境音乐发生轻微变化。
    

**GRACE**

Grandma?

Did the lights just...

**GRANDMA**

Hmm?

Grace looks at the ceiling.

**GRACE**

Never mind.

### Puzzle Setup

食谱保留首尾两步：

1. Cream the butter and sugar.
    
2. [Missing]
    
3. [Missing]
    
4. [Missing]
    
5. Bake the cookies.
    

厨房里有爸爸留下的三张简笔画步骤卡。

**Card A — Add eggs and flour**

位置：搅拌碗旁。

**Card B — Mix into dough**

位置：料理台附近。

**Card C — Shape the cookies**

位置：冰箱门上。

### Player Task

玩家需要找到全部三张图卡。

随后在料理台打开排序界面，将图卡放入食谱的三个空缺位置。

**Correct Order**

1. Cream butter and sugar.
    
2. Add eggs and flour.
    
3. Mix into dough.
    
4. Shape the cookies.
    
5. Bake the cookies.
    

### Wrong Order Feedback

如果排序错误：

**GRACE**

Hmm.

That doesn't look right.

Try again?

可以重复排序，无额外惩罚。

若多次失败，可提供更明确的提示，例如提醒玩家先将材料混合，再把面团分成小份。

### Completion Dialogue

玩家排列正确后：

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

食谱顺序恢复完成。

奶奶拿起木勺。

她看了看面前的材料，停顿片刻。

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

这里建议采用三个短小但需要玩家实际操作的阶段。

**Interaction 1 — Mixing**

玩家在料理台进行简单的搅拌操作。

可以使用短暂的方向输入、鼠标交互或按键操作。

奶奶同时参与，不需要复杂的独立 NPC 控制。

**Interaction 2 — Shaping**

玩家将面团分成几份，放到烤盘上。

可以设计为将面团依次放到指定位置，奶奶协助整理。

**Interaction 3 — The Biggest Cookie**

最后一个面团放上烤盘时，奶奶停下来。

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

玩家完成最后一个面团的摆放。

### Completion

**GRANDMA**

There.

That should do it.

短暂淡出。

播放烤箱声。

再次淡入时，桌上出现做好的曲奇。

**GRACE**

They smell good!

**GRANDMA**

They do, don't they?

We made them together.

### Gameplay Notes

协作阶段不要求复杂节奏判定，也不应因为输入失误触发奶奶病情恶化或羞愧等惩罚。

主要目的在于让玩家感受到：奶奶仍然可以参与熟悉的活动，而 Grace 可以通过提示和分工提供帮助。

---

# 7. Event K03 — The First Collapse

**Trigger:** 完成曲奇制作  
**Memory State:** S1 → S2

### Scene Direction

奶奶将曲奇放到蓝色托盘上。

**GRANDMA**

All done.

Let's bring these to your dad.

**GRACE**

Okay!

玩家取得装有曲奇的托盘。

Grace 向厨房出口走去。

突然，房间开始发生明显变化。

- 墙壁的颜色逐渐褪去。
    
- 原本清晰的家具边缘变得模糊。
    
- 墙上的挂钟失去分针。
    
- 通往客厅的门短暂消失，又重新出现。
    

**GRACE**

Grandma?

Something's wrong.

奶奶看向厨房出口。

**GRANDMA**

Where's the living room?

**GRACE**

It's right there.

玩家走向门口。

画面淡出。

再次出现时，Grace 却回到了厨房另一侧。

**GRACE**

Huh?

Wasn't I just here?

玩家再次尝试离开。

这一次，门外出现一条陌生的短走廊。

**Objective Updated:** Find the way to the living room.

---

# 8. Puzzle K2 — The Changing Hallway

**类型：** Spatial Puzzle + Environmental Clue Recognition

## 8.1 Setup

厨房外出现三扇外观相似的门。

玩家可以自由调查。

### Door A — The Clock

调查时传来时钟滴答声。

**GRACE**

I hear a clock.

打开后，Grace 被送回厨房。

再次进入走廊时仍可继续尝试。

### Door B — The Blue Curtains

门缝后可以看到一角蓝色窗帘。

光线略微温暖。

**GRACE**

Something blue...

It looks like curtains.

如果玩家此前调查了家庭照片：

**GRACE**

Like the ones in Dad's picture!

这是正确的客厅入口。

### Door C — The Water

调查时听到轻微的滴水声。

**GRACE**

Is that water?

打开后，进入一个短暂的模糊空间，随后返回走廊。

不需要制作完整的新房间。

## 8.2 Core Puzzle Logic

玩家需要利用厨房照片中的线索，判断真正的客厅应该有蓝色窗帘。

因此 Door B 是正确入口。

如果玩家此前没有调查照片，仍然可以回厨房查看，也可以通过反复调查门口获得提示。

### Correct Door

玩家打开 Door B。

门后出现一个温暖明亮的客厅。

Grace 向前走了一步，停下来。

她回头。

奶奶没有跟过来。

**GRACE**

Grandma?

...

Where is she?

### New Objective

Go back for Grandma.

---

# 9. Event K05 — Don't Leave Grandma Behind

**Location:** Grandma's Kitchen  
**Trigger:** 玩家找到正确门后返回厨房

### Scene Direction

厨房已经变得更加昏暗。

奶奶仍然站在料理台附近。

她望着厨房出口，显得有些不安。

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

玩家需要带奶奶回到走廊，通过 Door B。

### Narrative Purpose

这是本关卡最重要的行动之一。

玩家已经找到了能够离开的路，但没有直接通关，而是主动回去接奶奶。

这一行为与之前共同完成点心形成呼应：玩家不仅是在帮助奶奶完成某项任务，也开始意识到两人需要一起面对陌生的空间。

这里的“带奶奶同行”不需要额外的多步骤治疗小游戏。主要情感来自折返和牵手这两个具体行动。

---

# 10. Event K06 — By the Blue Curtains

**Location:** Living Room  
**Memory State:** S3 — Temporarily Stable

### Scene Direction

Grace 与奶奶终于来到客厅。

光线逐渐恢复温暖。

窗边挂着蓝色窗帘。

客厅的布置与厨房墙上的旧照片十分相似。

奶奶将蓝色托盘放在茶几上。

她拿起最大的一块曲奇，放在靠窗的位置。

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

奶奶缓缓走向玄关。

客厅的灯光开始变暗。

远处传来钥匙插进门锁的声音。

**GRACE**

Grandma?

Wait!

**SCENE END**

**Next Scene:** Act II, Scene 2 — The Coat by the Door

---

# 11. Unity Implementation Specification

本节用于定义游戏逻辑，不强制开发同学采用某种具体代码架构。

可使用 Unity 的 Collider2D、交互组件、对话系统、触发器、动画和场景状态控制实现。

## 11.1 建议的关卡状态

|State|Description|
|---|---|
|`BedroomIntro`|遗物箱转场|
|`KitchenIntro`|奶奶重逢|
|`IngredientPuzzle`|食材谜题|
|`RecipePuzzle`|图卡排序|
|`CooperativeBaking`|合作烘焙|
|`FirstCollapse`|厨房第一次明显崩塌|
|`HallwayPuzzle`|三门走廊|
|`ReturnForGrandma`|返回厨房寻找奶奶|
|`LivingRoomEnding`|客厅结尾|

状态按照流程推进，完成关键事件后进入下一阶段。

## 11.2 建议的事件标记

|Flag|Description|
|---|---|
|`photoExamined`|已调查家庭照片|
|`sugarClueFound`|已获得蓝丝带线索|
|`ingredientsReady`|正确食材已准备|
|`recipeSolved`|食谱排序完成|
|`bakingCompleted`|协作烘焙完成|
|`cookiesCollected`|已取得曲奇托盘|
|`hallwayActivated`|变形走廊已开启|
|`correctDoorFound`|已找到 Door B|
|`grandmaFollowing`|奶奶处于同行状态|
|`kitchenCompleted`|第一关完成|

这些名称是逻辑建议，不要求与现有 Unity 项目中的变量完全一致。

## 11.3 关键交互需求

**Object Interaction**

玩家靠近物品后，出现简单的调查提示。

需要支持：

- 查看物件说明或特写。
    
- 拾取/选择道具。
    
- 将物品用于对应位置。
    
- 播放对白或改变事件状态。
    

**Recipe Puzzle UI**

玩家收集三张图卡后，在一个界面中排列顺序。

可以使用拖放或按钮交换顺序的方式。

应保留玩家重新尝试的功能。

**Cooperative Baking**

可以使用简化的输入序列或交互提示完成。

无需复杂物理模拟。

**Memory House States**

建议使用预设阶段切换，而非实时复杂的程序化场景变形。

- S0：完整明亮的厨房。
    
- S1：轻微褪色，时钟停止。
    
- S2：明显褪色，出口循环。
    
- S3：走廊变化，局部空间重新连通。
    

可以通过切换 Tilemap、SpriteRenderer 状态、灯光、覆盖层与音效表现。

**Hallway Door System**

三个门对应预设的交互结果：

- Door A → 返回厨房。
    
- Door B → 解锁客厅入口。
    
- Door C → 短暂错误空间后返回走廊。
    

**Grandma Following**

找到正确门后，通过对话触发奶奶同行。

可以采用跟随组件，也可以使用有限的预设路径移动。

不要求完整的 NPC 寻路 AI。

## 11.4 防卡关设计

所有必需的谜题线索都应能够重复调查。

玩家选择错误食材、排错食谱或进入错误房门时，不造成永久失败。

若玩家未发现蓝窗帘线索，应允许返回厨房调查照片。

如果玩家在走廊多次尝试失败，可以逐步增加 Grace 的提示：

**GRACE**

The living room...

I've seen it before.

之后：

**GRACE**

Dad was sitting by the blue curtains.

在玩家找到正确房门但未接奶奶时，游戏应通过 Grace 的对白明确提示其下一步行动。

---

# 12. Narrative and Educational Notes

## 12.1 第一幕的伏笔如何回应

第一幕中，Grace 不理解为什么爸爸曾经对奶奶发火，现在却如此悲伤。

厨房场景不直接给出答案，而是呈现一项新的事实：爸爸也曾经通过便签、颜色标记和步骤卡，帮助奶奶维持熟悉的日常活动。

这为第二场关于照护压力的剧情提供基础。

## 12.2 奶奶的人物形象

奶奶不应该只是不断遗忘、不断需要帮助的角色。

在本场景中，她还会：

- 记得爸爸小时候偷吃糖的事情。
    
- 和 Grace 一起制作曲奇。
    
- 开玩笑，照顾 Grace。
    
- 习惯性地为儿子留下最大的一块曲奇。
    

她首先是一个拥有生活、性格和家庭关系的人，其次才是疾病患者。

## 12.3 照护方式的表现

通过视觉提示、步骤分解与合作，展示家庭成员可以怎样支持有认知困难的人参与熟悉活动。

这些做法不代表一定适用于每位患者，也不意味着完成任务便能够恢复认知能力。

## 12.4 世界崩塌的象征意义

记忆之屋的崩塌是故事的艺术表现，而非阿尔茨海默病患者的准确主观体验或医学进程模拟。

玩家可以暂时找到前进的道路，但无法完全阻止空间逐渐消散。

---

# 13. Production Priorities

**Priority A — 必须实现**

- 卧室遗物箱至厨房转场。
    
- 奶奶重逢与基础对话。
    
- 食材调查和选择。
    
- 食谱图卡排序。
    
- 简化的协作烘焙。
    
- 厨房状态切换。
    
- 三门走廊谜题。
    
- 返回接奶奶。
    
- 客厅结尾。
    

**Priority B — 建议保留**

- 家庭照片的可选对话。
    
- 爸爸的蓝丝带便签。
    
- 最大块曲奇的视觉细节。
    
- 八音盒的可选互动。
    
- 错误选择后的渐进式提示。
    

**Priority C — 时间允许再加入**

- 更丰富的角色动画。
    
- 拼图小游戏。
    
- 额外环境互动。
    
- 精细的空间褪色特效。
    
- 复杂的双人协作输入。
    
- 完整的空间稳定度 UI。
    

---

# 14. Current Open Questions

1. Unity 现有项目是否已经实现食材拾取、背包及物品使用？如果没有，需要判断采用简化交互还是加入轻量物品栏。
    
2. 食谱排序采用拖放、按钮交换还是对话选项？
    
3. 双人烘焙如何表现，才能既有操作感，又不成为独立的复杂小游戏？
    
4. 三门走廊在现有场景中如何布置，能否避免大量新增地图与素材？
    
5. 奶奶同行采用真实跟随还是预设移动路线？
    
6. 旧照片和八音盒是否与后续完整剧本统一使用？
    
7. 第一关的实际游玩时长及难度是否符合目标，需要通过 Unity 原型试玩验证。
    

**版本结论：** v0.4 保留原有厨房谜题的核心结构，吸收新版大纲中与人物关系和伏笔相关的设计，并控制第一关的演出和交互范围。后续关卡将继续围绕第一幕留下的家庭关系问题展开。