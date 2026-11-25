[English](DEPLOY.md) · **简体中文**

# 构建和发布

当前源码位于 RememberMeUnity/，公开版本为 [v10](https://jeddakholmes-byte.github.io/RememberGame/unity-v10/)。GitHub Pages 从 main 分支的仓库根目录发布静态文件，提交已构建的 WebGL 输出后由 Pages 执行部署。

## 构建

安装 Unity 6000.6.1f1 和 Web Build Support。在 Unity 中选择 Remember Me → Build for GitHub Pages，输出目录为 RememberMeUnity/Build/GitHubPages/docs/，包含第一、二幕及字体许可证。

命令行构建可从仓库根目录执行。将下面的编辑器路径替换为本机路径。

~~~powershell
$unityEditor = 'C:/Path/To/Unity/Editor/Unity.exe'
$projectPath = (Resolve-Path RememberMeUnity).Path
$outputPath = Join-Path $projectPath 'Build/GitHubPages/docs'
$logPath = Join-Path $projectPath 'Build/Review/WebBuild.log'
New-Item -ItemType Directory -Path (Split-Path $logPath) -Force | Out-Null
& $unityEditor -batchmode -nographics -quit -projectPath $projectPath -buildTarget WebGL -executeMethod RememberMe.Editor.WebBuild.Build -customBuildPath $outputPath -logFile $logPath
~~~

检查退出码和日志中的 REMEMBER_CLEAN_WEB_BUILD_PASS。构建会重新装配第一幕场景。

Windows 构建使用同一编辑器和工程，将目标改为 Win64，执行方法改为 RememberMe.Editor.Act1Builder.Build，省去 -customBuildPath，输出为 Build/Windows/RememberMe.exe。

## 验证

在 RememberMeUnity/ 目录执行浏览器验证。需要 Node.js 和 Playwright，可在 QA 目录安装本地测试依赖。

~~~sh
npm install --prefix QA --no-save --package-lock=false playwright
npx --prefix QA playwright install chromium
node QA/Verify-Web.cjs
~~~

已有 Playwright 时，可用 REMEMBER_PLAYWRIGHT 指向其模块。REMEMBER_BROWSER 可指定 Chrome／Chromium 可执行文件。脚本默认测试 Build/GitHubPages/docs/，完整通过后输出 REMEMBER_WEB_VERIFIED，截图写入 Build/Review/Web/。

脚本覆盖 WASD 菜单导航、两幕流程、卡片撤回、剧情行走帧和浏览器存档恢复。还需试玩本次变动涉及的具体位置及操作。

## 发布新版本

1. 在任务分支完成源码、素材和验证，准备新的成品目录，例如仓库根目录的 unity-v11/。
2. 将 Build/GitHubPages/docs/ 内的完整输出放入新版本目录，包括 .nojekyll、Build/、index.html 和 Tinos-LICENSE.txt。
3. 更新根 index.html 的试玩链接、unity/index.html 的固定入口，以及 README 中的当前版本。同步更新 Assets/WebGLTemplates/RememberMePages/index.html 中反馈入口的版本信息。
4. 提交源码、成品和入口修改，通过 Pull Request 合并至 main。
5. 查看 Actions 中对应提交的 pages build and deployment。部署成功后，从公开链接检查加载、键盘操作、厨房入口与反馈按钮。

现有版本目录承载已分享的链接。需要调整历史目录时，先检查入口、引用和对应分享地址。

## 早期工程与部署样例

原 RPG Maker MV 工程在 game/，可通过本地 HTTP 服务试玩，编辑入口为 game/Game.rpgproject。Unity 的当前发布流程如上。

[历史 GitHub Actions／GameCI 样例](../RememberMeUnity/Docs/Archive/LegacyDeployment/) 保存了早期发布方案。