restfulapi学习笔记
天灯祈福相关api链路分析

以庙宇列表为例
地址配置在 Assets/Scripts/Global/ApiPath.cs:159
public static string ListTempleUrl => Env.ServerUrl + "/api/play/list_temple"; // 廟宇列表

Assets/Scripts/Global/APISystem.cs:264 构造 token、uuid、AES 加密后的 apidata，并解析回包。
public static async UniTask ListTemple() {
    AlertUtil.ShowLoadingModal();
    RenewListTemple = true;

    try {
        WWWForm formData = new WWWForm();
        formData.AddField("token", PlayerPrefs.GetString("token"));
        formData.AddField("uuid", PlayerPrefs.GetString("uuid"));

        JObject requestData = new JObject();
        string apidata = AESCryptoTool.Encrypt(
            requestData.ToString(),
            PlayerPrefs.GetString("uuid").Replace("-", ""),
            Env.AESiv
        );
        formData.AddField("apidata", apidata);

        // 發送請求
        string url = ApiPath.ListTempleUrl;
        Canvases.ShowDebugLog("廟宇列表-url", url);
        var (ok, result) = await HttpApiUtil.PostUniTask(url, formData);

        if (!ok) {
            Debug.LogError("|Error|廟宇列表-回傳失敗: " + result);
            AlertUtil.ShowModal(result);
            return;
        }

        // 回傳結果處理
        try {
            JObject obj = JObject.Parse(result);
            string data = (string)obj["data"];
            string decrypted = Canvases.ShowAPILog("廟宇列表-回傳", data);

            JArray_play_lantern[1] = JArray.Parse(decrypted);
        }
        catch (Exception ex) {
            Debug.LogError("|Error|廟宇列表-回傳無法解析: " + ex.Message);
            Canvases.ShowDebugLog("廟宇列表-Exception", ex);
        }
    }
    catch (Exception ex) {
        Debug.LogError("|Error|廟宇列表-例外: " + ex);
        AlertUtil.ShowModal("出現錯誤，請稍後重試");
    }
    finally {
        RenewListTemple = false;
        AlertUtil.CloseLoadingModal();
    }
}

Assets/Scripts/Global/Util.cs:1016 统一检测网络、设置 10 秒超时、处理取消、验证外层 status == 200。
public static async UniTask<(bool isSuccess, string response)> PostUniTask(
        string url,
        WWWForm formData,
        CancellationToken cancellationToken = default) {
    // 檢查網路是否可用
    if (Application.internetReachability == NetworkReachability.NotReachable) {
        Debug.LogWarning("無法連線：請檢查網路");
        if (!Canvases.WindowConfirm2) {
            AlertUtil.ShowModal("無法連線：請檢查網路");
        }
        return (false, "無法連線");
    }

    using (UnityWebRequest www = UnityWebRequest.Post(url, formData)) {
        www.timeout = 10; // 設定 10 秒逾時（可調整）
        //await www.SendWebRequest().ToUniTask();
        try {
            await www.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException) {
            Debug.Log($"請求已取消：{url}");
            return (false, "請求已取消");
        }

        // 網路層錯誤（DNS錯誤、連線逾時、無伺服器回應等）
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError($"連線錯誤：{url} | {www.error}");

            if (Canvases.GridCanvas != null && Canvases.GridCanvas.activeSelf) {
                Canvases.GridOut();
            }

            return (false, www.error);
        }

        string response = www.downloadHandler.text;
        if (string.IsNullOrEmpty(response)) {
            return (false, "伺服器回應為空");
        }

        try {
            JObject json = JObject.Parse(response);

            // 檢查必要欄位
            if (json["status"] == null) {
                Debug.LogError($"回應缺少 status 欄位：{response}");
                return (false, "伺服器回應格式錯誤");
            }

            int status = (int)json["status"];
            string message = json["message"]?.ToString() ?? "未知錯誤";

            if (status == 200) {
                return (true, response);
            }
            else {
                Debug.LogError($"|Error|API 返回錯誤：{status} - {message}");
                return (false, message);
            }
        }
        catch (System.Exception e) {
            Debug.LogError($"|Error|JSON 解析失敗：{response}\n錯誤：{e.Message}");
            return (false, "伺服器回應格式錯誤");
        }
    }
}

Assets/Scripts/Play/Lantern/LanternSystem_Steps.cs:771 串行等待接口完成，再由 ShowTempleAsync 将 JArray 转成 TempleData 和 UI。
private async UniTask RefreshTempleData()
{

    APISystem.RenewListTemple = false;
    APISystem.JArray_play_lantern[1] = null;

    await APISystem.ListTemple();
    await ShowTempleAsync(); 


    Canvases.ShowDebugLog("廟宇資料重新取得數量", temples_list.Count);
    InputField searchInput = WindowStep7.transform.Find("InputField").GetComponent<InputField>();
    searchInput.text = "";

    RefreshTempleListUI(temples_list);

    isRefreshing = false;
    Canvases.ShowDebugLog("下拉刷新完成", null);
}

 // 超时时间
 private const int API_TIMEOUT_MS = 30000; // 30秒超时
 /// <summary>
 /// 庙宇列表
 /// </summary>
 private async UniTask ShowTempleAsync()
 {
     // 等数据
     bool isTimeout = await UniTask.WaitWhile(
         () => APISystem.RenewListTemple,
         cancellationToken: default
     ).TimeoutWithoutException(TimeSpan.FromMilliseconds(API_TIMEOUT_MS));

     if (isTimeout)
     {
         Canvases.ShowDebugLog("廟宇列表加載超時", null);
         ShowNetworkErrorDialog("加載超時", "廟宇列表加載超時，請檢查網絡連接後重試。");
         return;
     }
     
     temples_list.Clear();
     
     if (APISystem.JArray_play_lantern[1] != null && APISystem.JArray_play_lantern[1].Count > 0)
     {
         Canvases.ShowDebugLog("廟宇列表刷新中", null);
         foreach (JObject item in APISystem.JArray_play_lantern[1])
         {
             temples_list.Add(new TempleData
             {
                 id = (int)item["id"],
                 sales_id = (string)item["sales_id"],
                 name = (string)item["name"],
                 img = Resources.Load<GameObject>((string)item["img"]),
                 introduction = string.IsNullOrEmpty((string)item["introduction"]) ? "暫無描述..." : (string)item["introduction"],
                 is_direction = (int)item["is_direction"],
                 status = (int)item["status"],
                 longitude = (string)item["longitude"],
                 latitude = (string)item["latitude"],
                 deleted_at = (string)item["deleted_at"],
                 created_at = (string)item["created_at"],
                 updated_at = (string)item["updated_at"],
             });
         }
     }
     else
     {
         Canvases.ShowDebugLog("廟宇列表加載失敗", null);
         //ShowNetworkErrorDialog("廟宇列表加載失敗", "無法獲取廟宇列表數據，請檢查網絡連接。");
     }
 }

JArray_play_lantern 是一个“数组里装 JSON 数组”的结构：

  public static JArray[] JArray_play_lantern;

  含义分三层：

  - JArray_play_lantern：C# 数组
  - JArray_play_lantern[1]：数组第 2 个位置，类型是 JArray
  - JArray_play_lantern[1][0]：该 JSON 数组的第 1 个 JSON 元素，通常
    是一个 JObject

  项目初始化为：

  JArray_play_lantern = new JArray[2];

  约定是：

  JArray_play_lantern[0] // 天灯模型列表
  JArray_play_lantern[1] // 庙宇列表

  所以你的判断：

  if (APISystem.JArray_play_lantern[1] != null
      && APISystem.JArray_play_lantern[1].Count > 0)

  意思是：

  1. 庙宇列表对象已被 API 创建，而不是 null
  2. 庙宇列表至少有一笔数据

  这里的 .Count 不是单个元素的属性，而是 JArray 本身的元素数量。


  [
    { "id": 1, "name": "龙山寺" },
  ]

  那么：

  单个元素若是 JObject，其字段应通过键取得，例如：

  JObject temple = (JObject)APISystem.JArray_play_lantern[1][0];

  int id = (int)temple["id"];
  string name = (string)temple["name"];

  它和 JArray.Count 完全不同：前者是服务端数据字段，后者是列表长度。
  
• 相关数据定义和赋值位置如下。

  - 声明：Assets/Scripts/Global/APISystem.cs:104

  public static JArray[] JArray_play_lantern;

  - 初始化为长度 2 的数组：Assets/Scripts/Global/APISystem.cs:117

  JArray_play_lantern = new JArray[2];

  - JArray_play_lantern[0] 的赋值位置：Assets/Scripts/Global/
    APISystem.cs:244

  JArray_play_lantern[0] = DataPlay;

  它来自 PlayModelList("play_lantern")，表示天灯模型列表。

  - JArray_play_lantern[1] 的赋值位置：Assets/Scripts/Global/
    APISystem.cs:298

  JArray_play_lantern[1] = JArray.Parse(decrypted);

  它来自 ListTemple()，表示庙宇列表。

  - 你询问的非空、非空列表判断：Assets/Scripts/Play/Lantern/
    LanternSystem_API.cs:34

  if (APISystem.JArray_play_lantern[1] != null
      && APISystem.JArray_play_lantern[1].Count > 0)

  - 遍历庙宇列表的单个元素：Assets/Scripts/Play/Lantern/
    LanternSystem_API.cs:37

  foreach (JObject item in APISystem.JArray_play_lantern[1])

  其中 item 是一座庙宇的 JSON 对象；item["id"]、item["name"] 等字段被
  映射到 TempleData，从 Assets/Scripts/Play/Lantern/
  LanternSystem_API.cs:41 开始。

