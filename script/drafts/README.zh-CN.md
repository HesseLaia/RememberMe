[English](README.md) · **简体中文**

# 剧本草稿

本目录保存场景工作稿。历史 MV 格式与语气规范见[剧本指南](../README.zh-CN.md)，当前 Unity 实现遵循 [Unity 工程说明](../../RememberMeUnity/README.zh-CN.md)。

[厨房 v0.4 工作稿](scene1_draft_2.zh-CN.md)保留较早的设计，包括像素画方向与当时的实现问题。已发布的 v10 使用绘本风资源。修改游戏前先与 `KitchenPrototype.cs` 对照。每份工作稿默认提供英文入口，并配有 `.zh-CN.md` 中文版。

## 命名

```
ACT2_<场景名>_script.md        例：ACT2_Attic_script.md
```

每个场景使用一份文件，多人参与时指定编辑负责人。中文文件命名为 `ACT2_<场景名>_script.zh-CN.md`，并在两版中加入互相切换的语言链接。

## 改已有台词的写法

在草稿中记录事件或 Unity 触发条件，并写清原文与替换文本。历史 MV 版本同时记录地图、事件和坐标。更新引擎事件后重新导出 `../ACT1_SCRIPT.md`。示例：

```markdown
# 修改：Act I OldGrave_4 的错别字

## 事件
Map001 → 017 OldGrave_4（坐标 18,4）

## 原文
It seems to be abondoned a long time ago.

## 改成
It looks like no one has been here
for a long time.

## 原因
修 `abondoned` 拼写；同时更接近孩子的具体观察。
```

MV 修改注明**事件名和坐标**。Unity 修改注明**源文件与触发条件**，供实现者定位。

## 定稿之后

由一人把确认后的草稿整合进引擎工程，再**从 New Game 试玩**，核对对白位置与顺序。MV 修改完成后重新导出 `../ACT1_SCRIPT.md` 等阅读稿。文档的中英文版本同步更新。
