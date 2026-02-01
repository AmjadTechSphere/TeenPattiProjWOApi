using com.mani.muzamil.amjad;
using Newtonsoft.Json;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GoldWinLoose : ES3Cloud
{
    public enum Trans
    {
        win,
        lose,
        bet,
        gift,
        tip
    };
    public Trans trans;
    #region API URLs

    public const string RoomID = "room_id";
    public const string TokenID = "token_id";
    public const string GameName = "game_name";
    public const string TableName = "table_name";
    public const string TrasactionType = "transaction";
    public const string Chips = "chips";

    #endregion

    // instance Creating of Gold Transfer History Script
    #region Creating Instance;
    private static GoldWinLoose _instance;
    public static GoldWinLoose Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GoldWinLoose>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    protected GoldWinLoose(string url, string apiKey) : base(url, apiKey)
    {

    }
    #endregion


    #region Callback Functions
    public void SendGold(Trans transType, string chipsAmount)
    {
        string roomID = LocalSettings.GetSetRoomID;
        string gameName = LocalSettings.GetSetGameName;
        string tableName = LocalSettings.GetSetTableName;

        StartCoroutine(SendPlayerDataToAPIToCreatePlayer(roomID, gameName, tableName, transType, chipsAmount));
    }
    public void SendGold(string roomID, string gameName, string tableName, Trans transType, string chipsAmount)
    {

        StartCoroutine(SendPlayerDataToAPIToCreatePlayer(roomID, gameName, tableName, transType, chipsAmount));
    }
    public IEnumerator SendPlayerDataToAPIToCreatePlayer(string roomID, string gameName, string tableName, Trans transType, string chipsAmount)
    {
        string url = APIStrings.SendingWinLooseGold;
        formData = new List<KeyValuePair<string, string>>();

        AddPOSTField(RoomID, roomID);
        AddPOSTField(GameName, gameName);
        AddPOSTField(TokenID, PlayerTokenID());
        AddPOSTField(TableName, tableName);
        AddPOSTField(TrasactionType, transType.ToString());
        AddPOSTField(Chips, chipsAmount);
        //Debug.LogError("Room ID: " + roomID + "\ngame Name: " + gameName + "\nTable Name: " + tableName + "\nTransaction Type: " + transType.ToString() + "\nAmound: " + chipsAmount);
        WWWForm form = CreateWWWForm();

        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 25;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);

            int responseCode = (int)webRequest.responseCode;
            string responseText = webRequest.downloadHandler.text;
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Result: " + webRequest.result);
                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    Debug.LogError("error Type:" + webRequest.result + ",  " + transType + ": not sent : error code: " + responseCode + "\nError Detail: " + responseText);
                }
            }
            else
            {
                //Debug.LogError(transType.ToString() + "chips Added/subtracted");
                if (LocalSettings.IsMenuScene())
                {
                    Debug.Log("Updating player chips");
                    RestAPI.Instance.FetchData(LocalSettings.GetTokenID(), Menu_Manager.Instance.SetUserNameAndOtherThings);
                }
                else
                {
                    UpdatePlayerChipOnServerSide();
                }
            }
        }
    }

    #endregion

    #region ChipsUpdatedRecord

    PlayerChips updatePlayerChips;


    public void UpdatePlayerChipOnServerSide()
    {
        StartCoroutine(GetPlayerUpdatechipsCorouantine(LocalSettings.GetPlayerID().ToString()));
    }

    IEnumerator GetPlayerUpdatechipsCorouantine(string playerId)
    {
        //GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        string url = APIStrings.PlayerChipsUpdate + playerId;
        updatePlayerChips = new PlayerChips();
        using (var webRequest = UnityWebRequest.Get(url))
        {
            //  GoldTransfer.Instance.LoadingPanel.SetActive(true);
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error is: " + webRequest.result);
            }
            else
            {
                Debug.Log("success is: " + webRequest.result);
            }
            string jsonString = webRequest.downloadHandler.text;
            //Debug.LogError(jsonString);
            //  GoldTransfer.Instance.LoadingPanel.SetActive(false);

            updatePlayerChips = JsonConvert.DeserializeObject<PlayerChips>(jsonString);
            //  GoldTransfer.Instance.LoadingPanel.SetActive(false);

            //Debug.LogError("playerChips Status...." + updatePlayerChips.success + "...Get Total Chips...." + updatePlayerChips.total_chips);

        }
    }

    public void GetplayerGoldAndVIPStatusDate(string playerId, Action<PlayerChips> FunctionName)
    {
        StartCoroutine(GetPlayerUpdatechipsCorouantine(playerId, FunctionName));
    }
    IEnumerator GetPlayerUpdatechipsCorouantine(string playerId, Action<PlayerChips> FunctionName)
    {
        //GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        string url = APIStrings.PlayerChipsUpdate + playerId;
        updatePlayerChips = new PlayerChips();
        using (var webRequest = UnityWebRequest.Get(url))
        {
            //  GoldTransfer.Instance.LoadingPanel.SetActive(true);
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error is: " + webRequest.result);
            }
            else
            {
                Debug.Log("success is: " + webRequest.result);
            }
            string jsonString = webRequest.downloadHandler.text;
            Debug.Log(jsonString);
            //  GoldTransfer.Instance.LoadingPanel.SetActive(false);

            updatePlayerChips = JsonConvert.DeserializeObject<PlayerChips>(jsonString);
            //  GoldTransfer.Instance.LoadingPanel.SetActive(false);
            FunctionName?.Invoke(updatePlayerChips);
            Debug.Log("playerChips Status...." + updatePlayerChips.success + "...Get Total Chips...." + updatePlayerChips.total_chips);

        }
    }

    #endregion





    #region GeneralFunctions
    string PlayerTokenID()
    {
        return LocalSettings.GetTokenID();
    }
    #endregion
    #region Update Chips of player Json To C#
    public class PlayerChips
    {
        public string success { get; set; }
        public string total_chips { get; set; }
        public string total_diamond { get; set; }
        public string total_xp { get; set; }
        public string dealer_expiry_date { get; set; }
    }
    #endregion
}
