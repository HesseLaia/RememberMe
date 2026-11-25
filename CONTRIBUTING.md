# 参与 Remember Me 协作

[English project overview](README.md) · [中文项目说明](README.zh-CN.md)

## 开始任务

首次参与先克隆本仓库。已有副本时，在仓库根目录执行以下命令，将分支名替换为当前任务。

~~~sh
git switch main
git pull --ff-only origin main
git switch -c feature/your-task
~~~

存在未提交改动时，先提交到自己的任务分支或保存工作，再切换分支。每个分支围绕一个可说明、可验证的改动。

## 按角色交付

| 工作 | 位置与交付 |
| --- | --- |
| 玩法、界面、动画 | RememberMeUnity/Assets/Scripts/，附复现步骤与试玩结果 |
| 场景、碰撞 | RememberMeUnity/Assets/Editor/Act1Builder.cs 和相关场景，检查重建后的布局 |
| 已接入的美术、音频 | 对应 Unity Assets 目录，附 .meta、来源和许可证 |
| 新素材提案 | art/incoming/，注明用途、尺寸、透明背景、朝向与替换目标 |
| 剧情提案 | script/drafts/，注明幕、场景、触发条件、对白与下一步目标 |
| 试玩反馈 | 网页右上角 Feedback，注明版本、位置、操作、预期和实际结果 |

在 Unity Project 面板移动资源，让 .meta 跟随资源。修改共用场景前在任务或 PR 中说明涉及区域，便于协作者协调。

## 提交与评审

~~~sh
git add path/to/changed-files
git diff --cached
git commit -m "Describe the change"
git push -u origin feature/your-task
~~~

在 GitHub 创建目标分支为 main 的 Pull Request，说明改动、相关截图和测试步骤。评审通过后合并，其他成员再同步 main。

提交前检查新资源的引用和 .meta，检查键盘、鼠标、暂停后继续，以及涉及的完整剧情路径。修改存档时验证已有数据。输入和菜单行为遵循[交互约定](RememberMeUnity/Docs/Interaction.zh-CN.md)。

本地缓存、日志和构建输出由 RememberMeUnity/.gitignore 管理。网页发布时提交经过验证的版本目录及入口更新，步骤见[发布说明](docs/DEPLOY.zh-CN.md)。
