using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace com.mani.muzamil.amjad
{
    public class PokerManager : MonoBehaviour
    {
        public GameObject[] objectsToDisable;
        public GameObject[] objectsToEnable;
        public RectTransform[] pokerPositions;

        public GameObject BuyInCashPanel;

        public Transform FinalPokerBetAmountPoint;
        public RectTransform dummyCardInitialPos;
        PlayerStateManager psm;
        PhotonView photonView;
        [HideInInspector] public int sitPosAfterReset;
        #region Creating Instance
        private static PokerManager _instance;
        public static PokerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<PokerManager>();
                return _instance;
            }
        }
        #endregion
        private void Awake()
        {
            if (!MatchHandler.IsPoker())
            {
                gameObject.SetActive(false);
                return;
            }
            _instance = this;

        }
        // Start is called before the first frame update
        void Start()
        {
            if (!MatchHandler.IsPoker())
                return;
            SettingPlanOfPokerGame();
            LocalSettings.SetPosAndRect(Pot.instance.PotPanel, FinalPokerBetAmountPoint.GetComponent<RectTransform>(), Pot.instance.PotPanel.transform.parent);
            psm = PlayerStateManager.Instance;
            photonView = GetComponent<PhotonView>();
            LocalSettings.ToggleObjectState(objectsToDisable, false);
            LocalSettings.ToggleObjectState(objectsToEnable, true);
        }



        void SettingPlanOfPokerGame()
        {
            for (int i = 1; i < 5; i++)
            {

                LocalSettings.SetPosAndRect(GameManager.Instance.position_availability[i].Pos.gameObject, pokerPositions[i], GameManager.Instance.PlayerTable);


            }
        }

        public void SetStartingAmount()
        {
            BigInteger number = LocalSettings.GetPokerBuyInChips();
            if (PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.LocalPlayer.SetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey, number);
        }


        public void AdjustCurrentAmount()
        {

        }

        [ShowOnly] public int[] PokerRandomArrayCards = new int[52];
        public void SettingRandomCardsArrayToRoomProperty()
        {
            Array.Clear(PokerRandomArrayCards, 0, PokerRandomArrayCards.Length);
            PokerRandomArrayCards = new int[52];
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < GameManager.Instance.AllCards.Card.Length; i++)
                {
                    PokerRandomArrayCards[i] = i;
                }
                Shuffle(PokerRandomArrayCards);
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    if (PhotonNetwork.IsConnectedAndReady)
                        LocalSettings.GetCurrentRoom.SetCardsList(LocalSettings.Poker_card_listKey, PokerRandomArrayCards);
                    photonView.RPC(nameof(RandomCardsArrayRPC), RpcTarget.All, PokerRandomArrayCards);
                }
            }
        }

        [PunRPC]
        public void RandomCardsArrayRPC(int[] randArray)
        {
            //Array.Clear(PokerRandomArrayCards, 0, PokerRandomArrayCards.Length);
            //PokerRandomArrayCards = new int[52];
            Array.Copy(randArray, PokerRandomArrayCards, randArray.Length);
            DistributeHoleCards();
            GenerateCommunityCards();
        }

        void DistributeHoleCards()
        {
            int numberOfPlayers = PlayerStateManager.Instance.PlayingList.Count;
            for (int i = 0; i < numberOfPlayers; i++)
            {
                GenerateNewHoleCards(i);
            }
            PokerCheckWinner.Instance.CreateAndDistributeDummyPokerCards();
        }
        int temp_random_no = 0;
        void GenerateNewHoleCards(int playerIndex)
        {
            //temp_random_no = GetRandomNo(0, AllCards.Count);
            int cardIndex = PokerRandomArrayCards[temp_random_no];
            GameObject hole_card1 = Instantiate(GameManager.Instance.AllCards.Card[cardIndex].gameObject);
            PokerCheckWinner.Instance.HoleCardsToDestroy.Add(hole_card1);
            int card1 = cardIndex;
            temp_random_no++;

            //if(UIManager.Instance.GetMyPlayerInfo() != out)

            //temp_random_no = GetRandomNo(0, AllCards.Count);
            cardIndex = PokerRandomArrayCards[temp_random_no];
            GameObject hole_card2 = Instantiate(GameManager.Instance.AllCards.Card[cardIndex].gameObject);
            PokerCheckWinner.Instance.HoleCardsToDestroy.Add(hole_card2);
            int card2 = cardIndex;
            temp_random_no++;
            SetHoleCardForPlayer(hole_card1, hole_card2, playerIndex, card1, card2);
        }
        // Called in GenerateNewHoleCards #### 3
        void SetHoleCardForPlayer(GameObject hole_card1, GameObject hole_card2, int playerIndex, int card1, int card2)
        {

            //int 
            LocalSettings.SetPosAndRect(hole_card1, psm.PlayingList[playerIndex].card_1_RectTr, psm.PlayingList[playerIndex].card_1_RectTr.parent);
            LocalSettings.SetPosAndRect(hole_card2, psm.PlayingList[playerIndex].card_2_RectTr, psm.PlayingList[playerIndex].card_2_RectTr.parent);

            psm.PlayingList[playerIndex].Hole_Card1 = hole_card1.GetComponent<CardProperty>();
            psm.PlayingList[playerIndex].Hole_Card2 = hole_card2.GetComponent<CardProperty>();
            psm.PlayingList[playerIndex].player.SetCustomData(LocalSettings.pokerHoleCard1ForPlayer, card1);
            psm.PlayingList[playerIndex].player.SetCustomData(LocalSettings.pokerHoleCard2ForPlayer, card2);
            psm.PlayingList[playerIndex].Hole_Card1.gameObject.SetActive(false);
            psm.PlayingList[playerIndex].Hole_Card2.gameObject.SetActive(false);

        }


        void Shuffle(int[] array)
        {
            System.Random _random = new System.Random();
            int p = array.Length;
            for (int n = p - 1; n > 0; n--)
            {
                int r = _random.Next(0, n);
                int t = array[r];
                array[r] = array[n];
                array[n] = t;
            }
        }

        #region Reset Poker Game
        public void ResetPokerGame()
        {
            PlayerInfo playerInfo = UIManager.Instance.GetMyPlayerInfo();
            if (playerInfo)
                playerInfo.PokerPlayerCashAllIn = false;

            dropCardsIndex = 0;
            PokerActionPanel.Instance.isAllIn = false;
            foreach (GameObject obj in PokerCheckWinner.Instance.HoleCardsToDestroy)
            {
                if (obj)
                    Destroy(obj);
            }
            temp_random_no = 0;
            PokerActionPanel.Instance.CallAllInCheckBtnObj.SetActive(true);
            PokerActionPanel.Instance.BetRaiseSliderBtn.SetActive(true);
            PokerActionPanel.Instance.checkPokerBtn.SetActive(false);
            PokerActionPanel.Instance.StartForSelectAmount();
            PokerActionPanel.Instance.CurrentTargetBetAmount = 0;
            PokerActionPanel.Instance.amountPlacedOnBet = 0;
            PokerCheckWinner pcw = PokerCheckWinner.Instance;
            PokerActionPanel.Instance.BetAmountSlider.interactable = true;

            PokerActionPanel.Instance.pokerMinusBtn.interactable = true;
            PokerActionPanel.Instance.pokerPlusBtn.interactable = true;
            if (pcw.playersArray.Length > 0)
                Array.Clear(pcw.playersArray, 0, pcw.playersArray.Length);
            foreach (PlayerInfo pinfo in GameManager.Instance.playersList)
            {
                pinfo.PokerPlayerCashAllIn = false;
                pinfo.pokerTotalBetCash = 0;
                pinfo.PokerTotalCash = LocalSettings.GetPokerBuyInChips();
                pinfo.CashAllInIndicator.SetActive(false);
                pinfo.PokerPlayerCurrentRank = 0;
                pinfo.PokerScores = 0;
                pinfo.PokerTotalWholeBetAmount = 0;
                pinfo.DummyCardsParent.transform.GetChild(0).gameObject.SetActive(false);
                pinfo.DummyCardsParent.transform.GetChild(1).gameObject.SetActive(false);
                foreach (CardProperty cardProperty in pinfo.remainingCards)
                {
                    if (cardProperty)
                        Destroy(cardProperty);
                }
                pinfo.remainingCards.Clear();
                DestroyHoleCards(pinfo);
                if (pinfo.pokerBetAmountAnim)
                    Destroy(pinfo.pokerBetAmountAnim);
                pinfo.PackedText.text = "FOLD";
                pinfo.PackedText.gameObject.SetActive(false);
                pinfo.HandRankLabelTxt.transform.parent.gameObject.SetActive(false);
                pinfo.Dealer.SetActive(false);

                pinfo.isAllInCashBet = false;
                pinfo.CashAllInIndicator.SetActive(false);
                pinfo.isCircleCheckFlag = false;
                pinfo.isBetPlacedPocker = false;
                pinfo.CashAllInIndicator.SetActive(false);


            }

            UIManager uIManager = UIManager.Instance;
            if (uIManager.GetMyPlayerCurrentState() != null)
            {

                if (uIManager.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame && uIManager.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
                    if (uIManager.GetMyPlayerInfo().PokerTotalCash < LocalSettings.MinBetAmount)
                        uIManager.GetMyPlayerInfo().StandUp();
            }


            DestroyCommunityCards();
            if (PhotonNetwork.IsConnectedAndReady)
                LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.pockerNumberofdropCummunityCardsKey, 0);
        }

        #endregion Reset Poker Game

        #region Poker Community Cards

        [ShowOnly] public List<GameObject> CommunityCardsToDestroy;


        public void DestroyHoleCards(PlayerInfo info)
        {
            Destroy(info.Hole_Card1);
            Destroy(info.Hole_Card2);
        }

        public void DestroyCommunityCards()
        {
            foreach (var cardObject in CommunityCardsToDestroy)
            {
                if (cardObject != null)
                    Destroy(cardObject);
            }
        }

        public bool CheckIfAllBetsEqualOrAllIn()
        {
            bool isAllBetsEqualOrAllin = true;
            if (psm.PlayingList.Count < 2)
                return false;
            for (int i = 1; i < psm.PlayingList.Count; i++)
            {
                if (psm.PlayingList[0].pokerTotalBetCash != psm.PlayingList[i].pokerTotalBetCash)
                {
                    if (!psm.PlayingList[i].isAllInCashBet)
                        isAllBetsEqualOrAllin = false;
                }
            }
            if (psm.PlayingList[0].pokerTotalBetCash != psm.PlayingList[1].pokerTotalBetCash)
            {
                if (!psm.PlayingList[0].isAllInCashBet)
                    isAllBetsEqualOrAllin = false;
            }
            return isAllBetsEqualOrAllin;
        }


        void GenerateCommunityCards()
        {
            Transform parent = PokerCheckWinner.Instance.CommunityCardsRectTransform[0].transform.parent;
            PokerCheckWinner.Instance.Community_Cards = new CardProperty[5];
            int[] CommCardsAry = new int[5];
            for (int i = 0; i < PokerCheckWinner.Instance.Community_Cards.Length; i++)
            {
                //temp_random_no = GetRandomNo(0, AllCards.Count);
                int cardIndex = PokerRandomArrayCards[temp_random_no];
                CommCardsAry[i] = cardIndex;
                GameObject newCommunityCard = Instantiate(GameManager.Instance.AllCards.Card[cardIndex].gameObject);
                CommunityCardsToDestroy.Add(newCommunityCard);
                LocalSettings.SetPosAndRect(newCommunityCard, PokerCheckWinner.Instance.CommunityCardsRectTransform[i], parent);
                PokerCheckWinner.Instance.Community_Cards[i] = newCommunityCard.GetComponent<CardProperty>();
                newCommunityCard.SetActive(false);
                //AllCards.RemoveAt(temp_random_no);
                temp_random_no++;
            }
            if (PhotonNetwork.IsConnectedAndReady)
                LocalSettings.GetCurrentRoom.SetCardsList(LocalSettings.pokerCommunityCardsArray, CommCardsAry);
            SetPokerHistory();
        }
        #endregion
        #region Setting poker history
        int numberOfPlayers;
        void SetPokerHistory()
        {
            PokerHistory ph = PokerHistory.Instance;
            for (int i = 0; i < PokerCheckWinner.Instance.Community_Cards.Length; i++)
            {
                ph.playerPokerRecord[0].CommunityCardsIndex[i] = PokerCheckWinner.Instance.Community_Cards[i].CardIndexInArray;
            }
            numberOfPlayers = psm.PlayingList.Count;
            ph.playerPokerRecord[0].NumberOfPlayersInMatch = numberOfPlayers;
            for (int i = 0; i < numberOfPlayers; i++)
            {
                ph.playerPokerRecord[0].playerRecord[i].viewId = psm.PlayingList[i].photonView.ViewID;
                ph.playerPokerRecord[0].playerRecord[i].nameOfPlayer = psm.PlayingList[i].photonView.Controller.NickName;
                ph.playerPokerRecord[0].playerRecord[i].holeCard1 = psm.PlayingList[i].Hole_Card1.CardIndexInArray;
                ph.playerPokerRecord[0].playerRecord[i].holeCard2 = psm.PlayingList[i].Hole_Card2.CardIndexInArray;
                ph.playerPokerRecord[0].playerRecord[i].RankOfPlayer = "Fold";
                ph.playerPokerRecord[0].playerRecord[i].isWinner = false;

            }
        }
        public void HistoryFinalCall()
        {
            SetWinStatus();
        }
        void SetWinStatus()
        {
            PokerHistory ph = PokerHistory.Instance;
            for (int i = 0; i < numberOfPlayers; i++)
            {
              //  Debug.LogError("Updateing player rank");
                for (int j = 0; j < GameManager.Instance.playersList.Count; j++)
                {
                    if (GameManager.Instance.playersList[j].photonView.ViewID == ph.playerPokerRecord[0].playerRecord[i].viewId)
                    {
                        if (GameManager.Instance.playersList[j].currentPlayerStateRef.currentState == PlayerState.STATE.Packed || GameManager.Instance.playersList[j].currentPlayerStateRef.currentState == PlayerState.STATE.OutOfTable)
                        {
                            ph.playerPokerRecord[0].playerRecord[i].RankOfPlayer = "Fold"; GameManager.Instance.playersList[j].HandRankLabelTxt.text = "Fold";
                        }
                        else
                        {
                            ph.playerPokerRecord[0].playerRecord[i].RankOfPlayer = GameManager.Instance.playersList[j].HandRankLabelTxt.text;
                        }
                        if (GameManager.Instance.playersList[j].WinningIndicator.activeInHierarchy)
                            ph.playerPokerRecord[0].playerRecord[i].isWinner = true;
                    }
                }
            }
        }
        #endregion
        #region Show Community Card 1st step => First Three Cards, 2nd => 4th card,  3rd => 5th card
        [ShowOnly]
        public int dropCardsIndex = 0;
        PokerCheckWinner pcw;
        public void DropCommunityCard()
        {
            NowDropCommunityCard(0.5f);
        }

        public void NowDropCommunityCard(float delay)
        {
            //Debug.LogError("Now dropping community Cards: " + dropCardsIndex);
            StartCoroutine(DropCommunityCardInSeq(delay));
        }
        IEnumerator DropCommunityCardInSeq(float delay)
        {
            yield return new WaitForSeconds(delay);
            pcw = PokerCheckWinner.Instance;
            if (dropCardsIndex == 0)
                StartCoroutine(showFirst3CommunityCards());
            else if (dropCardsIndex == 1)
                StartCoroutine(show4thCommunityCards());
            else if (dropCardsIndex == 2)
                StartCoroutine(show5thCommunityCards());
            else
            {
                dropCardsIndex++;
                foreach (PlayerInfo pInfo in psm.PlayingList)
                {
                    pInfo.HandRankLabelTxt.transform.parent.gameObject.SetActive(true);
                    if (pInfo.Hole_Card1 != null)
                    {
                        pInfo.Hole_Card1.gameObject.SetActive(true);
                        pInfo.Hole_Card2.gameObject.SetActive(true);
                    }
                }
                Nowreset();
            }
        }

        public void Nowreset()
        {
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.WaitingForResults);


        }

        void methodAfterCompletingDummyCards()
        {
            // UIManager.Instance.GetMyPlayerInfo().GiveTurnToNextPlayerPocker();
            if (PokerActionPanel.Instance.CheckBoolAllIN())
            {
                NowDropCommunityCard(0);
                return;
            }
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsPlaying);
            if (UIManager.Instance)
                UIManager.Instance.GetMyPlayerInfo().ResetIsBetPlacedPocker();

        }
        IEnumerator showFirst3CommunityCards()
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 3; i++)
            {
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
                GameObject dmyCard = Instantiate(GameManager.Instance.DummyCardPrefab);
                CommunityCardsToDestroy.Add(dmyCard);
                LocalSettings.SetPosAndRect(dmyCard, dummyCardInitialPos, dummyCardInitialPos.transform.parent);
                dmyCard.SetActive(true);
                StartCoroutine(PlayAnimation(dmyCard.transform, pcw.CommunityCardsRectTransform[i].transform, pcw.Community_Cards[i].gameObject));
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.5f);
            dropCardsIndex++;
            if (PhotonNetwork.IsConnectedAndReady)
                LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.pockerNumberofdropCummunityCardsKey, dropCardsIndex);
            methodAfterCompletingDummyCards();
            PokerStatesManager.Instance.updatePokerState(PokerState.THREE_COMMUNITY_CARDS);
        }

        IEnumerator show4thCommunityCards()
        {
            yield return new WaitForSeconds(0.5f);
            GameObject dmyCard = Instantiate(GameManager.Instance.DummyCardPrefab);
            CommunityCardsToDestroy.Add(dmyCard);
            LocalSettings.SetPosAndRect(dmyCard, dummyCardInitialPos, dummyCardInitialPos.transform.parent);
            dmyCard.SetActive(true);
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
            StartCoroutine(PlayAnimation(dmyCard.transform, pcw.CommunityCardsRectTransform[3].transform, pcw.Community_Cards[3].gameObject));
            yield return new WaitForSeconds(0.5f);
            dropCardsIndex++;
            if (PhotonNetwork.IsConnectedAndReady)
                LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.pockerNumberofdropCummunityCardsKey, dropCardsIndex);
            methodAfterCompletingDummyCards();
            PokerStatesManager.Instance.updatePokerState(PokerState.FOUR_COMMUNITY_CARDS);
        }

        IEnumerator show5thCommunityCards()
        {
            yield return new WaitForSeconds(0.5f);
            GameObject dmyCard = Instantiate(GameManager.Instance.DummyCardPrefab);
            CommunityCardsToDestroy.Add(dmyCard);
            LocalSettings.SetPosAndRect(dmyCard, dummyCardInitialPos, dummyCardInitialPos.transform.parent);
            dmyCard.SetActive(true);
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
            StartCoroutine(PlayAnimation(dmyCard.transform, pcw.CommunityCardsRectTransform[4].transform, pcw.Community_Cards[4].gameObject));
            yield return new WaitForSeconds(0.5f);
            dropCardsIndex++;
            if (PhotonNetwork.IsConnectedAndReady)
                LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.pockerNumberofdropCummunityCardsKey, dropCardsIndex);
            methodAfterCompletingDummyCards();
            PokerStatesManager.Instance.updatePokerState(PokerState.FIVE_COMMUNITY_CARDS);
        }
        #endregion

        #region  Card animation to target

        public IEnumerator PlayAnimation(Transform ObjToAnimate, Transform targetPosition, GameObject ObjToActivate)
        {
            GameObject objectToActivate = ObjToActivate;
            yield return new WaitForSeconds(0);
            if (targetPosition != null)
                ObjToAnimate.DOMove(targetPosition.position, 0.4f, false).OnComplete(() => OnCompleteAnim(ObjToAnimate.gameObject, objectToActivate));
            float RotationOffSet = targetPosition.eulerAngles.z;
            ObjToAnimate.DOLocalRotate(new UnityEngine.Vector3(0, 0, 360 + RotationOffSet), 0.4f, RotateMode.FastBeyond360);
            ObjToAnimate.DOScale(new UnityEngine.Vector3(1.5f, 1.5f, 1.5f), 0.4f);
        }
        void OnCompleteAnim(GameObject AnimatedObj, GameObject objectToActivate)
        {
            objectToActivate.SetActive(true);
            AnimatedObj.SetActive(false);
        }
        #endregion


        #region Give_CommunityCard_To_Player_State_OutGame

        public void AssignCummintyCardToOutOFGame(int NumberofCards)
        {
            if (NumberofCards == 1)
            {
                dropCardsIndex = 1;
                ShowCardConmmunityCardToOutOFGamePlayer(3);
            }
            if (NumberofCards == 2)
            {
                dropCardsIndex = 2;
                ShowCardConmmunityCardToOutOFGamePlayer(4);
            }
            if (NumberofCards == 3)
            {
                dropCardsIndex = 3;
                ShowCardConmmunityCardToOutOFGamePlayer(5);
            }
        }


        void ShowCardConmmunityCardToOutOFGamePlayer(int loopNumber)
        {
            for (int i = 0; i < loopNumber; i++)
            {
                PokerCheckWinner.Instance.Community_Cards[i].gameObject.SetActive(true);
            }
        }

        public void GenerateCommunityCardsForOutOfGame()
        {
            Transform parent = PokerCheckWinner.Instance.CommunityCardsRectTransform[0].transform.parent;
            PokerCheckWinner.Instance.Community_Cards = new CardProperty[5];
            int[] commCards = new int[5];
            if (PhotonNetwork.IsConnectedAndReady)
                commCards = LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.pokerCommunityCardsArray);
            for (int i = 0; i < PokerCheckWinner.Instance.Community_Cards.Length; i++)
            {
                int cardIndex = commCards[i];
                GameObject newCommunityCard = Instantiate(GameManager.Instance.AllCards.Card[cardIndex].gameObject);
                CommunityCardsToDestroy.Add(newCommunityCard);
                LocalSettings.SetPosAndRect(newCommunityCard, PokerCheckWinner.Instance.CommunityCardsRectTransform[i], parent);
                PokerCheckWinner.Instance.Community_Cards[i] = newCommunityCard.GetComponent<CardProperty>();
                newCommunityCard.SetActive(false);
            }
        }

        #endregion

    }
}