# Remember Me · Unity 第一幕

[在线试玩](https://jeddakholmes-byte.github.io/RememberGame/unity/) · [美术规格与验收记录](Docs/Act1.zh-CN.md)

## 打开工程

1. 用 Git 或 GitHub Desktop 克隆 `jeddakholmes-byte/RememberGame`。
2. 在 Unity Hub 安装 **6000.6.1f1**。需要构建网页版的同学同时安装 **Web Build Support**。
3. 在 Unity Hub 选 **Projects → Add → Add project from disk**，打开仓库内的 `RememberMeUnity`，等待 Unity 导入。
4. 在 Unity 的 Project 面板打开 `Assets/Scenes/RememberMeAct1.unity`，点编辑器顶部的 **Play**。

## 素材和内容的位置

| 路径 | 内容 |
|---|---|
| [`Assets/Art/Actors`](Assets/Art/Actors) | 人物朝向、动作和墓园悼念者 |
| [`Assets/Art`](Assets/Art) | 地面、道具、对白立绘、幕卡、剧情特写和字体 |
| [`Assets/Audio`](Assets/Audio) | 配乐、环境声和音效 |
| [`Assets/Scenes`](Assets/Scenes) · [`Assets/Scripts`](Assets/Scripts) | 第一幕场景、玩法、对白、界面和存档 |
| [`Assets/Editor`](Assets/Editor) · [`QA`](QA) | 场景装配、构建脚本、验证脚本和试玩截图 |
| [`Packages`](Packages) · [`ProjectSettings`](ProjectSettings) | 依赖版本、2D 渲染、输入和其他工程设置 |

## 协作与发布

- 每项改动从 `main` 建一个分支。合并前在 Unity 里打开第一幕试玩，再提交 Pull Request。多人改同一个 `.unity` 场景前先在组内确认各自负责的部分。
- 在 Unity 的 Project 面板移动或重命名素材。提交时把素材及对应的 `.meta` 一起提交，Unity 用其中的 GUID 维护场景引用。`Library`、`Logs`、`UserSettings` 和本地 `Build` 由 Unity 生成。
- 素材已经随工程提交，可以直接编辑和使用。`Assemble-Art.ps1` 保留了原始素材导入流程，重新运行它需要原工作区的 `RememberMeAct1` 和相应原图。
- 网页版由仓库根目录的 [`unity/`](../unity/) 提供。更新网页时，从 Unity 点 **Remember Me → Build for GitHub Pages**，把 `Build/GitHubPages/docs/` 的输出更新到仓库的 `unity/`，再合并到 `main`。`Release/GitHubPages/` 保存另一套部署方案的参考文件。
