using com.mani.muzamil.amjad;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
public class FriendListTD : ES3Cloud
{
    public RectTransform FriendDetailField;
    public RectTransform FriendRequestField;
    public RectTransform SearchedFriendField;
    public TMP_InputField friendSearchInputField;
    public GameObject FriendListPanel;


    public const string increentedID = "incremented_id";
    public const string status = "status";

    public const string addFriendPlayerID = "player_id";
    public const string addFriendFriendID = "friend_id";
    // instance Creating of Friend list TD Script
    #region Creating Instance;
    private static FriendListTD _instance;
    public static FriendListTD Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<FriendListTD>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;


    }
    protected FriendListTD(string url, string apiKey) : base(url, apiKey)
    {

    }
    #endregion

    // Getting all friends List
    #region Getting all my friends List 

    public void GetListOfFriendsBtn()
    {
        GetListOfFriends(LocalSettings.GetPlayerID().ToString(), GetFriendsList);
        //GetListOfFriends("44873218", GetFriendsList);
    }
    List<GameObject> FriendsTDFieldsList = new List<GameObject>();
    void GetListOfFriends(string playerTokenID, Action<FriendListTDAPI> onDataReceived)
    {
        StartCoroutine(GetListOfFriendsAPI(playerTokenID, onDataReceived));
    }
    FriendListTDAPI FriendListClass;
    IEnumerator GetListOfFriendsAPI(string tokenIDFromServer, Action<FriendListTDAPI> onDataReceived)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        string url = APIStrings.FriendsListURLAPI + tokenIDFromServer;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            Debug.Log("Getting List of friends");
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            FriendListClass = new FriendListTDAPI();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error is: " + webRequest.result);
                onDataReceived?.Invoke(FriendListClass);
            }
            else
            {
                Debug.Log("Success is: " + webRequest.result);
            }

            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            string jsonString = webRequest.downloadHandler.text;
            Debug.Log("Json String is: " + jsonString);
            FriendListClass = JsonConvert.DeserializeObject<FriendListTDAPI>(jsonString);
            onDataReceived?.Invoke(FriendListClass);
        }
    }

    void GetFriendsList(FriendListTDAPI friendListRoot)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(false);
        if (friendListRoot == null)
        {
            Debug.Log("No record found");
            return;
        }

        if (FriendsTDFieldsList != null)
        {
            foreach (GameObject obj in FriendsTDFieldsList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            FriendsTDFieldsList.Clear();
        }
        if (friendListRoot.friends != null)
        {
            if (friendListRoot.friends.Count > 0)
            {
                if (FriendsTDFieldsList == null)
                    FriendsTDFieldsList = new List<GameObject>();
                for (int i = 0; i < friendListRoot.friends.Count; i++)
                {
                    GameObject rSet = Instantiate(FriendDetailField.gameObject);
                    LocalSettings.SetPosAndRect(rSet, FriendDetailField, FriendDetailField.transform.parent.transform);
                    rSet.SetActive(true);
                    FriendsTDFieldsList.Add(rSet);
                    rSet.GetComponent<FriendListFields>().MyFriendsListFill(friendListRoot, i);
                }
            }
        }
        else
        {
            Debug.Log("No Friend Found");
        }
    }

    #endregion

    // Navigate From friends of TD to gold transfer panel with other Player ID
    #region Redirecting to gold panel with other user ID

    public void GoToGoldTransferPanel(string otherPlayerID)
    {
        FriendListPanel.SetActive(false);
        GoldTransfer.Instance.GoldTransferPanel.SetActive(true);
        GoldTransfer.Instance.otherPlayerID.text = otherPlayerID;
    }

    #endregion

    // Getting searched friend using Other player ID
    #region  Get Search Friend list using Player ID

    //public void SearchBtnClick()
    //{
    //    if (friendSearchInputField.text.Length != 8 || friendSearchInputField.text == "")
    //    {
    //        Toaster.ShowAToast("Invalid ID please enter valid ID");
    //        return;
    //    }
    //    else if (friendSearchInputField.text == LocalSettings.GetPlayerID().ToString())
    //    {
    //        Toaster.ShowAToast("This Is You ID");
    //        return;
    //    }



    //}
    public void searchFriendsByPlayerID()
    {
        if (LocalSettings.IsMenuScene())
        {
            if (friendSearchInputField.text.Length != 8 || friendSearchInputField.text == "")
            {
                Toaster.ShowAToast("Invalid ID please enter valid ID");
                return;
            }
            else if (friendSearchInputField.text == LocalSettings.GetPlayerID().ToString())
            {
                Toaster.ShowAToast("This Is You ID");
                return;
            }
            string otherPlayerID = friendSearchInputField.text;
            StartCoroutine(searchFriendsByPlayerIDAPI(otherPlayerID, searchFriendList));
        }
        else
            StartCoroutine(searchFriendsByPlayerIDAPI(otherPlayerPlayerID, searchFriendList));

        //StartCoroutine(searchFriendsByPlayerIDAPI("44873218", searchFriendList));
    }
    string otherPlayerPlayerID;
    public void SearchFriendByPlayerIDInGamePlay(string otherplayerID)
    {
        otherPlayerPlayerID = otherplayerID;
        searchFriendsByPlayerID();

    }

    public void Emptyfunction()
    {
        GoldTransfer.Instance.showMessage("Send Your Firend Requestion");
    }


    SearchFriendAPI SearchFiendClass;
    IEnumerator searchFriendsByPlayerIDAPI(string otherPlayerID, Action<SearchFriendAPI> onDataReceived)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);

        //string url = APIStrings.SearchsFriendsPlayerIDURLAPI + otherPlayerID;
        string url = APIStrings.SearchsFriendsPlayerIDURLAPI + otherPlayerID + APIStrings.SearchsFriendsPlayerIDURLAPIPart2 + LocalSettings.GetPlayerID();
        using (var webRequest = UnityWebRequest.Get(url))
        {
            Debug.Log("Getting List of pending friends requests");
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            SearchFiendClass = new SearchFriendAPI();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error is: " + webRequest.result);
                GoldTransfer.Instance.LoadingPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Success is: " + webRequest.result);
            }

            string jsonString = webRequest.downloadHandler.text;
            //Debug.LogError(jsonString);
            SearchFiendClass = JsonConvert.DeserializeObject<SearchFriendAPI>(jsonString);
            onDataReceived?.Invoke(SearchFiendClass);
            if (!LocalSettings.IsMenuScene())
                FriendListFields.Instance.AddNewFriend();
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
        }
    }

    void searchFriendList(SearchFriendAPI SearchFiendClass)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(false);
        if (SearchFiendClass == null)
        {
            Toaster.ShowAToast("User not Exist");
            if (LocalSettings.IsMenuScene())
                SearchedFriendField.gameObject.SetActive(false);
            return;
        }

        if (SearchFiendClass.player != null)
        {
            //for (int i = 0; i < SearchFiendClass.players.Count; i++)
            //{
            //    GameObject rSet = Instantiate(FriendRequestField.gameObject);
            //    LocalSettings.SetPosAndRect(rSet, FriendRequestField, FriendRequestField.transform.parent.transform);
            //    rSet.SetActive(true);
            //    FriendsRequestsTDFieldsList.Add(rSet);
            //    rSet.GetComponent<FriendListFields>().PendingFriendRequestsListFill(SearchFiendClass, i);
            //}
            if (LocalSettings.IsMenuScene())
            {
                SearchedFriendField.gameObject.SetActive(true);
                SearchedFriendField.transform.GetComponent<FriendListFields>().FillSearchedPlayerFields(SearchFiendClass);
            }
            else
                FriendListFields.Instance.FillSearchedPlayerFields(SearchFiendClass);
        }
        else
        {
            if (LocalSettings.IsMenuScene())
                SearchedFriendField.gameObject.SetActive(false);
            Toaster.ShowAToast("User not Exist");
        }
    }
    #endregion

    // Getting List of all friend requests
    #region  Get List of Friend requests

    public void GetListOfFriendsRequests()
    {
        StartCoroutine(GetFriendsRequestAPI(LocalSettings.GetPlayerID().ToString(), GetPendingRequestsList));
        //StartCoroutine(GetFriendsRequestAPI("44873218", GetPendingRequestsList));
    }
    PendingFriendRequestsAPI PendingFriendRequestsClass;
    IEnumerator GetFriendsRequestAPI(string playerID, Action<PendingFriendRequestsAPI> onDataReceived)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        string url = APIStrings.PendingFriendsRequestsURLAPI + playerID;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            Debug.Log("Getting List of pending friends requests");
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            PendingFriendRequestsClass = new PendingFriendRequestsAPI();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error is: " + webRequest.result);
            }
            else
            {
                Debug.Log("Success is: " + webRequest.result);
            }

            string jsonString = webRequest.downloadHandler.text;
            //Debug.LogError(jsonString);
            PendingFriendRequestsClass = JsonConvert.DeserializeObject<PendingFriendRequestsAPI>(jsonString);
            onDataReceived?.Invoke(PendingFriendRequestsClass);
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
        }
    }
    List<GameObject> FriendsRequestsTDFieldsList = new List<GameObject>();
    void GetPendingRequestsList(PendingFriendRequestsAPI friendRequestListRoot)
    {
        GoldTransfer.Instance.LoadingPanel.SetActive(false);
        if (friendRequestListRoot == null)
        {
            Debug.Log("No record found");
            return;
        }

        if (FriendsRequestsTDFieldsList != null)
        {
            foreach (GameObject obj in FriendsRequestsTDFieldsList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            FriendsRequestsTDFieldsList.Clear();
        }
        else
            FriendsRequestsTDFieldsList = new List<GameObject>();
        if (friendRequestListRoot.players != null)
        {
            if (friendRequestListRoot.players.Count > 0)
            {
                if (FriendsTDFieldsList == null)
                    FriendsTDFieldsList = new List<GameObject>();
                for (int i = 0; i < friendRequestListRoot.players.Count; i++)
                {
                    GameObject rSet = Instantiate(FriendRequestField.gameObject);
                    LocalSettings.SetPosAndRect(rSet, FriendRequestField, FriendRequestField.transform.parent.transform);
                    rSet.SetActive(true);
                    FriendsRequestsTDFieldsList.Add(rSet);
                    rSet.GetComponent<FriendListFields>().PendingFriendRequestsListFill(friendRequestListRoot, i);
                }
            }
        }
        else
        {
            Debug.Log("No Friend Found");
        }
    }
    #endregion

    // Changing status of friend to pending,local_friend,local_reject_friend,local_unfriend,fb_unfriend,cancel_local_friend,accept_all,reject_all
    #region Change Status Of FriendOrFriendList

    public void ChangeStatusOfFriend(string IDOfOtherFriend, string statusString, Action methodNameToCall)
    {
        StartCoroutine(ChangeStatusFriendUsingAPI(IDOfOtherFriend, statusString, methodNameToCall));
    }

    public IEnumerator ChangeStatusFriendUsingAPI(string IDOfOtherFriend, string statusString, Action methodNameToCall)
    {

        string url = APIStrings.FriendStatusChangeURLAPI;
        formData = new List<KeyValuePair<string, string>>();
        AddPOSTField(increentedID, IDOfOtherFriend);
        AddPOSTField(status, statusString);
        Debug.Log("status string is: " + statusString + "     Status field is: " + status + "     Incre ID: " + IDOfOtherFriend);
        WWWForm form = CreateWWWForm();

        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Friend Status Change failed ");
                int responseCode = (int)webRequest.responseCode;
                string responseText = webRequest.downloadHandler.text;
                Debug.LogError("response COde: " + responseCode + "     : Response text: " + responseText);
            }
            else
            {
                Debug.Log("Friend Status Changed successfully ");
            }
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            methodNameToCall?.Invoke();
        }

    }

    #endregion

    // Accept/ Reject All friend requests at once
    #region Accept/Reject All Friend Requests

    public void AcceptAllFriendsRequests()
    {
        ChangeStatusOfFriend(LocalSettings.GetIncrementedPlayerID().ToString(), APIStrings.AcceptAllFriendRequests, GetListOfFriendsRequests);
    }
    public void RejectAllFriendsRequests()
    {
        ChangeStatusOfFriend(LocalSettings.GetIncrementedPlayerID().ToString(), APIStrings.RejectAllFriendRequests, GetListOfFriendsRequests);

    }

    #endregion

    // Adding a new friend
    #region Add New Friend 
    public void AddNewFriend(string myplayerIncrementedID, string friendIncrementedID, Action methodNameToCall)
    {
        StartCoroutine(AddNewFriendAPI(myplayerIncrementedID, friendIncrementedID, methodNameToCall));
    }

    public IEnumerator AddNewFriendAPI(string myplayerIncrementedID, string friendIncrementedID, Action methodNameToCall)
    {

        string url = APIStrings.AddNewFriendURLAPI;
        formData = new List<KeyValuePair<string, string>>();
        AddPOSTField(addFriendPlayerID, myplayerIncrementedID);
        AddPOSTField(addFriendFriendID, friendIncrementedID);
        //  Debug.LogError("status string is: " + statusString + "     Status field is: " + status + "     Incre ID: " + IDOfOtherFriend);
        WWWForm form = CreateWWWForm();

        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            GoldTransfer.Instance.LoadingPanel.SetActive(true);
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Add new friend field failed ");
                int responseCode = (int)webRequest.responseCode;
                string responseText = webRequest.downloadHandler.text;
                Debug.LogError("response COde: " + responseCode + "     : Response text: " + responseText);
            }
            else
            {
                Debug.Log("friend request send successfully ");
            }
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            methodNameToCall?.Invoke();
        }
    }
    #endregion
}


#region Friends list json to c# class

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class FriendListTDAPI
{
    public string success { get; set; }
    public MyDetail myDetail { get; set; }
    public List<Friend> friends { get; set; }
}

public class Friend
{
    public int id { get; set; }
    public string player_id { get; set; }
    public string friend_id { get; set; }
    public string status { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public Friend2 friend { get; set; }
    public Player4 player { get; set; }
}

public class Friend2
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
    public ActivityIsOnline activity { get; set; }
}


public class Player4
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
    public ActivityIsOnline activity { get; set; }
}

public class MyDetail
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

public class ActivityIsOnline
{
    public int id { get; set; }
    public string player_id { get; set; }
    public string is_online { get; set; }
    public string last_seen { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}


#endregion


#region Pending Friends Requests List jSon to C#

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class PendingFriendRequestsAPI
{
    public string success { get; set; }
    public List<PlayerPendingRequests> players { get; set; }
}
public class PlayerPendingRequests
{
    public int id { get; set; }
    public string player_id { get; set; }
    public string friend_id { get; set; }
    public string status { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public Player2 player { get; set; }
}

public class Player2
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

#region Search Friend jSon to C#
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class SearchFriendAPI
{
    public string status { get; set; }
    public SearchFriend player { get; set; }
    public Friendship friendship { get; set; }
}
public class Friendship
{
    public int id { get; set; }
    public string player_id { get; set; }
    public string friend_id { get; set; }
    public string status { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}

public class SearchFriend
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