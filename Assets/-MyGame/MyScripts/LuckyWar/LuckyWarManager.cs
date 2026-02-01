using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;


namespace com.mani.muzamil.amjad
{
    public class LuckyWarManager : MonoBehaviourPunCallbacks
    {
        public GameObject[] objectsToDisable;
        public GameObject[] objectsToEnable;
        public RectTransform[] LWSittingPositions;

        [ShowOnly] public List<GameObject> ObjectsToDestroy = new List<GameObject>();
        // Action Panel
        public GameObject LWActionPanel;

        public RectTransform LWDummyCardPos;
        [ShowOnly] public CardProperty LWFirstCard;
        public RectTransform LWFirstCardPosition;

        // Bet Amounts 

        public BigInteger TieTotalAmount;
        public BigInteger BetTotalAmount;
        public BigInteger TieExtraTotalBetAmount;
        public BigInteger BetExtraTotalBetAmount;
        PhotonView photonView;

        PlayerStateManager psm;



        #region Making Instance
        private static LuckyWarManager _instance;
        public static LuckyWarManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<LuckyWarManager>();
                return _instance;
            }
        }
        #endregion

        private void Awake()
        {
            if (!MatchHandler.IsLuckyWar())
            {
                gameObject.SetActive(false);
                return;
            }
            if (_instance == null)
                _instance = this;
        }

        private void Start()
        {
            if (!MatchHandler.IsLuckyWar())
            {
                gameObject.SetActive(false);
                return;
            }
            psm = PlayerStateManager.Instance;
            photonView = GetComponent<PhotonView>();
            LocalSettings.ToggleObjectState(objectsToDisable, false);
            LocalSettings.ToggleObjectState(objectsToEnable, true);

        }

        public void AdjustWLTableThings()
        {
            AdjustNewPositions();


            //if (LocalSettings.GetTotalChips() <= 0)
            //   LocalSettings.SetTotalChips(500000);
            TieTotalAmount = LocalSettings.MinBetAmount;
            BetTotalAmount = LocalSettings.MinBetAmount;
            //  SuperAndarTotalBetAmount = LocalSettings.MinABBetAmount;
            //  SuperBaharTotalBetAmount = LocalSettings.MinABBetAmount;
        }


        void AdjustNewPositions()
        {
            for (int i = 1; i < 5; i++)
            {
                LocalSettings.SetPosAndRect(GameManager.Instance.position_availability[i].Pos.gameObject, LWSittingPositions[i], GameManager.Instance.PlayerTable);
                //Debug.Log("Positions Are " + LWSittingPositions[i]);

            }
        }



        Coroutine CardDistributionCoRoutine;
        public void SettingRandomCardsArrayToRoomProperty()
        {
            // StartCoroutine(WaitingForBetFisnishOfAllPlayers());
            // if (PhotonNetwork.IsMasterClient)
            //  {
            Array.Clear(LWRandomArrayCards, 0, LWRandomArrayCards.Length);
            LWRandomArrayCards = new int[52];
            CardDistributionStart();

            // }
        }

        void CardDistributionStart()
        {

            if (PhotonNetwork.IsMasterClient)
            {
                //Debug.LogError("ON Master Client Switch");
                // if (!cardShuffleBool)
                if (!UIManager.Instance.GetMyPlayerInfo().cardShuffleBool)
                {
                    CardDistributionCoRoutine = StartCoroutine(WaitingForBetFisnishOfAllPlayers());

                }
            }
        }
        public void StopCardDistRoutine()
        {
            if (CardDistributionCoRoutine != null)
                StopCoroutine(CardDistributionCoRoutine);
        }

        public int[] LWRandomArrayCards = new int[52];
        IEnumerator WaitingForBetFisnishOfAllPlayers()
        {



            for (int i = 0; i < GameManager.Instance.AllCards.Card.Length; i++)
            {
                LWRandomArrayCards[i] = i;
            }
            Shuffle(LWRandomArrayCards);
            if (PhotonNetwork.IsConnectedAndReady)
            {
                LocalSettings.GetCurrentRoom.SetCardsList(LocalSettings.Poker_card_listKey, LWRandomArrayCards);
                yield return new WaitUntil(() => PlayerStateManager.Instance.PlayingList.All(x => x.LWTurn == false));
                UIManager.Instance.GetMyPlayerInfo().CardshuffleboolForMasterCleint(true);
                photonView.RPC(nameof(RandomCardsArrayRPC), RpcTarget.All, LWRandomArrayCards);
            }

        }

        [PunRPC]
        public void RandomCardsArrayRPC(int[] randArray)
        {

            Array.Copy(randArray, LWRandomArrayCards, randArray.Length);
            DistributeOriginalCards();
            CreateAndDistributeDummyCards();

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

        #region Deciding win loose for server
        string surrenderAmountOfBet;
        public void onBetBtnClick(string betAmount)
        {
            surrenderAmountOfBet = betAmount;
            int myPlayerIndex = PlayerStateManager.Instance.PlayingList.IndexOf(UIManager.Instance.GetMyPlayerInfo());
            int MycardNumber = LWRandomArrayCards[myPlayerIndex + 1];
            int firstCardNumber = LWRandomArrayCards[0];

            int firstCardPower = GameManager.Instance.AllCards.Card[firstCardNumber].Power;
            int myCardpower = GameManager.Instance.AllCards.Card[MycardNumber].Power;

            if (firstCardPower > myCardpower)
            {
                Debug.LogError("game loose on low card");
                LuckyWarAPI.Instance.LuckyWarSendBet(LuckyWarAPI.BetType.bet, betAmount, LuckyWarAPI.BetType.lose);
            }
            else if (firstCardPower < myCardpower)
            {
                Debug.LogError("game win");
                LuckyWarAPI.Instance.LuckyWarSendBet(LuckyWarAPI.BetType.bet, betAmount, LuckyWarAPI.BetType.win);
            }
            else
            {
                Debug.LogError("game loose on tie card");
                LuckyWarAPI.Instance.LuckyWarSendBet(LuckyWarAPI.BetType.bet, betAmount, LuckyWarAPI.BetType.lose);
            }
        }

        public void onTieBtnClick(string betAmount)
        {
            int myPlayerIndex = PlayerStateManager.Instance.PlayingList.IndexOf(UIManager.Instance.GetMyPlayerInfo());
            int MycardNumber = LWRandomArrayCards[myPlayerIndex + 1];
            int firstCardNumber = LWRandomArrayCards[0];

            int firstCardPower = GameManager.Instance.AllCards.Card[firstCardNumber].Power;
            int myCardpower = GameManager.Instance.AllCards.Card[MycardNumber].Power;

            if (firstCardPower == myCardpower)
            {
                Debug.LogError("Calling tie equal win");
                LuckyWarAPI.Instance.LuckyWarSendBet(LuckyWarAPI.BetType.tie, betAmount, LuckyWarAPI.BetType.win);
            }
            else
            {
                Debug.LogError("Calling tie loose");
                LuckyWarAPI.Instance.LuckyWarSendBet(LuckyWarAPI.BetType.tie, betAmount, LuckyWarAPI.BetType.lose);
            }
        }

        public void onSurrenderBtnClick()
        {
            LuckyWarAPI.Instance.LuckyWarSendBet(LuckyWarAPI.BetType.surrender, surrenderAmountOfBet, LuckyWarAPI.BetType.win);
        }

        public void onGotoWarBtnClick()
        {
            int myPlayerIndex = PlayerStateManager.Instance.PlayingList.IndexOf(UIManager.Instance.GetMyPlayerInfo());
            int MycardNumber = LWRandomArrayCards[myPlayerIndex + 1];
            int firstCardNumber = LWRandomArrayCards[0];

            int firstCardPower = GameManager.Instance.AllCards.Card[firstCardNumber].Power;
            int myCardpower = GameManager.Instance.AllCards.Card[MycardNumber].Power;

            if (firstCardPower > myCardpower)
            {
                Debug.LogError("game loose on low card");
                LuckyWarAPI.Instance.LuckyWarSendBet(LuckyWarAPI.BetType.war, surrenderAmountOfBet, LuckyWarAPI.BetType.lose);
            }
            else
            {
                Debug.LogError("game win");
                LuckyWarAPI.Instance.LuckyWarSendBet(LuckyWarAPI.BetType.war, surrenderAmountOfBet, LuckyWarAPI.BetType.win);
            }
        }


        #endregion

        // Distributing Original Cards
        #region Distribute Original Cards


        public void CardAssignToPlayerWhoOutOfGame(int cardIndex)
        {
            GameObject firstCard = Instantiate(GameManager.Instance.AllCards.Card[cardIndex].gameObject);
            ObjectsToDestroy.Add(firstCard);
            LocalSettings.SetPosAndRect(firstCard, LWFirstCardPosition, LWFirstCardPosition.transform.parent);
            LocalSettings.SetPosAndRect(firstCard, LWFirstCardPosition, LWFirstCardPosition.transform.parent);
            LWFirstCard = firstCard.GetComponent<CardProperty>();
            firstCard.gameObject.SetActive(true);
            LWFirstCard.gameObject.SetActive(true);
        }

        int tempRandomNumberForOrignalCard = 0;
        void DistributeOriginalCards()
        {
            tempRandomNumberForOrignalCard = 0;
            // Setting first original card
            Transform parent = LWFirstCardPosition.transform.parent;
            int cardIndex = LWRandomArrayCards[tempRandomNumberForOrignalCard];
            if (PhotonNetwork.IsConnectedAndReady)
                LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.LwFirstCardKey, cardIndex);
            tempRandomNumberForOrignalCard++;
            GameObject firstCard = Instantiate(GameManager.Instance.AllCards.Card[cardIndex].gameObject);
            ObjectsToDestroy.Add(firstCard);
            LocalSettings.SetPosAndRect(firstCard, LWFirstCardPosition, parent);
            LWFirstCard = firstCard.GetComponent<CardProperty>();
            firstCard.SetActive(false);
            // Setting player original cards
            for (int i = 0; i < psm.PlayingList.Count; i++)
            {
                parent = psm.PlayingList[i].LWDummyCardPrent.transform;
                cardIndex = LWRandomArrayCards[tempRandomNumberForOrignalCard];
                if (PhotonNetwork.IsConnectedAndReady)
                    psm.PlayingList[i].player.SetCustomData(LocalSettings.LwPlayerCardKey, cardIndex);
                tempRandomNumberForOrignalCard++;
                GameObject pCard = Instantiate(GameManager.Instance.AllCards.Card[cardIndex].gameObject);
                ObjectsToDestroy.Add(pCard);
                LocalSettings.SetPosAndRect(pCard, parent.GetChild(0).GetComponent<RectTransform>(), parent);
                psm.PlayingList[i].PlayerLWCard = pCard.GetComponent<CardProperty>();
                pCard.SetActive(false);
            }
        }

        #endregion

        // Creating and distributing dummy cards
        #region Creating and distributing Dummy Cards
        List<GameObject> DummyLWCards = new List<GameObject>();
        int numberOfDummyCards;
        void CreateAndDistributeDummyCards()
        {
            DummyLWCards = new List<GameObject>();
            numberOfDummyCards = psm.PlayingList.Count + 1;
            for (int i = 0; i < numberOfDummyCards; i++)
            {
                GameObject dumyCard = Instantiate(GameManager.Instance.DummyCardPrefab);
                ObjectsToDestroy.Add(dumyCard);
                DummyLWCards.Add(dumyCard);
                dumyCard.SetActive(true);
                LocalSettings.SetPosAndRect(dumyCard, LWDummyCardPos, LWDummyCardPos.transform.parent);
            }
            StartCoroutine(DistributeDummyCards());
        }
        IEnumerator DistributeDummyCards()
        {
            yield return new WaitForSeconds(0.5f);
            int cardIndex = 0;
            for (int i = 0; i < psm.PlayingList.Count; i++)
            {
                GameObject dmyCard = null;
                if (DummyLWCards.Count > cardIndex)
                    dmyCard = DummyLWCards[cardIndex];
                else
                {
                    GameObject dumyCard = Instantiate(GameManager.Instance.DummyCardPrefab);
                    ObjectsToDestroy.Add(dumyCard);
                    DummyLWCards.Add(dumyCard);
                    dumyCard.SetActive(true);
                    LocalSettings.SetPosAndRect(dumyCard, LWDummyCardPos, LWDummyCardPos.transform.parent);
                    dmyCard = dumyCard;
                }
                cardIndex++;
                GameObject objToAct = null;
                objToAct = psm.PlayingList[i].PlayerLWCard.gameObject;
                ObjectsToDestroy.Add(objToAct);
                StartCoroutine(GameManager.Instance.PlayAnimation(dmyCard.transform, psm.PlayingList[i].LWDummyCardPrent.transform.GetChild(0).transform, objToAct));

                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);

                yield return new WaitForSeconds(0.51f);

            }
            yield return new WaitForSeconds(0.75f);
            if (psm.PlayingList.Count > 0)
            {
                GameObject lastDummyCard = null;
                if (DummyLWCards.Count >= 1)
                {
                    lastDummyCard = DummyLWCards[DummyLWCards.Count - 1];
                }
                else
                {
                    GameObject dumyCard = Instantiate(GameManager.Instance.DummyCardPrefab);
                    ObjectsToDestroy.Add(dumyCard);
                    DummyLWCards.Add(dumyCard);
                    lastDummyCard = dumyCard;
                    dumyCard.SetActive(true);
                    LocalSettings.SetPosAndRect(dumyCard, LWDummyCardPos, LWDummyCardPos.transform.parent);
                }
                GameObject objToAct2 = LWFirstCard.gameObject;

                StartCoroutine(GameManager.Instance.PlayAnimation(lastDummyCard.transform, LWFirstCardPosition, objToAct2));

                yield return new WaitForSeconds(0.3f);
                if (!LWFirstCard.gameObject.activeInHierarchy)
                {
                    LWFirstCard.gameObject.SetActive(true);
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
                }
                ClearDummyCardsList();
                yield return new WaitForSeconds(1f);
                WinnerCheckingOFLuckyWar();

                //GameStartManager.Instance._1stCurrentChaalBool = true;
                //ChageRoomStateToGamePlaying();
                //UIManager.Instance.GetMyPlayerInfo().HandRankLabelTxt.transform.parent.gameObject.SetActive(true);
                //UpdateRankOnNoCardState();
                //UpdateRankOnThreeCardState();
                //UpdateRankOnFourCardState();
                //UpdateRankOnFiveCardState(UIManager.Instance.GetMyPlayerInfo());
            }

        }

        int counter = 0;
        void WinnerCheckingOFLuckyWar()
        {
            counter++;
            //Debug.LogError("Calling counter Start" + counter);

            if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                for (int i = 0; i < psm.PlayingList.Count; i++)
                {
                    if (psm.PlayingList[i].PlayerLWCard.Power == LWFirstCard.Power)
                    {
                        psm.PlayingList[i].TurnOnOffTieWinLW(true);
                        if (psm.PlayingList[i].player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) > 0)
                        {
                            DeclareTieWinner(psm.PlayingList[i]);
                        }
                    }
                    else if (psm.PlayingList[i].PlayerLWCard.Power > LWFirstCard.Power)
                    {
                        //  Debug.LogError("Cehck Setp.....2....");
                        DeclareBetWinner(psm.PlayingList[i]);

                    }
                    else
                    {
                        DeclareLose(psm.PlayingList[i]);
                    }
                }
                if (checkTieWinner())
                {
                    TieBetWinner();

                    //  Debug.LogError("Check IT TieWinner");
                }
                else
                {
                    //Debug.LogError("Game Reset Point .... 1....");
                    //   UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
                    // Debug.LogError("check Total Hands win...7");
                    ResetLWGame();
                }
            }
            else if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
            {
                winnerCheckofTieMatch();
            }

        }


        void winnerCheckofTieMatch()
        {
            for (int i = 0; i < psm.PlayingList.Count; i++)
            {

                if (psm.PlayingList[i].PlayerLWCard.Power >= LWFirstCard.Power)
                {
                    //Debug.LogError("Cehck Setp.....1....");
                    DeclareBetWinner(psm.PlayingList[i]);
                }
                else if (psm.PlayingList[i].PlayerLWCard.Power < LWFirstCard.Power)
                {
                    BetbetTotalAmountLose(psm.PlayingList[i]);
                }
                else
                {
                    DeclareLose(psm.PlayingList[i]);
                }
            }
            if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.Packed)
                OnSurrenderBetAmountGone(UIManager.Instance.GetMyPlayerInfo());
            //Debug.LogError("Game Reset Point .... 2....");
            // UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
            //  Debug.LogError("check Total Hands win...6");
            ResetLWGame();
        }




        public void TieBetWinner()
        {
            StartCoroutine(ResetValueForTieWinner());
        }


        IEnumerator ResetValueForTieWinner()
        {
            yield return new WaitForSeconds(2f);
            if (LWFirstCard)
                Destroy(LWFirstCard.gameObject);
            foreach (PlayerInfo pInfo in GameManager.Instance.playersList)
            {
                if (pInfo.PlayerLWCard)
                    Destroy(pInfo.PlayerLWCard.gameObject);
                pInfo.WinningIndicator.SetActive(false);
            }
            if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame &&
            UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
            {
                if (!UIManager.Instance.GetMyPlayerInfo().TieBetWinLw)
                {
                    UIManager.Instance.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.Watching);
                }
            }
            yield return new WaitForSeconds(1f);
            psm.PlayingList.Clear();
            psm.UpdatePlayingList();
            // Debug.LogError("assginBattleWar");
            AssignGoToBattleWar();
            StopCardDistRoutine();
            UIManager.Instance.GetMyPlayerInfo().CardshuffleboolForMasterCleint(false);
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ABFirstTurn);
        }
        void AssignGoToBattleWar()
        {
            foreach (PlayerInfo item in psm.PlayingList)
            {
                item.TurnOnOffLW(true);


            }
            if (UIManager.Instance.GetMyPlayerInfo().TieBetWinLw)
            {
                LocalSettings.Vibrate();
                LWActionPanelScript.Instance.goToWarPanel.SetActive(true);
                PlayerTurnManager.Instance.turnManager.BeginTurn();
                PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationLuckyWar;

            }
            //PlayerTurnManager.Instance.turnManager.BeginTurn();
            //PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationLuckyWar;
        }

        public bool checkTieWinner()
        {
            for (int i = 0; i < psm.PlayingList.Count; i++)
            {
                if (psm.PlayingList[i].TieBetWinLw)
                    return true;

            }
            return false;
        }

        public void ResetLWGame()
        {
            // Debug.LogError("check Total Hands win...1");

            StartCoroutine(ResetGameManagerCall());
        }

        IEnumerator ResetGameManagerCall()
        {
            if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame && UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
            {
                UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
            }
            yield return new WaitForSeconds(4f);
            GameResetManager.Instance.ResetLWGame();
        }
        #region LuckyWar Bets
        void DeclareTieWinner(PlayerInfo pInfo)
        {

            BigInteger TieAwaredAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) * 11;
            GameObject TieBetAmountParent = pInfo.TieBetAmoutTxt.transform.parent.gameObject;

            GameObject TieRewardBox = Instantiate(TieBetAmountParent);
            ObjectsToDestroy.Add(TieRewardBox);
            LocalSettings.SetPosAndRect(TieRewardBox, TieBetAmountParent.GetComponent<RectTransform>(), TieBetAmountParent.transform.parent.gameObject.transform);
            pInfo.TieBetAmoutTxt.text = "";
            TieRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(TieAwaredAmount);
            if (UIManager.Instance.GetMyPlayerInfo().photonView.ViewID == pInfo.photonView.ViewID)
            {
                UIManager.Instance.TotalWinsAmount += (TieAwaredAmount);
                //if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
                //    UIManager.Instance.TotalWinHands++;
                pInfo.WinningIndicator.SetActive(true);
                if (pInfo.photonView.IsMine)
                {
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                    //Debug.LogError("Win sound is playing");
                }

                // adding Super Tie panel Text show x10{type here  your Code}
                PlayAnimation(TieRewardBox.transform, UIManager.Instance.PlayerTotalChipsTxt.transform, 1, false, 2f, TieRewardBox);
                StartCoroutine(AddingLWWinAmount(pInfo, TieRewardBox, TieAwaredAmount));

            }
            else
            {
                pInfo.WinningIndicator.SetActive(true);
                PlayAnimation(TieRewardBox.transform, pInfo.transform, 1, false, 1, TieRewardBox);

            }






        }

        void TiebetAmountLose(PlayerInfo pInfo)
        {
            BigInteger TieAwaredAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey); GameObject TieBetAmountParent = pInfo.TieBetAmoutTxt.transform.parent.gameObject;
            GameObject TieRewardBox = Instantiate(TieBetAmountParent);
            ObjectsToDestroy.Add(TieRewardBox);
            LocalSettings.SetPosAndRect(TieRewardBox, TieBetAmountParent.GetComponent<RectTransform>(), TieBetAmountParent.transform.parent.gameObject.transform);
            pInfo.TieBetAmoutTxt.text = "";
            TieRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(TieAwaredAmount);
            PlayAnimation(TieRewardBox.transform, Pot.instance.targeToTipGirl.transform, 1, false, 1, TieRewardBox);
        }
        void BetbetAmountLose(PlayerInfo pInfo)
        {
            BigInteger BetAwaredAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey);
            if (pInfo.TieBetWinLw)
                BetAwaredAmount = BetAwaredAmount / 2;
            GameObject BetBetAmountParent = pInfo.BetBetAmoutTxt.transform.parent.gameObject;
            GameObject BetRewardBox = Instantiate(BetBetAmountParent);
            ObjectsToDestroy.Add(BetRewardBox);
            LocalSettings.SetPosAndRect(BetRewardBox, BetBetAmountParent.GetComponent<RectTransform>(), BetBetAmountParent.transform.parent.gameObject.transform);
            pInfo.BetBetAmoutTxt.text = "";
            BetRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(BetAwaredAmount);
            PlayAnimation(BetRewardBox.transform, Pot.instance.targeToTipGirl.transform, 1, false, 1, BetRewardBox);
        }
        void BetbetTotalAmountLose(PlayerInfo pInfo)
        {
            BigInteger BetAwaredAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey) * 2;
            GameObject BetBetAmountParent = pInfo.BetBetAmoutTxt.transform.parent.gameObject;
            GameObject BetRewardBox = Instantiate(BetBetAmountParent);
            ObjectsToDestroy.Add(BetRewardBox);
            LocalSettings.SetPosAndRect(BetRewardBox, BetBetAmountParent.GetComponent<RectTransform>(), BetBetAmountParent.transform.parent.gameObject.transform);
            pInfo.BetBetAmoutTxt.text = "";
            BetRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(BetAwaredAmount);
            PlayAnimation(BetRewardBox.transform, Pot.instance.targeToTipGirl.transform, 1, false, 1, BetRewardBox);
        }



        void DeclareBetWinner(PlayerInfo pInfo)
        {
            BigInteger BetAwaredAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey) * 2;
            if (pInfo.TieBetWinLw)
            {
                if (pInfo.player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) > 0)
                    BetAwaredAmount += pInfo.player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey);
                else
                    BetAwaredAmount *= 2;
            }

            GameObject BetBetAmountParent = pInfo.BetBetAmoutTxt.transform.parent.gameObject;
            GameObject BetRewardBox = Instantiate(BetBetAmountParent);
            ObjectsToDestroy.Add(BetRewardBox);
            LocalSettings.SetPosAndRect(BetRewardBox, BetBetAmountParent.GetComponent<RectTransform>(), BetBetAmountParent.transform.parent.gameObject.transform);
            pInfo.BetBetAmoutTxt.text = "";
            BetRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(BetAwaredAmount);
            if (UIManager.Instance.GetMyPlayerInfo().photonView.ViewID == pInfo.photonView.ViewID)
            {
                UIManager.Instance.TotalWinsAmount += BetAwaredAmount;
                UIManager.Instance.TotalWinAmountFor1Game += BetAwaredAmount;
                if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
                {
                    UIManager.Instance.SetTotalWinHandByPlayer();
                    GameManager.Instance.AddXPToMyPlayer(true);
                }
                pInfo.WinningIndicator.SetActive(true);
                if (pInfo.photonView.IsMine)
                {
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                    // Debug.LogError("Win sound is playing");
                }
                PlayAnimation(BetRewardBox.transform, UIManager.Instance.PlayerTotalChipsTxt.transform, 1, false, 2f, BetRewardBox);

                StartCoroutine(AddingLWWinAmount(pInfo, BetRewardBox, BetAwaredAmount));
            }
            else
            {
                pInfo.WinningIndicator.SetActive(true);
                PlayAnimation(BetRewardBox.transform, pInfo.transform, 1, false, 1, BetRewardBox);
            }
            if (pInfo.player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) > 0 && !pInfo.TieBetWinLw)
                TiebetAmountLose(pInfo);


        }
        void TieMatchHalfBetWinner(PlayerInfo pInfo)
        {
            BigInteger BetAwaredAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey) / 2;
            GameObject BetBetAmountParent = pInfo.BetBetAmoutTxt.transform.parent.gameObject;
            GameObject BetRewardBox = Instantiate(BetBetAmountParent);
            ObjectsToDestroy.Add(BetRewardBox);
            LocalSettings.SetPosAndRect(BetRewardBox, BetBetAmountParent.GetComponent<RectTransform>(), BetBetAmountParent.transform.parent.gameObject.transform);
            pInfo.BetBetAmoutTxt.text = "";
            BetRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(BetAwaredAmount);
            if (UIManager.Instance.GetMyPlayerInfo().photonView.ViewID == pInfo.photonView.ViewID)
            {
                UIManager.Instance.TotalWinsAmount += BetAwaredAmount;
                UIManager.Instance.TotalWinAmountFor1Game += BetAwaredAmount;
                if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin || UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.Packed)
                    if (pInfo.player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) > 0)
                    {
                        UIManager.Instance.SetTotalWinHandByPlayer();
                        GameManager.Instance.AddXPToMyPlayer(true);
                    }
                if (pInfo.photonView.IsMine)
                {
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                }
                PlayAnimation(BetRewardBox.transform, UIManager.Instance.PlayerTotalChipsTxt.transform, 1, false, 2f, BetRewardBox);

                StartCoroutine(AddingLWWinAmount(pInfo, BetRewardBox, BetAwaredAmount));
            }
            else
            {
                PlayAnimation(BetRewardBox.transform, pInfo.transform, 1, false, 1, BetRewardBox);

            }
            if (pInfo.player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) > 0 && !pInfo.TieBetWinLw)
                TiebetAmountLose(pInfo);


        }
        void DeclareLose(PlayerInfo pInfo)
        {
            if (pInfo.player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) > 0 && !pInfo.TieBetWinLw)
            {
                //   Debug.LogError("checkBetAmount...." + pInfo.player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey));
                TiebetAmountLose(pInfo);
            }
            if (pInfo.player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey) > 0)
            {
                // Debug.LogError("checkBetAmount...." + pInfo.player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey));
                BetbetAmountLose(pInfo);
            }
        }
        #endregion 
        IEnumerator AddingLWWinAmount(PlayerInfo pInfo, GameObject obj, BigInteger WinAmount)
        {
            LocalSettings.SetTotalChips(WinAmount);
            //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, WinAmount.ToString());
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Reward, false);
            pInfo.player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            yield return new WaitUntil(() => obj == null);


            //pInfo.playerTotalCash.text = LocalSettings.Rs(pInfo.player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));

            //  Debug.LogError("Check Total Cash key Of Player" + LocalSettings.Rs(pInfo.player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey)));
            if (PhotonNetwork.IsConnectedAndReady)
                pInfo.UpdateLWPlayerCash(LocalSettings.Rs(pInfo.player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey)));


            UIManager.Instance.PlayerTotalChipsTxt.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
           // SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);

        }

        void ClearDummyCardsList()
        {
            foreach (GameObject card in DummyLWCards)
            {
                if (card)
                    Destroy(card);
            }
            DummyLWCards.Clear();
        }

        public void OnSurrenderBetAmountGone(PlayerInfo info)
        {
            int viewID = info.photonView.ViewID;
            onSurrenderBtnClick();
            photonView.RPC(nameof(RemainingBetLose), RpcTarget.All, viewID);
        }

        [PunRPC]
        void RemainingBetLose(int viewID)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StopCardDistRoutine();
                UIManager.Instance.GetMyPlayerInfo().CardshuffleboolForMasterCleint(false);

            }

            if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.Packed)
            {
                int Id = UIManager.Instance.GetMyPlayerInfo().photonView.ViewID;
                photonView.RPC(nameof(DistributleHalfBetToPackedPlayers), RpcTarget.All, Id);
                //ResetLWGame();
            }
            DistributleHalfBetToPackedPlayers(viewID);
            // Debug.LogError("check Total Hands win...5");
            ResetLWGame();
        }


        [PunRPC]
        void DistributleHalfBetToPackedPlayers(int viewID)
        {
            PlayerInfo pInfo = null;
            foreach (PlayerInfo info in GameManager.Instance.playersList)
            {
                if (info.photonView.ViewID == viewID)
                    pInfo = info;
            }
            BetbetAmountLose(pInfo);
            TieMatchHalfBetWinner(pInfo);
        }

        #region oneCard to tie Player
        //public void OriginalCardToTiePlayer(PlayerInfo pInfo)
        //{
        //    int viewID = pInfo.photonView.ViewID;
        //    photonView.RPC(nameof(OriginalCardToTiePlayerRPC), RpcTarget.All, viewID);
        //}

        //[PunRPC]
        //public void OriginalCardToTiePlayerRPC(int  viewid)
        //{
        //    PlayerInfo pInfo = null;
        //    foreach (PlayerInfo info in GameManager.Instance.playersList)
        //    {
        //        if(info.photonView.ViewID == viewid)
        //            pInfo = info;
        //    }
        //    giveoriginalCardToTiePlayer(pInfo);
        //    DummyCardForTiePlayer(pInfo);
        //}
        //void giveoriginalCardToTiePlayer(PlayerInfo pInfo)
        //{
        //    Transform parent = pInfo.LWDummyCardPrent.transform;
        //    int cardIndex = LWRandomArrayCards[tempRandomNumberForOrignalCard];
        //    tempRandomNumberForOrignalCard++;
        //    GameObject pCard = Instantiate(GameManager.Instance.AllCards.Card[cardIndex].gameObject);
        //    ObjectsToDestroy.Add(pCard);
        //    LocalSettings.SetPosAndRect(pCard, parent.GetChild(0).GetComponent<RectTransform>(), parent);
        //    pInfo.PlayerLWCard = pCard.GetComponent<CardProperty>();
        //    pCard.SetActive(false);
        //}

        //void DummyCardForTiePlayer(PlayerInfo pInfo)
        //{
        //    GameObject dumyCard = Instantiate(GameManager.Instance.DummyCardPrefab);
        //    ObjectsToDestroy.Add(dumyCard);
        //    DummyLWCards.Add(dumyCard);
        //    dumyCard.SetActive(true);
        //    LocalSettings.SetPosAndRect(dumyCard, LWDummyCardPos, LWDummyCardPos.transform.parent);
        //    SendDummyCardToTiePlayer(pInfo, dumyCard);
        //}
        //void SendDummyCardToTiePlayer(PlayerInfo pInfo, GameObject dmyCard)
        //{
        //    GameObject objToAct = null;
        //    objToAct = pInfo.PlayerLWCard.gameObject;
        //    StartCoroutine(GameManager.Instance.PlayAnimation(dmyCard.transform, pInfo.LWDummyCardPrent.transform.GetChild(0).transform, objToAct));
        //}
        #endregion
        #endregion

        // Bet animation
        #region Animation of Bet
        void PlayAnimation(Transform ObjToAnimate, Transform targetPosition, float ScaleMultiplier, bool isRotate, float TimeToReach, GameObject ObjToDestroy)
        {
            if (targetPosition != null)
                ObjToAnimate.DOMove(targetPosition.position, TimeToReach, false).OnComplete(() => ObjDestroy(ObjToDestroy));
            if (isRotate)
            {
                float RotationOffSet = targetPosition.eulerAngles.z;
                ObjToAnimate.DOLocalRotate(new UnityEngine.Vector3(0, 0, 360 + RotationOffSet), TimeToReach, RotateMode.FastBeyond360);
            }
            RectTransform rt = targetPosition.gameObject.GetComponent<RectTransform>();
            // ObjToAnimate.DOScale(UnityEngine.Vector3.one * ScaleMultiplier, TimeToReach).OnComplete(() => cardSetAnchor(ObjToAnimate));

        }
        void ObjDestroy(GameObject obj)
        {
            if (obj != null)
            {
                Destroy(obj);

            }


        }

        #endregion

        // Reset luckwar game basic
        #region Reset LuckyWar
        public void ResetLuckywarGame()
        {
            LWActionPanelScript.Instance.ResetActionBtn();
            StopCardDistRoutine();
            if (LWFirstCard)
                Destroy(LWFirstCard.gameObject);
            ClearDummyCardsList();
            DestroyObjectsOnReset();
            ObjectsToDestroy.Clear();
            foreach (PlayerInfo pInfo in GameManager.Instance.playersList)
            {
                if (pInfo.PlayerLWCard)
                    Destroy(pInfo.PlayerLWCard.gameObject);
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    pInfo.player.SetCustomBigIntegerData(LocalSettings.LWTieAmountKey, 0);
                    pInfo.player.SetCustomBigIntegerData(LocalSettings.LWBetAmountKey, 0);
                }
                pInfo.BetBetAmoutTxt.text = "";
                pInfo.TieBetAmoutTxt.text = "";

                //pInfo.TurnOnOffLW(true);
                pInfo.TurnOnOffTieWinLW(false);
                pInfo.CardshuffleboolForMasterCleint(false);

            }
            Pot.instance.BetTotalBetPlaced = 0;
            Pot.instance.TieTotalBetPlaced = 0;
            TieTotalAmount = 0;
            BetTotalAmount = 0;
            BetExtraTotalBetAmount = 0;
            TieExtraTotalBetAmount = 0;
            LWActionPanelScript.Instance.isTurnPass = false;
            LWActionPanelScript.Instance.goToWarPanel.SetActive(false);

            if (UIManager.Instance.GetMyPlayerInfo() != null)
                if (LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount)
                    Game_Play.Instance.StandUp();
        }
        public void DestroyObjectsOnReset()
        {
            if (ObjectsToDestroy.Count > 0)
            {
                foreach (GameObject obj in ObjectsToDestroy)
                {
                    if (obj)
                        Destroy(obj);
                }
            }
            ObjectsToDestroy.Clear();
        }




        #endregion

        // Pun Call Backs
        #region PunCall backs
        IEnumerator WaitForAllPlayerTurns()
        {
            yield return new WaitUntil(() => PlayerStateManager.Instance.PlayingList.All(o => o.LWTurn = false));

        }
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (MatchHandler.IsLuckyWar())
            {
                // Debug.LogError("TestHere.....Master Cient...Step...1");
                if ((RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn) && PlayerStateManager.Instance.PlayingList.Count <= 1)
                {
                    // Debug.LogError("TestHere.....Master Cient...Step...3");
                    if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.AbleToJoin)
                        GameResetManager.Instance.ResetLWGame();
                }

                if ((RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying && !UIManager.Instance.GetMyPlayerInfo().cardShuffleBool))
                {
                    //Debug.LogError("TestHere.....Master Cient...Step...2");
                    CardDistributionStart();

                }
            }
        }
        #endregion
    }

}