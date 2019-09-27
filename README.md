## 运行
使用 Unity 打开工程，打开 Init 场景，便可直接运行


## Hotfix 修改更新
每次修改 Hotfix 中的代码后，在 Unity 中使用 `tools` 工具，执行：
- `Tools/ILRuntime/Build Hotfix(Release)` 编译出 Release 版本的 Hotfix.dll
- `Tools/ILRuntime/Copy Hotfix Files(Release)` 将新的 Hotfix.dll 复制到 StreamingAssets 目录下

