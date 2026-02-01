using com.mani.muzamil.amjad;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class GoldTransfer : ES3Cloud
{
    public const string GuestString = "guest";
    public const string PlayerString = "player";
    public const string DealerString = "dealer";
    public const string FreezeString = "freeze";

    public GameObject GoldTransferBtn;
    public GameObject GoldTransferPanel;
    public GameObject MessagePanel;
    public GameObject ConfirmTransferPanel;
    public GameObject ReceiptPanel;
    public GameObject LoadingPanel;



    public TMP_Text otherPlayerName;
    public TMP_Text PlayerTotalChips;
    public TMP_Text MessageText;
    public TMP_InputField otherPlayerID;
    public TMP_InputField ChipsToSendInputF;


    bool isInvalid;

    public const string ChipsToSend = "chips";
    public const string SenderIncrementedID = "sender_id";
    public const string ReceiverIncrementedID = "player_id";

    #region Confirm Transfer Variables
    [Header("Sender ")]
    public Image SenderImage;
    public TMP_Text SenderName;
    public TMP_Text SenderPlayerID;
    public TMP_Text SendingAmount;
    public TMP_Text SenderServiceCharges;

    [Header("Receiver")]
    public Image ReceiverImage;
    public TMP_Text ReceiverName;
    public TMP_Text ReceiverPlayerID;
    public TMP_Text ReceiverAmount;
    public TMP_Text ReceiverServiceCharges;

    BigInteger chipsToTransfer;
    #endregion


    #region Receipt Variables

    public TMP_Text ReceivingStatusReceipt;

    public Image SenderImageReceipt;
    public TMP_Text SenderNameReceipt;
    public TMP_Text SenderPlayerIDReceipt;

    public Image ReceiverImageReceipt;
    public TMP_Text ReceiverNameReceipt;
    public TMP_Text ReceiverPlayerIDReceipt;
    public TMP_Text TransferedAmountReceipt;

    public TMP_Text DateAndTimeReceipt;

    #endregion

    MyPlayerData otherPlayerData = new MyPlayerData();


    // instance Creating of Gold Transfer Script
    #region Creating Instance;
    private static GoldTransfer _instance;
    public static GoldTransfer Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GoldTransfer>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    protected GoldTransfer(string url, string apiKey) : base(url, apiKey)
    {

    }
    #endregion

    void Start()
    {
        if (LocalSettings.IsMenuScene())
            Invoke(nameof(SetGoldTransferPanel), 1);
        GoldTransferPanel.SetActive(false);
        ConfirmTransferPanel.SetActive(false);
        ReceiptPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        PlayerTotalChips.text = LocalSettings.Rs(LocalSettings.GetTotalChips());

    }


    public void SetGoldTransferPanel()
    {
        //if (LocalSettings.GetAuthType() == 2 || LocalSettings.GetAuthType() == 3)
        GoldTransferBtn.SetActive(true);
        //else
        //    GoldTransferBtn.SetActive(false);

    }

    void GetOtherPlayerDetail(MyPlayerData ReceiverPlayerData)
    {
        if (ReceiverPlayerData.player != null)
        {
            if (ReceiverPlayerData.player.username != null)
            {
                if (ReceiverPlayerData.player.playerID == LocalSettings.GetPlayerID().ToString())
                {
                    otherPlayerName.text = "Can't send to My Self";
                    otherPlayerName.color = Color.red;
                    isInvalid = true;
                    return;
                }
                otherPlayerName.text = ReceiverPlayerData.player.username;
                otherPlayerName.color = Color.green;
                otherPlayerData = ReceiverPlayerData;
                Debug.Log("name is: " + otherPlayerName);
            }
        }

        else
        {
            otherPlayerName.text = "Invalid ID";
            otherPlayerName.color = Color.red;
        }
    }


    public void OnValueChangeOfPlayerID()
    {
        string othrPlyrID = otherPlayerID.text;
        if (othrPlyrID.Length == 8)
        {
            Debug.Log("8 complete;");
            RestAPI.Instance.FetchData(othrPlyrID, GetOtherPlayerDetail);
        }
    }

    public void ShowConfirmTransfer()
    {
        string othrPlyrID = otherPlayerID.text;
        if (othrPlyrID.Length < 8 || otherPlayerName.text == "Invalid ID")
        {
            showMessage("User ID does not exist");
            return;
        }
        else if (otherPlayerName.text == "Can't send to My Self")
        {
            showMessage("Can't send to Your Self");
            return;
        }
        if (ChipsToSendInputF.text == "please enter the amount" || ChipsToSendInputF.text == "")
        {

            //showMessage("Please Enter Amount in Cr");
            showMessage("Please Enter Amount in Cr");
            return;
        }

        double sendGoldValue = double.Parse(ChipsToSendInputF.text);
        if (sendGoldValue < 1)
        {
            showMessage("You can send minimum 1 Cr");
            return;
        }

        sendGoldValue *= 10000000;
        string goldValue = sendGoldValue.ToString();
        Debug.Log("value of Send Gold Value..." + sendGoldValue);
        // chipsToTransfer = BigInteger.Parse(ChipsToSendInputF.text);
        chipsToTransfer = BigInteger.Parse(goldValue);
        if (chipsToTransfer > LocalSettings.GetTotalChips())
        {
            showMessage("You Dont have enough chips");
            MessagePanel.SetActive(true);
            return;
        }
        ConfirmTransferPanel.gameObject.SetActive(true);
        FillConfirmForm();
    }

    public void GoldTranferForGoldProtection(string aaaa)
    {
        SendGold();
    }

    public void SendGold()
    {
        if (LocalSettings.isPasswordRequired && !LocalSettings.IsPasswordChecked)
        {
            GoldProtection.Instance.AuthenticateWithPassword(GoldTranferForGoldProtection, "tempJustForFieldFill");
            return;
        }
        LoadingPanel.SetActive(true);
        string senderID = LocalSettings.GetIncrementedPlayerID().ToString();
        string receiverID = otherPlayerData.player.id.ToString();
        SendChipsToOtherPlayer(senderID, receiverID, chipsToTransfer.ToString());

    }
    BigInteger receivedAmount = 0;
    void FillConfirmForm()
    {
        // Setting sender data
        //BigInteger receivedAmount = 0;

        SenderImage.sprite = LocalSettings.IsMenuScene() ? UpdateAvatar.Instance.ProfileImage[0].sprite : LocalSettings.ServerSideImge;

        SenderName.text = LocalSettings.GetPlayerName();
        SenderPlayerID.text = "ID: " + LocalSettings.GetPlayerID();
        SendingAmount.text = LocalSettings.Rs(chipsToTransfer);
        if (LocalSettings.GetPlayerStatus() == DealerString)
        {
            SenderImage.transform.GetChild(0).gameObject.SetActive(true);
            receivedAmount = chipsToTransfer;
            ReceiverServiceCharges.text = "0";
            ReceiverServiceCharges.text = "This transfer is free of service charge as " + LocalSettings.GetPlayerName() + " owns a Free Gold Transfer (30 Days)";
        }
        else
        {
            SenderImage.transform.GetChild(0).gameObject.SetActive(false);
            receivedAmount = (chipsToTransfer / 100) * 10;
            SenderServiceCharges.text = LocalSettings.Rs(receivedAmount);
            ReceiverServiceCharges.text = "This transfer has " + LocalSettings.Rs(receivedAmount) + " service charge as " + LocalSettings.GetPlayerName() + " is not a VIP member";
            receivedAmount = chipsToTransfer - receivedAmount;
        }


        // Setting reveicer data

        RestAPI.Instance.RetrieveImageFromDB(otherPlayerData.player.image.ToString(), showProfileImg);
        LocalSettings.ProfilePicName = otherPlayerData.player.image.ToString();
        NetworkSettings.Instance.AssignPicToPlayerPropertiesStringForm(LocalSettings.ProfilePicName);
        ReceiverName.text = otherPlayerData.player.username;
        ReceiverPlayerID.text = "ID: " + otherPlayerData.player.playerID;

        ReceiverAmount.text = LocalSettings.Rs(receivedAmount);

    }
    public void showProfileImg(Sprite sprite)
    {
        ReceiverImage.sprite = sprite;
        ReceiverImage.sprite.name = "Profile Image";
        ReceiverImage.name = "Profile Image";
        //SetReceiptVariables("Waiting to be collected", SenderImage.sprite, ReceiverImage.sprite, LocalSettings.GetPlayerName(), LocalSettings.GetPlayerID().ToString(), otherPlayerData.player.username, otherPlayerData.player.playerID, ReceiverAmount.text, System.DateTime.Now.ToString());
        SetReceiptVariables("Waiting to be collected", SenderImage.sprite, ReceiverImage.sprite, LocalSettings.GetPlayerName(), LocalSettings.GetPlayerID().ToString(), otherPlayerData.player.username, otherPlayerData.player.playerID, receivedAmount.ToString(), System.DateTime.Now.ToString());

        Texture2D pic = SpriteToTexture2D(sprite);
        if (LocalSettings.IsMenuScene())
            Menu_Manager.Instance.ProfileImageTexture = pic;
    }
    //private Texture2D SpriteToTexture2D(Sprite sprite)
    //{
    //    // Get the Sprite's texture
    //    Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
    //    texture.SetPixels(sprite.texture.GetPixels(
    //        (int)sprite.textureRect.x,
    //        (int)sprite.textureRect.y,
    //        (int)sprite.textureRect.width,
    //        (int)sprite.textureRect.height));
    //    texture.Apply();

    //    return texture;
    //}
    private Texture2D SpriteToTexture2D(Sprite sprite)
    {
        // Get the Sprite's texture
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

        // Calculate the offset to copy pixels from the sprite's texture
        int xOffset = Mathf.RoundToInt(sprite.rect.x);
        int yOffset = Mathf.RoundToInt(sprite.rect.y);

        texture.SetPixels(sprite.texture.GetPixels(xOffset, yOffset, texture.width, texture.height));
        texture.Apply();

        return texture;
    }
    public void showMessage(string message)
    {
        MessageText.text = message;
        MessagePanel.SetActive(true);
    }

    #region Sending Gold to other Player

    public void SendChipsToOtherPlayer(string Sender_IncrementedID, string receiver_IncrementedID, string amountToSend)
    {
        Debug.Log("<color=yellow> Sending gold</color>");

        StartCoroutine(SendChipsToOtherPlayerUsingAPI(Sender_IncrementedID, receiver_IncrementedID, amountToSend));
    }
    public IEnumerator SendChipsToOtherPlayerUsingAPI(string Sender_IncrementedID, string receiver_IncrementedID, string amountToSend)
    {

        string url = APIStrings.SendGoldWithPlayerIDAPIURL;
        formData = new List<KeyValuePair<string, string>>();
        Debug.Log("check your URl" + url + APIStrings.SendGoldWithPlayerIDAPIURL);
        AddPOSTField(SenderIncrementedID, Sender_IncrementedID);
        AddPOSTField(ReceiverIncrementedID, receiver_IncrementedID);
        AddPOSTField(ChipsToSend, amountToSend);

        WWWForm form = CreateWWWForm();

        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    //FetchData(tokenID, Menu_Manager.Instance.SetUserNameAndOtherThings);
                }
                Debug.LogError("Player chips sent failed ");
                int responseCode = (int)webRequest.responseCode;
                string responseText = webRequest.downloadHandler.text;
                showMessage("Transfer Failed. Try Again Later");
                //Debug.LogError("response COde: " + responseCode + "     : Response text: " + responseText);
            }
            else
            {
                Debug.Log("Player chips sent successfully ");
                ReceiptPanel.SetActive(true);
                DateAndTimeReceipt.text = System.DateTime.Now.ToString();

                if (goldtransferhistory.Instance != null)
                    goldtransferhistory.Instance.GetPlayerGoldSentRecord();
                else
                {
                    Debug.Log("Instance issue");
                    if (LocalSettings.IsMenuScene())
                        RestAPI.Instance.FetchData(LocalSettings.GetTokenID(), Menu_Manager.Instance.SetUserNameAndOtherThings);
                }
            }
            LoadingPanel.SetActive(false);

        }

    }
    #endregion
    #region Receipt 

    public void SetReceiptVariables(string CollectedStatus, Sprite senderImage, Sprite receiverImage, string senderName, string senderID, string receiverName, string receiverID, string transferedAmount, string transferedDateTime)
    {
        ReceivingStatusReceipt.text = CollectedStatus;
        SenderImageReceipt.sprite = senderImage;
        ReceiverImageReceipt.sprite = receiverImage;
        SenderNameReceipt.text = senderName;
        SenderPlayerIDReceipt.text = senderID;

        ReceiverNameReceipt.text = receiverName;
        ReceiverPlayerIDReceipt.text = receiverID;
        TransferedAmountReceipt.text = LocalSettings.Rs(transferedAmount);
        DateAndTimeReceipt.text = transferedDateTime;

    }

    /// <summary>
    /// Old method Obslete
    /// </summary>
    public void ShareReceipt()
    {
        string filename = "screenshot";
        ShareScreenshot();

    }

    void ShareScreenshot()
    {
        // Check for external storage permission
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            return;
        }
        showMessage("taking ss");
        // Capture screenshot
        Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] screenshotBytes = screenshotTexture.EncodeToPNG();

        // Save screenshot to a temporary path
        string tempPath = Path.Combine(Application.persistentDataPath, "tempScreenshot.png");
        File.WriteAllBytes(tempPath, screenshotBytes);

        // Create intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "image/png");

        // Attach the screenshot
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + tempPath);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

        // Set package name for WhatsApp
        intentObject.Call<AndroidJavaObject>("setPackage", "com.whatsapp");

        // Start activity
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
        showMessage("Sharing done");
    }
    #endregion

    #region Json To c# Classes

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class PlayerGoldTransfer
    {
        public string chips { get; set; }
        public string commission { get; set; }
        public string sender_id { get; set; }
        public string player_id { get; set; }
        public string status { get; set; }
        public string transaction { get; set; }
        public int id { get; set; }
    }

    public class GoldTransferAPI
    {
        public string success { get; set; }
        public PlayerGoldTransfer player { get; set; }
    }


    #endregion
}
