**English** · [简体中文](CONTRIBUTING.zh-CN.md)

# Contributing to Remember Me

[Project overview](README.md) · [Current handoff](docs/HANDOFF.md)

## Start a task

Clone the repository for a first checkout. For an existing checkout, run these commands from the repository root and choose a branch name for the task.

~~~sh
git switch main
git pull --ff-only origin main
git switch -c feature/your-task
~~~

Save or commit existing work on its task branch before switching branches. Keep each branch focused on a change that reviewers can explain and verify.

## Deliverables

| Work | Location and handover |
| --- | --- |
| Gameplay, UI and animation | RememberMeUnity/Assets/Scripts/, with reproduction steps and playtest results |
| Layout and collision | RememberMeUnity/Assets/Editor/Act1Builder.cs and affected scenes, checked after rebuilding |
| Integrated artwork and audio | The relevant Unity Assets directory, with .meta files, source and license |
| New artwork proposals | art/incoming/, with purpose, dimensions, transparency, directions and replacement target |
| Narrative proposals | script/drafts/, with act, scene, trigger, dialogue and next objective |
| Playtest feedback | The web player's Feedback link, with version, location, actions, expected and actual result |

Move assets through Unity's Project window so their .meta files follow them. Identify affected scene areas in the task or PR so contributors can coordinate shared edits.

## Submit and review

~~~sh
git add path/to/changed-files
git diff --cached
git commit -m "Describe the change"
git push -u origin feature/your-task
~~~

Open a pull request targeting main. Describe the change, relevant screenshots and verification steps. Merge after review, then synchronize main on other checkouts.

Check asset references, .meta files, keyboard and mouse input, resuming after pause and the affected story route. For save changes, test existing data. Follow the [interaction rules](RememberMeUnity/Docs/Interaction.md).

The Unity project's .gitignore covers local caches, logs and build output. Publish verified web output and entry updates using the [deployment guide](docs/DEPLOY.md).

## Documentation

Maintain an English .md file and a matching Chinese .zh-CN.md file, with reciprocal language links. English is the default entry. State findings, procedures and remaining decisions directly, preserving technical conditions and character voice. The upload preflight checks language pairs and local links.