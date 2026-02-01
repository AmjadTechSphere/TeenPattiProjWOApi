using com.mani.muzamil.amjad;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class EmailCollectChips : ES3Cloud
{
    public GameObject EmailpanelWhole;

    public GameObject NoMoreEmailsIcon;
    public GameObject EmailsBox;
    public RectTransform EmailListItem;

    public GameObject EmailDetailpanel;
    #region Email read Fields

    public TMP_Text EmailDate;
    public TMP_Text DetailEmailText;
    public TMP_Text AmountToCollect;



    #endregion

    #region Recall Sent Amount if not accepted and VIP member
    // instance Creating of Gold Transfer History Script

    #endregion

    #region Creating Instance;
    private static EmailCollectChips _instance;
    public static EmailCollectChips Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<EmailCollectChips>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        EmailpanelWhole.SetActive(false);
    }
    protected EmailCollectChips(string url, string apiKey) : base(url, apiKey)
    {

    }
    #endregion

    public void GetEmailList()
    {
        RecallGoldVIPMember(LocalSettings.GetPlayerID().ToString());
    }

    #region Getting email list
    void RecallGoldVIPMember(string PlayerIDForEmailList)
    {
        StartCoroutine(GetEmailListSPI(PlayerIDForEmailList));
    }
    EmailList emailList;
    IEnumerator GetEmailListSPI(string PlayerIDForEmailList)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        string url = APIStrings.EmailsListURLAPI + PlayerIDForEmailList;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            GoldTransfer.Instance.LoadingPanel.SetActive(true);
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            emailList = new EmailList();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                
                Debug.LogError("Error is: " + webRequest.result);
            }
            //else
            //{
            //    Debug.LogError("success is: " + webRequest.result);
            //}
            string jsonString = webRequest.downloadHandler.text;
            //Debug.LogError(jsonString);
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            emailList = JsonConvert.DeserializeObject<EmailList>(jsonString);
            SetEmailFields(emailList);
        }
    }
    List<GameObject> EmailListList = new List<GameObject>();
    void SetEmailFields(EmailList emailList)
    {
        if (EmailListList != null)
        {
            foreach (GameObject obj in EmailListList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            EmailListList.Clear();
        }
        else
        {
            EmailListList = new List<GameObject>();
        }
        if (emailList != null)
        {
            if (emailList.players != null)
            {
                if (emailList.players.Count > 0)
                {
                    EmailsBox.SetActive(true);
                    NoMoreEmailsIcon.SetActive(false);
                    for (int i = 0; i < emailList.players.Count; i++)
                    {
                        GameObject rSet = Instantiate(EmailListItem.gameObject);
                        LocalSettings.SetPosAndRect(rSet, EmailListItem, EmailListItem.transform.parent.transform);
                        rSet.SetActive(true);
                        EmailListList.Add(rSet);
                        rSet.GetComponent<EmailFields>().EmailListFieldsFill(emailList, i);
                    }
                }
                else
                {
                    NoMoreEmailsIcon.SetActive(true);
                    EmailsBox.SetActive(false); ;
                }
            }
            else
            {
                NoMoreEmailsIcon.SetActive(true);
                EmailsBox.SetActive(false); ;
            }
        }
    }

    #endregion


    public void OpenEmail(int playerid, string dateupdated)
    {
        // opening email
        //Debug.LogError("opening email");
        EmailDate.text = dateupdated;
        StartCoroutine(GetEmailDetail(playerid.ToString()));
        EmailDetailpanel.SetActive(true);
    }

    #region Get one Email Detail
    EmailDetail emailDetail;

    IEnumerator GetEmailDetail(string EmailId)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        string url = APIStrings.EmailDetailURLAPI + EmailId;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            emailDetail = new EmailDetail();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error is: " + webRequest.result);
                GoldTransfer.Instance.LoadingPanel.SetActive(false);
            }
            //else
            //{
            //    Debug.LogError("success is: " + webRequest.result);
            //}
            string jsonString = webRequest.downloadHandler.text;
            //Debug.LogError(jsonString);
            GoldTransfer.Instance.LoadingPanel.SetActive(false);

            emailDetail = JsonConvert.DeserializeObject<EmailDetail>(jsonString);
            fillEmailDetail();
        }
    }
    void fillEmailDetail()
    {
        string emailDetailstring = "Dear Player,\n\nThe transfer you made to " + emailDetail.gold.player.username + "(ID:" + emailDetail.gold.player.playerID + ") on " + EmailDate.text + " has been cancelled. Gold has been returned.\n\nPlease collect it now.\n\nBest Regards,\nTPD Team.";
        DetailEmailText.text = emailDetailstring;
        AmountToCollect.text = LocalSettings.Rs(emailDetail.gold.chips);
    }

    public void CollectGoldFromDetailEmail()
    {
        StartCoroutine(SendPlayerDataToAPIToCreatePlayer(emailDetail.gold.id.ToString()));
    }

    public IEnumerator SendPlayerDataToAPIToCreatePlayer(string collectID)
    {
        string url = APIStrings.CollectGoldFromEmailURLAPI + collectID;
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error is: " + webRequest.result);
                GoldTransfer.Instance.LoadingPanel.SetActive(false);
            }
            else
            {
                //Debug.LogError("Success is: " + webRequest.result);
                GoldTransfer.Instance.LoadingPanel.SetActive(false);
                GoldTransfer.Instance.showMessage("You have Collect " + LocalSettings.Rs(emailDetail.gold.chips));
            }
            int responseCode = (int)webRequest.responseCode;
            string responseText = webRequest.downloadHandler.text;
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            GetEmailList();
            EmailDetailpanel.SetActive(false);
            RestAPI.Instance.FetchData(LocalSettings.GetTokenID(), Menu_Manager.Instance.SetUserNameAndOtherThings);
        }
    }

    #endregion
}
#region Email List Json to C# Class
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class EmailListTitleDetail
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
}

public class EmailList
{
    public string success { get; set; }
    public List<EmailListTitleDetail> players { get; set; }
}
#endregion


#region Getting Email Detail Json To C# Class

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class EmailDetail
{
    public string success { get; set; }
    public GoldEmailDetail gold { get; set; }
}

public class GoldEmailDetail
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
    public ReceiverCanceled player { get; set; }
}

public class ReceiverCanceled
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