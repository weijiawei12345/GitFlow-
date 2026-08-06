# 天灯 REST API 练习项目

这是一个使用 Unity `2022.3.62f2` 创建的强类型 REST API 客户端练习项目。
它与 `Gagapi_unity` 完全隔离，不会修改原项目的源代码、场景、包配置或全局状态。

## 当前目标

第一阶段只练习读取数据：获取庙宇列表和庙宇详情。
创建祈福记录的 `POST /api/v1/lanterns` 会留到后续阶段，避免一开始同时处理写入、鉴权等复杂问题。

## 后续运行步骤

1. 使用 Unity Hub 和 Unity `2022.3.62f2` 打开当前文件夹。
2. 后续创建 mock 服务后，在终端运行 `node MockServer/server.js`。
3. 在 Unity 的 Create 菜单中创建 `TempleApiSettings` 资源，保存到 `Assets/Settings`。
4. 开发时保留默认接口地址 `http://127.0.0.1:5057`；发布时在该资源中切换为生产环境并填写真实 REST API 地址。
5. 后续创建场景和 `TempleListController` 后，将配置资源和 UI 字段赋值给控制器，再运行场景。

接口格式、返回示例和错误处理约定见 `Docs/api-contract.md`；旧项目与练习项目的架构差异见 `Docs/architecture.md`。
