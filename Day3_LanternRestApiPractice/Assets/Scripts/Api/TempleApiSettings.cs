using UnityEngine;

namespace LanternRestApiPractice.Api
{
    /// <summary>
    /// API 所使用的环境。
    /// 在 Unity Inspector 中选择开发环境或生产环境，BaseUrl 会据此返回对应的服务器地址。
    /// </summary>
    public enum ApiEnvironment
    {
        // 本机开发时使用，例如连接本地 mock REST 服务。
        Development,

        // 应用发布后使用，连接正式部署的 REST 服务。
        Production
    }

    /// <summary>
    /// 保存天灯 REST API 的连接配置。
    ///
    /// 这是 ScriptableObject，不需要挂在 GameObject 上。创建成资源文件后，
    /// 可在 Inspector 中修改接口地址和超时时间，再提供给 API 客户端使用。
    /// </summary>
    [CreateAssetMenu(menuName = "Lantern API/Temple API Settings", fileName = "TempleApiSettings")]
    public sealed class TempleApiSettings : ScriptableObject
    {
        // 当前选择的运行环境。默认使用开发环境，方便本机连接 mock 服务。
        [SerializeField] private ApiEnvironment environment = ApiEnvironment.Development;

        // 开发环境 API 的根地址。mock 服务会运行在这个地址。
        [SerializeField] private string developmentBaseUrl = "http://127.0.0.1:5057";

        // 生产环境 API 的根地址。实际部署时需要替换为真实服务地址。
        [SerializeField] private string productionBaseUrl = "https://api.example.com";

        // 单次 HTTP 请求最多等待的秒数。Min(1) 表示 Inspector 中不能设为小于 1。
        [SerializeField, Min(1)] private int timeoutSeconds = 15;

        // 向外提供当前环境，其他脚本只能读取，不能直接修改配置字段。
        public ApiEnvironment Environment => environment;

        // 根据当前环境返回正确的 API 根地址，调用方不必自行判断环境。
        public string BaseUrl => environment == ApiEnvironment.Development
            ? developmentBaseUrl
            : productionBaseUrl;

        // 向 API 客户端提供请求超时时间。
        public int TimeoutSeconds => timeoutSeconds;
    }
}
