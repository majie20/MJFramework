# StarkMini Changelog

## [1.5.12] [1.5.13] - 2021-09-10
- 新增：Session编辑和依赖分析工具 - 支持星号模糊匹配，支持正则匹配，支持忽略大小写
- 新增：AA提供Tools选项：实验功能：AllowAAInResourcesFolder 允许 AA asset 放在 Resources 目录下
  - 该选项菜单位于AA配置窗口 Tools 下拉按钮中。
- 改进：EventViewer 工具，高亮RefCount条目，增加Dump功能、分类筛选功能；
- 改进：Handle 增加可以读取获取 DebugReferenceCount 的接口
- 修正：AA在 AssetDatabase模式、Simulate模式下，初始化出错问题、初始化Complete时机问题、运行时分析状态None的问题；
- 修正：同步加载失败可能 ArgumentNullException 异常的问题
- 修正：AA的ResourceManager的Update callback会泄露、不断叠加调用次数的问题 | Fix resource manager callback leak.

## [1.5.10] [1.5.11] - 2021-08-23
- 更新changelog

## [1.5.8] [1.5.9] - 2021-08-18
- 新增：`Addressables.GetInitializeResultStatus()` 获取Initialize初始化的结果状态。 
- 新增：`bool Addressables.HasResourceLocation()` 接口：是否是有效的资源 key. 注：key可以是 address, label.
- 新增：`bool Addressables.HasStartedInitialize()` 接口：是否开始过 Addressables Initialize. 注：不仅包括手动执行`Initialize`方法，任何Addressables加载功能都能触发`Initialize`.
- 优化：`Addressables.InitializeDirect()` 现在返回 handle，可以判断handle.Status是否成功
- 优化：`Addressables.GetDownloadSizeSync()` 现在返回值可以判断是否出错：Returns InvalidDownloadSize (-1L) if failed with error.

**重要：默认 Cdn Url 域名迁移：**
- 迁移 ttcdn 的 CdnRootUrl 至新的域名。 旧域名预计将于2020年9月起失效。
- `AddressableConfig.CdnRootUrl` 的默认值，已更新为最新正确的ttcdn的url.
- 参阅通知：↗[【很重要】字节CDN域名调整操作指南](https://bytedance.feishu.cn/docs/doccn6pdadXcDJFXnzDlauydmmg#) 

**后台下载 `DownloadAllInTheBackground` 相关功能改进：**
- 修正：并发请求数提高到3以上时，在手机设备可能明显提高CPU占用、耗电和发烫 的问题
- 优化：并发性能、GC Alloc、CPU耗时，避免性能卡顿Jank
- 调整：后台下载 DownloadAll 相关功能、接口、参数
  - 新增：获取后台下载总共需要大小: `GetDownloadAllTotalSizeSync()`
  - 新增：下载全部完成的监听事件 `DownloadAllCompletedEvent`
  - 新增：下载过程中发生错误的监听事件 `DownloadAllErrorEvent`
  - 调整：参数 `queueIntervalTime` 也放入 `ConcurrencyOptions` 结构体
  - 新增：实现后台下载 Retry 机制，允许设置 重试次数、重试的延迟时间
    - 新增：`DownloadAllFileRetryCount`： DownloadAll 每个下载文件失败时重试次数。 默认5。
    - 新增：`DownloadAllFileRetryDelayTime`： DownloadAll 每个下载文件失败时做重试的延迟时间。 默认1秒。

**Editor 编辑器相关功能改进：**
- 修正：运行中pause状态批量操作，Editor卡住。
- 依赖分析工具：
  - 新增：打通Stark Asset Analyser的 StarkProxyList (白名单设置) 管理功能
  - 新增：新增一列，判断并显示是否被 StarkProxy 列为启用名单。
  - 新增：支持ResList中资源内容的解析。
  - 修正：Session编辑，拖拽到面板后列出资源可能与实际拖动资源不一致的问题。
- 运行时加载分析：
  - 新增功能：导入列表文件 ResList。 -- 以便于将在已收集的列表基础上，做资源加载分析、做首包资源整理。
- 新增：Editor菜单："StarkMini/Experimental/Dump All AA Group Assets Infos"。

**批量转化工具 Stark Asset Analyser `[1.2.30]` `[1.2.31]` 改进：**
  - 新增：支持NGUI转化
  - 新增：支持SpineMaterial转化
  - 新增：转化命令：强制转化（忽略MD5缓存）
  - 新增：转化工具分析结果列表中，打通直接批量执行 ProxyList 白名单管理
  - 优化：大幅提升转化速度
  - 优化：自动检查资源MD5变动
  - 优化：现在自动校验AA是否初始化、自动校验是否同步接口可用。
  - 优化：现在通过设置 `Stark.AssetAnalyser.ProxyResourceManager.AAInitializeMode` 可以自定义选择一种初始化模式。 默认自动异步初始化。
  - 优化：现在自动校验key location是否有效，并在失效时提供足够的调试信息。
  - 修正：无法设置Mesh进白名单的问题
  - 修正：Breakdown Prefab的转化处理
  - 修正：AnimatorController的转化处理
  - 修正：转化时，对于asset名变了的资源没有正确判断 proxy data isDirty 的问题。
  - 修正：动态设置的低清材质，无法在运行时替换的问题
  - 修正：下载等待队列管理逻辑错误，可能会导致相同runtimeKey的不同类型proxy的资源，在下载完成后仍被滞留在队列中，导致无法触发高清替换的问题。
  - 修正：意外触发同步AA Initialize、或异步初始化过程中触发，并导致初始化出错的问题；
  - 修正：已不存在address导致后续proxy逻辑错误。

**新工具开发中：**
  - 下载顺序功能：
    - 录制下载顺序，顺序列表key更新，开关接口，BackgroundDownloadAll自动读取下载顺序列表


## [1.5.2] [1.5.3] - 2021-06-16
- 新增：`Addressables.IsBundleDownloadLogVerbose` - AA Bundle下载的Log Verbose开关
- 新增：后台下载 DownloadAll 支持并发：
  - 新增：属性 `Addressables.DownloadAllConcurrencyOptions` - 设置并发请求数、并发的文件大小限制，支持mobile, wifi区别参数
- 新增：获取后台下载 DownloadAll 结果状态、进度的接口：
  - `GetDownloadAllResultStatus()`
  - `GetDownloadAllProgress()`
  - `GetDownloadAllProgressData()`
- 新增：增加Editor菜单：DumpAliveAAResourcesInfo
- 修正：构建命令行batchmode中 如果正在 isCompiling 会导致 AASettings 报错的问题
- 优化：EventViewer 工具，现在实时显示 Alive Count （活跃未释放的handle数）


## [1.5.1] - 2021-05-20
- 新增：`1.5.1`起，同时发布支持**Unity 2018.4 LTS**的兼容性版本。  
  - 例：`1.5.0`对应支持`2019.4`，`1.5.1`对应支持`2018.4`。
  - 用户可以在BGDT插件管理器中，自行选择安装对应兼容性版本。

## [1.5.0] - 2021-05-20
- 修正优化：依赖分析工具、批量改造工具，对大量资源分析和处理时的性能卡顿问题
- 修正：分析工具，Play mode中搜索、批量处理导致文件修改的问题
- 新增：依赖分析工具、批量改造工具，新增Session功能：支持使用自定义路径列表，支持include、exclude路径。

## [1.4.3] - 2021-03-31
- 修正：依赖分析工具、运行时加载分析工具，批量处理设置更改group时，性能效率差、卡很久的问题。
- 修正：AA：配置窗口中，批量设置大量资源的label，性能效率差、卡很久的问题。
- 修正：运行时加载分析工具，配置在创建时没有正确保存的问题。
- 修正：`LoadAssetAsync` 参数使用 AssetReference 时，可能返回没有结果的无效handle的问题。
- 修正：同步`LoadResource`失败时，Status是None, IsDone是false
- 优化：`ResLoader`中`PreDownload`, `PreDownloadAsync` 方法，增加callback参数 `onProgress`。
- 优化：`ResLoader`中`LoadSceneByCache`相关方法实现、注释，逻辑与预下载相关处理更一致。
- 优化：`ResLoader`中`DownloadAsync`相关方法实现、注释，明确会常驻保存的handle的处理，并新增相关方法：`IsDownloadHandleSaved`, `ReleaseAllSavedDownloadHandles`。
- 优化：新安装使用时，创建示例的sample group的数量。

## [1.4.2 Beta] - 2021-03-15
- 工具：**StarkMini - Res Load Analysis 运行时加载分析**
  - 优化：也收集分析DownloadAsync的信息
  - 优化：Bundle大小分析，区分remote, local, cached，并统计大小。
- 修正：依赖分析工具，间接依赖到atlas的特定情况下，是否processable的判断出现错误的问题。
- 修正：新安装StarkMini时 dll meta 的勾选平台设置错误的问题。

## [1.4.1 Beta] - 2021-03-11
- 改进：已通过 BGDT v2 新版发布。

## [1.4.0 Beta] - 2021-03-01
**新功能：**
- 工具：**StarkMini - Asset Deps Analysis 资源依赖分析**
  - 界面改版：支持多选批量操作，支持按列排序。
  - 新增：最近历史选择功能。
  - 新增：右键操作：选中Asset；打开Asset；在场景中查找Asset。
  - 新增：右键操作：工具跳转：选择Asset进行依赖分析；在AA配置中查找 Asset、Group。
  - 优化：进度条显示，百分比粒度。
- 工具：**StarkMini - Res Load Analysis 运行时加载分析**
  - 界面改版：支持多选批量操作。
  - 新增：预加载阶段规则编辑器，分析列表按自定义配置样式显示；
  - 新增：右键操作：打通工具跳转。
  - 新增：分析Bundle加载完成状态、Size大小、耗时时间。
  - 修正：一些Bundle、AA SubEntry 分析出错、批量操作出错的问题；
  统计 Bundle size, 增加 Bundles only模式，多次引用的bundle正确分析状态、消耗时间
- `LoadTaskList` 新增属性：Count, DoneCount。
- StarkMini 菜单 > Addressables Tools > 中，新增菜单项："AA Scenes And Create StartUp Scene" 功能：批量转化原有 Scenes In Build 转化为AA资源、并创建新的首场景：Startup Scene，用于加载原来的场景资源。
- StarkMini 菜单 > AutoBuilder 新增菜单项，支持Build："1 apk: 32+64 MixArchs"。
- AA：DownloadAll方法： `DownloadAllInTheBackground` （注：此功能实验中、改进中）。

**修正与优化：**
- 修正：StarkMini 菜单 > AutoBuilder 一些菜单功能，在工程设置为特定 arm arch 时，会抛异常 ArgumentOutOfRangeException 的问题。
- 修正：原Example代码 AddressableConfig 中的 `CdnProjPath` 容易与 profile 配置中的 `[CdnProjPath]` 混淆的问题。
  - 现在改为不同的命名，并推荐直接使用 profile 设置中的 `[CdnProjPath]` 变量。
  - 优化各个方法和属性注释。
- 修正：接口 `PreLoadAssets<TObject>(key)` 参数 `forceRepeat = false`，在成功加载一次后第二次调用，不会回调`onLoaded`，造成逻辑不统一的问题。
- 修正：AA配置窗口中，批量rename address - Use groupname/filename 替换后会得到重复两遍的字符串的问题。
- 修正：AA配置窗口中，Copy Address 功能，在SubEntry上不可用的问题。
- 优化：AA配置窗口中，选择了SubEntry时，混合选择了Group和Entry时，都能使用右键菜单。
- 修正：AA：主动 UnloadSceneAsync 会使 引用计数减少2次的问题。
- 修正：AA：游戏中调用 `AssetBundle.UnloadAllAssetBundles` 后，总是导致后续AA加载错误，报错 Unable to load dependent bundle 的问题。
- 修正：AA：`ENABLE_CACHING` 宏没有正确打开，导致 `ComputeSize` 没有读取已缓存的AB的问题。
- 修正：AA：关闭hosting时，加载失败后，同步加载方法会抛异常 Exception 的问题。
- 修正：AA：`LoadResource`同步加载的资源无法使用AA相关接口释放的问题。

# StarkMini Changelog v1.3 and before
## [1.3.0] - 2020-12-15
- **`ResLoader` 新增同步版功能接口**
    - `LoadResource<TObject>(object key)` 同步读取asset
    - `LoadResources<TObject>(object key)` 同步读取复数个assets
    - `LoadResources<TObject>(IList<object> keys)` 同步读取复数个assets
    - `GetDownloadSizeSync(object key)` 
        * 同步接口：确认所需下载的大小，包括其依赖。 注：已全部下载有缓存文件时大小是 0.
    - `CreateEditorResList()`  
        * 用于Editor编辑器环境的运行时：根据运行后所有已读取过的key，生成列表文件`ResList`。<br/>
        * 生成文件路径：`Assets/Res/ResList[Index].asset`, 其中 Index 从1开始<br/>
        * 可配合`PrepareResource`接口加载所有准备资源。
    - `PrepareResListAsync(string reslistKey)`  
        * 准备好列表文件中reslist中的所有的key的内容，返回Handle结构体。为异步加载，可通过Handle等待其完成。<br/>
        * 参数：reslistKey 应使用列表文件`ResList[Index].asset`设置为 AA 后的 address。
    - `SetSyncLoadErrorAction(Action action)` 注册同步加载功能读取 Error 时的 Action.
- **`ResLoader`** 
    - 修正 `LoadAsset` 成功过后，下一次Load的complete回调，可能比await handle.Task的执行更晚的问题
    - 调整：解除SC环境检查的限制
- **`ResCache`** 
    - 新增：`SetKeyCaseMode` 方法：自定义设置 key 大小写忽略或敏感的模式。
- **`AddressableConfig`**  修正 可能会被strip而导致AA profile访问不到的问题

- **工具：**
    - 新增：**StarkMini > AA Sync: Create ResList**菜单，生成用于同步功能的列表文件`ResList`
    - 优化：**StarkMini > 资源依赖分析** 工具
        - 新增：在scene中查找定位到引用asset的GameObject节点的功能.
        - 新增：搜索范围：ScenesInAssets，AddressableScenes。
        - 新增：增加批量处理功能
        - 新增：Tools按钮，包含菜单：AA Sync: Create ResList 生成用于同步功能的列表文件`ResList`
        - 优化：GUI显示优化、折叠options选项。

- **增强AA库：[1.15.1-r17]**
    - 修正：批量修改group的属性后，无法正确保存到setting文件、修改会丢失的问题。
    - 新增：同步加载功能接口：
        - `LoadResource<TObject>(object key)` 同步读取asset
        - `LoadResources<TObject>(object key)` 同步读取复数个assets
        - `LoadResources<TObject>(IList<object> keys)` 同步读取复数个assets
        - `GetDownloadSizeSync(object key)` 
            * 同步接口：确认所需下载的大小，包括其依赖。 注：已全部下载有缓存文件时大小是 0.
        - `CreateResListAsset()`  
            * 用于Editor编辑器环境的运行时：根据运行后所有已读取过的key，生成列表文件。<br/>
            * 生成文件路径：`Assets/Res/ResList[Index].asset`, 其中 Index 从1开始<br/>
            * 可配合`PrepareResource`接口加载所有准备资源。
        - `PrepareResource(string reslistKey)`  
            * 准备好列表文件中reslist中的所有的key的内容（异步加载）<br/>
            * 参数：reslistKey 应使用列表文件`ResList[Index].asset`设置为 AA 后的 address。
        - `SetDirectLoadErrorPopupAction(Action action)` 注册同步加载功能读取 Error 时的 Action.

## [1.2.9] - 2020-12-03
- 运行时：
    - 优化：`LoadTaskList` 的`ReleaseHandles`方法，对加载失败的任务也能正确释放。
    - 优化：`ResLoader` 的`ReleaseHandle`等释放资源的方法，对加载失败的任务也能正确释放。   
        对应新增 `AllowReleaseOnFailure` 开关，默认`true`。
- 工具：
    - 调整：移除 **StarkBuilder** 菜单，收入到StarkMini内部菜单、且强制设置appid。  
        现在建议使用独立插件工具：**Stark SDK Unity Tools**（可以从BGDT安装）.
- **增强AA库：[1.15.1-r15]**
    - **重要**：修正：弱网加载scene失败后，可能导致加载一直报错 can't be loaded because another AssetBundle with the same files is already loaded 的问题。
    - 修正：EventViewer 工具、Diagnostic 逻辑中，在handle InValid 时的一些异常问题。
    - 修正：一处 ChainOperation 中 dep IsValid 检查；
    - 修正：一处 Scene Unload 中 Complete Release 的处理顺序。
    - 优化：增加一些 GroupOperation SceneProvider AssetBundleProvider 的调试信息。

## [1.2.8] - 2020-11-27
- 工具：**StarkMini - Asset Deps Analysis 资源依赖分析**：
    - 新增：搜索范围选择：单个资源、全工程资源、Resoueces、ScenesInBuild。
    - 新增：支持以AA的 **AssetGroup** 为分析目标，支持解析Group的所有entry依赖。
    - 新增：支持以Assets中任意 **文件夹** 为分析目标，支持解析目录内所有资源。
    - 新增：增加快捷选择 AA group 的下拉菜单。
    - 新增：分析结果【处理】按钮，支持直接进行AA分组改造： Move, Add, Remove AA Group。
    - 新增：支持解析 atlas依赖、atlas子图依赖。
    - 新增：支持勾选过滤：Packages 目录、Editor 目录、.cs脚本。
    - 新增：支持列表过滤：Show All, InAA, InAtlas, Prefab, Scene, SpriteAtlas，并显示每种类型的数量。
- 工具：**StarkMini - Res Load Analysis 资源加载分析**：
    - 新增：自动分析加载触发的bundle依赖。
    - 新增：支持保持滚动底部； 优化GUI性能，解决条目多时卡顿问题。
- 新增菜单组：**StarkMini - Addressable Tools**
    - 优化：整合原来的几个AA推荐配置、修复功能菜单。
    - 新增：创建设置单个AA的profile：Default, full, localhost, ttcdn。
- 优化工具：**StarkBuilder** 菜单：
    - 新增：支持设置 Set Build APK Compress 选项：LZ4, LZ4HC, Off。
    - 优化：自动检查是否使用了旧的测试工程的appid；
- `LoadTaskList`
    - 新增：`GetDownloadStatus()` 返回下载进度，会综合计算依赖的每个bundle、及其文件大小，  
    返回`DownloadStatus`结构体，可以进一步取得 Percent 百分比、DownloadedBytes、TotalBytes 字节数。
- `ResLoader`
    - 改动：`LoadSceneAsync` 增加`bool activateOnLoad`参数，是否加载完成时激活，默认`true`，  
    注意：如果 `activateOnLoad` 参数设为 false，必须加载完成后相应地执行 `handle.Result.Activate()` 激活Scene，否则可能导致其他异步加载任务被阻塞。
    - 新增：`DownloadAsync` 接口，返回 handle。
    - 改动：`LoadAudioClipAsync`，增加参数，是否加载完成时Play()播放，默认`false`。
- Utils 优化：
    - `TaskUtil` 优化：
        - 优化 `TaskFromHandle` 不再需要 exception 处理，返回值改为(bool) isSuccess是否加载成功。
        - 新增 接口：`AwaitHandleThen`, `HandleThen` ：等待 handle ，然后用回调处理 成功、失败。
        - 修正 `TaskFromCoroutine` 之前无法保证StartCouroutine、也无法保证正确等待完成的问题。
    - `DebugUtil` 优化平台差异的可靠性：
        - 新增接口`GetCallInfo`、以及其他原接口，改为使用的`[CallerMemberName]`参数。
- 增强AA库：[1.15.1-r13]
    - profile 变量中，增加支持标签 `[Version]` ，使用工程版本号。 
    - 构建AA资源时，log提示当前profile变量。
- 增强AA库：[1.15.1-r14]
    - 新增设置 **Folder SubAssets Removes Ext** 勾选设置以使AA化的目录的 SubAssets 的 address 都不带文件后缀.
    - 调整Resources目录的资源自动提示转换的目录`Resources_moved`，调整为 **`Res`**.
    - 新增计算并提示AA构建时长，自动检查是否localhost服务错误并给出Error。
    - 优化构建AA资源的按钮和log提示。
    - 优化自动log输出第一个请求的url，便于调试，log文字：`AssetBundle - First request`。
    - 升级依赖的package版本号：`com.unity.scriptablebuildpipeline`: `1.13.1`

## [1.2.7] - 2020-11-18
- 更新AA库：**[1.15.1-r12]**
    - 修正 LoadSceneAsync 下载依赖的 bundle 超时后，能导致Exception报错，并导致上层逻辑异常的问题。
    - 修正 Hosting 工具的 log 在有些情况下仍然不更新日志、且无法scroll滚动的问题。
    - 新增 Build下拉菜单中增加一个更明显的 “构建 AA 资源” 按钮。

## [1.2.6] - 2020-11-11
- `LoadTaskList` 修正和优化，封装一组资源加载任务
    - 修正 大量使用时可能导致Editor下加载卡住的问题。
    - 增加 `float PercentComplete` 属性，加载任务进度。
    - 补全注释。
- 工具：增加资源加载分析工具
    - 菜单位于：**ByteGame > StarkMini > Res Load Analysis**.
    - 实时监控分析所有运行时动态资源加载发生的time、key、type、status 等等。
    - 目前监控加载对应类包括 `ResLoader`, `ResLegacy`.

## [1.2.5] - 2020-11-09
- 工具：**StarkBuilder** 修正和优化：
    - 优化 一些菜单项文字、分组分隔位置，更清晰化。
    - 修正 有时菜单命令卡死无法停止的问题。现在会自动检查过长超时，并提示是否要中断命令。
    - Build And Run 菜单：
        - 修正 菜单中，没有列出localhost的问题。
        - 修正 BuildAndroid for ttcdn 菜单命令，没有同时build AA资源的问题。
        - 移除 菜单命令：BuildAndroid for ttcdn (skip AA)，由于它容易产生误解和导致错误打包。
    - Push and Run 菜单：
        - 修正 在 Win PC 上apk路径包含空格可能导致报错的问题
        - 优化 新增选项开关：delete data 或 keep data。
        - 优化 菜单项自动检查apk文件是否存在，更明确显示。
- `ResLoader` 接口重构、功能修正、优化：
    - **重要：** 重构 `ResLoader` 各个方法改为 **static 静态方法**，以与其他`StarkMini`的类用法统一。 原 `ResLoader.Instance` 属性现在标记为Obsolete。
    - **重要：** 优化 `LoadAssetsAsync` 接口，现在也会**统一做 `ResCache` 资源缓存**。
    - 优化 通过 `PreLoadAssets`和`LoadAssetsAsync` 预加载的类型`<T>`，现在在使用`ResCache.Get<T>`读取时，会自动做父类子类的类型兼容处理。
    - 废弃 `PreDownloadScenes` 、`TryLoadAsyncHandle` 接口，标记为：Obsolete.
    - 更名 `TryLoadAsyncTask` 接口为 `TryLoadFuncAsync` ，并新增参数 `Func<Task> failureHandler`.
    - 更名 `LoadUISprite` 接口为 `LoadUISpriteAsync`.
    - 新增 接口 `ReleaseAllPreLoadDownloadHandles`，释放所有 PreLoad,Download 的 handles.
    - 补全所有方法注释。
- `ResCache` 接口调整：
    - 修正 通过 `ResCache.Get<T>` 使用父类T: `Object`或`ScriptableObject` 读取不到预加载的`ScriptableObject`子类资源的问题。 
    - 废弃 `GetAsset<TObject>`接口，标记为：Obsolete.
    - 移除 不使用的`ToTargetResType`接口
- `ResUtil` 优化：
    - 新增方法`ConvertToSprite` 转换到 Sprite
    - 新增方法`ConvertToTexture2D` 转换到 Texture2D
- 更新AA库：**[1.15.1-r11]**
    - 新增功能：Address重命名Custom Rename工具，支持插入特殊替换tag：[GroupName], [AssetName] ...
    - Hosting工具改进，检查并去掉无效的ip，增加一个重启按钮。


## [1.1.16] - 2020-10-29
- 增加注释文档

## [1.1.15] - 2020-10-29
- 修正：创建常用group时local group属性被设置为remote的问题。

## [1.1.14] - 2020-10-28
- 修正：AA Setings 修复脚本中造成 Play mode 乱序的问题。
- 修正：AA Setings 修复脚本对 Packed Assets 的 script type 丢失时无法修复的问题。
- 更新AA库：[1.15.1-r9]
    - 修正：自定义重命名工具对话框中，替换预览在 WinPC 上没有正常刷新显示的问题。
    - 新增：Address快速重命名工具，增加菜单：Use filenname。
    - 新增：新工程自动提示创建推荐的groups、profiles，提供快捷按钮。
- 在BGDT插件管理器中，显示最近更新记录。

## [1.1.13] - 2020-10-27
- 工具菜单："ByteGame > StarkMini"：
    - 增加：创建常用示例 Addressable Groups 配置；
    - 增加：创建常用示例 Addressable Profiles 配置；
- 修正：修复 AA settings 引用问题时，没有正确修复Packed Assets.asset的问题。
- 优化：自动检测并提示修复 AA settings 脚本引用错误。
- 更新AA库：[1.15.1-r8]
    - 修正play modes选项的排序。
    - 优化：支持starkmini检测和修复 AddressableAssetSettings 脚本引用丢失，并在设置界面显著提示。

## [1.1.12] - 2020-10-26
- 增加在构建时读取预定义符号

## [1.1.11] - 2020-10-23
- 调整构建打包：按渠道更新package文件、脚本。

## [1.1.10] - 2020-10-22
- 修正一个Mac上adb调试脚本错误
- 修正优化ci脚本
- 更新AA库：[1.15.1-r7]
    - 合入功能：bundle文件名的hash加入crc、unity version的因素，修正bundle文件hash相同却CRC不匹配的问题。

## [1.1.8] - 2020-10-22
- 调整ci

## [1.1.7] - 2020-10-21
- 调整各个Editor菜单，整合到ByteGame
- 加入对CP版本的编译开关

## [1.1.6] - 2020-10-20
- 修正：CI访问不到builder方法的问题，增加 public AutoBuilderAPI

## [1.1.5] - 2020-10-20
- 修正：StarkMiniUICanvas的界面，如果发生场景中重复实例后，再调用Instance.Show，会报错和Exception的问题。

## [1.1.4] - 2020-10-20
- 更新AA库：[1.15.1-r6]
    - 增强批量修改address功能
        - 菜单和对话框都支持：ignoreCase, toLowercase, removeExt
        - 对话框中增加搜索替换的结果预览

## [1.1.3] - 2020-10-20
- 修正：源码 internal class error 的问题
- 更新AA库：[1.15.1-r5]
    - 修正 Assembly.Load

## [1.1.2] - 2020-10-19
- 更新AA库： Enhanced 1.15.1-r4
    - 修正 SceneProvider 在没有依赖DepOp时检查Status导致的错误问题。

## [1.1.1] - 2020-10-16
- 修正 AddressableAssetData 脚本

## [1.1.0] - 2020-10-16
- starkmini dll化

# StarkMini Changelog v1.0

## [1.0.49] - 2020-10-15
- 修正：builder自增版本号，在10以上数字时错误的问题

## [1.0.48] - 2020-10-14
- WkTool 加固工具更新到 wkpacker_1006，增加参数 --sv 1008
- Builder 增加支持 aa profile: localhost

## [1.0.47] - 2020-10-13
- 修正：一处编译
- 修正：工具 FindBuildinResources 优化支持分页显示，解决卡顿问题

## [1.0.46] - 2020-10-12
- 修正：AA 1.15 断网情况下加载报错 *Attempting to use an invalid operation handle*, 后续正常网络无法恢复加载 的问题
- 新增：`ResUtil.ValidateCacheResVersion` 检查已记录的缓存资源版本号，不匹配时清理Cache

## [1.0.45] - 2020-09-30
- AutoBuilder 重构部分函数。
- AdbTool 支持快速选择游戏apk。

## [1.0.44] - 2020-09-29
- Builder
  - 添加版本号自增功能到菜单项： Version +1.
  - 优化：版本号自增，增加支持带额外后缀[a-z][\d].
  - 优化：开启development时，apk文件名自动添加：_dev.
  - 增加接口： Close, Kill, SafeAbort 运行中的cmd.
- WkTool
  - 添加菜单项：立即执行加固.
  - 修正：进度条无法取消、中断的问题.

## [1.0.43] - 2020-09-26
- bundleVersion只设置一次

## [1.0.42] - 2020-09-26
- 构建号自动增加1

## [1.0.41] - 2020-09-26
- 命令行构建增加版本号参数

## [1.0.40] - 2020-09-26
- StarkBuilder
  - 修正Build Export选项打开时，无法build出apk的问题。
  - 修正有些情况下出现 UnityException: Moving final Android package(s) failed 报错的问题。
- StarkAdbTools
  - 优化脚本兼容性，adb shell schema的调用也支持一加等设备。
  - 修正一些命令脚本对包含空格的路径的支持。
  - 增加命令：安装游戏apk。
- StarkMini
  - 增加几个Editor命令： Open Caching folder; Reveal / Delete PlayerPrefs; Open / Delete PersistentData.

## [1.0.39] - 2020-09-17
- AdbTools 优化
  - 增加命令 push and run
  - 修正 adb 启动uc游戏的脚本在部分Android设备schema解析错误问题。现在支持 schema_type 参数。
  - 修正 adb 杀进程脚本报错问题。
- Builder 优化
  - 修正CheckAndroid output命令在setting未读取时的异常错误

## [1.0.38] - 2020-09-17
- 修正一个.bat脚本 set APP_ID 的错误

## [1.0.37] - 2020-09-14
- 微调文字

## [1.0.36] - 2020-09-14
- StarkAdbTools 优化
  - 支持对 douyin,toutiao主端 的 push local (Main.apk)
  - 自动检查缺少的必须参数，并显著提示错误
  - 增加adb脚本state检查，解决 no devices 时未报错的问题
- StarkBuilder 优化
  - 修正部分命令在setting未读取时的异常错误


## [1.0.35] - 2020-09-14
- Builder
  - 支持读取和设置 Development Build 开关，并在build确认弹框提示；
  - 优化setting初始化时自动判断 apk filename，现在使用工程目录名;
  - 修正一处代码bug；
  - 简化几个菜单项文字；
- ResLoader
  - 增加 IsPreDownloaded 接口: 判断是否已经被预下载成功。  对应相关接口：PreDownloadScenes, PreDownload, PreDownloadAsync.

## [1.0.34] - 2020-09-11
- StarkBuilder 优化和新功能
  - 修正：autobuilder 构建完后，会修改掉工程设置的 arm arch 的问题
  - 优化：各个Build子菜单项，使其清晰化；
  - 优化修正：build前确认窗口信息，和取消按钮的部分情况下的bug；
  - 增加：拉起运行ucgame各个版本环境，删除各个版本环境数据。
- StarkAdbTools 优化
  - 增加：脚本已全面支持windows版；
  - 修正：一些脚本中检测adb的问题。

## [1.0.33] - 2020-09-11
- 增加StarkAdbTools
  - 功能："adb devices", "adb disconnect", "运行宿主app", "杀宿主进程"
  - 功能："安装宿主apk", "push小程序插件", "全自动清空并安装小程序插件",
  - 功能："清空数据 (包括缓存)", "删除指定缓存", "运行ucgame"(选择版本环境)
  - 目前支持Mac版，待支持windows版

## [1.0.32] - 2020-09-08
- 修正构建脚本Tools~未正确打包的问题.

## [1.0.31] - 2020-09-08
- 修正加固脚本在Mac上的错误.

## [1.0.30] - 2020-09-08
- 优化加固工具
  - 增加支持Windows。
  - 支持 py 脚本 log 输出同步到UnityEditor Console，避免卡住无反馈。
  - 支持自动 pip install 安装脚本所需插件库.
  - 迁移 Tools 到 Tools~/ 目录，避免被Editor作为Assets、和不需要的meta。

## [1.0.29] - 2020-09-07
- Fix bug; 增加支持build mix archs (32+64 一个apk)

## [1.0.28] - 2020-09-07
- 重构整理AutoBuilder的菜单项、函数实现，增加功能项。
    - 菜单名称改为StarkBuilder。
    - 对各个 build 和 push douyin 增加支持目前所有AA profile和arch的组合。
    - 增加支持单独执行 build AA Res。
    - 增加支持单独执行 output apk 检查。

## [1.0.27] - 2020-09-07
- WkTool加壳加固 现在支持在Unity中直接执行。 注：需要依赖 python.

## [1.0.26] - 2020-09-04
- 新增：Editor工具: AutoBuilder: WkTool加壳加固 （第一版），显示加固脚本命令，暂时不支持自动执行。

## [1.0.25] - 2020-09-04
- Bug fix 

## [1.0.24] - 2020-09-04
- 构建apk及资源

## [1.0.23] - 2020-09-04
- 增加命令行构建参数读取

## [1.0.22] - 2020-09-04
- 修复bug：拉起32位uc问题

## [1.0.21] - 2020-09-03
- 修复bug

## [1.0.20] - 2020-09-03
- 修复bug

## [1.0.19] - 2020-09-03
- 增加自动拉起uc游戏接口

## [1.0.18] - 2020-09-03
- 增加不构建Apk自动拉起uc游戏接口
- 增加appId设置，拉起uc游戏时传入相应的appId

## [1.0.17] - 2020-09-03
- 修复自动拉起uc游戏：有空格路径问题

## [1.0.16] - 2020-09-03
- 自动构建脚本调用命令行时，传入adb路径

## [1.0.15] - 2020-09-03
-  自动设置adb路径

## [1.0.14] - 2020-09-03
- 修复 Tools/auto_run.sh 文本格式为unix文本格式

## [1.0.13] - 2020-09-03
- 增加 AutoBuilder.BuildAndRunAndroid32Full 及 AutoBuilder.BuildAndRunAndroid64Full 函数，自动编译Apk并自动拉起抖音小游戏

## [1.0.12] - 2020-08-31
- 修正 LocaleManager CurrentLanguage 初始值null导致异常问题。
- 修正 StarkMiniUICanvas.prefab 默认sort order太低，容易导致被游戏原有界面挡住的问题，调整为默认1000。


## [1.0.11] - 2020-08-31
- 增加 界面prefab： `Examples/UI/StarkMiniUICanvas.prefab` 用于游戏微端通用界面，将此prefab加入游戏scene以接入。目前界面包括：加载界面，加载失败弹窗界面。
- 增加 UI类： `StarkMiniUICanvas` 通用界面画布（根结点），使用了单例模式 + DontDestroyOnLoad。
- 增加 UI类： `LoadingView` 通用加载界面。通常调用 `LoadingManager` 类的相关接口，以相应显示。  
  `LoadingManager` 类的相关接口例如：  `StartShowLoading()`, `StopShowLoading()`, `ShowLoadingTask()`, `ShowLoadingHandle()` 等.
- 增加 UI类： `LoadErrorPopupView` 通用加载失败弹窗界面。 调用 static 的 `Popup()`, `PopupAsync()` 接口，以弹框。 使用 `ErrorMsg`, `OKText`, `RetryText` 等string型属性，作为通用的文本。  
  可以配合使用 `LocaleManager` 类，以增加和使用自定义的多语言文本。
- 增加 `LocaleManager`类：通用的本地化多语言管理。 通常调用`Get()`以 key 获取本地化语言文本。 支持通过接口增加自定义的本地化语言文本。
- 调整 `ResLoader` ： 
  - 修正 接口: `LoadSceneByCache()` 在Editor下，如果AA play mode 选择是前两个模式时，加载总是失败的bug。
  - 修正 接口: `PreLoadAssets()`, `PreDownload()` 在失败时也会标记为已加载的bug。
  - 调整 几个 TryLoad\*, TryPreload\* 的支持抛出异常的接口，最后参数 throwsException 默认值改为 false。
- 调整 `LoadingManager` ： 
  - `ShowLoadingTaskAsync()` 接口的最后参数 throwsException 默认值改为 false。
  - 微调调用Loading显示开始和停止时的log输出。
- 增加 `TaskUtil` 接口功能: `CoroutineFromTask<TObject>()`, `TaskFromCoroutine()` 支持 couroutine Task 互转.
- 增加 `ResUtil` 接口功能： `ClearAssetBundlesCache()`, `DeleteFilesInDir()`
- 增加 工具菜单：*StarkMini/Clear AB Caching* 的 *Hard Delete* 可选项，强制删除AssetBundle的Cache目录文件。

## [1.0.10] - 2020-08-24
- 增加 AutoBuilder 的工具命令，支持单个命令 32+64 build + 跳过AA build.
- 增加 ResLoader 接口： PreDownloadScenes /// 预下载 场景资源 （及其依赖）
- 增加 ResLoader 接口： LoadSceneByCache /// 使用缓存数据加载场景，应当提前 PreDownload 预下载。
- 增加 ResCache 对基本 object 类型的支持（note: alpha use only for now）

## [1.0.9] - 2020-08-23
- 增加ResLoader中几个异步批量加载、场景加载的耗时计算，便于调试。
- 调整SimpleTimer 部分接口命名，涉及接口: StartCheck, FinishCheck.
- 微调AddressableConfig Cdn设置改为setter，增加注释.

## [1.0.8] - 2020-08-21
- ResLoader 新增方法： TryPreLoadAssets /// 加载 Assets 尝试自动重试一定次数

## [1.0.7] - 2020-08-21
- 修正：autobuilder 总是中断取消的问题。

## [1.0.6] - 2020-08-21
- 修正：autobuilder 配置文件创建时，如果工程没有Editor目录，会报错的问题。
- 修正：autobuilder aa profile name 大小写敏感，可能导致意外失败的问题。优化为通过查找匹配自动修复大小写问题。

## [1.0.5] - 2020-08-20
- AutoBuilder 工具改进为可通用可配置，配置文件保存到工程内。
  - 可配置工程使用的 apk name。
  - 可分别配置 cdn, full 两种build方案的 aa profile name 和对应 apk 后缀。

## [1.0.4] - 2020-08-17
- 调整几个类的默认 debug log 开关：
  - BackgroundLoader 默认关闭
  - ResCache 默认关闭
  - ResLoader 默认关闭
  - ResLegacy 默认 = Debug.isDebugBuild

## [1.0.3-preview] - 2020-08-14
- ResLoader 功能改进：
  - 新增：PreLoadAssets<TObject> 预加载，并自动解析每个 asset address, 缓存到 ResCache
  - 新增：PreDownload 预下载，仅触发相应bundle，不加载资源对象。
  - 新增：TryLoadScene 加载 Scene 尝试自动重试一定次数. 注意: throwsException 默认 true.
  - 新增：TryLoadAsyncHandle<TObject> 执行传入的 `AsyncOperationHandle<TObject> LoadHandle()` 方法，等待其返回handle，尝试自动重试一定次数.
  - 新增：TryLoadAsyncTask 执行传入的 `async Task<bool> LoadFunc()` 方法，等待其返回isSuccess，尝试自动重试一定次数.
- BackgroundLoader 功能改进：
  - 重构：AddAsset<TObject> 添加单个Asset到后台加载队列中
  - 新增：AddAssets<TObject> 添加一组Assets到后台加载队列中，通常 key 为 label。
  - 新增：AddDownloadOnly 添加仅下载的任务。
  - 新增：各个添加任务支持priority参数立即加载。
  - 新增：自动避免重复加载。
- SimpleTimer 功能改进：
  - 新增：CheckStart 检查计时开始，并设置一个name
  - 新增：CheckFinish 检查计时结束，如果指定name有在计时中，计算并log它的消耗时间、帧数。
- 调整：AddressableConfig 的属性调整，以供在游戏层代码的初始化过程中设置 cdn url，不再必须直接修改源码.
- 调整：TaskUtil 各个 then 相关接口，对finish回调做try catch保护。

## [1.0.2-preview] - 2020-08-10
- 增加 ResLoader.LoadSceneAsync 接口，异步加载AA场景。
- 更新 AddressableConfig 中 url_tt_cdn 到新的CDN地址。

## [0.0.1-preview] - 2020-08-04
- Initial version
