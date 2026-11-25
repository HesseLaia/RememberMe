**English** · [简体中文](DEPLOY.zh-CN.md)

# Build and publish

The source project is in RememberMeUnity/. The current public release is [v10](https://jeddakholmes-byte.github.io/RememberGame/unity-v10/). GitHub Pages publishes static files from the root of main.

## Build

Install Unity 6000.6.1f1 with Web Build Support. In Unity, choose Remember Me → Build for GitHub Pages. The output is RememberMeUnity/Build/GitHubPages/docs/ and includes both acts and the font license.

For command-line builds, run the following from the repository root after replacing the Editor path.

~~~powershell
$unityEditor = 'C:/Path/To/Unity/Editor/Unity.exe'
$projectPath = (Resolve-Path RememberMeUnity).Path
$outputPath = Join-Path $projectPath 'Build/GitHubPages/docs'
$logPath = Join-Path $projectPath 'Build/Review/WebBuild.log'
New-Item -ItemType Directory -Path (Split-Path $logPath) -Force | Out-Null
& $unityEditor -batchmode -nographics -quit -projectPath $projectPath -buildTarget WebGL -executeMethod RememberMe.Editor.WebBuild.Build -customBuildPath $outputPath -logFile $logPath
~~~

Check the exit code and REMEMBER_CLEAN_WEB_BUILD_PASS in the log. Building regenerates the Act I scene.

For Windows, use the same Editor and project with build target Win64 and execute method RememberMe.Editor.Act1Builder.Build. Omit -customBuildPath. Output goes to Build/Windows/RememberMe.exe.

## Verify

From RememberMeUnity/, install Node.js and prepare the browser test dependencies.

~~~sh
npm install --prefix QA --no-save --package-lock=false playwright
npx --prefix QA playwright install chromium
node QA/Verify-Web.cjs
~~~

REMEMBER_PLAYWRIGHT can point to an existing Playwright module. REMEMBER_BROWSER can specify a Chrome or Chromium executable. The script tests Build/GitHubPages/docs/, writes captures to Build/Review/Web/ and reports REMEMBER_WEB_VERIFIED when complete.

Coverage includes WASD menu navigation, both acts, recipe undo, scripted walking frames and browser save persistence. Also play the specific route affected by your change.

## Publish

1. Complete source changes and verification on a task branch.
2. Copy the complete build into a new root release directory, such as unity-v11/. Include .nojekyll, Build/, index.html and Tinos-LICENSE.txt.
3. Update the root play link, unity/index.html redirect, current README release links and the feedback version in Assets/WebGLTemplates/RememberMePages/index.html.
4. Commit source, assets and published output, then merge the release pull request into main.
5. Watch pages build and deployment for that commit. Verify the public page, keyboard input, Kitchen entry and feedback link after deployment succeeds.

Existing version directories serve shared URLs. Check references and shared links before reorganizing those directories.

## Historical material

The original RPG Maker MV project is game/Game.rpgproject, with a browser player in game/. [Earlier Actions and GameCI deployment samples](../RememberMeUnity/Docs/Archive/LegacyDeployment/) are archived for reference.