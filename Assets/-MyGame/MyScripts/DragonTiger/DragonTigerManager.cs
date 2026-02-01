using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
//using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class DragonTigerManager : MonoBehaviour
    {
        public Color[] Colors;
        public GameObject[] objectsToDisable;
        public GameObject[] objectsToEnable;
        public GameObject[] BetAmountSelectAnim;
        public GameObject winImage;
        public GameObject BetAmountsParent;
        public GameObject DTHistoryPanel;
        public GameObject DTHistoryBtnsParent;
        public GameObject dragonWinCardAnim;
        public GameObject tigerWinCardAnim;

        public UnityEngine.Canvas canvas;

        public RectTransform[] DTPlayerSittingPositions;
        public RectTransform ChipPrefab;
        public Transform ChipsHome;
        public Transform ChipsParent;
        public Transform[] BetPointBtnsDT;
        Transform BetPoint;

        public TMP_Text BetWillStartCount;
        public TMP_Text BetStartStopTxt;
        public TMP_Text BetWillEndTxt;
        public TMP_Text TotalHandsAndTies;
        public TMP_Text TotalDragonAndTiger;



        [ShowOnly] public bool isBetting = false;
        bool isRebetHistoryShouldClear;
        bool isRebettingBtnPressed;

        public UnityEngine.Vector3 touchPos;

        [ShowOnly] public BigInteger DTCurrentBetAmount = 0;
        public BigInteger TotalRewardOnWin = 0;
        [ShowOnly] public BigInteger bet1xInt;
        [ShowOnly] public BigInteger bet10xInt;
        [ShowOnly] public BigInteger bet50xInt;
        [ShowOnly] public BigInteger bet100xInt;

        PlayerInfo MyPlayerInfo;
        public Button rebetBtn;
        public Button Button_1000;
        public Button Button_10000;
        public Button Button_50000;
        public Button Button_100000;

        public RectTransform DummyCardsInitialPos;
        public RectTransform DummyCards1Pos;
        public RectTransform DummyCards2Pos;
        public RectTransform textArea;

        [ShowOnly] public CardProperty DTOrgCard0;
        [ShowOnly] public CardProperty DTOrgCard1;

        public DOTweenAnimation dragonAnim;
        public DOTweenAnimation tigerAnim;
        public DOTweenAnimation vSAnim;

        [ShowOnly] public List<GameObject> ObjectsToDestroyOnReset = new List<GameObject>();
        [ShowOnly] public List<GameObject> AllCreatedChips = new List<GameObject>();

        PhotonView photonView;

        [HideInInspector]
        public List<RebetFieldDT> ReBetHistory;

        UIManager uIManager;
        GameManager gameManager;
        [ShowOnly]
        public int WinningNumber;
        float screenHeight;
        public BigInteger TotalAmountPlacedOnBet = 0;
        #region Creating Instance
        private static DragonTigerManager _instance;
        public static DragonTigerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<DragonTigerManager>();
                return _instance;
            }
        }
        #endregion


        private void Awake()
        {
            if (!MatchHandler.isDragonTiger())
            {
                gameObject.SetActive(false);
                return;
            }
            if (_instance == null)
                _instance = this;
            this.photonView = GetComponent<PhotonView>();
        }
        // Start is called before the first frame update
        void Start()
        {
            if (!MatchHandler.isDragonTiger())
            {
                gameObject.SetActive(false);
                return;
            }
            uIManager = UIManager.Instance;
            gameManager = GameManager.Instance;
            LocalSettings.ToggleObjectState(objectsToDisable, false);
            LocalSettings.ToggleObjectState(objectsToEnable, true);
            LocalSettings.SetPosAndRect(GameStartManager.Instance.GameStartWaitText.transform.parent.gameObject, textArea, gameManager.PlayerTable);
            //  LocalSettings.SetPosAndRect(UIManager.Instance.InfoObj, textArea, gameManager.PlayerTable);
            AdjustNewPositions();
            if (LocalSettings.GetTotalChips() >= LocalSettings.MinBetAmount)
                AssignButtonReferenceOfDragonTiger(); ;

            if (LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.DTCard0LastRecordsKey) == null)
                FirstTimeSettingRoomRecord();
            else
                ShowLastRecordInUI();
            SetStartingHistory();
        }


        public UnityEngine.Vector2 getMyPositions(Transform imageTransform)
        {

            RectTransform imageRectTransform = imageTransform.GetComponent<RectTransform>();


            UnityEngine.Vector2 pivotOffset = new UnityEngine.Vector2(imageRectTransform.pivot.x * imageRectTransform.rect.width, imageRectTransform.pivot.y * imageRectTransform.rect.height);
            UnityEngine.Vector2 startingPosition = (UnityEngine.Vector2)imageRectTransform.localPosition - pivotOffset;
            UnityEngine.Vector2 imageSize = imageRectTransform.rect.size;
            UnityEngine.Vector2 endingPosition = startingPosition + imageSize;

            // Convert local positions to screen positions
            UnityEngine.Vector2 startingScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, canvas.transform.TransformPoint(startingPosition));
            UnityEngine.Vector2 endingScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, canvas.transform.TransformPoint(endingPosition));

            float offSetx = (endingScreenPos.x - startingScreenPos.x) / 10f;
            float offSety = (startingScreenPos.y - endingScreenPos.y) / 5f;
            offSety = Mathf.Abs(offSety);

            UnityEngine.Vector2 StartPos = new UnityEngine.Vector2(startingScreenPos.x + offSetx, (startingScreenPos.y) + offSety);
            UnityEngine.Vector2 EndPos = new UnityEngine.Vector2(endingScreenPos.x - offSetx, (endingScreenPos.y) - offSety);

            UnityEngine.Vector2 tuchPos = new UnityEngine.Vector2(UnityEngine.Random.Range(StartPos.x, EndPos.x), UnityEngine.Random.Range(EndPos.y, StartPos.y));

            return Camera.main.ScreenToWorldPoint(tuchPos);

        }

        //private int fingerId = -1;
        //bool fingerReleased;
        //public void GetTouchpos()
        //{
        //    //touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        //    if (Input.touchCount > 0)
        //    {
        //        // Iterate through all the touches
        //        foreach (Touch touch in Input.touches)
        //        {
        //            // Check if the touch phase is Ended (finger lifted off the screen)
        //            if (touch.phase == TouchPhase.Ended)
        //            {
        //                // Check if this touch matches the tracked finger ID
        //                if (touch.fingerId == fingerId)
        //                {
        //                    // Get the position of the finger when it is released
        //                    UnityEngine.Vector2 releasedPosition = touch.position;
        //                    touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        //                    //Debug.Log("Finger released at: " + releasedPosition);

        //                    // Reset the finger tracking
        //                    fingerId = -1;
        //                    fingerReleased = true;
        //                }
        //            }
        //            // Check if the touch phase is Began (finger touches the screen)
        //            else if (touch.phase == TouchPhase.Began)
        //            {
        //                // Track the finger by storing its ID
        //                fingerId = touch.fingerId;
        //                fingerReleased = false;
        //            }
        //        }
        //    }
        //}

        void AdjustNewPositions()
        {
            for (int i = 0; i < 5; i++)
            {
                gameManager.position_availability[i].Pos.position = DTPlayerSittingPositions[i].position;
                LocalSettings.SetPosAndRect(gameManager.position_availability[i].Pos.gameObject, DTPlayerSittingPositions[i], DTPlayerSittingPositions[i].transform.parent);
            }
        }




        public void Playanim()
        {
            foreach (PlayerInfo item in gameManager.playersList)
            {
                item.iSDragonTigerStart = true;
            }
            //Debug.LogError("brother Yahan Animation ka issue araha hai....");
            //dragonAnim.onComplete.AddListener(() => asdfs(network_Number));
            tigerAnim.DORewind();
            dragonAnim.DORewind();
            vSAnim.DORewind();
            dragonAnim.DOPlayForward();
            vSAnim.DOPlayForward();
            tigerAnim.DOPlayForward();

        }

        // On Animation COmplete of Dragon

        public void BettingOpenGameStartedDT()
        {
            if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsStarting)
            {
                resetGame();
                if (PhotonNetwork.IsMasterClient)
                    GenerateCardsIntArrayDT();
            }
            else if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                winImage.SetActive(false);
                BetPointBtnsDT[0].GetChild(1).gameObject.SetActive(true);
                RewardOfDT();
            }

        }

        public void WiningComplete()
        {
            if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                winImage.SetActive(false);
                BetPointBtnsDT[2].GetChild(1).gameObject.SetActive(true);
                RewardOfDT();
            }

        }
        //Betting End 
        bool isDone;
        public void BetWillEnd(float remainingSeconds)
        {
            if (!BetWillEndTxt.transform.parent.gameObject.activeInHierarchy && !isDone && UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
            {
                isDone = true;
                BetWillEndTxt.transform.parent.gameObject.SetActive(true);
                BetStartStopTxt.text = "BETTING OPEN...";
                BetStartStopTxt.gameObject.SetActive(true);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Notification, false);
                //Debug.LogError("Betting open sound is playing");
            }
            BetWillEndTxt.text = "Betting will end in " + (int)remainingSeconds + " seconds";
        }

        // Show result for everyone
        #region Winning Session
        public void ShowResultDragonTiger()
        {
            StartCoroutine(ShowOriginalCards());
        }

        IEnumerator ShowOriginalCards()
        {
            yield return new WaitUntil(() => !BetStartStopTxt.gameObject.activeInHierarchy);
            if (gameManager != null || PhotonNetwork.IsConnectedAndReady)
                Array.Copy(LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.DT_card_listKey), DTRandomArrayCards, gameManager.AllCards.Card.Length);
            CreateOriginalCards();
            foreach (var item in DummyDTCards)
            {
                item.SetActive(false);
            }


            SavingRecordInRoomProperty(DTRandomArrayCards[0], DTRandomArrayCards[1]);

            DTOrgCard0.gameObject.SetActive(true);
            DTOrgCard1.gameObject.SetActive(true);
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
            if (DTOrgCard0.Power > DTOrgCard1.Power)
            {
                //Dragon WOn
                dragonAnim.DORewind();
                dragonAnim.DOPlay();
                dragonWinCardAnim.SetActive(true);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.DragonSound, false);
                winImage.SetActive(true);
                WinningNumber = 0;

                if (PhotonNetwork.IsMasterClient)
                {
                    int totalwins = 0;
                    if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalDragonWinKey) != null)
                        totalwins = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalDragonWinKey);
                    totalwins++;
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.DTTotalDragonWinKey, totalwins);
                }

            }
            else if (DTOrgCard0.Power < DTOrgCard1.Power)
            {
                //Tiger Won
                tigerAnim.DORewind();
                tigerAnim.DOPlay();
                tigerWinCardAnim.SetActive(true);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.TigerSound, false);
                winImage.SetActive(true);
                BetPointBtnsDT[2].GetChild(1).gameObject.SetActive(true);
                WinningNumber = 2;
                if (PhotonNetwork.IsMasterClient)
                {
                    int totalwins = 0;
                    if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalTigerWinKey) != null)
                        totalwins = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalTigerWinKey);
                    totalwins++;
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.DTTotalTigerWinKey, totalwins);
                }
            }
            else
            {
                // Tie Won
                dragonWinCardAnim.SetActive(true);
                tigerWinCardAnim.SetActive(true);
                WinningNumber = 1;
                BetPointBtnsDT[1].GetChild(1).gameObject.SetActive(true);
                RewardOfDT();
                if (PhotonNetwork.IsMasterClient)
                {
                    int totalwins = 0;
                    if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalTieWinKey) != null)
                        totalwins = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalTieWinKey);
                    totalwins++;
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.DTTotalTieWinKey, totalwins);
                }
            }
        }



        void RewardOfDT()
        {
            uIManager.GetMyPlayerInfo().SendChipsToEveryOneDT();
            //LocalSettings.SetTotalChips(TotalRewardOnWin);
            if (TotalRewardOnWin > 1)
            {
                uIManager.TotalWinAmountFor1Game = TotalRewardOnWin;

                uIManager.SetTotalWinHandByPlayer();
                //winnerBetAmountTxt.text = "Cong! You Win:- " + LocalSettings.Rs(TotalRewardOnWin).ToString();
                GameManager.Instance.AddXPToMyPlayer(true);
            }
            //uIManager.GetMyPlayerInfo().SendChipsToEveryOne();
            LocalSettings.SetTotalChips(TotalRewardOnWin);
            //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, TotalRewardOnWin.ToString());
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Reward, false);
            uIManager.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
            uIManager.GetMyPlayerInfo().MyTotalCashTextUpdate();
            //Debug.LogError("checking Cash");

            ResetDragonTimeUsingDelay(4);
            //Debug.LogError("Resetting game on complete ------------------");
        }


        #endregion

        // Creating and distributing original cards
        #region Create Original Cards
        public int[] DTRandomArrayCards = new int[52];
        public void GenerateCardsIntArrayDT()
        {
            for (int i = 0; i < gameManager.AllCards.Card.Length; i++)
            {
                DTRandomArrayCards[i] = i;
            }
            ShuffleArray(DTRandomArrayCards);

            if (PhotonNetwork.IsConnectedAndReady)
            {
                LocalSettings.GetCurrentRoom.SetCardsList(LocalSettings.DT_card_listKey, DTRandomArrayCards);
                photonView.RPC(nameof(RandomCardsArrayRPC), RpcTarget.All, DTRandomArrayCards);
            }

        }


        [PunRPC]
        public void RandomCardsArrayRPC(int[] randArray)
        {
            Array.Copy(randArray, DTRandomArrayCards, randArray.Length);
            //CreateOriginalCards();
            CreateAbdDustributeDummyCards();
        }

        GameObject DTCard0;
        GameObject DTCard1;
        void CreateOriginalCards()
        {
            // Creating DT Card 1
            DTCard0 = Instantiate(gameManager.AllCards.Card[DTRandomArrayCards[0]].gameObject);
            ObjectsToDestroyOnReset.Add(DTCard0);
            DTOrgCard0 = DTCard0.GetComponent<CardProperty>();
            if (DTOrgCard0.Power == 14)
                DTOrgCard0.Power = 1;
            LocalSettings.SetPosAndRect(DTCard0, DummyCards1Pos, DummyCards1Pos.transform.parent);
            DTCard0.SetActive(false);
            // Creating DT Card 2
            DTCard1 = Instantiate(gameManager.AllCards.Card[DTRandomArrayCards[1]].gameObject);
            ObjectsToDestroyOnReset.Add(DTCard1);
            DTOrgCard1 = DTCard1.GetComponent<CardProperty>();
            if (DTOrgCard1.Power == 14)
                DTOrgCard1.Power = 1;
            LocalSettings.SetPosAndRect(DTCard1, DummyCards2Pos, DummyCards2Pos.transform.parent);
            DTCard1.SetActive(false);
        }
        void ShuffleArray(int[] array)
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
        #endregion

        // Creating and distributing dummy cards
        #region Creating and distributing Dummy cards

        List<GameObject> DummyDTCards = new List<GameObject>();
        void CreateAbdDustributeDummyCards()
        {
            DummyDTCards = new List<GameObject>();
            for (int i = 0; i < 2; i++)
            {
                GameObject dumyCrd = Instantiate(gameManager.DummyCardPrefab);
                ObjectsToDestroyOnReset.Add(dumyCrd);
                DummyDTCards.Add(dumyCrd);
                dumyCrd.SetActive(true);
                LocalSettings.SetPosAndRect(dumyCrd, DummyCardsInitialPos, DummyCardsInitialPos.transform.parent);

            }
            StartCoroutine(DistributeDummyCards());
        }

        public void AssingDummyCardPlayerOutOfGame()
        {
            DummyDTCards = new List<GameObject>();
            for (int i = 0; i < 2; i++)
            {
                GameObject dumyCrd = Instantiate(gameManager.DummyCardPrefab);
                ObjectsToDestroyOnReset.Add(dumyCrd);
                DummyDTCards.Add(dumyCrd);
                RectTransform targetPos = null;
                if (i == 0)
                    targetPos = DummyCards1Pos;
                else
                    targetPos = DummyCards2Pos;

                LocalSettings.SetPosAndRect(dumyCrd, targetPos, DummyCardsInitialPos.transform.parent);

                dumyCrd.SetActive(true);
            }

        }


        IEnumerator DistributeDummyCards()
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < DummyDTCards.Count; i++)
            {
                Transform targetPos = null;
                if (i == 0)
                    targetPos = DummyCards1Pos.transform;
                else
                    targetPos = DummyCards2Pos.transform;
                StartCoroutine(PlayAnimation(DummyDTCards[i].transform, targetPos));
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(0.5f);
            // Debug.LogError("BettingOpenCount8
            resetGame();
            LocalSettings.Vibrate();
            GameStartManager.Instance.OpenBettingForDT();
            yield return new WaitUntil(() => !BetStartStopTxt.gameObject.activeInHierarchy);
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.CardDistributing);
        }

        public void resetGame()
        {
            if (GameStartManager.Instance._currentNumberOfPlayers < 1)
            {
                // Call Reset Function
                GameResetManager.Instance.ResetDragonTigerGame();
                return;

            }
        }



        public IEnumerator PlayAnimation(Transform ObjToAnimate, Transform targetPosition)
        {
            yield return new WaitForSeconds(0);
            if (targetPosition != null)
                ObjToAnimate.DOMove(targetPosition.position, 0.3f, false);
            float RotationOffSet = targetPosition.eulerAngles.z;
            ObjToAnimate.DOLocalRotate(new UnityEngine.Vector3(0, 0, 360 + RotationOffSet), 0.3f, RotateMode.FastBeyond360);
            ObjToAnimate.DOScale(new UnityEngine.Vector3(1.5f, 1.5f, 1.5f), 0.3f);
        }


        #endregion

        // Betting points functionality
        #region Betting point

        void AssignButtonReferenceOfDragonTiger()
        {
            bet1xInt = LocalSettings.MinBetAmount;
            bet10xInt = LocalSettings.MinBetAmount * 10;
            bet50xInt = LocalSettings.MinBetAmount * 50;
            bet100xInt = LocalSettings.MinBetAmount * 100;

            string Bet1x = LocalSettings.FormatNumber(bet1xInt.ToString());
            string Bet10x = LocalSettings.FormatNumber(bet10xInt.ToString());
            string Bet50x = LocalSettings.FormatNumber(bet50xInt.ToString());
            string Bet100x = LocalSettings.FormatNumber(bet100xInt.ToString());

            Button_1000.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet1x;
            Button_10000.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet10x;
            Button_50000.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet50x;
            Button_100000.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet100x;

            Button_1000.onClick.AddListener(() => OnBetAmountsBtnClick(bet1xInt));
            Button_10000.onClick.AddListener(() => OnBetAmountsBtnClick(bet10xInt));
            Button_50000.onClick.AddListener(() => OnBetAmountsBtnClick(bet50xInt));
            Button_100000.onClick.AddListener(() => OnBetAmountsBtnClick(bet100xInt));

            OnBetAmountsBtnClick(bet1xInt);
        }
        public void OnBetAmountsBtnClick(BigInteger BetAmount)
        {
            CheckEnoughAmountAvailableToBet();
            DTCurrentBetAmount = BetAmount;
            if (bet1xInt == BetAmount)
                SetOneObjectActive(0, BetAmountSelectAnim);
            else if (bet10xInt == BetAmount)
                SetOneObjectActive(1, BetAmountSelectAnim);
            else if (bet50xInt == BetAmount)
                SetOneObjectActive(2, BetAmountSelectAnim);
            else if (bet100xInt == BetAmount)
                SetOneObjectActive(3, BetAmountSelectAnim);

            #region For Future Need
            //switch (BetAmount)
            //{
            //    case 1000:
            //        SetOneObjectActive(0, BetAmountSelectAnim);
            //        break;
            //    case 10000:
            //        SetOneObjectActive(1, BetAmountSelectAnim);
            //        break;
            //    case 50000:
            //        SetOneObjectActive(2, BetAmountSelectAnim);
            //        break;
            //    case 100000:
            //        SetOneObjectActive(3, BetAmountSelectAnim);
            //        break;
            //}
            #endregion
        }
        void SetOneObjectActive(int index, GameObject[] ObjectsArray)
        {
            foreach (GameObject obj in ObjectsArray)
            {
                if (obj)
                    if (obj.activeSelf)
                        obj.SetActive(false);
                ObjectsArray[index].SetActive(true);
            }
        }

        public void OnBetPointBtnClick(int PointNumber)
        {
            // Return if the player is out of table
            if (uIManager.GetMyPlayerCurrentState().currentState == PlayerState.STATE.OutOfTable)
                return;

            // if the room state is game is playing
            if (!isBetting && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                if (LocalSettings.GetTotalChips() < DTCurrentBetAmount)
                {
                    uIManager.quickShop.SetActive(true);
                    return;
                }

                BigInteger currentAmountOnPoint = BetPointBtnsDT[PointNumber - 1].GetComponent<PointBetAmount>().MineBetAmount;
                if (currentAmountOnPoint >= Pot.instance.maximumBet)
                {
                    return;
                }

                if (isRebetHistoryShouldClear)
                {
                    isRebetHistoryShouldClear = false;
                    if (ReBetHistory.Count > 0)
                        ReBetHistory.Clear();
                    rebetBtn.gameObject.SetActive(false);
                }

                // Adding bet in history
                if (!isRebettingBtnPressed)
                {
                    RebetFieldDT rebetFieldDT = new RebetFieldDT();
                    rebetFieldDT.PointNumber = PointNumber;
                    rebetFieldDT.BetAmount = DTCurrentBetAmount;
                    rebetFieldDT.betAmountInString = DTCurrentBetAmount.ToString();
                    rebetFieldDT.CoinPos = touchPos;
                    ReBetHistory.Add(rebetFieldDT);
                }

                // Subtract bet amount
                LocalSettings.SetTotalChips(-DTCurrentBetAmount);
                //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, DTCurrentBetAmount.ToString());
                int PointNumberInt = PointNumber - 1;
                DragonTigerAPI.Instance.DTSendBet(PointNumberInt.ToString(), DTCurrentBetAmount.ToString(), getWinPoint().ToString());
                TotalAmountPlacedOnBet += DTCurrentBetAmount;
                if (MyPlayerInfo == null)
                    MyPlayerInfo = uIManager.GetMyPlayerInfo();
                //Debug.LogError("Remaining Cash: " + LocalSettings.GetTotalChips());
                MyPlayerInfo.playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
                CheckEnoughAmountAvailableToBet();
                BetPoint = BetPointBtnsDT[PointNumber - 1];

                //#if !UNITY_EDITOR
                //                GetTouchpos();
                //#endif

                //touchPos = getMyPositions(BetPoint);
                MyPlayerInfo.BetOnDragonTiger(DTCurrentBetAmount, PointNumberInt, touchPos, isRebettingBtnPressed);
                uIManager.TotalBetPlacedAmount += DTCurrentBetAmount;
                uIManager.TotalBetPlaceFor1Game += DTCurrentBetAmount;
                uIManager.isPlayerPlayedThisHand = true;


            }
        }

        CardsContainer AllCards;
        int getWinPoint()
        {
            if (AllCards == null)
                AllCards = GameManager.Instance.AllCards;
            CardProperty Card0 = AllCards.Card[DTRandomArrayCards[0]];
            CardProperty Card1 = AllCards.Card[DTRandomArrayCards[1]];
            int winPoint = 0;
            if (Card0.Power < Card1.Power)
                winPoint = 2;
            else if (Card0.Power == Card1.Power)
                winPoint = 1;
            return winPoint;
        }

        void CheckEnoughAmountAvailableToBet()
        {
            BigInteger amount = LocalSettings.GetTotalChips();
            if (amount >= bet100xInt)
            {
                Button_1000.interactable = true;
                Button_10000.interactable = true;
                Button_50000.interactable = true;
                Button_100000.interactable = true;

            }
            else if (amount >= bet50xInt)
            {
                Button_1000.interactable = true;
                Button_10000.interactable = true;
                Button_50000.interactable = true;
                Button_100000.interactable = false;
            }
            else if (amount >= bet10xInt)
            {
                Button_1000.interactable = true;
                Button_10000.interactable = true;
                Button_50000.interactable = false;
                Button_100000.interactable = false;
            }
            else if (amount >= bet1xInt)
            {
                Button_1000.interactable = true;
                Button_10000.interactable = false;
                Button_50000.interactable = false;
                Button_100000.interactable = false;
            }
            else
            {
                Button_1000.interactable = false;
                Button_10000.interactable = false;
                Button_50000.interactable = false;
                Button_100000.interactable = false;
            }
        }

        public GameObject CreateChipToShow()
        {
            GameObject chip = Instantiate(ChipPrefab.gameObject);
            chip.transform.parent = ChipsParent.transform;
            AllCreatedChips.Add(chip);
            return chip;
        }

        public void OnRebetBtnClick()
        {
            StartCoroutine(RebetOnPoint());
        }

        IEnumerator RebetOnPoint()
        {
            rebetBtn.gameObject.SetActive(false);
            isRebetHistoryShouldClear = false;
            isRebettingBtnPressed = true;
            BtnsEnableDisable(false);
            yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            foreach (RebetFieldDT rebetPoint in ReBetHistory)
            {
                DTCurrentBetAmount = rebetPoint.BetAmount;
                touchPos = rebetPoint.CoinPos;
                OnBetPointBtnClick(rebetPoint.PointNumber);
                //yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
            isRebettingBtnPressed = false;
            OnBetAmountsBtnClick((int)DTCurrentBetAmount);
            BtnsEnableDisable(true);
        }
        void BtnsEnableDisable(bool IsEnable)
        {
            foreach (Transform tr in BetPointBtnsDT)
                tr.GetComponent<Button>().enabled = IsEnable;
        }
        #endregion

        // Resetting DT Game 
        #region Reset Dragon Tiger Game 

        public void ResetDragonTimeUsingDelay(float delayTime)
        {
            StartCoroutine(ResetDragonTrigerCoroutine(delayTime));
        }
        IEnumerator ResetDragonTrigerCoroutine(float delayTime)
        {
            isRebetHistoryShouldClear = true;
            yield return new WaitForSeconds(delayTime);
            GameResetManager.Instance.ResetDragonTigerGame();
        }

        public void ResetDTForGameResetMamager()
        {
            foreach (PlayerInfo item in gameManager.playersList)
            {
                item.iSDragonTigerStart = false;
            }
            GameStartManager.Instance._1stCurrentChaalBool = true;
            if (DummyDTCards.Count > 1)
            {
                if (DummyDTCards[0])
                    Destroy(DummyDTCards[0]);
                if (DummyDTCards[1])
                    Destroy(DummyDTCards[1]);
            }
            DummyDTCards.Clear();
            foreach (GameObject obj in ObjectsToDestroyOnReset)
            {
                if (obj)
                    Destroy(obj);
            }

            foreach (GameObject chip in AllCreatedChips)
            {
                if (chip)
                    Destroy(chip);
            }
            AllCreatedChips.Clear();
            ObjectsToDestroyOnReset.Clear();
            TotalRewardOnWin = 0;
            CheckEnoughAmountAvailableToBet();
            foreach (Transform betPoint in BetPointBtnsDT)
            {
                if (betPoint.GetChild(1).gameObject.activeInHierarchy)
                    betPoint.GetChild(1).gameObject.SetActive(false);
                betPoint.gameObject.GetComponent<Button>().interactable = true;
                betPoint.GetComponent<PointBetAmount>().ResetBetAmounts();
            }
            uIManager.isPlayerPlayedThisHand = false;
            isBetting = false;
            BetWillEndTxt.transform.parent.gameObject.SetActive(false);
            isDone = false;
            PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationDragonTiger;
            if (uIManager.GetMyPlayerInfo().photonView.IsMine)
                PositionsManager.Instance.AssignMyLocalPositionWithAllOtherClients();
            if (LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount && uIManager.GetMyPlayerInfo().photonView.IsMine)
            {
                uIManager.quickShop.SetActive(true);
                uIManager.GetMyPlayerInfo().StandUp();
            }
            dragonWinCardAnim.SetActive(false);
            tigerWinCardAnim.SetActive(false);
        }



        #endregion

        // Dragon Tiger history Panel 
        #region Dragon Tiger History Show

        void FirstTimeSettingRoomRecord()
        {
            int[] WinNumbersRecordCard0 = new int[25];
            int[] WinNumbersRecordCard1 = new int[25];
            //int[] AndarBaharWinIndex = new int[25];
            for (int i = 0; i < WinNumbersRecordCard0.Length; i++)
            {
                WinNumbersRecordCard0[i] = UnityEngine.Random.Range(0, 52);
                WinNumbersRecordCard1[i] = UnityEngine.Random.Range(0, 52);
                //int Probability = UnityEngine.Random.Range(0, 20);
                //AndarBaharWinIndex[i] = Probability == 3 ? 2 : UnityEngine.Random.Range(0, 2);
            }
            LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.DTCard0LastRecordsKey, WinNumbersRecordCard0);
            LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.DTCard1LastRecordsKey, WinNumbersRecordCard1);

            Invoke(nameof(ShowLastRecordInUI), 2f);
        }

        void SavingRecordInRoomProperty(int Card0Val, int Card1Val)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int[] tempArrForCard0 = new int[25];
                int[] tempArrForCard1 = new int[25];

                tempArrForCard0 = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.DTCard0LastRecordsKey);
                tempArrForCard1 = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.DTCard1LastRecordsKey);

                for (int i = tempArrForCard0.Length - 1; i > 0; i--)
                {
                    tempArrForCard0[i] = tempArrForCard0[i - 1];
                    tempArrForCard1[i] = tempArrForCard1[i - 1];
                }
                tempArrForCard0[0] = Card0Val;
                tempArrForCard1[0] = Card1Val;

                LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.DTCard0LastRecordsKey, tempArrForCard0);
                LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.DTCard1LastRecordsKey, tempArrForCard1);
            }

            Invoke(nameof(ShowLastRecordInUI), 1f);
        }

        public void ShowLastRecordInUI()
        {
            int[] tempArrForCard0 = new int[25];
            int[] tempArrForCard1 = new int[25];

            if (PhotonNetwork.IsConnectedAndReady)
            {
                tempArrForCard0 = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.DTCard0LastRecordsKey);
                tempArrForCard1 = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.DTCard1LastRecordsKey);
            }

            for (int i = 0; i < tempArrForCard0.Length; i++)
            {
                DTHistoryBtnsParent.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = gameManager.AllCards.Card[tempArrForCard0[i]].GetComponent<Image>().sprite;
                DTHistoryBtnsParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = gameManager.AllCards.Card[tempArrForCard1[i]].GetComponent<Image>().sprite;

                if (gameManager.AllCards.Card[tempArrForCard0[i]].Power > gameManager.AllCards.Card[tempArrForCard1[i]].Power)
                {
                    if (gameManager.AllCards.Card[tempArrForCard0[i]].Power != 14)
                    {

                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(4).gameObject.SetActive(false);

                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Colors[0];
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = Colors[1];
                    }
                    else
                    {
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(3).gameObject.SetActive(true);
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(4).gameObject.SetActive(false);

                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Colors[1];
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = Colors[0];

                    }
                }
                else if (gameManager.AllCards.Card[tempArrForCard0[i]].Power < gameManager.AllCards.Card[tempArrForCard1[i]].Power)
                {
                    if (gameManager.AllCards.Card[tempArrForCard1[i]].Power != 14)
                    {
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(3).gameObject.SetActive(true);
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(4).gameObject.SetActive(false);

                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Colors[1];
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = Colors[0];
                    }
                    else
                    {
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(4).gameObject.SetActive(false);

                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Colors[0];
                        DTHistoryBtnsParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = Colors[1];
                    }

                }
                else
                {
                    DTHistoryBtnsParent.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                    DTHistoryBtnsParent.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
                    DTHistoryBtnsParent.transform.GetChild(i).GetChild(4).gameObject.SetActive(true);

                    DTHistoryBtnsParent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Colors[0];
                    DTHistoryBtnsParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = Colors[0];
                }

            }

            // Showing total Dragon wins
            float totalwinsDragon = 0;
            if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalDragonWinKey) != null)
                totalwinsDragon = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalDragonWinKey);

            // Showing total Tiger wins
            float totalwinsTiger = 0;
            if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalTigerWinKey) != null)
                totalwinsTiger = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalTigerWinKey);

            // Showing total tie wins
            float totalwinsTie = 0;
            if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalTieWinKey) != null)
                totalwinsTie = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalTieWinKey);

            float totalGames = totalwinsDragon + totalwinsTiger + totalwinsTie;

            TotalHandsAndTies.text = "Hands: " + totalGames + "\nTie: " + totalwinsTie + " (" + ((totalGames == 0) ? 0 : ((((totalwinsTie / totalGames) * 100))).ToString("F2")) + "%)";
            TotalDragonAndTiger.text = "Dragon: " + totalwinsDragon + " (" + ((totalGames == 0) ? 0 : (((totalwinsDragon / totalGames) * 100)).ToString("F2")) + "%)" + "\nTiger: " + totalwinsTiger + " (" + ((totalGames == 0) ? 0 : (((totalwinsTiger / totalGames) * 100)).ToString("F2")) + "%)";
        }
        void SetStartingHistory()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DTTotalDragonWinKey) == 0)
                {
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.DTTotalDragonWinKey, UnityEngine.Random.Range(105, 160));
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.DTTotalTigerWinKey, UnityEngine.Random.Range(105, 160));
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.DTTotalTieWinKey, UnityEngine.Random.Range(6, 13));
                }
            }
        }
        #endregion
    }

    [System.Serializable]
    public class RebetFieldDT
    {
        public int PointNumber;
        public BigInteger BetAmount;
        public string betAmountInString;
        public UnityEngine.Vector3 CoinPos;
    }
}