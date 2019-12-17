using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;

public class scrollViewManager : MonoBehaviour {
    // ScrollViewに表示するログ
    public static string Logs = "";
    // ログの差分を取得するための入れ物
    private string oldLogs = "";
    // ScrollViewのScrollRect
    private ScrollRect scrollRect;
    // ScrollViewのText
    private Text textLog;


	void Start () {
        scrollRect = this.gameObject.GetComponent<ScrollRect>();
        textLog = scrollRect.content.GetComponentInChildren<Text>();
	}
	

    void Update()
    {
        // LogsとoldLogsが異なるときにTextを更新
        if (scrollRect != null && Logs != oldLogs)
        {
            textLog.text = Logs;
            // Textが追加されたとき 5 フレーム後に自動でScrollViewのBottomに移動する
            StartCoroutine(DelayMethod(5, () =>
            {
                scrollRect.verticalNormalizedPosition = 0;
            }));

            oldLogs = Logs;
        }

    }

    // ---------------------------------------------------------------------------- //
    // ログを表示するメソッド - public関数のため外部から使用可能
    // ---------------------------------------------------------------------------- //
    public static void Log(string logText)
    {
        Logs += (logText + "\n");
        //Debug.Log(logText);
    }

    // ---------------------------------------------------------------------------- //
    // ログをクリアするメソッド - public関数のため外部から使用可能
    // ---------------------------------------------------------------------------- //
    public static void ClearText()
    {
        Logs = "";
    }

    // ---------------------------------------------------------------------------- //
    // 指定したフレーム数後にActionが実行される
    // ---------------------------------------------------------------------------- //
    IEnumerator DelayMethod(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }
}
