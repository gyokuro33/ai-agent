using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sebastien;

public class login : MonoBehaviour {
 
    public Device device;

    public InputField DeviceIdField; // DeviceID表示用
    public Button CreateTokenButton; // DeviceToken取得ボタン
    public Text messageText; // メッセージ表示用


	void Start () {
        
        // ---------------------------------------------------------------------------- //
        //
        // Device に用意されているコールバック関数の登録（受信側は下部に記載）
        //
        // ---------------------------------------------------------------------------- //
        //Device.setOnIssueId += this.onIssuedId;
        Device.setOnCreateToken += this.onCreateToken;
        Device.setOnUpdateToken += this.onUpdateToken;
        Device.setOnValidateToken += this.onValidateToken;

        // ---------------
        // DO NOT CHANGE
        device.Client_Secret = "6612508e-3c18-4e90-be37-a29b30ea2140";
        // ---------------

        CreateTokenButton.onClick.AddListener(OnClickCreateTokenButton); // DeviceToken取得ボタンにイベントリスナーを登録
        
        bool bDeviceId = PlayerPrefs.HasKey("device_id");
        bool bDeviceToken = PlayerPrefs.HasKey("device_token");


        if (bDeviceId)// DeviceIDが端末に保存済みかどうか
        {
            if (bDeviceToken)// DeviceTokenが端末に保存済みかどうか
            {
                // DeviceTokenが取得済みで端末に保存されていたとき
                messageText.text = "認証中です。しばらくお待ちください。";
                device.validateToken(); // DeviceTokenの認証
            }
            else
            {
                // DeviceIDを登録せずにアプリを終了してしまったとき
                DeviceIdField.text = PlayerPrefs.GetString("device_id");//一度入力したものを表示する

                messageText.text = "UDSで DeviceID を登録後、DeviceToken を取得してください。";

                Debug.Log("端末に保存済み : " + PlayerPrefs.GetString("device_id"));
            }
        }
        else
        {
            // DeviceIDが端末に保存されていないとき
            // 端末に保存されている情報をすべて削除する
            PlayerPrefs.DeleteKey("device_id");
            PlayerPrefs.DeleteKey("device_token");
            PlayerPrefs.DeleteKey("refresh_token");

            messageText.text = "UDSで DeviceID を登録後、DeviceToken を取得してください。";
        }
    }


    // ---------------------------------------------------------------------------- //
    //  DeviceToken取得ボタンを押下した時に呼び出される
    // ---------------------------------------------------------------------------- //
    void OnClickCreateTokenButton()
    {
        if (DeviceIdField.text != "")
        {
            PlayerPrefs.SetString("device_id", DeviceIdField.text);
            PlayerPrefs.Save();

            messageText.text = "DeviceToken を取得しています。しばらくお待ちください。";
            device.createToken();
        }
        else
        {
            messageText.text = "DeviceID を入力してください。";
        }   
    }
	

    // ---------------------------------------------------------------------------- //
    //  mainシーンに遷移した時に呼び出される
    // ---------------------------------------------------------------------------- //
    void OnDestroy()
    {
        //コールバックの解除
        //Device.setOnIssueId -= this.onIssuedId;
        Device.setOnCreateToken -= this.onCreateToken;
        Device.setOnUpdateToken -= this.onUpdateToken;
        Device.setOnValidateToken -= this.onValidateToken;
    }



    // ---------------------------------------------------------------------------- //
    //
    //  Device に用意されている コールバック関数の受信側
    //
    // ---------------------------------------------------------------------------- //
    //
    // ---------------------------------------------------------------------------- //
    //  DeviceIDの取得時に呼ばれるメソッド
    //  
    //  登録例 : Device.setOnIssueId += this.onIssuedId;
    //  引数(string) : DeviceID取得結果
    //                "valid" - 正常
    //                その他の文字列 - エラー
    // ---------------------------------------------------------------------------- //
    void onIssuedId(string result)
    {
        Debug.Log("LoginClass onIssuedId : " + result);

        if (result == "valid")
        {
            DeviceIdField.text = PlayerPrefs.GetString("device_id");
            
            messageText.text = "UDSで DeviceID を登録後、DeviceToken を取得してください。";
        } else {
            messageText.text = "エラーが発生しました。アプリケーションを終了させて再度実行してください。" + "\n" + result;
        }
    }

    // ---------------------------------------------------------------------------- //
    //  DeviceTokenの取得時に呼ばれるメソッド
    //  
    //  登録例 : Device.setOnCreateToken -= this.onCreateToken;
    //  引数(string) : DeviceToken取得結果
    //                "valid" - 正常
    //                "not registered" - DeviceIDがUDSで登録されていなかった時のエラー
    //                "not entered" - DeviceIDが未入力の場合のエラー
    //                "duplicate request" - DeviceIDがすでに一度使用されていた場合のエラー
    //                "JSON parse error" - DeviceIDが不正でJSONのparseに失敗した場合のエラー
    //                その他の文字列 - エラー
    // ---------------------------------------------------------------------------- //
    void onCreateToken(string result)
    {
        Debug.Log("LoginClass onCreateToken : " + result);

        if (result == "valid")
        {
            // mainシーンに遷移する
            SceneManager.LoadSceneAsync("main", LoadSceneMode.Single);
        }
        else if (result == "not registered")
        { 
            messageText.text = "UDSで DeviceID が登録されていません。";
        }
        else if (result == "not entered")//DeviceIDが未入力の場合
        {                       
            messageText.text = "DeviceID を入力してください。";
        }
        else if (result == "duplicate request")//DeviceIDがすでに一度使用されていた場合
        {
            messageText.text = "DeviceID がすでに使用されています。";
        }
        else if (result == "JSON parse error")//DeviceIDが不正でJSONのparseに失敗した場合
        {
            messageText.text = "DeviceID が不正です。";
        }
        else //エラー発生時
        {
            messageText.text = "DeviceTokenの取得に失敗しました。";
        }
    }

    // ---------------------------------------------------------------------------- //
    //  DeviceTokenのupdate時に呼ばれるメソッド
    //  
    //  登録例 : Device.setOnUpdateToken -= this.onUpdateToken;
    //  引数(string) : update結果
    //                "valid" - 正常
    //                その他の文字列 - エラー
    // ---------------------------------------------------------------------------- //
    void onUpdateToken(string result)
    {
        Debug.Log("LoginClass onUpdateToken : " + result);

        if (result == "valid"){
            // mainシーンに遷移する
            SceneManager.LoadSceneAsync("main", LoadSceneMode.Single);
        } 
        else // エラー発生時
        {
            messageText.text = "DeviceTokenのアップデートに失敗しました。";
        }

    }

    // ---------------------------------------------------------------------------- //
    //  DeviceTokenの認証時に呼ばれるメソッド
    //  
    //  登録例 : Device.setOnValidateToken -= this.onValidateToken;
    //  引数(string) : 認証結果
    //                "valid" - 正常
    //                その他の文字列 - エラー
    // ---------------------------------------------------------------------------- //
    void onValidateToken(string result)
    {
        Debug.Log("LoginClass onValidateToken : " + result);

        if (result == "valid")
        {
            // mainシーンに遷移する
            SceneManager.LoadSceneAsync("main", LoadSceneMode.Single);
        }
        else // エラー発生時
        {
            messageText.text = "DeviceTokenの認証に失敗しました。";
        }

    }
}
