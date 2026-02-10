
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LuckyWarAPI : MonoBehaviour
{
    #region Creating Instance
    private static LuckyWarAPI _instance;
  

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
        yield return new WaitForSeconds(0);
    }
    #endregion


}
