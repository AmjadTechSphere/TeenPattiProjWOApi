
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class PlayerGiftTransfer : MonoBehaviour
    {

        public GameObject gift1stPosOfPlayer;
        public GameObject gift2ndPosOfPlayer;
        [ShowOnly]
        public GameObject giftObject;
        public GameObject giftGameObject;
        public RectTransform giftInsPos;
        GameManager gameManager;

        PhotonView photonView
        {
            get
            {
                return GetComponent<PhotonView>();
            }
        }
        private PlayerInfo playerInfo
        {
            get
            {
                return transform.GetComponent<PlayerInfo>();
            }

        }
        GiftTranferManager giftTranferManager;
        UIManager uIManager;
        public bool sendToAll = false;
        public bool giftBool;

        [ShowOnly] public int senderID;
        [ShowOnly] public int receiverID;
        private void Start()
        {

            giftTranferManager = GiftTranferManager.Instance;
            uIManager = UIManager.Instance;
            gameManager = GameManager.Instance;
        }
        public void TransferGift()
        {
            if (playerInfo.currentPlayerStateRef.currentState == PlayerState.STATE.OutOfTable)
                return;


            senderID = uIManager.GetMyPlayerInfo().photonView.ViewID;
            receiverID = playerInfo.photonView.ViewID;

            photonView.RPC(nameof(AssingPosition), RpcTarget.All, senderID, receiverID);
            //photonView.RPC(nameof(AssingPosition), RpcTarget.All, senderID, receiverID);
            uIManager.GiftTranferpanel.SetActive(true);


        }

        [PunRPC]
        void AssingPosition(int senderID, int receiverID)
        {
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                if (gameManager.playersList[i].photonView.ViewID == senderID || gameManager.playersList[i].photonView.ViewID == receiverID)
                {
                    gameManager.playersList[i].playerGiftTransfer.senderID = senderID;
                    gameManager.playersList[i].playerGiftTransfer.receiverID = receiverID;
                   // Debug.LogError("sender Id...." + gameManager.playersList[i].playerGiftTransfer.senderID + "........receiverID...." + gameManager.playersList[i].playerGiftTransfer.receiverID);
                }
            }
        }


        public void CheckGiftQuality(bool istrue)
        {
            photonView.RPC(nameof(CheckGift), RpcTarget.All, istrue);
        }

        [PunRPC]
        void CheckGift(bool istrue)
        {
            giftBool = istrue;
        }


        public void TranferGiftToPlayers(int IndexSprite)
        {
            int giftCost = giftBool ? giftTranferManager.giftPopularCollection.giftPopularArray[IndexSprite].giftCost : giftTranferManager.giftAllCollection.giftPopularArray[IndexSprite].giftCost;

            if (LocalSettings.GetTotalChips() < giftCost)
            {
                uIManager.quickShop.SetActive(true);
                return;
            }
            else
            {
                gameManager.PlayerTotalChipsUpdate(giftCost);
                GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.gift, giftCost.ToString());
                uIManager.GetMyPlayerInfo().playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
                uIManager.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());

            }

            photonView.RPC(nameof(GiftInstantiateForPlayers), RpcTarget.All, IndexSprite, senderID, receiverID);
        }

        [PunRPC]
        void GiftInstantiateForPlayers(int indexSprite, int senderID, int receiverID)
        {
            playerId(senderID, receiverID, indexSprite);

        }

        public void playerId(int senderID, int ReceiverID, int indexSprite)
        {

            PlayerInfo senderpos = null;
            PlayerInfo receiverpos = null;



            for (int i = 0; i < gameManager.playersList.Count; i++)
            {


                if (gameManager.playersList[i].photonView.ViewID == senderID)
                    senderpos = gameManager.playersList[i];
                if (gameManager.playersList[i].photonView.ViewID == ReceiverID)
                    receiverpos = gameManager.playersList[i];

            }

            if (senderpos.currentPlayerStateRef.currentState == PlayerState.STATE.OutOfTable || receiverpos.currentPlayerStateRef.currentState == PlayerState.STATE.OutOfTable)
                return;

            senderpos.playerTotalCash.text = LocalSettings.Rs(senderpos.player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
            if (receiverpos.playerGiftTransfer.giftObject != null)
                Destroy(receiverpos.playerGiftTransfer.giftObject);
            receiverpos.playerGiftTransfer.giftObject = Instantiate(giftGameObject);
            LocalSettings.SetPosAndRect(receiverpos.playerGiftTransfer.giftObject, giftTranferManager.giftInspos, gameManager.PlayerTable.transform.parent.transform);
            receiverpos.playerGiftTransfer.giftObject.transform.GetComponent<Image>().sprite = giftBool ? giftTranferManager.giftPopularCollection.giftPopularArray[indexSprite].Sprites : giftTranferManager.giftAllCollection.giftPopularArray[indexSprite].Sprites;

            if (giftBool)
            {
                receiverpos.playerGiftTransfer.giftObject.transform.GetComponent<Image>().sprite = giftTranferManager.giftPopularCollection.giftPopularArray[indexSprite].Sprites;
                //SoundManager.Instance.PlayAudioClip(giftTranferManager.giftPopularCollection.giftPopularArray[indexSprite].GiftSound, false);
                receiverpos.playerGiftTransfer.giftObject.transform.GetComponent<GiftTransferAnimScript>().giftSound = giftTranferManager.giftPopularCollection.giftPopularArray[indexSprite].GiftSound;
            }
            else
            {
                receiverpos.playerGiftTransfer.giftObject.transform.GetComponent<Image>().sprite = giftTranferManager.giftAllCollection.giftPopularArray[indexSprite].Sprites;
                //SoundManager.Instance.PlayAudioClip(giftTranferManager.giftAllCollection.giftPopularArray[indexSprite].GiftSound, false);
                receiverpos.playerGiftTransfer.giftObject.transform.GetComponent<GiftTransferAnimScript>().giftSound = giftTranferManager.giftAllCollection.giftPopularArray[indexSprite].GiftSound;
            }

            // receiverpos.playerGiftTransfer.giftObject.GetComponent<RectTransform>().localScale *= 100;
            receiverpos.playerGiftTransfer.giftObject.transform.GetComponent<GiftTransferAnimScript>().ReceiverID = receiverpos.photonView.ViewID;
            receiverpos.playerGiftTransfer.giftObject.transform.GetComponent<GiftTransferAnimScript>().SenderID = senderpos.photonView.ViewID;
            receiverpos.playerGiftTransfer.giftObject.transform.GetComponent<GiftTransferAnimScript>().targetPos = receiverpos.gameObject;
            //RectTransform MyPlayerRectTransform = receiverpos.playerGiftTransfer.giftObject.GetComponent<RectTransform>();
            //RectTransform ALReadyObjPos = senderpos.playerGiftTransfer.giftInsPos;
            //MyPlayerRectTransform.localScale = ALReadyObjPos.localScale;
            //MyPlayerRectTransform.localPosition = ALReadyObjPos.localPosition;
            //MyPlayerRectTransform.anchorMin = ALReadyObjPos.anchorMin;
            //MyPlayerRectTransform.anchorMax = ALReadyObjPos.anchorMax;
            //MyPlayerRectTransform.anchoredPosition = ALReadyObjPos.anchoredPosition;
            //MyPlayerRectTransform.sizeDelta = ALReadyObjPos.sizeDelta;
            //MyPlayerRectTransform.localRotation = ALReadyObjPos.localRotation;
            receiverpos.playerGiftTransfer.giftObject.SetActive(true);

        }

        public void forAllPlayerSendGift(int indexSprite)
        {
            GameManager gameManager = GameManager.Instance;

            for (int i = 0; i < gameManager.position_availability.Length; i++)
            {

                for (int j = 0; j < gameManager.playersList.Count; j++)
                {
                   // Debug.LogError("Check here Gift Error..........");
                    if (gameManager.playersList[j].GetComponent<RectTransform>().position == gameManager.position_availability[i].Pos.position)
                    {
                        photonView.RPC(nameof(GiftInstantiateForPlayers), RpcTarget.All, indexSprite, uIManager.GetMyPlayerInfo().photonView.ViewID, gameManager.playersList[j].GetComponent<PlayerInfo>().photonView.ViewID);
                    }



                }

                //if (gameManager.position_availability[i].is_reserved != null)
                //{
                //    gameManager.position_availability[i].is_reserved.GetComponent<PlayerGiftTransfer>().TranferGiftToPlayers(indexSprite);

                //    photonView.RPC(nameof(GiftInstantiateForPlayers), RpcTarget.All, indexSprite, uIManager.GetMyPlayerInfo().photonView.ViewID, gameManager.position_availability[i].is_reserved.GetComponent<PlayerInfo>().photonView.ViewID);

                //}
            }
            sendToAll = false;
            giftTranferManager.markSign.SetActive(sendToAll);


        }

    }
}