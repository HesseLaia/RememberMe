[English](README.md) · **简体中文**

# 新素材投稿区 incoming/

历史 RPG Maker MV 工程的素材先提交到这里，经评审后接入 `game/`，便于追踪提交并保持工程可玩。当前 Unity 素材流程见[协作流程](../../CONTRIBUTING.zh-CN.md)。

## 命名

| 素材 | 命名 | 例子 |
|---|---|---|
| 脸图 | `RM_Face_<角色名>.png` | `RM_Face_Grace.png` |
| 单人行走图 | `$RM_<角色名>.png` | `$RM_Grace.png` |
| 场景 / 道具 | `RM_<物件名>.png` | `RM_Gravestone.png` |
| 音乐 / 音效 | `RM_<用途>`，`.ogg` 和 `.m4a` 两份 | `RM_DoorCreak.ogg`、`RM_DoorCreak.m4a` |

按照[美工素材指南](../README.zh-CN.md)中的精确尺寸与网格提交，使 MV 能正确切分和显示图像。

## 替换版本

替换已有素材时，例如重画 `$RM_Grace.png`，在提交文件名中加入日期，并保留旧版：

```
$RM_Grace_20261005.png
```

这样旧版还在，能对比、能回滚。

## 交完之后

在群里说一声交了哪个素材、替换/新增的是哪一句台词或哪个场景。由负责工程的人复制进 `game/`，然后跑一遍游戏确认。

将新脸图的文件名同步给编剧，由其在对话事件中连接对应图像。
