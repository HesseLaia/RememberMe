**English** · [简体中文](README.zh-CN.md)

# Archived builds

web/ contains the Unity v2–v9 browser builds. The current release is unity-v10/ at the repository root, and unity/ is the stable play entry.

Root 404.html redirects earlier shared version URLs to their archived location, preserving query parameters and fragments. GitHub Pages initially returns the 404 document, whose script performs the browser redirect.

Start from the [project overview](../README.md) and [handoff notes](../docs/HANDOFF.md) for current development.

After preparing the browser dependencies in the deployment guide, run node tools/check-archive-links.cjs from the repository root to check all eight old entries. REMEMBER_PLAYWRIGHT and REMEMBER_BROWSER can select an existing module and browser.