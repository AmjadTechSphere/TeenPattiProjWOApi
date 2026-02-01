using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class EmailHistroy : ES3Cloud
{
    public GameObject historyBox;
    public GameObject noMoreHistory;

    public RectTransform EmailHistoryItem;

    public TMP_Text DateAtTitle;
    public TMP_Text HistoryEmailDetail;
    public TMP_Text GoldAmount;

    public GameObject HistoryEmailreadFullpanel;

    //public static int aa;
    #region Creating Instance;
    private static EmailHistroy _instance;
    public static EmailHistroy Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<EmailHistroy>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    protected EmailHistroy(string url, string apiKey) : base(url, apiKey) { }
    #endregion


    int EmailHistoryDeletePlayerID;

    //public void OpenhistoryEmail(int playerid, string dateupdated)
    public void OpenhistoryEmail(int emailIndex)
    {
        // opening email
        Debug.LogError("opening email");
        HistoryEmailreadFullpanel.SetActive(true);
        DateAtTitle.text = emailHistory.players[emailIndex].updated_at.ToString();
        HistoryEmailDetail.text = "Dear Player,\n\nThe transfer you made to " + emailHistory.players[emailIndex].player.username + "(ID:" + emailHistory.players[emailIndex].player.playerID + ") on " + emailHistory.players[emailIndex].updated_at + " has been cancelled. Gold has been returned.\n\nBest Regards,\nTPD Team.";
        GoldAmount.text = LocalSettings.Rs(emailHistory.players[emailIndex].chips);
        EmailHistoryDeletePlayerID = emailHistory.players[emailIndex].id;
    }

    public void GetEmailHistoryList()
    {
        GetEmailHistoryListByPlayerID(LocalSettings.GetTokenID());
    }

    #region Getting email list
    void GetEmailHistoryListByPlayerID(string PlayerIDForEmailList)
    {
        StartCoroutine(GetEmailHistroyListAPI(PlayerIDForEmailList));
    }
    EmailHistoryList emailHistory;
    IEnumerator GetEmailHistroyListAPI(string PlayerIDForEmailList)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        string url = APIStrings.EmailsHistoryListURLAPI + PlayerIDForEmailList;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            GoldTransfer.Instance.LoadingPanel.SetActive(true);
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            emailHistory = new EmailHistoryList();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {

                Debug.LogError("Error is: " + webRequest.result);
            }
            else
            {
                Debug.LogError("success is: " + webRequest.result);
            }
            string jsonString = webRequest.downloadHandler.text;
            Debug.LogError(jsonString);
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            emailHistory = JsonConvert.DeserializeObject<EmailHistoryList>(jsonString);
            SetEmailFields(emailHistory);
        }
    }
    List<GameObject> EmailHistroyList = new List<GameObject>();
    void SetEmailFields(EmailHistoryList emailHistoryList)
    {
        if (EmailHistroyList != null)
        {
            foreach (GameObject obj in EmailHistroyList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            EmailHistroyList.Clear();
        }
        else
        {
            EmailHistroyList = new List<GameObject>();
        }
        if (emailHistoryList != null)
        {
            if (emailHistoryList.players != null)
            {
                if (emailHistoryList.players.Count > 0)
                {
                    historyBox.SetActive(true);
                    noMoreHistory.SetActive(false);
                    for (int i = 0; i < emailHistoryList.players.Count; i++)
                    {
                        GameObject rSet = Instantiate(EmailHistoryItem.gameObject);
                        LocalSettings.SetPosAndRect(rSet, EmailHistoryItem, EmailHistoryItem.transform.parent.transform);
                        rSet.SetActive(true);
                        EmailHistroyList.Add(rSet);
                        rSet.GetComponent<EmailFields>().EmailHistoryListFieldsFill(emailHistoryList, i);
                    }
                }
                else
                {
                    noMoreHistory.SetActive(true);
                    historyBox.SetActive(false); ;
                }
            }
            else
            {
                noMoreHistory.SetActive(true);
                historyBox.SetActive(false); ;
            }
        }
    }

    #endregion


    #region Delete Email History
    public void DeleteThisEmail()
    {
        StartCoroutine(sendToDeleteEmailHistorAPI(EmailHistoryDeletePlayerID.ToString()));
    }


    IEnumerator sendToDeleteEmailHistorAPI(string deleteEmailID)
    {
        string url = APIStrings.DeleteEmailsHistoryURLAPI + deleteEmailID;
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error is:  " + webRequest.result);
                GoldTransfer.Instance.LoadingPanel.SetActive(false);
            }
            else
            {
                GoldTransfer.Instance.LoadingPanel.SetActive(false);
                GetEmailHistoryList();
                HistoryEmailreadFullpanel.SetActive(false);
            }
        }
    }
    #endregion

}

#region Email Histroy JSON to c# 
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class EmailHistoryList
{
    public string success { get; set; }
    public List<EmailHistoryDetail> players { get; set; }
}
public class EmailHistoryDetail
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
    public EmailHistroyreceiver player { get; set; }
}

public class EmailHistroyreceiver
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



#endregion