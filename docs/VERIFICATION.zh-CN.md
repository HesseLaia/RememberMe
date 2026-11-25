[English](VERIFICATION.md) · **简体中文**

# 历史 MV 验证记录

本文记录 RPG Maker MV 验证。当前 Unity v10 的验证证据见[接力说明](HANDOFF.zh-CN.md)。

## MV 版本 · 2026-11-19

- 两套 Node 检查通过：MV 图片尺寸、地图连通性、事件接线、关键物品及 Ogg／M4A 文件格式。
- 使用本地 HTTP 和 Chrome 运行 MV。测试了跑蹲、动态雨、拾取丝带与物品栏，以及从新游戏进入闪回、八音盒关闭后重开、与奶奶重逢、暂停和恢复。
- 覆盖范围：移动端、旧存档和音频听感待人工测试。以下初次发布记录和截图对应此前的 MV 版本。

## 历史记录 · 初次发布


本仓库首次发布前，`game/` 中的构建在浏览器中从 **New Game** 运行至第一幕结束。以下保留该次运行的方法、观察结果与覆盖范围。

截图在 [`screenshots/`](screenshots/) 中。

---

## 方法

- 通过 HTTP 提供 `game/`（`python3 -m http.server`），并在 Chrome 中加载 `index.html`。
- 通过 DevTools 协议驱动浏览器，调用游戏输入层（`Input._onKeyDown` / `Input._onKeyUp`）。MV 每帧轮询一次输入，因此按键保持足够时间以确保登记。
- 在按键之间读取引擎的实时状态：当前场景、`$gameMap._mapId`、玩家位置、八个开关、`$gameMessage.isBusy()`、`$gameMap.isEventRunning()`。
- 游戏在测试浏览器中稳定运行在 60 fps。

---

## 结果：这一章能走完

| 步骤 | 预期 | 实际观察 |
|---|---|---|
| 启动 | 标题画面显示 `GAME START / CONTINUE / SETTINGS` | ✓ |
| 开始新游戏 | `Scene_Map`，Map 1，玩家位于 (12,12)，开场消息正在运行 | ✓ 按 1 次后 |
| 播放冷开场 | 结束时开关 1（`OpeningComplete`）为 ON，玩家可以自由移动 | ✓ 按 12 次后 |
| 进入闪回 | Map 2，玩家位于 (12,13)，闪回的自动执行事件正在播放 | ✓ |
| 播放闪回 | 结束时开关 3 为 OFF、开关 4（`FlashbackComplete`）为 ON，玩家回到 Map 1 的 (13,7) | ✓ 按 14 次后 |
| 播放结局 | 结束时开关 5（`Act1Complete`）为 ON | ✓ 按 24 次后 |
| 结局之后 | 回到标题画面（指令 354 = *Return to Title Screen*） | ✓ |

本次运行完成，JavaScript 错误与画面卡住均为零。`data/*.json` 引用的全部 229 个图片与音频文件都存在于磁盘，运行期间的浏览器请求均返回 HTTP 200。最终状态为：

```
{"scene":"Scene_Title","map":1,"x":13,"y":7,"sw":"11111000","busy":false,"run":true}
```

`sw:"11111000"` 是开关 1–8，也就是 1 到 5 为 ON、6–8 为 OFF — 正是结局本该留下的状态。

---

## 截图显示了什么

| 文件 | 说明 |
|---|---|
| `00_title_screen.png` | 带指令窗口的标题美术。`System.json` 中的 `optDrawTitle` 为 `false`，所以美术是由开场事件绘制，而不是由引擎绘制。 |
| `01_opening_over_black.png` | Grace 的第一句台词，逐字显示到一半。开场事件在前两句台词之前淡出至黑，之后淡入。 |
| `02_cemetery_free_roam.png` | 玩家获得自由行动后的墓地，正下着雨。Grace、Mom、Dad、Uncle James、Aunt 和 Grandpa 都带着各自的行走图出现在画面中。 |
| `03_flashback.png` | 闪回的室内场景，带有 Grandma 的头像和英文对话。 |
| `04_back_in_cemetery.png` | 闪回结束后回到墓地。 |
| `05_back_at_title_after_the_ending.png` | 结局返回之后，再次出现的标题画面。 |

---

## 过程中做过的检查

- **自定义图片的几何尺寸** — 所有 `RM_*` 素材都符合引擎的网格规则：`$RM_*` 行走图为 144 × 192（3 × 4，每格 48 × 48）。脸图为 576 × 288（4 × 2，每格 144 × 144）。标题为 816 × 624。两张视差图为 1152 × 864。碰撞图块组基于 48 px 网格。`Window.png` 为 192 × 192。
- **素材覆盖** — `data/*.json` 引用的图片和音频文件均存在于磁盘，引用的每首音频均提供 `.ogg` 和 `.m4a` 两种格式。
- **明文加载** — `System.json` 省略了 `hasEncryptedImages` / `hasEncryptedAudio` 键，明文工程文件可直接从静态主机加载。
- **碰撞层行为正常** — `Tilesets.json` 中图块 1536 的标志位是 `1536`（低半字节为零 → 可通行），图块 1537 的标志位是 `1551`（低半字节 `0x0F` → 四个方向都不可通行），这与图块组自身的备注一致：*“A5 tile 1 = walkable, tile 2 = blocked.”*
- **事件指令内部自洽** — 对两张地图中的开关与变量读写做了追踪。每个在某处被判断的开关都在某处被赋值，八个开关也都有名称。

---

## 覆盖范围与观察到的问题

1. **编辑器：**本次覆盖运行时。MV 1.6.1 打开与重新保存、插件参数编辑和图块组指定待验证。
2. **步行路线：**测试直接设置 `VisitedGrave`，以 Dad 事件相同的状态转换进入闪回。走向 Dad、墓碑交互、花与旧墓调查、Mom 的呼喊和四名配角的交互均未通过输入覆盖。
3. **菜单：**开场菜单锁定和之后的菜单访问待验证。本次运行未输入 `Esc`。
4. **原版观察结果：**闪回播放了两次画面震动和两个 `RM_KnockHard` 音效。系统区域设置为 `zh_CN`，系统文本为中文。这些观察对应初次发布的构建。
5. **平台：**已测桌面 Chrome。Safari、Firefox 及 Safari 的 `.m4a` 音频播放待验证。
6. **人工评审：**本次为自动化运行，本地人工试玩和音频验证待完成。

---

## 如何重跑

```bash
cd game
python3 -m http.server 8080
```

打开[本地游戏](http://127.0.0.1:8080/)，从 New Game 开始。后续 MV 流程见[第一幕工程参考](GAME_REFERENCE.zh-CN.md#流程)。以上首次发布结果保留其历史范围。
