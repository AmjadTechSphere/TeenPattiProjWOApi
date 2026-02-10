using com.mani.muzamil.amjad;
using Newtonsoft.Json;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GoldWinLoose : MonoBehaviour
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
        yield return new WaitForSeconds(0);
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

        //  GoldTransfer.Instance.LoadingPanel.SetActive(false);

        //Debug.LogError("playerChips Status...." + updatePlayerChips.success + "...Get Total Chips...." + updatePlayerChips.total_chips);


    }

    public void GetplayerGoldAndVIPStatusDate(string playerId, Action<PlayerChips> FunctionName)
    {
        StartCoroutine(GetPlayerUpdatechipsCorouantine(playerId, FunctionName));
    }
    IEnumerator GetPlayerUpdatechipsCorouantine(string playerId, Action<PlayerChips> FunctionName)
    {
        //GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);


        //  GoldTransfer.Instance.LoadingPanel.SetActive(false);

        //  GoldTransfer.Instance.LoadingPanel.SetActive(false);
        FunctionName?.Invoke(updatePlayerChips);
        Debug.Log("playerChips Status...." + updatePlayerChips.success + "...Get Total Chips...." + updatePlayerChips.total_chips);


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
