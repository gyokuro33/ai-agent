using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Sebastien;

public class putTextManager : MonoBehaviour {
    public Speak speak;
    public InputField inputField;
    public Button sendButton;


    void Start()
    {
        sendButton.onClick.AddListener(OnSendButtonClick);// 送信ボタンにイベントリスナーを登録
    }

    // ---------------------------------------------------------------------------- //
    //  送信ボタン押下時に呼ばれるメソッド
    // ---------------------------------------------------------------------------- //
    public void OnSendButtonClick(){
		if (inputField.text == "") {
			return;
		}

        string mes = inputField.text.Replace("\n", "");
        scrollViewManager.Log(mes);

        //speak.putText (mes);
        string meta = "{\"clientData\":{\"deviceInfo\":{\"playTTS\":\"on\"}},\"clientVer\":\"0.5.1\",\"language\":\"ja-JP\",\"voiceText\":\"" + mes + "\"}";
        //Debug.Log("putTextManager meta : " + meta);
        speak.putMeta(meta);
        inputField.text = "";
	}

    // ---------------------------------------------------------------------------- //
	//  Enterキー押下時に送信ボタンを押下した時と同等の処理を行う
    // ---------------------------------------------------------------------------- //
	public void AddMessage(string message)
    {
        
        if ( message.IndexOf("\n", System.StringComparison.Ordinal) == -1) {
            return;
		}

		message = message.Trim ();
		
        if (message.Length > 0) {
			OnSendButtonClick ();
		}

		inputField.text = "";
		inputField.ActivateInputField ();
	}
}
