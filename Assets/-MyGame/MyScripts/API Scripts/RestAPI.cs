using com.mani.muzamil.amjad;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestAPI : ES3Cloud//MonoBehaviour
{
    [Header("Profile Pic Related Data")]
    public Sprite ProfilePic;
    public Image ProfilePicImage;

    public const string Image = "image";
    public const string UserName = "username";
    public const string Email = "email";
    public const string PhoneNumber = "phone_number";
    public const string DeviceID = "deviceId";

    public const string TokenID = "token_id";
    public const string AuthType = "auth_type";
    public const string Status = "status";
    public const string PlayerID = "playerID";
    public GameObject VIPStatus;

    //public string PlayerTokenID = ;
    public int id { get; set; }
    Action<Sprite> SetProfileImage;

    [Header("Device Id")]
    [ShowOnly]
    public string MyDeviceId;

    public MyPlayerData myPlayerData = new MyPlayerData();
    protected RestAPI(string url, string apiKey) : base(url, apiKey)
    {

    }

    // instance Creating of Rest API Script
    #region Creating Instance;
    private static RestAPI _instance;
    public static RestAPI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RestAPI>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        GetDeviceUniqueID();
        SetProfileImage += UpdateProfileImageAfterReceivingFromServer;

    }
    public void GetAndSetPlayerDetail()
    {

        FetchData(LocalSettings.GetTokenID(), LocalSettings.IsMenuScene() ? Menu_Manager.Instance.SetUserNameAndOtherThings : setUserThingForGamePlayGoldTransfer);
    }
    public void GetAndSetPlayerDetailForGamePlay()
    {
        if (!LocalSettings.IsMenuScene())
            goldtransferhistory.Instance.GetPlayerGoldSentRecord();

    }

    void setUserThingForGamePlayGoldTransfer(MyPlayerData myPlayerData)
    {
        StartCoroutine(SetPlayerAmountAndOTherThingsInGamePlayScene(myPlayerData));
    }

    IEnumerator SetPlayerAmountAndOTherThingsInGamePlayScene(MyPlayerData myPlayerData)
    {
        if (LocalSettings.IsMenuScene())
        {
            yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
        }
        if (myPlayerData.player == null)
            yield return null;



        // set server chips to local settings
        LocalSettings.SetTotalServerChips(myPlayerData.total_chips);
        XPLevelCalculator.Instance.UpDateXpFromServer(myPlayerData.player.xp);
        LocalSettings.TotalXpMyPlayer = int.Parse(myPlayerData.player.xp);
        UIManager.Instance.PlayerTotalChipsTxt.text = LocalSettings.Rs(LocalSettings.GetTotalChips());

        UIManager.Instance.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
        if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
            UIManager.Instance.GetMyPlayerInfo().sendGoldTranferAndUpdateChips();


        Debug.Log("Text Update Succe..." + LocalSettings.Rs(LocalSettings.GetTotalChips()));

        LocalSettings.SetVIPStatus(myPlayerData.player.status);
        if (myPlayerData.player.status == "dealer")
        {
            VIPStatus.SetActive(true);

        }
        else
        {
            VIPStatus.SetActive(false);

        }

        GoldTransfer.Instance.PlayerTotalChips.text = LocalSettings.IsMenuScene() ? LocalSettings.Rs(myPlayerData.total_chips) : LocalSettings.Rs(LocalSettings.GetTotalChips());
        if (LocalSettings.IsMenuScene())
            GoldProtection.Instance.GetGoldProtectionDetail();
    }

    // Creating Player using API
    #region Creating Player
    public void CreatePlayer(string authType, string tokenID, string deviceID, string playerName, string emailID, string status, Image profilePic)
    {
        Debug.LogError("<color=yellow>API Called _____________________22222_______  " + Time.timeScale + "</color>");

        StartCoroutine(SendPlayerDataToAPIToCreatePlayer(authType, tokenID, deviceID, playerName, emailID, status, profilePic));
    }
    public IEnumerator SendPlayerDataToAPIToCreatePlayer(string authType, string tokenID, string deviceID, string playerName, string emailID, string status, Image profilePic)
    {
        LoginWithAllAuth.Instance.ProgressTxt.text = "Verifying User...";
        string url = APIStrings.CreateUserAPIURL;
        formData = new List<KeyValuePair<string, string>>();

        AddPOSTField(UserName, playerName);
        AddPOSTField(TokenID, tokenID);
        AddPOSTField(DeviceID, deviceID);
        AddPOSTField(Email, emailID);
        AddPOSTField(Status, status);
        AddPOSTField(AuthType, authType);

        Debug.LogError("authType: " + authType + "   tokenID: " + tokenID + "     deviceID: " + deviceID + "     playerName: " + playerName + "     emailID: " + emailID + "     status: " + status);
        WWWForm form = CreateWWWForm();
        byte[] imageBytes = GetSpriteBytes(profilePic.sprite);
        form.AddBinaryData(Image, imageBytes);
        Debug.LogError("Posting form...");
        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Result: " + webRequest.result);
                LoginWithAllAuth.Instance.AuthPanel.SetActive(true);
                LoginWithAllAuth.Instance.WaitPanel.SetActive(false);
                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    FetchData(tokenID, Menu_Manager.Instance.SetUserNameAndOtherThings);
                }
            }
            else
            {
                Debug.LogError("Player created successfully, Data is uploaded successfully ");
                LoginWithAllAuth.Instance.AuthPanel.SetActive(false);
                LoginWithAllAuth.Instance.WaitPanel.SetActive(false);
                FetchData(tokenID, Menu_Manager.Instance.SetUserNameAndOtherThings);
            }

        }

    }
    #endregion

    // Editing Player Detail
    #region Creating Player
    public void EditPlayerDetail(string newname, string emailID, Image profilePic)
    {
        StartCoroutine(EditPlayerDetailToAPI(newname, emailID, profilePic));
    }
    public IEnumerator EditPlayerDetailToAPI(string newname, string emailID, Image profilePic)
    {
        LoginWithAllAuth.Instance.ProgressTxt.text = "Editing User Detail ...";
        string url = APIStrings.EditUserDetailURLAPI;
        formData = new List<KeyValuePair<string, string>>();

        AddPOSTField(TokenID, LocalSettings.GetTokenID());
        if (newname != "")
            AddPOSTField(UserName, newname);

        if (emailID != "")
            AddPOSTField(Email, emailID);

        WWWForm form = CreateWWWForm();
        if (profilePic != null)
        {
            byte[] imageBytes = GetSpriteBytes(profilePic.sprite);
            form.AddBinaryData(Image, imageBytes);
        }
        Debug.Log("Posting form...");
        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Result: " + webRequest.result);
                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    FetchData(LocalSettings.GetTokenID(), Menu_Manager.Instance.SetUserNameAndOtherThings);
                }
            }
            else
            {
                Debug.Log("Player created successfully, Data is uploaded successfully ");
                FetchData(LocalSettings.GetTokenID(), Menu_Manager.Instance.SetUserNameAndOtherThings);
            }
        }
    }
    #endregion

    // Get Player Detail using Token ID
    #region Getting Player Detail

    public void FetchData(string tokenIDFromServer, Action<MyPlayerData> onDataReceived)
    {
        if (LocalSettings.IsMenuScene())
            LoginWithAllAuth.Instance.ProgressTxt.text = "Fetching User Data...";
        StartCoroutine(GetPlayerDetailAPI(tokenIDFromServer, onDataReceived));
    }
    IEnumerator GetPlayerDetailAPI(string tokenIDFromServer, Action<MyPlayerData> onDataReceived)
    {

        yield return new WaitForSecondsRealtime(1f);
        if (LocalSettings.IsMenuScene())
            if (!Menu_Manager.Instance.ModePanel())
                GoldTransfer.Instance.LoadingPanel.SetActive(true);
        string url = APIStrings.GetUserDetailAPIURL + tokenIDFromServer;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            myPlayerData = new MyPlayerData();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error is: " + webRequest.result);
                onDataReceived?.Invoke(myPlayerData);
            }
            //else
            //{
            //    Debug.Log("success is: " + webRequest.result);
            //}
            string jsonString = webRequest.downloadHandler.text;

            //Debug.Log(jsonString);

            //if (myPlayerData.player == null)
            //    yield return null;
            //Debug.Log("cehck Jason FIle String:       " + jsonString);
            Debug.LogError("user not: " + jsonString);
            if (jsonString.Contains("oops!, player not found"))
            {
                //Menu_Manager.Instance.LogOut();
                //PhotonNetwork.LoadLevel(0);
                yield break;
            }
            myPlayerData = JsonConvert.DeserializeObject<MyPlayerData>(jsonString);
            GoldTransfer.Instance.LoadingPanel.SetActive(false);
            yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
            //if (myPlayerData.player != null)
            //{
            //    Debug.Log("Player ID: " + myPlayerData.player.playerID + "\n" + "User Name: " + myPlayerData.player.username + "\n" + "Email: " + myPlayerData.player.email + "\n" + "Device ID: " + myPlayerData.player.deviceId + "\n" + "Phone: " + myPlayerData.player.phone + "\n" + "Token ID: " + myPlayerData.player.token_id + "\n" + "Auth Type: " + myPlayerData.player.auth_type + "\n" + "Status: " + myPlayerData.player.status + "\n" + "Chips: " + myPlayerData.total_chips + "\n" + "XP: " + myPlayerData.total_xp + "\n" + "Diamond: " + myPlayerData.total_diamond + "\n" + "Image: " + myPlayerData.player.image + "\n");
            //}

            onDataReceived?.Invoke(myPlayerData);
        }
    }


    public static byte[] GetSpriteBytes(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        byte[] bytes = null;

        if (texture != null)
        {
            bytes = texture.EncodeToPNG();
        }
        return bytes;
    }

    public void RetrieveImageFromDB(string imagePath)
    {
        StartCoroutine(DownloadImageAndConvertToSprite(APIStrings.ImageURLAPI + imagePath, SetProfileImage));
    }

    public void RetrieveImageFromDB(string imagePath, Action<Sprite> onCompleteMethod)
    {
        StartCoroutine(DownloadImageAndConvertToSprite(APIStrings.ImageURLAPI + imagePath, onCompleteMethod));
    }

    private IEnumerator DownloadImageAndConvertToSprite(string imageUrl, Action<Sprite> onCompleteMethod)
    {
        WWW www = new WWW(imageUrl); // Start downloading the image

        yield return www; // Wait for the download to complete

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error downloading image: " + www.error);
            yield break;
        }

        Texture2D texture = www.texture; // Get the downloaded texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), UnityEngine.Vector2.one * 0.5f);

        //imageDisplay.sprite = sprite; // Set the sprite on the Image component

        onCompleteMethod.Invoke(sprite);
    }

    #endregion


    #region Gereral Functions
    void GetDeviceUniqueID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        if (string.IsNullOrEmpty(deviceID))
        {
            Debug.LogError("Device ID is not available on this platform.");
        }
        else
        {
            //Debug.Log("Device ID: " + deviceID);
            MyDeviceId = deviceID;

        }
    }
    public void UpdateProfileImageAfterReceivingFromServer(Sprite sprite)
    {
        ProfilePicImage.sprite = sprite;
        UpdateAvatar.Instance.AllAvatarSpriteSame(sprite);


    }
    #endregion

}
#region JSON to C#

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class MyPlayerData
{
    public string success { get; set; }
    public string total_chips { get; set; }
    public string total_diamond { get; set; }
    public string total_xp { get; set; }
    public PlayerData2 player { get; set; }
}

public class PlayerData2
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
    public string xp { get; set; }
    public string collected_at { get; set; }
    public List<PlayerDetail> player_details { get; set; }
}

public class PlayerDetail
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
}

#endregion