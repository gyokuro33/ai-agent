using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using Sebastien;


// ----------------------------------------
// AgentCraftとの連携例
// ----------------------------------------
//Json定義
[System.Serializable]
public class onMetaOutJson
{
    public string speaker;
    public SystemText systemText;
    public string version;
    public string type;
    public Option option;


    public string JSON_test1; // ←　AgentCraftで定義したコマンド
    public string JSON_test2; // ←　AgentCraftで定義したコマンド
    public string JSON_test3; // ←　AgentCraftで定義したコマンド
    public string emotion;

    [System.Serializable]
    public class SystemText
    {
        public string utterance;
        public string expression;
    }

    [System.Serializable]
    public class Option
    {
    }

    public static onMetaOutJson CreateFromJSON(string json)
    {
        return JsonUtility.FromJson<onMetaOutJson>(json);
    }

    public string ToJSON(string json)
    {
        return JsonUtility.ToJson(this);
    }
}
// ----------------------------------------


public class example : MonoBehaviour {

    public Speak speak;
    public Button logoutButton;
    public Button micButton;


    // マイクボタン制御用
    private EventTrigger eventTrigger;
    private EventTrigger.Entry entry1;
    private EventTrigger.Entry entry2;
    
    //表情のアニメを入れる
    public AnimationClip[] faceAnime;
    public AnimationClip[] bodyAnime;

    //Animatorコンポーネントを入れる
    Animator anim;

    private bool isLoadLoginScene = false;

    void Awake()
    {
        // ---------------------------------------------------------------------------- //
        // SDKの設定
        //
        // 最優先で実行される必要があるため、example.cs に対して
        // DefaultExcutionOrder を マイナスの値（例：-100）に設定する必要がある。
        //
        // Unityのメニュー　[Edit] - [Project Settings] - [Script Excution Order] で設定する
        // ---------------------------------------------------------------------------- //
        speak.Host = "spf-v2.sebastien.ai"; // SDK接続先のホスト名を設定する
        speak.Port = "443";              // SDK接続先サーバーのポート番号を設定する
        speak.UrlPath = "/talk";         // SDK接続先のURL Pathを設定する
        speak.AccessToken = PlayerPrefs.GetString("device_token"); // UDSから取得した DeviceToken を設定する
        speak.UseSSL = true;             // SSL通信を使用するか否かを設定する
        speak.MicMute = true;           // スタート時にマイクをmute状態にするか否かを設定する


        // ---------------------------------------------------------------------------- //
        // DeviceID・AccessTokenが取得済みかどうかチェックする
        // ---------------------------------------------------------------------------- //
        bool bDeviceId = PlayerPrefs.HasKey("device_id");
        bool bDeviceToken = PlayerPrefs.HasKey("device_token");

        if (bDeviceId)// DeviceIDが端末に保存済みかどうか
        {
            Debug.Log("ExampleClass DeviceID : " + PlayerPrefs.GetString("device_id"));// DeviceIDをコンソールに表示する

            if (!bDeviceToken)// AccessTokenが端末に保存済みかどうか
            {
                // loginシーンに遷移する
                SceneManager.LoadSceneAsync("login", LoadSceneMode.Single);
            }
        }
        else
        {
            // loginシーンに遷移する
            SceneManager.LoadSceneAsync("login", LoadSceneMode.Single);
        }
    }


    void Start () {
        // ---------------------------------------------------------------------------- //
        //
        // Speak に用意されているコールバック関数の登録（受信側は下部に記載）
        //
        // ---------------------------------------------------------------------------- //
        Speak.setOnTextOut += this.onTextOut;
        Speak.setOnMetaOut += this.onMetaOut;
        Speak.setOnPlayStart += this.onPlayStart;
        Speak.setOnPlayEnd += this.onPlayEnd;

        Speak.setOnVoiceTextOut += this.onVoiceTextOut;
        Speak.setOnSystemTextOut += this.onSystemTextOut;

        Speak.setOnErrorTextOut += this.onErrorTextOut;

        Speak.setOnSDKStart += this.onSDKStart;
        Speak.setOnSDKStop += this.onSDKStop;


        // マイクボタン制御
        // EventTriggerコンポーネントを取り付ける
        eventTrigger = micButton.gameObject.AddComponent<EventTrigger>();
        // マイクボタンを押下した時のイベントリスナー登録（ラムダ式で設定）
        entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerDown;
        entry1.callback.AddListener(data => OnMicButtonPointerDown((BaseEventData)data));
        eventTrigger.triggers.Add(entry1);
        // マイクボタンを離した時のイベントリスナー登録（ラムダ式で設定）
        entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerUp;
        entry2.callback.AddListener(data => OnMicButtonPointerUp((BaseEventData)data));
        eventTrigger.triggers.Add(entry2);


        // ログアウトボタンを押下した時のイベントリスナーを登録
        logoutButton.onClick.AddListener(OnLogoutButtonClick);

        //Animatorコンポーネントを取得
        anim = GetComponent<Animator>();
        anim.SetLayerWeight(1, 1f);

        // SDKのスタート
        speak.start();
        speak.cancelPlay();//音声再生をキャンセルする
        speak.unmute();
    }

    // ---------------------------------------------------------------------------- //
    //  マイクボタンを押下した時に呼び出される
    // ---------------------------------------------------------------------------- //
    void OnMicButtonPointerDown(BaseEventData data)
    {
        speak.cancelPlay();//音声再生をキャンセルする
        speak.unmute();
    }

    // ---------------------------------------------------------------------------- //
    //  マイクボタンを離した時に呼び出される
    // ---------------------------------------------------------------------------- //
    void OnMicButtonPointerUp(BaseEventData data)
    {
        speak.mute(1.0f);
    }

    // ---------------------------------------------------------------------------- //
    //  ログアウトボタンを押下した時に呼び出される
    // ---------------------------------------------------------------------------- //
    void OnLogoutButtonClick()
    {
        PlayerPrefs.DeleteKey("device_id");
        PlayerPrefs.DeleteKey("device_token");
        PlayerPrefs.DeleteKey("refresh_token");
        // loginシーンに遷移する
        SceneManager.LoadScene("login", LoadSceneMode.Single);
        // 再起動を促すメッセージ表示
        // logoutCanvas.SetActive(true);
    }

    // ---------------------------------------------------------------------------- //
    //  loginシーンに遷移した時に呼び出される
    // ---------------------------------------------------------------------------- //
    void OnDestroy()
    {
        // コールバックの解除
        Speak.setOnTextOut -= this.onTextOut;
        Speak.setOnMetaOut -= this.onMetaOut;
        Speak.setOnPlayStart -= this.onPlayStart;
        Speak.setOnPlayEnd -= this.onPlayEnd;

        Speak.setOnVoiceTextOut -= this.onVoiceTextOut;
        Speak.setOnSystemTextOut -= this.onSystemTextOut;

        Speak.setOnErrorTextOut -= this.onErrorTextOut;

        Speak.setOnSDKStart -= this.onSDKStart;
        Speak.setOnSDKStop -= this.onSDKStop;


        // スクロールビューのテキストをクリアする
        scrollViewManager.ClearText();
    }



    // ---------------------------------------------------------------------------- //
    //
    //  Speak に用意されているコールバック関数の受信側
    //
    // ---------------------------------------------------------------------------- //
    //
    // ---------------------------------------------------------------------------- //
    //  対話テキストを受信した時に呼ばれるメソッド
    //  
    //  登録例 : Speak.setOnTextOut += this.onTextOut;
    //  引数(string) : JSON形式の対話テキスト情報
    // ---------------------------------------------------------------------------- //
    void onTextOut(string txt)
    {
        Debug.Log("ExampleClass onTextOut : " + txt);
    }

    // ---------------------------------------------------------------------------- //
    //  メタ情報を受信した時に呼ばれるメソッド
    //  
    //  登録例 : Speak.setOnMetaOut += this.onMetaOut;
    //  引数(string) : JSON形式のメタ情報
    // ---------------------------------------------------------------------------- //
    void onMetaOut(string txt)
    {
        Debug.Log("ExampleClass onMetaOut : " + txt);

        // ----------------------------------------
        // AgentCraftとの連携例
        // ----------------------------------------
        //Jsonをパース
        var metaData = JsonUtility.FromJson<onMetaOutJson>(txt.ToString());

        if (metaData.emotion == "neutral")
        {
            //何らかの処理を記述
            Debug.Log("JSON_test1 : " + metaData.emotion);
            anim.Play(faceAnime[0].name);
            anim.Play(bodyAnime[0].name);
        }
        if (metaData.emotion == "happy")
        {
            //何らかの処理を記述
            Debug.Log("JSON_test2 : " + metaData.emotion);
            anim.Play(faceAnime[1].name);
            anim.Play(bodyAnime[1].name);
        }
        if (metaData.emotion == "surprise")
        {
            //何らかの処理を記述
            Debug.Log("JSON_test3 : " + metaData.JSON_test3);
            anim.Play(faceAnime[2].name);
            anim.Play(bodyAnime[2].name);
        }
        if (metaData.emotion == "sad")
        {
            //何らかの処理を記述
            Debug.Log("JSON_test3 : " + metaData.JSON_test3);
            anim.Play(faceAnime[3].name);
            anim.Play(bodyAnime[3].name);
        }
        // ----------------------------------------
    }

    // ---------------------------------------------------------------------------- //
    //  合成音声の再生が始まる時に呼ばれるメソッド
    //
    //  登録例 : Speak.setOnPlayStart += this.onPlayStart;
    //  引数(string) : "play-start"
    // ---------------------------------------------------------------------------- //
    void onPlayStart(string txt){
        Debug.Log("ExampleClass onPlayStart : " + txt);

        speak.mute();// マイクをミュートする
    }

    // ---------------------------------------------------------------------------- //
    //  合成音声の再生が終わる時に呼ばれるメソッド
    //
    //  登録例 : Speak.setOnPlayEnd += this.onPlayEnd;
    //  引数(string) : "play-end"
    // ---------------------------------------------------------------------------- //
    void onPlayEnd(string txt)
    {
        Debug.Log("ExampleClass onPlayEnd : " + txt);
        anim.Play(faceAnime[0].name);
        speak.unmute();
    }

    // ---------------------------------------------------------------------------- //
    //  ユーザ発話テキストを受信した時に呼ばれるメソッド
    //  
    //  登録例 : Speak.setOnVoiceTextOut += this.onVoiceTextOut;
    //  引数(string) : ユーザ発話文字列(音声認識結果)
    // ---------------------------------------------------------------------------- //
    void onVoiceTextOut(string txt){
        Debug.Log("ExampleClass onVoiceTextOut : " + txt);

        // スクロールビューにテキストを表示する
        scrollViewManager.Log(txt);
    }

    // ---------------------------------------------------------------------------- //
    //  システム発話テキストを受信した時に呼ばれるメソッド
    //  
    //  登録例 : Speak.setOnSystemTextOut += this.onSystemTextOut;
    //  引数(string) : システム発話文字列(エージェントの発話内容)
    // ---------------------------------------------------------------------------- //
    void onSystemTextOut(string txt){
        Debug.Log("ExampleClass onSystemTextOut : " + txt);

        // スクロールビューにテキストを表示する
        scrollViewManager.Log(txt);
    }

    // ---------------------------------------------------------------------------- //
    //  エラーテキストを受信した時に呼ばれるメソッド
    //
    //  登録例 : Speak.setOnErrorTextOut += this.onErrorTextOut;
    //  引数(string) : エラー内容の文字列
    // ---------------------------------------------------------------------------- //
    void onErrorTextOut(string txt)
    {
        Debug.Log("ExampleClass onErrorTextOut : " + txt );

        if (txt == "WebSocket Close Message: UDS:Invalid Signature" ||
            txt == "WebSocket Close Message: UDS:Authentication Failed" ||
            txt == "WebSocket Close Message: UDS:Signature Expired" ||
            txt == "Unauthorized")
        {
            isLoadLoginScene = true;
            // loginシーンに遷移する
            SceneManager.LoadSceneAsync("login", LoadSceneMode.Single);
        }
        else if (txt == "WebSocket Close Message: An exception has occurred while connecting.")
        {
            // 起動時にネットワークトラブルなどでサーバに繋がらなかった場合ここにくる
        }
        else if (txt == "WebSocket Close Message: ppserver.hpp:392: Heartbeat timeout")
        {
            // WiFi接続が途切れた場合ここにくる
        }
        else if (txt == "WebSocket Close Message: ")
        {
            // this.stop()を手動で動かした場合はここにくる
        }
    }

    // ---------------------------------------------------------------------------- //
    //  SDKをスタートした時に呼ばれるメソッド
    //
    //  登録例 : Speak.setOnSDKStart += this.onSDKStart;
    //  引数(string) : "start"
    // ---------------------------------------------------------------------------- //
    void onSDKStart(string txt)
    {
        Debug.Log("ExampleClass onSDKStart : " + txt);
    }

    // ---------------------------------------------------------------------------- //
    //  SDKをストップした時に呼ばれるメソッド
    //
    //  登録例 : Speak.setOnSDKStop+= this.onSDKStop;
    //  引数(string) : "stop"
    // ---------------------------------------------------------------------------- //
    void onSDKStop(string txt)
    {
        Debug.Log("ExampleClass onSDKStop : " + txt);

        if (!isLoadLoginScene)
        {
            speak.start();// SDKを再スタートする
        }
    }
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.K))
        {
            speak.cancelPlay();//音声再生をキャンセルする
            speak.unmute();
            //anim.Play(faceAnime[1].name);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            //speak.mute(1.0f);
            //anim.Play(faceAnime[2].name);
        }
    }

}
