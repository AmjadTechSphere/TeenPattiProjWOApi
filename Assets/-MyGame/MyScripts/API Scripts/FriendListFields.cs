using com.mani.muzamil.amjad;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendListFields : MonoBehaviour
{
    public Color onlineColor;
    public Image Pic;
    public TMP_Text UserNameTxt;
    public TMP_Text PlayerIDTxt;
    public TMP_Text LastLoginStatusTxt;
    string PicName;

    [ShowOnly] public string UserName;
    [ShowOnly] public string OtherPlayerID;
    [ShowOnly] public string LastLoginStatus;


    public GameObject[] buttonArray;

    string IncrementedID;
    bool isFromSearch;
    public string statusStringFriendUnFriend;


    #region Creating Instance
    private static FriendListFields _instance;
    public static FriendListFields Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<FriendListFields>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    #endregion
    void Start()
    {

    }
    #region Filling fields 
    public void FillSearchedPlayerFields(SearchFriendAPI searchFriendAPI)
    {
        UserName = searchFriendAPI.player.username;
        OtherPlayerID = searchFriendAPI.player.playerID;
        PicName = searchFriendAPI.player.image;
        if (LocalSettings.IsMenuScene())
        {
            UserNameTxt.text = UserName;
            PlayerIDTxt.text = OtherPlayerID;
            ResetBTNs();
        }
        isFromSearch = true;
        if (searchFriendAPI.friendship == null)
        {
            // He is not my friend
            if (LocalSettings.IsMenuScene())
            {
                buttonArray[4].SetActive(true);
                buttonArray[1].SetActive(true);
            }
            IncrementedID = searchFriendAPI.player.id.ToString();
            Debug.LogError(" Pt  1");
            statusStringFriendUnFriend = "";
        }
        else if (searchFriendAPI.friendship.status == "local_friend")
        {
            // We are already friends
            if (LocalSettings.IsMenuScene())
            {
                buttonArray[0].SetActive(true);
                buttonArray[1].SetActive(true);
            }
            IncrementedID = searchFriendAPI.friendship.id.ToString();
            Debug.LogError(" Pt  2");
            statusStringFriendUnFriend = searchFriendAPI.friendship.status;
        }
        else if (searchFriendAPI.friendship.status == "local_reject_friend" || searchFriendAPI.friendship.status == "cancel_local_friend")
        {
            // He is Reject to me as when i send friend request
            if (LocalSettings.IsMenuScene())
            {
                buttonArray[4].SetActive(true);
                buttonArray[1].SetActive(true);
            }
            IncrementedID = searchFriendAPI.friendship.id.ToString();
            Debug.LogError(" Pt  3");
            statusStringFriendUnFriend = searchFriendAPI.friendship.status;
        }
        else if (searchFriendAPI.friendship.player_id == LocalSettings.GetIncrementedPlayerID().ToString())
        {
            // I send friend request to other
            if (LocalSettings.IsMenuScene())
            {
                buttonArray[5].SetActive(true);
                buttonArray[1].SetActive(true);
            }
            IncrementedID = searchFriendAPI.friendship.id.ToString();
            Debug.LogError(" Pt  4");
            statusStringFriendUnFriend = searchFriendAPI.friendship.status;
        }
        else if (searchFriendAPI.friendship.player_id != LocalSettings.GetIncrementedPlayerID().ToString())
        {
            // Other Friend sending me friendrequest
            if (LocalSettings.IsMenuScene())
            {
                buttonArray[2].SetActive(true);
                buttonArray[3].SetActive(true);
            }
            IncrementedID = searchFriendAPI.friendship.id.ToString();
            Debug.LogError(" Pt  5");
            statusStringFriendUnFriend = searchFriendAPI.friendship.status;
        }

        if (LocalSettings.IsMenuScene())
            StartCoroutine(SetPic(PicName));
            
    }


    void ResetBTNs()
    {
        foreach (GameObject item in buttonArray)
        {
            item.SetActive(false);
        }
    }

    public void MyFriendsListFill(FriendListTDAPI FriendListRoot, int index)
    {
        Friend frnd = FriendListRoot.friends[index];
        string myID = FriendListRoot.myDetail.id.ToString();
        if (myID == frnd.player_id)
        {
            UserName = frnd.friend.username;
            OtherPlayerID = frnd.friend.playerID;

            //if (frnd.friend.activity == null)
            //{
            //    Debug.LogError("In frnd friend activity is null");
            //}

            //if (frnd.player.activity == null)
            //{
            //    Debug.LogError("In frnd player activity is null");
            //}
            if (frnd.friend.activity != null)
            {
                if (frnd.friend.activity.is_online == "1")
                {
                    LastLoginStatus = "On Line";
                    LastLoginStatusTxt.color = onlineColor;
                }
                else
                    LastLoginStatus = frnd.friend.activity.last_seen;
            }
            else
                LastLoginStatus = "Many Day(s) ago.";

            PicName = frnd.friend.image;
        }
        else
        {
            UserName = frnd.player.username;
            OtherPlayerID = frnd.player.playerID;
            //if (frnd.friend.activity == null)
            //{
            //    Debug.LogError("In plyr friend activity is null");
            //}

            //if (frnd.player.activity == null)
            //{
            //    Debug.LogError("In plyr player activity is null");
            //}
            if (frnd.player.activity != null)
            {
                if (frnd.player.activity.is_online == "1")
                {
                    LastLoginStatus = "On Line";
                    LastLoginStatusTxt.color = onlineColor;
                }
                else
                    LastLoginStatus = frnd.player.activity.last_seen;
            }
            else
                LastLoginStatus = "Many Day(s) ago.";
            PicName = frnd.player.image;
        }



        IncrementedID = frnd.id.ToString();
        UserNameTxt.text = UserName;
        PlayerIDTxt.text = OtherPlayerID;
        LastLoginStatusTxt.text = LastLoginStatus;
        StartCoroutine(SetPic(PicName));
    }

    public void PendingFriendRequestsListFill(PendingFriendRequestsAPI friendRequestListRoot, int index)
    {
        if (friendRequestListRoot == null)
            return;
        PlayerPendingRequests requestedPlayer = friendRequestListRoot.players[index];
        UserName = requestedPlayer.player.username;
        OtherPlayerID = requestedPlayer.player.playerID;
        PicName = requestedPlayer.player.image;
        IncrementedID = requestedPlayer.id.ToString();

        UserNameTxt.text = UserName;
        PlayerIDTxt.text = OtherPlayerID;
        StartCoroutine(SetPic(PicName));
    }
    #endregion

    #region Button Function

    public void UnFriend()
    {
        if (!isFromSearch)
            FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.UnFriend, FriendListTD.Instance.GetListOfFriendsBtn);
        else
            FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.UnFriend, FriendListTD.Instance.searchFriendsByPlayerID);
    }

    public void GoldTransfer()
    {
        FriendListTD.Instance.GoToGoldTransferPanel(OtherPlayerID);
    }


    public void AcceptFriendRequest()
    {
        if (!isFromSearch)
            FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.LocalFriendString, FriendListTD.Instance.GetListOfFriendsRequests);
        else
            FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.LocalFriendString, FriendListTD.Instance.searchFriendsByPlayerID);
    }

    public void RejectFriendRequest()
    {

        if (!isFromSearch)
            FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.RejectFriendRequest, FriendListTD.Instance.GetListOfFriendsRequests);
        else
            FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.RejectFriendRequest, FriendListTD.Instance.searchFriendsByPlayerID);
    }

    public void AddNewFriend()
    {
        if (statusStringFriendUnFriend == "local_reject_friend" || statusStringFriendUnFriend == "cancel_local_friend")
        {
            Debug.LogError("checkIncrementedId" + IncrementedID);
            if (LocalSettings.IsMenuScene())
                FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.Pending, FriendListTD.Instance.searchFriendsByPlayerID);
            else
                FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.Pending, FriendListTD.Instance.Emptyfunction);

        }
        else
        {
            if (LocalSettings.IsMenuScene())
                FriendListTD.Instance.AddNewFriend(LocalSettings.GetIncrementedPlayerID().ToString(), IncrementedID, FriendListTD.Instance.searchFriendsByPlayerID);
            else
                FriendListTD.Instance.AddNewFriend(LocalSettings.GetIncrementedPlayerID().ToString(), IncrementedID, FriendListTD.Instance.Emptyfunction);
            Debug.LogError("checkIncrementedId" + IncrementedID);
        }
    }

    public void CancelFriendRequest()
    {
        FriendListTD.Instance.ChangeStatusOfFriend(IncrementedID, APIStrings.CancelFriendRequest, FriendListTD.Instance.searchFriendsByPlayerID);

    }



    #endregion

    #region Retrieving Image from server using name
    private IEnumerator SetPic(string ImageName)
    {
        WWW www = new WWW(APIStrings.ImageURLAPI + ImageName); // Start downloading the image
        yield return www; // Wait for the download to complete
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error downloading image: " + www.error);
            yield break;
        }
        Debug.Log("image received");
        Texture2D texture = www.texture; // Get the downloaded texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), UnityEngine.Vector2.one * 0.5f);

        Pic.sprite = sprite;
        //onCompleteMethod.Invoke(sprite);
    }
    #endregion
}