# Remember Me Web Deployment · Historical Draft

English | [简体中文](DEPLOY.zh-CN.md)

This draft records the early GitHub Actions/GameCI approach. Current releases follow the [repository deployment guide](../../../../docs/DEPLOY.md).

The original release package contained the Unity project and GitHub Actions configuration. GitHub Pages hosts the web game so classmates can play with a keyboard in a desktop browser.

## Build and publish with GitHub Actions

1. Place the package's `RememberMeUnity/` and `.github/workflows/remember-me-web.yml` at the root of the `RememberGame` repository. The Unity project directory must directly contain `Assets/`, `Packages/`, and `ProjectSettings/`.
2. Select **Settings → Pages → Build and deployment → Source → GitHub Actions**. For a private repository, check that the GitHub plan and Pages access settings support sharing with the class. [GitHub Pages publishing settings](https://docs.github.com/en/pages/getting-started-with-github-pages/configuring-a-publishing-source-for-your-github-pages-site)
3. Configure the Unity authorization required by GameCI under **Settings → Secrets and variables → Actions**. The Unity Personal workflow reads `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD`. Prepare these values using GameCI's activation instructions for the applicable license, and enter them only in GitHub Secrets. [GameCI activation guide](https://game.ci/docs/github/activation/)
4. Commit and push to the default branch. Open **Actions → Build Remember Me for GitHub Pages** to inspect the result or run the workflow manually. After deployment succeeds, copy the link from that run's `github-pages` deployment result. With the default Pages domain, the URL is `https://jeddakholmes-byte.github.io/RememberGame/`. [GitHub Actions deployment guide](https://docs.github.com/en/pages/getting-started-with-github-pages/using-custom-workflows-with-github-pages)

## Build on a computer with Web Build Support

Open the project in Unity **6000.6.1f1**, set the active platform to **Web**, and select **Remember Me → Build for GitHub Pages**. Output goes to `RememberMeUnity/Build/GitHubPages/docs/`. For the draft's direct-upload approach, place that built `docs/` directory at the repository root and select **Deploy from a branch → default branch → `/docs`** in Pages. Include `docs/.nojekyll` in the commit.

The web player uses Gzip with browser decompression fallback and can launch from a subpath such as `RememberGame/`. It embeds the Tinos font, with the license included in the release files. Saves remain in the current browser. Revisiting the same URL resumes them.
