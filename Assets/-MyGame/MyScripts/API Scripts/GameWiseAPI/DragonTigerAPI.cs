using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class DragonTigerAPI : ES3Cloud
{
    #region Creating Instance
    private static DragonTigerAPI _instance;

    public DragonTigerAPI(string url, string apiKey) : base(url, apiKey)
    {
    }

    public static DragonTigerAPI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<DragonTigerAPI>();
            return _instance;
        }
    }

    void Awake()
    {
        if (!MatchHandler.isDragonTiger())
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
    public void DTSendBet(string selectedPoint, string betAmount, string winPoint)
    {
        string roomID = LocalSettings.GetSetRoomID;
        string incrementedID = LocalSettings.GetIncrementedPlayerID().ToString();
        string tableName = LocalSettings.GetSetTableName;

      

        StartCoroutine(SendPlayerPlayerBetToServerAPI(incrementedID, roomID, tableName, selectedPoint, betAmount, winPoint));
    }

    public IEnumerator SendPlayerPlayerBetToServerAPI(string incrementedID, string roomID, string tableName, string selPoint, string betAmount, string winPoint)
    {
        string url = APIStrings.DragonTigerURLAPI;
        formData = new List<KeyValuePair<string, string>>();

        AddPOSTField(PlayerIncrementedID, incrementedID);
        AddPOSTField(RoomID, roomID);
        AddPOSTField(TableName, tableName);
        AddPOSTField(SelectedPoint, selPoint);
        AddPOSTField(BetAmount, betAmount);
        AddPOSTField(WinningPoint, winPoint);
        WWWForm form = CreateWWWForm();



        Debug.Log("Check Incremented Id:...." + incrementedID + "  Check RoomId   " + roomID + "     checkableName....." + tableName + "   check Selelct point....." + selPoint + "   Check BetAmount....." + betAmount +  "     Check winingNumber.... " + winPoint);
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
