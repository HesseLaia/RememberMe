# 接力入口

开始工作先阅读 docs/HANDOFF.zh-CN.md，核对当前版本、上传状态、验证记录和下一步任务。

准备上传先运行 node tools/check-repo.cjs，读取一行摘要，再按需查看本机上传报告和相关变更。后续发布遵循 docs/UPLOAD.zh-CN.md，复用既有脚本与已记录的验证结果。

当前游戏源工程在 RememberMeUnity/。修改 Unity 文件时同时阅读 RememberMeUnity/AGENTS.md 及其引用的交互约定。

历史网页集中在 archive/web/，旧链接由根目录 404.html 跳转。提交目录调整时检查文档引用与已有试玩入口。交接更新写入 docs/HANDOFF.zh-CN.md。
