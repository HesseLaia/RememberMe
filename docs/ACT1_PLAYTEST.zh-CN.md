# Remember Me · 第一幕美术与试玩

**范围**：保留葬礼和原有室内闪回。回到墓园后，玩家找 Mom 拿遗物，亲手打开并转动八音盒，走进记忆中的家，与奶奶重逢。合照和其他调查是可选内容；不加倒计时、追逐或失败惩罚。

## 实装与检查

- 在 RPG Maker MV 打开 `game/Game.rpgproject`，按 F12，从原封面的 **New Game** 开始。WASD／方向键移动，Shift 跑，C 切换蹲行，E／Enter／Space 调查，I 打开物品栏，Esc 暂停。
- 主线：沿碎石路找白花墓碑 → 和 Dad 说话 → 看原室内闪回 → 找 Mom → 到长椅打开八音盒，选 **Turn the key** → 在记忆屋找奶奶。选 **Close the lid** 会回到墓园，可以再打开。重逢后仍可以自由调查屋里的照片、毯子和柜子。
- 墓园和记忆屋采用 1.32× 跟随镜头；固定取景的闪回不缩放。两张探索地图的碰撞仍用 MV 原生瓦片和事件，现有角色行走图、脸图继续使用。
- 左下的枯花墓、长苔石碑、尖顶板岩与摆石子的矮碑各有调查文本。可蹲下从洋甘菊里拾取蓝丝带；八音盒和记忆屋的家庭照片也会进入物品栏。它们都是 MV 数据库的关键物品，可直接由后续事件判定。

## 素材清单

| 工程内路径 | PNG 规格 | 制作用途 |
| --- | --- | --- |
| `img/parallaxes/!RM_CemeteryOvercast.png` | 1152×864，24×18 格 | 内置图像生成：原封面笔触、阴天湿石与五座可辨认的墓；无静止雨丝 |
| `img/parallaxes/!RM_CemeteryCanopyStory.png` | 1152×864，透明 | 从新版底图提取树冠，按同一坐标叠到角色前 |
| `img/parallaxes/!RM_HouseOvercast.png` | 1152×864，24×18 格 | 内置图像生成：冷灰窗光、旧木地板，表现压抑的闪回 |
| `img/parallaxes/!RM_HomeStorybook.png` | 1152×864，24×18 格 | 内置图像生成：暖光记忆屋，照片、毯子和茶杯可调查 |
| `img/characters/$RM_Grace_Run.png`、`$RM_Grace_Crouch.png` | 各 144×192，透明 | 内置图像生成；12 格单角色切片，48×48；行序下、左、右、上，三帧同脚底锚点 |
| `img/pictures/RM_MusicBoxStorybook.png`、`RM_MemoryChapter.png` | 各 816×624 | 内置图像生成：八音盒特写、进记忆屋的全屏章节插画 |
| `img/titles1/RM_Title.png` | 816×624 | 恢复原封面，不覆盖原稿；标题菜单只排列在下方 |
| `img/system/RM_StoryPaper.png` | 192×192，透明 | 按 MV 窗口皮肤分区绘制纸色、细线框与选中态 |

MV 素材规范以安装目录内的英文帮助 `Help/page/01_11_01.html` 为准。地图仍用 48 像素格；`!` 前缀保留地图坐标同步。墓园由 MV 在运行时绘制低强度动态雨，不让底图雨丝固定。新增原创标题、墓园、闪回和八音盒配乐，共用同一组旋律；配套重做雨声、室内底噪、木盖、菜单、滴水和三种地面脚步。所有新增音频都提供 Ogg／M4A，制作源在 `tools/render-act1-audio.py`。

## 验证

- 在工作区运行 `node tools/check-act1-maps.cjs` 检查图片规格、地图连通性及事件接线。
- `node tools/check-act1-assets.cjs` 检查封面、跑蹲图、章节画面、动态天气和关键物品配置。
- 在浏览器以工程自带的 MV 运行时加载，自动推进主线并检查暂停和恢复。检查使用新游戏，不覆盖现有存档。
