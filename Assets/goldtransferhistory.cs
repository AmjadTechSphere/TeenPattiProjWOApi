using com.mani.muzamil.amjad;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static GoldWinLoose;

public class goldtransferhistory : ES3Cloud
{
    public GameObject PlayerVIPIndicator;
    public TMP_Text PlayerVIPExpiryDaysTxt;
    public RectTransform RecordSet;
    public RectTransform GoldReceivedSlotSet;
    public RectTransform RecallConfirmPanel;
    public RectTransform RedDotToCollectGoldIndicator;

    // instance Creating of Gold Transfer History Script
    #region Creating Instance;
    private static goldtransferhistory _instance;
    public static goldtransferhistory Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<goldtransferhistory>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        sentGoldFieldList = new List<GameObject>();
        ReceiveGoldFieldList = new List<GameObject>();
    }
    protected goldtransferhistory(string url, string apiKey) : base(url, apiKey)
    {

    }
    #endregion

    private void OnEnable()
    {
        GetPlayerGoldSentRecord();
        RecallConfirmPanel.gameObject.SetActive(false);

        if (LocalSettings.GetVIPStatus() == GoldTransfer.DealerString)
        {
            PlayerVIPIndicator.SetActive(true);
            PlayerVIPExpiryDaysTxt.gameObject.SetActive(true);
            GoldWinLoose.Instance.GetplayerGoldAndVIPStatusDate(LocalSettings.GetPlayerID().ToString(), ShowVipStatus);
        }
        else
        {
            PlayerVIPIndicator.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }


    void ShowVipStatus(PlayerChips plyrChips)
    {
        coroutine = StartCoroutine(dateeee(plyrChips.dealer_expiry_date));

    }
    public void GetPlayerGoldSentRecord()
    {
        GetPlayerGoldSentRecord(LocalSettings.GetPlayerID().ToString(), getGoldSentRecord);
    }


    List<GameObject> sentGoldFieldList = new List<GameObject>();
    List<GameObject> ReceiveGoldFieldList = new List<GameObject>();

    public void SetGoldReceivedRecordFields(PlayerGoldReceivedRecord GoldReceived)
    {
        if (GoldReceived == null)
        {
            Debug.LogError("No record found");
            return;
        }
        foreach (GameObject obj in sentGoldFieldList)
        {
            if (obj != null)
                Destroy(obj);
        }
        sentGoldFieldList.Clear();
        if (GoldReceived.players != null)
        {
            if (GoldReceived.players.Count > 0)
            {
                for (int i = 0; i < GoldReceived.players.Count; i++)
                {
                    GameObject rSet = Instantiate(GoldReceivedSlotSet.gameObject);
                    LocalSettings.SetPosAndRect(rSet, GoldReceivedSlotSet, GoldReceivedSlotSet.transform.parent.transform);
                    rSet.SetActive(true);
                    sentGoldFieldList.Add(rSet);

                    rSet.GetComponent<HistoryFields>().SetFieldsOfCollectGold(GoldReceived.players[i].status, GoldReceived.players[i].sender.username, GoldReceived.players[i].sender.playerID, GoldReceived.players[i].chips, GoldReceived.players[i].created_at.ToString(), GoldReceived.players[i].sender.image, GoldReceived.players[i].id);
                }
            }
        }

    }

    void getGoldSentRecord(PlayerGoldSentRecord sentRecord)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(false);
        if (sentRecord == null)
        {
            Debug.LogError("No record found");
            return;
        }
        foreach (GameObject obj in ReceiveGoldFieldList)
        {
            if (obj != null)
                Destroy(obj);
        }
        ReceiveGoldFieldList.Clear();
        if (sentRecord.players != null)
        {
            if (sentRecord.players.Count > 0)
            {
                if (sentRecord.pending_counter > 0)
                    RedDotToCollectGoldIndicator.gameObject.SetActive(true);
                else
                    RedDotToCollectGoldIndicator.gameObject.SetActive(false);
                for (int i = 0; i < sentRecord.players.Count; i++)
                //for (int i = sentRecord.players.Count - 1; i >= 0; i--)
                {

                    GameObject rSet = Instantiate(RecordSet.gameObject);
                    LocalSettings.SetPosAndRect(rSet, RecordSet, RecordSet.transform.parent.transform);
                    rSet.SetActive(true);
                    ReceiveGoldFieldList.Add(rSet);
                    //rSet.GetComponent<HistoryFields>().SetRecordFieldsOfSent(sentRecord.players[i].status, sentRecord.players[i].player.username, sentRecord.players[i].player.playerID, sentRecord.players[i].chips, sentRecord.players[i].created_at.ToString(), sentRecord.players[i].player.image, sentRecord.players[i].id);

                    rSet.GetComponent<HistoryFields>().SetRecordFieldsOfSent(sentRecord, i);
                    //Debug.LogError("Sent date: " + sentRecord.players[i].created_at);
                }
            }
        }
    }



    #region Getting list of sent gold to other Players Detail

    public void GetPlayerGoldSentRecord(string playerIDFromServer, Action<PlayerGoldSentRecord> onDataReceived)
    {
        StartCoroutine(GetPlayerGoldSentRecordAPI(playerIDFromServer, onDataReceived));
    }
    PlayerGoldSentRecord playerGoldSentRecord;
    IEnumerator GetPlayerGoldSentRecordAPI(string playerIDFromServer, Action<PlayerGoldSentRecord> onDataReceived)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        Debug.LogError("Token ID: " + playerIDFromServer);
        string url = APIStrings.GettingSentGoldRecordAPIURL + playerIDFromServer;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            GoldTransfer.Instance.LoadingPanel.SetActive(true);
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            playerGoldSentRecord = new PlayerGoldSentRecord();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error is: " + webRequest.result);
                onDataReceived?.Invoke(playerGoldSentRecord);
            }
            else
            {
                Debug.Log("success is: " + webRequest.result);
            }
            string jsonString = webRequest.downloadHandler.text;
            Debug.Log("Gold History: " + jsonString);
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            playerGoldSentRecord = JsonConvert.DeserializeObject<PlayerGoldSentRecord>(jsonString);
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            onDataReceived?.Invoke(playerGoldSentRecord);
        }
    }
    #endregion

    #region Getting List of Players who send gold to me

    public void GetGoldReceivedRecordBtn()
    {
        getListOfReceivedGoldRecord(LocalSettings.GetPlayerID().ToString(), SetGoldReceivedRecordFields);
    }

    void getListOfReceivedGoldRecord(string playerTokenID, Action<PlayerGoldReceivedRecord> onDataReceived)
    {
        StartCoroutine(GetPlayerGoldReceivedRecordAPI(playerTokenID, onDataReceived));
    }
    PlayerGoldReceivedRecord playerGoldReceivedRecord;
    IEnumerator GetPlayerGoldReceivedRecordAPI(string tokenIDFromServer, Action<PlayerGoldReceivedRecord> onDataReceived)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        string url = APIStrings.GettingReceivedGoldRecordAPIURL + tokenIDFromServer;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            Debug.LogError("Getting gold reveived record started");
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            playerGoldReceivedRecord = new PlayerGoldReceivedRecord();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error is: " + webRequest.result);
                onDataReceived?.Invoke(playerGoldReceivedRecord);
            }
            else
            {
                Debug.LogError("Success is: " + webRequest.result);
            }

            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            string jsonString = webRequest.downloadHandler.text;
            //Debug.LogError(jsonString);
            playerGoldReceivedRecord = JsonConvert.DeserializeObject<PlayerGoldReceivedRecord>(jsonString);
            onDataReceived?.Invoke(playerGoldReceivedRecord);
        }
    }
    #endregion

    #region Accept chips sent from others

    public void AcceptGoldSentFromOther(string IDToAcceptGold, BigInteger sentOrReceivedAmount)
    {
        StartCoroutine(AcceptGoldFromOtherAPI(IDToAcceptGold, sentOrReceivedAmount));
    }

    IEnumerator AcceptGoldFromOtherAPI(string IDToAcceptGold, BigInteger sentOrReceivedAmount)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        string url = APIStrings.AcceptGoldFromOtherAPIURL + IDToAcceptGold;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error is: " + webRequest.result);
                //Menu_Manager.Instance.GoldCollectedPanel(0);
                Toaster.ShowAToast("Your Gold cancelled by the dealer. Contact with dealer");
            }
            else
            {
                Debug.LogError("Success is: " + webRequest.result);
                if (LocalSettings.IsMenuScene())
                    Menu_Manager.Instance.GoldCollectedPanel(sentOrReceivedAmount);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.GoldCollectFromSenderTick, false);
            }
            int responseCode = (int)webRequest.responseCode;
            string responseText = webRequest.downloadHandler.text;
            //Debug.LogError("response COde: " + responseCode + "     : Response text: " + responseText);

            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            GetGoldReceivedRecordBtn();
            GetPlayerGoldSentRecord();
        }
    }
    #endregion

    #region Recall Sent Amount if not accepted and VIP member
    [ShowOnly]
    public string RecallIDToGetBackAmount;

    public void OnClickConfirmRecall()
    {
        Debug.LogError("Recall ID is: " + RecallIDToGetBackAmount);
        RecallGoldVIPMember(RecallIDToGetBackAmount);
    }
    bool startCounter = false;
    void RecallGoldVIPMember(string IDToRecallGold)
    {
        StartCoroutine(RecallGoldVIPMemnerAPI(IDToRecallGold));
    }

    IEnumerator RecallGoldVIPMemnerAPI(string IDToRecallGold)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        string url = APIStrings.RecallGoldForVIPMemberAPIURL + IDToRecallGold;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error is: " + webRequest.result);
                GoldTransfer.Instance.LoadingPanel.SetActive(false);
                RecallConfirmPanel.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Success is: " + webRequest.result);
                GoldTransfer.Instance.LoadingPanel.SetActive(false);
                RecallConfirmPanel.gameObject.SetActive(false);
                GetPlayerGoldSentRecord();

            }
            int responseCode = (int)webRequest.responseCode;
            string responseText = webRequest.downloadHandler.text;
            RestAPI.Instance.FetchData(LocalSettings.GetTokenID(), Menu_Manager.Instance.SetUserNameAndOtherThings);
            Debug.LogError("response COde: " + responseCode + "     : Response text: " + responseText);
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
        }
    }

    #endregion


    #region VIP member remaining Date and time

    //        8/16/2023 2:09:29 PM

    Coroutine coroutine = null;

    IEnumerator dateeee(string ExpiryString)
    {
        yield return new WaitForSeconds(1f);
        string dateString1 = ExpiryString;
        DateTime date1 = DateTime.Parse(dateString1);
        while (gameObject.activeInHierarchy)
        {
            DateTime date2 = DateTime.Now;
            TimeSpan timeDifference = date1 - date2;
            if (date1 < date2)
            {
                PlayerVIPIndicator.SetActive(false);
                StopCoroutine(coroutine);
                break;
            }
            // Extract days, hours, minutes, and seconds
            int totalDays = (int)timeDifference.TotalDays;
            int hours = timeDifference.Hours;
            int minutes = timeDifference.Minutes;
            int seconds = timeDifference.Seconds;

            PlayerVIPExpiryDaysTxt.text = $"Expires: {totalDays}Days, {hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}";

            yield return new WaitForSeconds(1);
        }
    }
    #endregion

}

#region send gold to other players detail list Json To C#

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class PlayerGoldSentRecord
{
    public string success { get; set; }
    public int pending_counter { get; set; }
    public List<PlayerSenderSent> players { get; set; }
}
public class PlayerSenderSent
{
    public int id { get; set; }
    public string player_id { get; set; }
    public string admin_id { get; set; }
    public string sender_id { get; set; }
    public string vip_card_id { get; set; }
    public object card_expiry_date { get; set; }
    public string transaction { get; set; }
    public string status { get; set; }
    public string chips { get; set; }
    public string commission { get; set; }
    public string diamond { get; set; }
    public string xp { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public ReceiverPlayer player { get; set; }
    public Sender sender { get; set; }
}

public class ReceiverPlayer
{
    public int id { get; set; }
    public string playerID { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string image { get; set; }
    public string deviceId { get; set; }
    public string facebook { get; set; }
    public string google { get; set; }
    public string phone { get; set; }
    public string token_id { get; set; }
    public string frame { get; set; }
    public string auth_type { get; set; }
    public string status { get; set; }
}


public class Sender
{
    public int id { get; set; }
    public string playerID { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string image { get; set; }
    public string deviceId { get; set; }
    public string facebook { get; set; }
    public string google { get; set; }
    public string phone { get; set; }
    public string token_id { get; set; }
    public string frame { get; set; }
    public string auth_type { get; set; }
    public string status { get; set; }
}

//public class PlayerGoldSentRecord
//{
//    public string success { get; set; }
//    public int pending_counter { get; set; }
//    public List<PlayerSenderSent> players { get; set; }
//}
//// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
//public class PlayerSenderSent
//{
//    public int id { get; set; }
//    public string player_id { get; set; }
//    public string admin_id { get; set; }
//    public string sender_id { get; set; }
//    public string vip_card_id { get; set; }
//    public object card_expiry_date { get; set; }
//    public string transaction { get; set; }
//    public string status { get; set; }
//    public string chips { get; set; }
//    public string commission { get; set; }
//    public string diamond { get; set; }
//    public string xp { get; set; }
//    public DateTime created_at { get; set; }
//    public DateTime updated_at { get; set; }
//    public PlayerReceiverSent player { get; set; }
//}

//public class PlayerReceiverSent
//{
//    public int id { get; set; }
//    public string playerID { get; set; }
//    public string username { get; set; }
//    public string email { get; set; }
//    public string image { get; set; }
//    public string deviceId { get; set; }
//    public string facebook { get; set; }
//    public string google { get; set; }
//    public string phone { get; set; }
//    public string token_id { get; set; }
//    public string frame { get; set; }
//    public string auth_type { get; set; }
//    public string status { get; set; }
//}

#endregion


#region Get List of Players who send gold to me Json to C#

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class PlayerGoldReceivedRecord
{
    public string success { get; set; }
    public List<MyPlayerReceivedGold> players { get; set; }
}


public class MyPlayerReceivedGold
{
    public int id { get; set; }
    public string player_id { get; set; }
    public string admin_id { get; set; }
    public string sender_id { get; set; }
    public string vip_card_id { get; set; }
    public object card_expiry_date { get; set; }
    public string transaction { get; set; }
    public string status { get; set; }
    public string chips { get; set; }
    public string commission { get; set; }
    public string diamond { get; set; }
    public string xp { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public SenderlayerRecord sender { get; set; }
}
public class SenderlayerRecord
{
    public int id { get; set; }
    public string playerID { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string image { get; set; }
    public object deviceId { get; set; }
    public string facebook { get; set; }
    public string google { get; set; }
    public string phone { get; set; }
    public string token_id { get; set; }
    public string frame { get; set; }
    public string auth_type { get; set; }
    public string status { get; set; }
}

#endregion