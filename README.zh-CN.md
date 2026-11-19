[English](README.md) · **简体中文**

# Remember Me — Act I

**CSS5190 Game Your Psychology: AI, Creativity & Mental Health**（香港中文大学（深圳），2026 秋季）期末项目的回合制叙事游戏，RPG Maker MV 1.6.1 制作。

这个仓库同时是**可玩的游戏**和**小组的工作台**：素材规范、剧本规范、文件清单、最新工程，都在这里。

---

## 你该看哪个文件

| 你是 | 看这个 | 里面有什么 |
|---|---|---|
| **美工** | [`art/README.md`](art/README.zh-CN.md) | 每类素材放在哪个文件夹、文件尺寸和格子怎么算、命名规则、现有素材清单、交付流程、投稿区 |
| **剧本** | [`script/README.md`](script/README.zh-CN.md) | 一句台词在游戏里怎么写、每行多少字、颜色码和表情索引、角色语气、可直接抄的模板、草稿放哪 |
| 想知道某个文件是干嘛的 | [`docs/FILE_GUIDE.md`](docs/FILE_GUIDE.zh-CN.md) | 仓库里每个文件夹、每个关键文件的用途，以及"能不能改" |
| 想知道某句话、某个开关在哪 | [`docs/GAME_REFERENCE.md`](docs/GAME_REFERENCE.zh-CN.md) | 三张地图、开关、关键物品和通关流程 |
| 想读完整的剧本 | [`script/ACT1_SCRIPT.md`](script/ACT1_SCRIPT.md) | 第一幕全部游戏内文本，按出现顺序 |
| 要往仓库里加文件 | [`docs/DEPLOY.md`](docs/DEPLOY.zh-CN.md) | 怎么放、怎么提交、怎么发布上线 |
| 想看本版素材清单与试玩步骤 | [第一幕美术与试玩](docs/ACT1_PLAYTEST.zh-CN.md) | 新场景、动作、输入和音频规格 |
| 想直接玩 | <https://jeddakholmes-byte.github.io/RememberGame/> | 浏览器里直接玩，不用装任何东西 |

---

## 玩 / 打开

| | |
|---|---|
| **在线玩** | <https://jeddakholmes-byte.github.io/RememberGame/> |
| **游戏直链** | <https://jeddakholmes-byte.github.io/RememberGame/game/> |
| **用 MV 打开改** | 用 RPG Maker MV 1.6.1 打开 `game/Game.rpgproject` |
| **在自己电脑上跑** | `cd game && python3 -m http.server 8080`，然后开 <http://127.0.0.1:8080/>。**双击 `index.html` 打不开**，必须走 HTTP |

操作：**WASD／方向键**移动，**Shift** 跑，**C** 蹲，**E／Enter／空格**调查，**I** 打开物品栏，**Esc** 暂停。画面 816 × 624。

---

## 仓库里有什么

```
RememberGame/
├── README.md              ← 英文版（默认）
├── README.zh-CN.md        ← 你正在看的这一份
├── index.html             ← 落地页；默认英文，右上角可切中文
├── art/                   ← 美工
│   ├── README.md              素材规范、尺寸、命名、清单、交付流程
│   ├── specs/                 自动生成的对照图（每个角色的 8 个表情、行走图格子）
│   └── incoming/              新素材先放这里，别直接扔进 game/
├── script/                ← 剧本
│   ├── README.md              写作规范、格式、语气、模板
│   ├── ACT1_SCRIPT.md         第一幕全文（从工程导出，只有英文）
│   └── drafts/                新剧本草稿
├── docs/                  ← 技术与运维
│   ├── FILE_GUIDE.md          每个文件是干嘛的
│   ├── GAME_REFERENCE.md      地图 / 事件 / 开关 / 流程
│   ├── DEPLOY.md              怎么加文件、怎么发布
│   ├── VERIFICATION.md        上线前的浏览器实跑记录
│   └── screenshots/           游戏截图
└── game/                  ← 游戏本体，RPG Maker MV 1.6.1 工程
```

**两种语言。** 除了 `script/ACT1_SCRIPT.md`，上面每份文档都有中文版：把路径加上 `.zh-CN.md` 就是——`README.zh-CN.md`、`art/README.zh-CN.md`，以此类推。每份文件第一行都有切换另一种语言的链接。`ACT1_SCRIPT.md` 没有中文版，因为它就是游戏里真正播的英文台词。

---

**`game/` 是当前完整工程。** 它同时是两样东西：

- 用 RPG Maker MV 打开的工程，和
- GitHub Pages 正在托管的那份网页版。

这不是巧合。RPG Maker MV 的游戏本身就是 HTML5 + JavaScript，没有"转换"这一步——所以网页版是原版 1:1 的拷贝，而不是重写。

---

## 游戏是什么

七岁的 Grace 在奶奶的葬礼上沿碎石路来到墓碑前，和爸爸交谈后看见一段闪回。回到墓园，她找妈妈拿到八音盒，在长椅旁亲手转动发条，走进记忆里的家。第一幕收在她与奶奶重逢；对话结束后还可以自由调查。

| 地图 | 内容 |
|---|---|
| `Act1_Cemetery`（Map001） | 阴雨葬礼、五座可辨认的墓碑和可选调查 |
| `Act1_House_Flashback`（Map002） | 冷灰窗光下的剧情闪回 |
| `Act1_Memory_Home`（Map003） | 暖光记忆屋、家庭照片和重逢 |

## 当前版本

- 主菜单保留原版封面；关键剧情使用全屏插画。
- 探索地图有跟随镜头、当前目标和近距离调查提示。
- 八音盒、蓝丝带、家庭照片都是 MV 原生关键物品；第一幕物品栏供查看，留给后续解谜调用。
- 新作四段共享动机的音乐及环境、脚步和物件音效，提供 Ogg、M4A 两种格式。
- 本版请从 **New Game** 开始，旧存档会携带原版的开关与事件状态。参见[验证记录](docs/VERIFICATION.zh-CN.md)。

---

## 加文件之前

- **新素材先进 `art/incoming/`**，新剧本先进 `script/drafts/`。`game/` 里永远是能跑的干净版本，别直接往里扔半成品。
- **`game/img/` 和 `game/audio/` 里已有文件不要改名、不要覆盖。** 游戏是按文件名的字符串去找图的，改名不会报错，只会静默地不显示。
- **加完东西一定要跑一遍，并且从 New Game 开始。** 读旧存档会把旧的开关状态一起载进来，让正确的改动看起来是坏的。
- 提交和发布的具体步骤见 [`docs/DEPLOY.md`](docs/DEPLOY.zh-CN.md)。

---

## 版权与素材来源

用 **RPG Maker MV 1.6.1**（Kadokawa / Degica）制作。`game/img/` 和 `game/audio/` 里大部分文件是 RPG Maker MV 自带的 Run Time Package 素材，随工程一起分发是 RTP 本身的用途。

**本项目制作的素材使用 `RM_` 命名**，包括场景、人物、插画、纸色皮肤及音频。
