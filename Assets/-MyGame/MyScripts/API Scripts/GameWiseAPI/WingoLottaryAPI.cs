using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WingoLottaryAPI : ES3Cloud
{
    #region Creating Instance
    private static WingoLottaryAPI _instance;

    public WingoLottaryAPI(string url, string apiKey) : base(url, apiKey)
    {
    }

    public static WingoLottaryAPI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<WingoLottaryAPI>();
            return _instance;
        }
    }

    void Awake()
    {
        if (!MatchHandler.isWingoLottary())
        {
            gameObject.SetActive(false);
            return;
        }
        if (_instance == null)
            _instance = this;

    }
    #endregion


    #region API Field Names String

    public const string PlayerIncrementedID = "player_id";
    public const string RoomID = "room_id";
    public const string TableName = "table_name";

    public const string SelectedPoint = "selected_point";
    public const string WinningPoint = "winning_point";
    public const string BetAmount = "bet_amount";

    #endregion


    #region Callback Functions
    public void WingoLottarySendBet(string selectedPoint, string betAmount, string winPoint)
    {
        string roomID = LocalSettings.GetSetRoomID;
        string incrementedID = LocalSettings.GetIncrementedPlayerID().ToString();
        string tableName = LocalSettings.GetSetTableName;

        StartCoroutine(WingoLottaryBetSendAPI(incrementedID, roomID, tableName, selectedPoint, betAmount, winPoint));
    }

    IEnumerator WingoLottaryBetSendAPI(string incrementedID, string roomID, string tableName, string selPoint, string betAmount, string winPoint)
    {
        string url = APIStrings.WingoLottaryBetSendURLAPI;
        formData = new List<KeyValuePair<string, string>>();

        AddPOSTField(PlayerIncrementedID, incrementedID);
        AddPOSTField(RoomID, roomID);
        AddPOSTField(TableName, tableName);
        AddPOSTField(SelectedPoint, selPoint);
        AddPOSTField(BetAmount, betAmount);
        AddPOSTField(WinningPoint, winPoint);
        WWWForm form = CreateWWWForm();

        Debug.Log("Incremented Id:...." + incrementedID + "  RoomId   " + roomID + "     TableName....." + tableName + "   Selelct point....." + selPoint + "   BetAmount....." + betAmount + "     winingNumber.... " + winPoint);
        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 20;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);

            int responseCode = (int)webRequest.responseCode;
            string responseText = webRequest.downloadHandler.text;
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Result: " + webRequest.result);
                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    Debug.LogError("error Type:" + webRequest.result + ",  " + ": not sent : error code: " + responseCode + "\nError Detail: " + responseText);
                }
            }
            else
            {
                Debug.Log("Dragon Tiger bet sent successfully");
            }
        }
    }
    #endregion
}
