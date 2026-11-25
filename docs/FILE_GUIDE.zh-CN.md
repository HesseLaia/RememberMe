[English](FILE_GUIDE.md) · **简体中文**

> 本文保留原 RPG Maker MV 首次发布时的目录、数量与插件状态。后续 MV 地图与事件见[第一幕工程参考](GAME_REFERENCE.zh-CN.md)。当前 Unity 源码、代码入口和发布目录见[文档目录](README.zh-CN.md)。

# 文件指南

原 MV 工程的目录、文件职责与编辑流程。

**可编辑？** 一列的图例：

- **否** — 引擎或生成文件，通过所属工具或配置维护。
- **谨慎** — 共享工程状态。改动后加载游戏验证。
- **是** — 协作者维护的项目内容。
- **新增** — 游戏之外的仓库支持内容。

---

## 1. 仓库根目录

| 路径 | 说明 | 可编辑？ |
|---|---|---|
| `README.md` | 首页：游戏是什么、谁该读哪份文档、如何游玩、当前构建状态、致谢。 | 是 |
| `index.html` | 原落地页，一个链接到 `game/` 的 “Play Act I” 入口。 | 是 |
| `.nojekyll` | 空标记文件。让 GitHub Pages 原样提供该文件夹，而不执行 Jekyll 构建。删除它会让 Pages 跳过以 `_` 开头的文件夹，可能弄坏站点。 | 否 |
| `.gitignore` | 把 macOS 垃圾文件（`.DS_Store`、`._*`）挡在仓库之外。 | 是 |
| `art/` | 美术所需的一切：素材规范、自动生成的规格表，以及新美术资源的收件箱。见 §7。 | 是 |
| `script/` | 编剧所需的一切：写作指南、导出的第一幕文本，以及草稿区。见 §7。 | 是 |
| `docs/` | 技术与操作参考。见 §7。 | 是 |
| `game/` | 游戏本体。一个完整的 RPG Maker MV 1.6.1 工程。见 §2–§6。 | — |

---

## 2. `game/` — 工程根目录

| 路径 | 说明 | 可编辑？ |
|---|---|---|
| `Game.rpgproject` | 只有一行：`RPGMV 1.6.1`。这是你在 RPG Maker MV 编辑器中打开的文件。双击它即可启动 MV。 | 否 |
| `index.html` | 网页入口。先加载六个库，再加载六个引擎脚本，然后是 `js/plugins.js` 和 `js/main.js`。这就是 GitHub Pages 提供的文件。 | 谨慎 |
| `package.json` | 桌面封装（NW.js）的元数据：窗口标题、窗口尺寸 816 × 624、图标。不影响浏览器构建。 | 谨慎 |
| `data/` | 游戏本体 — 地图、事件、对话、开关、系统设置。见 §3。 | 谨慎 |
| `js/` | 引擎。见 §4。 | 谨慎 |
| `img/` | 全部图片。见 §5。 | 谨慎 |
| `audio/` | 全部音乐与音效。见 §6。 | 谨慎 |
| `fonts/` | `gamefont.css` 加上 `mplus-1m-regular.ttf`。该 CSS 声明了引擎所要求的字体族。 | 谨慎 |
| `icon/icon.png` | 128 × 128 的应用图标，用作 favicon，也供桌面封装使用。 | 否 |
| `movies/` | 为 RPG Maker MV 工程结构保留的空文件夹。 | 否 |
| `save/` | `config.rpgsave` — 本地试玩保存的音量与冲刺设置。浏览器设置使用 localStorage。 | 否 |

---

## 3. `game/data/` — 游戏内容

这 16 个 JSON 文件保存首次发布时的地图、事件、对话和数据库，由 MV 编辑器读写。外部编辑时先关闭编辑器，在副本中修改，并在加载前验证 JSON 格式。

| 文件 | 大小 | 内容 | 备注 |
|---|---|---|---|
| `MapInfos.json` | <1 KB | 地图列表：两个条目，`Act1_Cemetery`（顺序 1）和 `Act1_House_Flashback`（顺序 2）。 | 决定编辑器地图树中的顺序。 |
| `Map001.json` | 51 KB | **Act1_Cemetery。**24 × 18 图块，视差图 `!RM_Cemetery`，20 个事件。整个墓地章节。 | 项目中改动最频繁的文件。 |
| `Map002.json` | 14 KB | **Act1_House_Flashback。**24 × 18 图块，视差图 `!RM_House`，3 个事件。闪回与结局。 | 闪回是一个很长的自动执行事件。 |
| `System.json` | 6 KB | 全局设置：游戏标题（`RememberMe`）、8 个开关名称、队伍成员、每个菜单中的 UI 文本、标题/菜单操作音效、`optDrawTitle: false`、`titleBgm`。 | 语言、音量、开关名称与术语的共享配置，改动后逐项验证。 |
| `Actors.json` | <1 KB | 一个角色：`Grace`，职业 1，行走图 `$RM_Grace`，脸图 `RM_Face_Grace` 第 3 帧。 | |
| `Classes.json` | 14 KB | Grace 的职业及其等级曲线。未改动的默认数据。 | RPG Maker 会填充它。游戏从不升级。 |
| `CommonEvents.json` | <1 KB | 四个空的公共事件。游戏中没有任何地方调用它们。 | 可以忽略。 |
| `Tilesets.json` | 17 KB | 一个图块组 `Act1_TransparentCollision`，由 `RM_Collision_A5` 和 `RM_Empty_B` 构成。 | 它的备注栏写着：*“A5 tile 1 = walkable, tile 2 = blocked.”* 美术是视差图，所以这个图块组只用于绘制不可见的碰撞层。 |
| `Animations.json` | 382 KB | 117 个自带的战斗动画。 | 一个都没用上 — 游戏没有战斗。保留是因为 MV 自带它们。 |
| `Enemies.json` | 2 KB | 四个自带敌人（蝙蝠、史莱姆、兽人、牛头人）。 | 未使用。 |
| `Troops.json` | 1.5 KB | 四个自带敌群。 | 未使用。 |
| `Items.json` / `Weapons.json` / `Armors.json` | 小 | 自带的道具与装备列表。 | 未使用。 |
| `Skills.json` / `States.json` | 4–5 KB | 自带的技能与状态。 | 未使用。 |

首次发布的自定义内容位于 `Map001.json`、`Map002.json`、`MapInfos.json`、`Actors.json`、`Tilesets.json` 和 `System.json`。其余默认 MV 数据库用于支持工程加载。

---

## 4. `game/js/` — 引擎

### 4.1 核心脚本（`game/js/`）

| 文件 | 说明 | 可编辑？ |
|---|---|---|
| `rpg_core.js` | 图形、输入、音频、位图和场景管理器。 | 否 |
| `rpg_managers.js` | 数据、配置、存储、声音和战斗管理器。 | 否 |
| `rpg_objects.js` | 游戏对象：队伍、地图、玩家、事件、事件解释器。你想改的引擎行为大多在这里。 | 否 |
| `rpg_scenes.js` | 场景类：标题、地图、菜单、消息、游戏结束。 | 否 |
| `rpg_sprites.js` | 精灵类。 | 否 |
| `rpg_windows.js` | 窗口类：消息框、菜单、存档画面。 | 否 |
| `js/plugins.js` | 插件总控台。一个 `{name, status, parameters}` 列表。**插件的启用与关闭在这里，参数修改也在这里。** | 谨慎 — 但这正是该动手的地方 |
| `js/main.js` | 引导一切启动，并设置画布缩放。 | 否 |

### 4.2 `game/js/libs/` — 第三方库

`pixi.js`（4.5.4 WebGL 渲染器）、`pixi-tilemap.js`、`pixi-picture.js`、`fpsmeter.js`、`lz-string.js`（存档压缩）、`iphone-inline-video.browser.js`。工程保留这些第三方库，由 `index.html` 按上述顺序加载。

### 4.3 `game/js/plugins/` — 插件

十二个文件。只有标记为 **ON** 的三个处于启用状态。其余闲置在文件夹里没人用。

| 插件 | 状态 | 用途 | 自定义？ |
|---|---|---|---|
| `Community_Basic.js` | **ON** | 官方插件。设置 `screenWidth: 816`、`screenHeight: 624`、缓存上限 20、`alwaysDash: off`。通过这些参数配置分辨率。 | 否 |
| `MadeWithMv.js` | **ON** | 在标题之前显示 “Made with MV” 启动画面，淡入淡出各 120 帧，停留 160 帧。 | 否 |
| `RM_EnglishTypography.js` | **ON** | **我们自己写的。**强制消息窗口使用英文字体，不受工程区域设置影响：`Arial, Helvetica, sans-serif`，字号 26，描边 2。写它是因为 `locale: zh_CN` 的工程否则会选用 CJK 字体，把拉丁文字渲染得很难看。 | **是** |
| `RM_Act1_Experience.js` | **off** | **我们自己写的。**打磨插件：由事件备注 `<RMHint:>` 驱动的屏幕提示 `Enter / Space: …`、行走时安静脚步 SE，以及精简菜单（`Resume / Save / Options / Game End`）。它已经写好并放着，但**没有在 `plugins.js` 中启用**，地图上也还没有 `<RMHint:>` 备注，所以单独启用它，除了菜单和脚步声之外没有任何可见效果。安装它是待办工作的一部分。 | **是** |
| `RM_Act1_Polish.js` | **off** | **我们自己写的，已被取代。**更早的标题 BGM 兜底与脚步 SE，由 `RM_Act1_Experience.js` 接替。两者最多启用一个，使脚步声保持单一来源。 | **是** |
| `TitleCommandPosition.js` | off | 移动标题指令窗口。 | 否 |
| `AltMenuScreen.js` | off | 替代版菜单布局。 | 否 |
| `AltSaveScreen.js` | off | 替代版存档/读档布局。 | 否 |
| `ItemBook.js` | off | 道具图鉴画面。 | 否 |
| `EnemyBook.js` | off | 敌人图鉴画面。 | 否 |
| `WeaponSkill.js` | off | 为每种武器指定攻击技能。 | 否 |
| `SimpleMsgSideView.js` | off | 横版战斗中显示简短技能名。 | 否 |

> 插件顺序很重要。`Community_Basic` 必须排在最前面，这样屏幕尺寸会在任何东西测量它之前设置好。任何新插件都自上而下加载。

---

## 5. `game/img/` — 图片

RPG Maker MV 按固定网格读取普通 PNG。使用下列尺寸，使图像帧正确切分。

| 文件夹 | 数量 | 说明 | 自定义文件 | 网格规则 |
|---|---|---|---|---|
| `titles1/` | 21 | 标题画面美术。只用到 `RM_Title.png`（816 × 624）。注意 `System.json` 中的 `optDrawTitle` 为 `false`，所以标题画面是黑的，美术改由开场事件绘制。 | `RM_Title.png` | 816 × 624 |
| `titles2/` | 2 | 标题前景叠加图。未使用。 | — | 816 × 624 |
| `parallaxes/` | 17 | 那两张*就是*地图的背景图。 | `!RM_Cemetery.png`、`!RM_House.png` | 1152 × 864，自由尺寸。`!` 前缀表示“不平铺” |
| `pictures/` | 2 | 由显示图片指令显示的全屏图片。 | `RM_Gravestone.png`（墓碑特写）、`RM_Box.png`（遗物盒） | 816 × 624 |
| `characters/` | 34 | 行走图。 | `$RM_Grace`、`$RM_Aunt`、`$RM_Grandpa`、`$RM_UncleJames`、`RM_Family` | 两种布局。以 `$` 开头的文件装**一个**角色：3 列 × 4 行、每格 48 × 48，即 144 × 192。不带 `$` 的文件装**八个**角色：横向四个角色块、纵向两个，每块 144 × 192，即 576 × 384 — `RM_Family.png` 就是这种，事件的角色索引 0–7 选择对应块。**丢掉 `$` 会让 MV 把单个行走图读成八个，并渲染出错误块的某个角。** |
| `faces/` | 17 | 显示在消息框左侧的头像图。 | `RM_Face_Grace`、`_Mom`、`_Dad`、`_Grandma`、`_Grandpa`、`_Aunt`、`_UncleJames` | 576 × 288 = 4 列 × 2 行、每格 144 × 144，所以每张图 8 个头像，**索引 0–7**。`RM_Face_Grace` 这张图是 576×288，但在消息框内以 144×144 显示。 |
| `system/` | 14 | 界面皮肤：`Window.png`（九宫格的消息/菜单窗口）、`IconSet.png`、`Balloon.png`、`ButtonSet.png`、`Loading.png`、`MadeWithMv.png`、`GameOver.png`、`Shadow1/2.png`、`Damage.png`、`States.png`、`Weapons1–3.png`。 | 无 | 其中几个有严格尺寸。`Window.png` 是 192 × 192 |
| `tilesets/` | 64 | 图块图集。 | `RM_Collision_A5.png`（384 × 768，装着可走/阻挡标记）、`RM_Empty_B.png`（768 × 768，刻意留空） | 每图块 48 × 48。B–E 图集为 768 × 768 = 16 × 16 |
| `animations/` | 117 | 战斗动画图集。 | 无 | 未使用 — 没有战斗。占仓库 52 MB。 |
| `battlebacks1/`、`battlebacks2/` | 各 50 | 战斗背景。 | 无 | 未使用 — 98 MB。 |
| `enemies/`、`sv_enemies/`、`sv_actors/` | 70 / 70 / 20 | 敌人与战斗角色美术。 | 无 | 未使用。 |

`img/tilesets/*.txt` 中保留的 31 个小文本文件是 RTP 附带的 VX/MV 图块备注。

---

## 6. `game/audio/` — 音乐与音效

每首曲子都有两种格式：`.ogg`（桌面端和 Firefox/Chrome 使用）和 `.m4a`（Safari 和 iOS 需要）。**两者都必须保留。**文件夹保持扁平 — RPG Maker 按纯文件名查找音频，没有子文件夹。

| 文件夹 | 数量 | 说明 | 自定义文件 |
|---|---|---|---|
| `bgm/` | 44（22 首 × 2 种格式） | 背景音乐。 | `RM_Remember`（标题）、`RM_Act1_Cemetery`、`RM_Flashback`、`RM_Box` |
| `bgs/` | 24 | 背景环境音。 | `RM_Rain`（墓地）、`RM_Room`（闪回中的房子） |
| `me/` | 36 | 短音乐效果。 | 无 — 自带 |
| `se/` | 468 | 音效。 | 16 个原创：`RM_Confirm`、`RM_Chime`、`RM_Swell`、`RM_WindGust`、`RM_CrowCaw`、`RM_Step1–3`、`RM_Cloth`、`RM_DoorHandle`、`RM_DoorCreak`、`RM_Cardboard`、`RM_KnockHard`、`RM_DoorPalm`、`RM_Heartbeat` |

`RM_Rain` 与 `RM_Room` 放在 **BGS** 中作为循环环境音，四首配乐放在 `bgm/`。事件按这些目录查找音频。

`RM_Heartbeat` 和 `RM_KnockHard` 只被闪回使用。它们是计划中的打磨环节要移除的两个声音。

---

## 7. `art/`、`script/` 与 `docs/`

### `art/` — 给美术

| 文件 | 说明 |
|---|---|
| `README.md` / `README.zh-CN.md` | 英文与中文素材规范：位置、精确像素尺寸与网格、命名、素材清单、交付流程与检查清单。 |
| `specs/grid_rules.png` | 两种网格规则的示意图。 |
| `specs/faces_<character>.png` | 每个脸图组一张真实对照表，8 个格子并标出各自索引。绿色边框 = 剧本当前使用的表情。灰色 = 已画但未使用。美术若有变化就重新生成。 |
| `specs/sprites_<character>.png` | 每张行走图一张带标注的对照表：3 × 4 个格子，各行依次标注下/左/右/上，中间一列标为站立帧。 |
| `incoming/` | 新美术资源接入 `game/` 前的评审收件箱。README 说明交付命名。 |

### `script/` — 给编剧

| 文件 | 说明 |
|---|---|
| `README.md` / `README.zh-CN.md` | 英文与中文写作指南：台词结构、消息框 4 行上限、每行 40 字符上限、颜色代码、脸图索引、角色口吻、模板与检查清单。 |
| `ACT1_SCRIPT.md` | 第一幕完整的游戏内文本，从地图数据导出。只读参考 — 编辑它不会改变游戏。 |
| `drafts/` | 新剧本草稿，每个文件一个场景。它的 README 说明如何对已有台词提出修改。 |

### `docs/` — 技术与操作

| 文件 | 说明 |
|---|---|
| `FILE_GUIDE.md` | 本文档。 |
| `GAME_REFERENCE.md` | 地图、每个事件的触发方式与用途、开关表、通关流程，以及每句对话所在的位置。改动任何事件之前该读的文件。 |
| `DEPLOY.md` | 如何添加文件、提交与发布。 |
| `VERIFICATION.md` | 历史浏览器通关结果、方法与覆盖范围。 |
| `screenshots/` | 那次通关验证中截取的画面，用于落地页和查阅参考。 |

---

## 8. 维护检查

| 改动 | 处理方式 |
|---|---|
| 重命名 `img/` 或 `audio/` 下的文件 | 每一处引用都以纯字符串存在于 `data/*.json` 中。改了文件名，游戏就会悄无声息地什么都不显示 / 什么都不播放。 |
| 去掉行走图里的 `$` | `$` 表示“这张图只装一个角色”。没有它，MV 会把文件当成八角色图来读，并渲染出错误块的某个角。 |
| 去掉视差图里的 `!` | `!` 表示“不平铺”。没有它，背景会重复平铺。 |
| 已有存档时修改地图 | 从 **New Game** 测试，使用新的开关状态。旧存档会恢复其保存的状态。 |
| 在 `game/` 里放一个 `README.md` | 对 GitHub 没问题，但它会出现在 MV 编辑器的工程文件夹里。把仓库文档放在 `game/` 之外。 |
| MV 编辑器开着时在 `data/*.json` 里手动改地图 | 编辑器把地图保存在内存中，保存时会覆写你的修改。通过编辑器改，或者先关掉它。 |
| 移动或重命名 `game/` | 它是 GitHub Pages 的入口。已发布的 URL 会随之改变。 |
| 删除 `movies/` 或 `save/` | MV 要求两者都存在。 |
| 裁剪未使用的 RTP 素材 | 原工程约 400 MB 的内容中，大量是 MV 编辑器素材列表提供的战斗美术与环境音。保留共享源素材库，在单独的分发副本中裁剪。 |
