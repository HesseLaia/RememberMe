[English](README.md) · **简体中文**

# 历史成品归档

web/ 集中保存 Unity v2 至 v9 的网页成品。当前发布为仓库根目录的 unity-v10/，固定试玩入口为 unity/。

旧版本的分享入口由仓库根目录 404.html 跳转至对应归档位置，保留查询参数和页面片段。GitHub Pages 首次请求旧地址时会返回 404 页面，浏览器随后执行跳转。

当前源码和协作入口见[仓库首页](../README.zh-CN.md)，下一位协作者从[接力说明](../docs/HANDOFF.zh-CN.md)开始。

安装发布指南中的浏览器测试依赖后，可从仓库根目录运行 node tools/check-archive-links.cjs，检查八个旧入口的跳转。已有环境可通过 REMEMBER_PLAYWRIGHT 和 REMEMBER_BROWSER 指定模块与浏览器。
