[English](GAME_REFERENCE.md) · **简体中文**

> 本文记录原 RPG Maker MV 的地图与事件。当前 Unity 工程与实现入口见[文档目录](README.md)。

# 第一幕工程参考

RPG Maker MV 1.6.1，816 × 624。以下以 `game/data/` 和已启用的 `RM_Act1_Storybook` 为准。

## 流程

1. 从新游戏开始，沿碎石路调查奶奶的墓碑；靠近爸爸并与他交谈。
2. 看完冷调室内闪回，回墓园找妈妈领取八音盒。
3. 调查长椅旁的八音盒，选择 **Turn the key**；选择关上可留在墓园、稍后重试。
4. 暖色记忆屋可自由走动。与奶奶交谈后第一幕完成，但可以继续调查。

## 地图与事件

### Map001 — `Act1_Cemetery`

24 × 18 · `!RM_CemeteryOvercast`

| ID | 事件 | 坐标 |
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

| ID | 事件 | 坐标 |
|---|---|---|
| 01 | `FlashbackController` | (0, 0) |
| 02 | `Grandma` | (12, 6) |
| 03 | `FlashbackDad` | (11, 7) |

### Map003 — `Act1_Memory_Home`

24 × 18 · `!RM_HomeStorybook`

| ID | 事件 | 坐标 |
|---|---|---|
| 01 | `Grandma` | (12, 8) |
| 02 | `FamilyPhotograph` | (5, 7) |
| 03 | `GrandmasBlanket` | (5, 10) |
| 04 | `Cabinet` | (19, 7) |
| 05 | `Door` | (12, 4) |
| 06 | `Arrival` | (0, 0) |
| 07 | `Chair` | (18, 11) |
| 08 | `TeaCup` | (19, 11) |

## 开关与物品

| ID | 开关 |
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

关键物品：

- 5 — Music box
- 6 — Faded ribbon
- 7 — Family photograph

## 维护提示

- `Act1_TransparentCollision` 的碰撞格与底图分开；调整家具、树干或墓碑底座时，同时检查可达格和事件坐标。
- 调查事件用 `<RMInspect: label>`；蓝丝带还带 `<RMCrouch>`，要求先蹲下。
- 墓园雨使用 MV 天气事件，底图没有雨丝。闪回保持固定取景，探索地图使用跟随镜头。
- `0001` 变量仍统计旧墓调查；蓝丝带与家庭照片都不是主线门槛。
