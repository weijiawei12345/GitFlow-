 1. BtnLogin 的真实位置

场景层级：
UICanvas
└── ScreenSize
    └── IndexCanvas
        └── Btn
            └── BtnLogin

脚本位置：
Assets/Scripts/IndexCanvas/IndexCanvas.cs:94

//初始化时，脚本通过路径获取按钮：
BtnLogin = this.transform.Find("Btn/BtnLogin").GetComponent<Button>();
//然后绑定点击事件：
//async修饰:允许函数内部使用 await
//await：等待异步登录流程完成(autoLogin == true的情况)
BtnLogin.onClick.AddListener(async () =>
{
    //点击 BtnLogin 后，系统先播放一次按钮音效
    SoundSystem.PlayOnce("Btn");
    if (autoLogin)
    //说明系统已经有可用的登录信息
    //通常来自：PlayerPrefs["uuid"]PlayerPrefs["token"]
    {
        // 自動登入
        await Canvases.Login();
        //Canvases.Login() 位于：Assets/Scripts/Global/Canvases.cs:470
        //1. 刷新全部道具数据
        //2. 执行 IndexOut()
        //3. 从登录页切换到首页
    }
    else
    {
        // 遊客登入
        Window.CheckOpen(Contract);
        //// 檢查物件開啟，此处打开
        //public static void CheckOpen(GameObject gameObject) {
        //if (gameObject && !gameObject.activeSelf) {
        //  gameObject.SetActive(true);
        //}
        //因此项目中的页面切换很多时候并不是切换 Unity Scene，
        //而是通Window.CheckOpen/CheckClose 控制页面对象的激活状态。
        //Contract 是 IndexCanvas 脚本中的一个静态私有变量
        //它在 Awake() 中通过层级路径获取：
        //Contract = this.transform.Find("Contract").gameObject;
        //隐私条款面板，初始就存在在场景，不是动态加载
    }
    }
});


关于autoLogin标识符的获取：

登录页启用 (调用OnEnable) 后，先执行 Init()：
void OnEnable()
{
    Init();
}

Init函数被async修饰，其中需要检查本地存储的uid是否存在，并异步调用 AutoLogin()函数

在AutoLogin()函数中，会从 PlayerPrefs 取出 token 和 uuid，请求：
POST ApiPath.AutoLoginUrl
真正的异步等待点：
Assets/Scripts/IndexCanvas/IndexCanvas.cs:416
var (ok, result) = await HttpApiUtil.PostUniTask(url, formData);
接口成功后才设置：
CheckLogin(true);
CheckLogin(true) 的实际作用是：
autoLogin = true
BtnLogin 文案改为“进入游戏”

点击了登入按钮后，如果autoLogin = true
执行Canvases.Login
public static async UniTask Login()
{
    ItemSystem.RenewItemAll(); // 需更新全道具
    await IndexOut();//异步执行，中间存在延迟1s执行逻辑
}

// 登入
public static async UniTask IndexOut()
{
    string uuid = PlayerPrefs.GetString("uuid");
    string token = PlayerPrefs.GetString("token");       

    Window.CheckClose(HomeCanvasMask);//关闭首页遮罩
    Window.CheckOpen(HomeCanvas);//打开 HomeCanvas
    await UniTask.Delay(TimeSpan.FromSeconds(1)); // 延遲 1 秒
    animatorCanvases.SetTrigger("IndexOut");
    //触发 IndexOut 动画         仅设置 Animator Trigger
    //当前状态
     │
     └── Any State + IndexOut Trigger
              │
              ▼
          IndexOut
              │ 播放完成
              ▼
          HomeIn
              │ 播放完成
              ▼
          HomeCanvas
              │ 播放完成
              ▼
    //        Stay
    //触发登录页退出动画
      -> 关闭 IndexCanvas
      -> 播放 HomeCanvas 进入动画
      -> 中途显示 Role 和 Monster
      -> 首页动画完成
    // -> 进入稳定状态 Stay
    isOpenWindow = false;
    SoundSystem.PlayLoop("Main"); // 主畫面音樂
    
    GameSocket.I.Connect(uuid, token);
    //GameSocket139
    //建立 WebSocket 长连接的入口。
    //I是单例模式对象
    //检查状态
    //保存 uuid/token
    //拼接 URL
    //创建 WebSocket
    //注册 OnOpen/OnError/OnMessage
    //调用底层连接await ws.Connect();(与单例管理类的connect区分开)
    //有问题处理重连MarkDisconnectedAndScheduleReconnect();
}

GameSocket.Update() 负责：

  DispatchMessageQueue()即把底层 WebSocket 已经收到、暂存在队列里的消息，转交给 Unity 主线程处理。
  检查 ChatSystem 注册
  补发暂存聊天消息
  执行自动重连

  聊天与系统推送

  两者都订阅：

  GameSocket.OnEvent

  但处理策略不同。

  ChatSystem 处理：

  world.chat
  dm.message

  特点：

  可能延迟注册
  持续重试订阅
  缓存聊天消息
  处理 UserID 尚未加载的私聊
  尽量不丢失消息

  GameSocketPushHandler 处理：

  mail.unread.updated
  grid.heat.updated
  grid.topics.updated
  topic.comments.updated

  特点：

  常驻订阅
  按照 HomeCanvas/GridCanvas.activeSelf 过滤
  页面未打开时可跳过处理
  重新打开页面时再拉取最新状态

  因此，聊天是“消息流”模型，系统推送是“状态刷新通知”模型。

  • 它们是两层不同的消息队列：

  NativeWebSocket 内部消息队列
          │
          └── DispatchMessageQueue()
                  │
                  ▼
            OnMessage 回调
                  │
                  ▼
          TryInvokeEvent()
                  │
                  └── 没有业务订阅者
                          ▼
                   _pendingChatEvents

  ## 1. DispatchMessageQueue()

  这是传输层队列。

  服务器消息到达后，先暂存在 NativeWebSocket 内部。每帧调用：

  ws?.DispatchMessageQueue();

  它会把底层消息取出来，触发：

  ws.OnMessage

  然后进入消息解析流程。

  ## 2. _pendingChatEvents

  这是业务层队列。

  当消息已经被 DispatchMessageQueue() 派发出来，但项目当前没有 GameSocket.OnEvent 订阅者时：

  if (count == 0 && isChatEvent)
  {
      _pendingChatEvents.Add((eventName, json));
      return;
  }

  聊天消息会进入：

  _pendingChatEvents

  最多缓存 50 条。

  ## 3. 后续如何补发

  当 ChatSystem 注册成功后，GameSocket.Update() 会把暂存消息重新分发：

  foreach (var (eventName, json) in _pendingChatEvents)
  {
      OnEvent?.Invoke(eventName, json);
  }
  _pendingChatEvents.Clear();

  ## 区别总结

   队列                        所属层        等待什么
  ━━━━━━━━━━━━━━━━━━━━━━━━━━  ━━━━━━━━━━━━  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
   NativeWebSocket 内部队列    网络传输层    等 Unity 主线程调用 DispatchMessageQueue()
  ──────────────────────────  ────────────  ────────────────────────────────────────────
   _pendingChatEvents          业务层        等 ChatSystem 等业务订阅者注册

  所以：

  DispatchMessageQueue()
  = 把网络消息从底层取出来

  _pendingChatEvents
  = 消息取出来后，因暂时没有业务订阅者而继续缓存

  如果不调用 DispatchMessageQueue()，消息不会进入 OnMessage，自然也不会进入 _pendingChatEvents。
