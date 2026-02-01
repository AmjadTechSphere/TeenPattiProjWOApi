
using com.mani.muzamil.amjad;
using System;
using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryFields : MonoBehaviour
{

    public TMP_Text PendingStatustxt;
    public TMP_Text ReceiverNameTxt;
    public TMP_Text ReceiverIDTxt;
    public TMP_Text ReceiverAmountTxt;
    public TMP_Text DateAndTimeTxt;

    public Image ReceiverImage;

    public Image reCallIcon;
    public Sprite ResendSprite;
    public Sprite recallSprite;

    public Color[] Colors;
    //public Button ReceiveGoldBtn;

    string status;
    string playerName;
    string PlayerID;
    string sentAmount;
    int cancelIDToGetBackGold;


    // New
    string senderName;
    string senderID;
    string receiverName;
    string receiverID;
    public string sentOrReceivedAmount;

    string senderImageName;
    string receiverImageName;
    DateTime DateOfReceivedOrSent;


    //string receivedAmount;
    int receiveIDWhoSentGold;


    public Sprite senderImageTemp;
    public Sprite receiverImageTemp;

    bool isImSender;
    #region Gold sent to players and collected gold from other players filling fields 
    public void SetRecordFieldsOfSent(PlayerGoldSentRecord sentRecord, int indexToAccess)
    {
        if (sentRecord == null)
        {
            Debug.Log("No record found");
            return;
        }

        if (sentRecord.players != null)
        {
            if (sentRecord.players.Count > 0)
            {
                PlayerSenderSent SentRecord = sentRecord.players[indexToAccess];
                senderImageName = SentRecord.sender.image;
                receiverImageName = SentRecord.player.image;

                senderName = SentRecord.sender.username;
                senderID = SentRecord.sender.playerID;
                receiverName = SentRecord.player.username;
                receiverID = SentRecord.player.playerID;
                sentOrReceivedAmount = SentRecord.chips;
                int senderIDD = int.Parse(SentRecord.sender_id);
                //Debug.LogError("Sent record status:    _________:   " + SentRecord.status);
                //if (senderIDD == SentRecord.sender.id)
                if (senderIDD == LocalSettings.GetIncrementedPlayerID())
                {
                    // Sender is me
                    cancelIDToGetBackGold = SentRecord.id;
                    isImSender = true;
                    if (LocalSettings.GetPlayerStatus() == "dealer" && SentRecord.status == "pending")
                    {
                        // I am dealer
                        reCallIcon.sprite = recallSprite;
                        PendingStatustxt.text = "Waiting To Accept";
                        status = SentRecord.status;
                        PendingStatustxt.color = Colors[0];
                    }
                    else
                    {
                        // No i am not dealer
                        reCallIcon.sprite = ResendSprite;
                        if (SentRecord.status == "pending")
                        {
                            PendingStatustxt.text = "Waiting To Accept";
                            status = SentRecord.status;
                            PendingStatustxt.color = Colors[0];
                        }
                        else if (SentRecord.status == "rejected")
                        {
                            PendingStatustxt.text = "Recalled";
                            status = SentRecord.status;
                            PendingStatustxt.color = Colors[1];
                        }
                        else
                        {
                            PendingStatustxt.text = "Sent to";
                            status = SentRecord.status;
                            PendingStatustxt.color = Colors[4];
                        }

                    }
                    RetrieveImageFromDB(receiverImageName);
                    DateOfReceivedOrSent = SentRecord.created_at;
                    //Debug.LogError("Created at: " + SentRecord.created_at);
                    ReceiverNameTxt.text = receiverName;
                    ReceiverIDTxt.text = receiverID;
                }
                else
                {
                    // Sender is other, I am receiver
                    isImSender = false;
                    reCallIcon.GetComponent<Button>().interactable = false;
                    // reCallIcon.color = Colors[3];
                    PendingStatustxt.text = "Gold Collected";
                    status = SentRecord.status;
                    PendingStatustxt.color = Colors[2];
                    DateOfReceivedOrSent = SentRecord.updated_at;
                    RetrieveImageFromDB(senderImageName);
                    ReceiverNameTxt.text = senderName;
                    ReceiverIDTxt.text = senderID;
                }
                DateAndTimeTxt.text = DateOfReceivedOrSent.ToString();
                ReceiverAmountTxt.text = LocalSettings.Rs(sentOrReceivedAmount);
            }
        }
    }


    //public void SetRecordFieldsOfSent(string pendingStatus, string name, string playrID, string Amount, string dateAndTime, string picName, int IDToCancel)
    //{
    //    status = pendingStatus;
    //    playerName = name;
    //    PlayerID = playrID;
    //    sentAmount = Amount;
    //    cancelIDToGetBackGold = IDToCancel;
    //    //Debug.LogError("Cancel id: " + cancelIDToGetBackGold);

    //    PendingStatustxt.text = pendingStatus;
    //    ReceiverNameTxt.text = name;
    //    ReceiverIDTxt.text = playrID;
    //    ReceiverAmountTxt.text = LocalSettings.Rs(Amount);
    //    DateAndTimeTxt.text = dateAndTime;

    //    RetrieveImageFromDB(picName);
    //    if (LocalSettings.GetPlayerStatus() == "dealer" && status == "pending")
    //    {
    //        reCallIcon.sprite = recallSprite;
    //    }
    //    else
    //    {
    //        reCallIcon.sprite = ResendSprite;
    //    }
    //}
    #endregion

    #region gold received, sent from others 
    public void SetFieldsOfCollectGold(string pendingStatus, string name, string playerID, string Amount, string dateAndTime, string picName, int IDToReceive)
    {
        status = pendingStatus;
        playerName = name;
        PlayerID = playerID;
        //receivedAmount = Amount;
        sentOrReceivedAmount = Amount;
        receiveIDWhoSentGold = IDToReceive;


        ReceiverNameTxt.text = playerName;
        ReceiverIDTxt.text = PlayerID;
        ReceiverAmountTxt.text = LocalSettings.Rs(Amount);
        DateAndTimeTxt.text = dateAndTime;
        if (status == "pending")
        {
            //ReceiveGoldBtn.interactable = true;
            PendingStatustxt.text = "Collect";

        }
        else
        {
            //ReceiveGoldBtn.interactable = false;
            //PendingStatustxt.text = "Collected";
            gameObject.SetActive(false);
        }
        RetrieveImageFromDB(picName);
    }
    #endregion
    public void RecallOrResend()
    {
        if (LocalSettings.GetPlayerStatus() == "dealer" && status == "pending")
        {
            // Call api to 
            recallAmount();
        }
        else
        {
            Debug.Log("Recalling btn:  " + receiverID);
            if (receiverID != "")
                GoldTransfer.Instance.otherPlayerID.text = receiverID;

        }
    }
    public void recallAmount()
    {
        goldtransferhistory.Instance.RecallIDToGetBackAmount = cancelIDToGetBackGold.ToString();
        goldtransferhistory.Instance.RecallConfirmPanel.gameObject.SetActive(true);
        //goldtransferhistory.Instance.RecallGoldVIPMember(cancelIDToGetBackGold.ToString());
        Debug.Log("Cancel id    2: " + cancelIDToGetBackGold);
    }

    public void CollectAmountSentFromOther()
    {
        // API called to collect gold
        goldtransferhistory.Instance.AcceptGoldSentFromOther(receiveIDWhoSentGold.ToString(), LocalSettings.StringToBigInteger(sentOrReceivedAmount));
    }

    public void showReceipt()
    {
        //GoldTransfer.Instance.SetReceiptVariables(status, GoldTransfer.Instance.SenderImage.sprite, ReceiverImage.sprite, LocalSettings.GetPlayerName(), senderID: LocalSettings.GetPlayerID().ToString(), playerName, PlayerID, receivedAmount, DateAndTimeTxt.text);

        if (senderImageTemp == null)
        {
            //senderImageTemp.sprite = UpdateAvatar.Instance.ProfileImage[1].sprite;
            senderImageTemp = UpdateAvatar.Instance.AvatorSpritesSquare.Sprites[0].Sprites;


        }
        else if (receiverImageTemp == null)
        {
            receiverImageTemp = UpdateAvatar.Instance.ProfileImage[0].sprite;


        }
        GoldTransfer.Instance.SetReceiptVariables(PendingStatustxt.text, senderImageTemp, receiverImageTemp, senderName, senderID, receiverName, receiverID, sentOrReceivedAmount, DateOfReceivedOrSent.ToString());


        GoldTransfer.Instance.ReceiptPanel.SetActive(true);
    }

    #region Image setting
    public void RetrieveImageFromDB(string imagePath)
    {
        if (gameObject.activeInHierarchy)
        {

            StartCoroutine(DownloadImageAndConvertToSprite(APIStrings.ImageURLAPI + imagePath));
        }
    }
    private IEnumerator DownloadImageAndConvertToSprite(string imageUrl)
    {
        if (!gameObject.activeInHierarchy)
        {
            yield return null;
        }
        WWW www = new WWW(imageUrl); // Start downloading the image
       // if (LocalSettings.IsMenuScene())
            GoldTransfer.Instance.LoadingPanel.gameObject.SetActive(true);
        yield return www; // Wait for the download to complete
       // if (LocalSettings.IsMenuScene())
            GoldTransfer.Instance.LoadingPanel.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error downloading image: " + www.error);
            yield break;
        }

        Texture2D texture = www.texture; // Get the downloaded texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), UnityEngine.Vector2.one * 0.5f);

        //imageDisplay.sprite = sprite; // Set the sprite on the Image component

        Debug.Log("Image Name: " + sprite.name + "       Successssss");

        ReceiverImage.sprite = sprite;
        if (isImSender)
        {
            receiverImageTemp = sprite;
        }
        else
            senderImageTemp = sprite;
    }
    #endregion
}
