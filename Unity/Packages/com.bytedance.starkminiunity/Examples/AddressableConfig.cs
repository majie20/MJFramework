// created by StarkMini

using System;
using StarkMini;
using UnityEngine.Scripting;

// ReSharper disable once CheckNamespace
/// <summary>
/// Example Code：一份示例的 AddressableConfig 代码实现动态配置 AA CDN url 相关设置。
/// </summary>
[Preserve]
public static class AddressableConfig {
    /// <summary>
    /// <para>
    /// 设置或获取 CDN 的 root url，用于和子目录路径拼接。<br/>
    /// 注：推荐用于 Addressable profile 的 `RemoteLoadPath` 变量。<br/>
    /// 注：建议在你自己的游戏层实现的启动代码中，运行时设置此属性。
    /// 注：相关设置参考《StarkMini 微端库指南》文档。<br/>
    /// </para>
    /// <para>
    /// Set root url for CDN service.<br/>
    /// Note: Suggested: use it in the `RemoteLoadPath` variable in your Addressable profile.<br/>
    /// Note: Set this value in your own Game startup code.
    /// </para>
    /// 
    /// <para>Example:<code>
    /// AddressableConfig.CdnRootUrl = "https://your-cdn-site.com/obj/res"; // 举例：使用你自定义的CDN站点的根目录
    /// </code></para>
    /// </summary>
    [Preserve]
    public static string CdnRootUrl { get; set; } = "https://lf3-stark-cdn.bdgp.cc/obj/game-res-cn/ucmc";

    /// <summary>
    /// 已废弃： CDN 上工程子目录的路径。<br/>
    /// 注：推荐直接使用 profile 设置中的 `[CdnProjPath]` 变量。<br/>
    /// 相关设置参考《StarkMini 微端库指南》文档。
    /// </summary>
    /// obsolete from v1.4
    [Preserve]
    [Obsolete("Use `DynamicCdnSubFolder` instead. Or, use profile setting `[CdnProjPath]` directly. Search for `CdnProjPath` in our StarkMini document (https://bytedance.feishu.cn/docs/doccnSzijs7L5sWGUQy8qyXbCW6) for detail.", true)]
    public static string CdnProjPath { get; set; } = "YourGameName/dev";

    /// <summary>
    /// 自定义动态解析的 CDN 子目录，用于拼接 `RemoteCDNUrl` 得到完整 CDN url。<br/>
    /// 注：仅用于自定义动态设置和解析目录的需求；没有高度动态自定义需求情况下，推荐直接使用 profile 设置窗口中的 `[CdnProjPath]` 变量来拼接完整 CDN url。<br/>
    /// </summary>
    [Preserve]
    public static string DynamicCdnSubFolder { get; set; } = "YourGameName/dev";

    /// <summary>
    /// 自定义动态解析的 `RemoteCDNUrl` CDN 完整路径。<br/>
    /// 注：仅用于自定义动态设置和解析目录的需求；没有高度动态自定义需求情况下，推荐直接在 profile 设置窗口中拼接固定的完整 CDN url。<br/>
    /// 注：需要先设置 `CdnRootUrl`, `DynamicCdnSubFolder` 属性。<br/>
    /// 注：可用于 Addressable profile 的 `RemoteLoadPath` 变量。<br/>
    /// <para>Example:<code>
    /// `{AddressableConfig.RemoteCDNUrl}/[BuildTarget]`
    /// </code></para>
    /// </summary>
    [Preserve]
    public static string RemoteCDNUrl {
        get {
            if (!_isInitialized) {
                InitUrls();
            }
            return _remoteCDNUrl;
        }
    }

    /// <summary>
    /// 完全自定义的cdnurl,starkMini 不再对url做任何处理/拼接
    /// 适用场景：cdn资源以及catalog请求路径完全在业务中自定义和拼接，在Addressable初始化完成前赋值即可
    /// </summary>
    [Preserve]
    public static string CustomRemoteUrl {
        set;
        get;
    } = "https://127.0.0.1/CustomRemoteUrl/Error/NotSet";

    /// <summary>
    /// 检查已记录的缓存资源版本号，不匹配时清理Cache。
    /// </summary>
    [Preserve]
    public static void ValidateCacheResVersion(string resVersion) {
        ResUtil.ValidateCacheResVersion(resVersion, true);
    }

    private static void InitUrls() {
        string path = CdnRootUrl.TrimEnd('/') + "/" + DynamicCdnSubFolder.TrimStart('/');
        _remoteCDNUrl = path;
        _isInitialized = true;
    }

    private static bool _isInitialized = false;
    private static string _remoteCDNUrl = "";
}