
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LuckyWarAPI : ES3Cloud
{
    #region Creating Instance
    private static LuckyWarAPI _instance;
    public LuckyWarAPI(string url, string apiKey) : base(url, apiKey)
    {
    }

    public static LuckyWarAPI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<LuckyWarAPI>();
            return _instance;
        }
    }

    void Awake()
    {
        if (!MatchHandler.IsLuckyWar())
        {
            gameObject.SetActive(false);
            return;
        }
        if (_instance == null)
            _instance = this;

    }
    #endregion

    public enum BetType
    {
        bet,
        tie,
        surrender,
        war,

        lose,
        win
    };

    //public BetType Current;

    #region API Field Names String

    public const string PlayerIncrementedID = "player_id";
    public const string RoomID = "room_id";
    public const string TableName = "table_name";

    public const string SelectedPoint = "selected_point";
    public const string Result = "result";
    public const string BetAmount = "bet_amount";

    #endregion

    private void Start()
    {
        //LuckyWarSendBet(BetType.bet, "2000", BetType.win);
    }

    #region Callback Functions
    public void LuckyWarSendBet(BetType selectedPt, string betAmount, BetType Result)
    {
        string roomID = LocalSettings.GetSetRoomID;
        string incrementedID = LocalSettings.GetIncrementedPlayerID().ToString();
        string tableName = LocalSettings.GetSetTableName;

        string selectedPoint = selectedPt.ToString();
        string ResultString = Result.ToString();
        StartCoroutine(SendLWBetToServerAPI(incrementedID, roomID, tableName, selectedPoint, betAmount, ResultString));
    }

    public IEnumerator SendLWBetToServerAPI(string incrementedID, string roomID, string tableName, string selPoint, string betAmount, string winPoint)
    {
        string url = APIStrings.LuckyWarBetSendURLAPI;
        formData = new List<KeyValuePair<string, string>>();
        AddPOSTField(PlayerIncrementedID, incrementedID);
        AddPOSTField(RoomID, roomID);
        AddPOSTField(TableName, tableName);
        AddPOSTField(SelectedPoint, selPoint);
        AddPOSTField(BetAmount, betAmount);
        AddPOSTField(Result, winPoint);
        WWWForm form = CreateWWWForm();

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
                Debug.LogError("Lucky War bet sent successfully");
            }
        }
    }
    #endregion


}
