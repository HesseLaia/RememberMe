# Remember Me 网页版发布 · 历史草案

[English](DEPLOY.md) | 简体中文

本草案记录早期 GitHub Actions／GameCI 方案。当前发布遵循[仓库发布说明](../../../../docs/DEPLOY.zh-CN.md)。

原发布包包含 Unity 工程和 GitHub Actions 配置。正式网页由 GitHub Pages 托管，同学用电脑浏览器打开链接后，可以用键盘游玩。

## 用 GitHub Actions 构建并发布

1. 将发布包中的 `RememberMeUnity/` 和 `.github/workflows/remember-me-web.yml` 放到 `RememberGame` 仓库根目录。Unity 工程目录里应直接看到 `Assets/`、`Packages/` 和 `ProjectSettings/`。
2. 在仓库选 **Settings → Pages → Build and deployment → Source → GitHub Actions**。如果仓库为私有，先确认当前 GitHub 方案及 Pages 访问权限适合班级分享。[GitHub Pages 发布设置](https://docs.github.com/en/pages/getting-started-with-github-pages/configuring-a-publishing-source-for-your-github-pages-site)
3. 在 **Settings → Secrets and variables → Actions** 配置 GameCI 所需的 Unity 授权。Unity Personal 的工作流读取 `UNITY_LICENSE`、`UNITY_EMAIL` 和 `UNITY_PASSWORD`。按 GameCI 对适用许可证的激活步骤准备这些值，并只在 GitHub Secrets 页面录入。[GameCI 授权说明](https://game.ci/docs/github/activation/)
4. 提交并推送到默认分支。打开 **Actions → Build Remember Me for GitHub Pages** 查看结果，也可以从那里手动运行。发布成功后，从该运行的 `github-pages` 部署结果复制链接。若仓库用默认 Pages 域名，链接格式为 `https://jeddakholmes-byte.github.io/RememberGame/`。[GitHub Actions 部署步骤](https://docs.github.com/en/pages/getting-started-with-github-pages/using-custom-workflows-with-github-pages)

## 在有 Web Build Support 的电脑上打包

用 Unity **6000.6.1f1** 打开工程，把活动平台切换为 **Web**，然后点 **Remember Me → Build for GitHub Pages**。输出在 `RememberMeUnity/Build/GitHubPages/docs/`。按本草案直接上传已构建目录时，将 `docs/` 放在仓库根目录，并在 Pages 选择 **Deploy from a branch → 默认分支 → `/docs`**。将 `docs/.nojekyll` 一起提交。

网页播放器用 Gzip 和浏览器解压回退，可从 `RememberGame/` 这样的子路径启动。浏览器版嵌入 Tinos 字体，其许可证随发布文件附带。存档保留在当前浏览器，打开同一网址可继续。
