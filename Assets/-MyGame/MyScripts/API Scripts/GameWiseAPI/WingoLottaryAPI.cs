using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WingoLottaryAPI : MonoBehaviour
{
    #region Creating Instance
    private static WingoLottaryAPI _instance;

    

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
        yield return new WaitForSeconds(0);
    }
    #endregion
}
