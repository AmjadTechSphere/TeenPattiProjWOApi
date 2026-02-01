using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class WingoManager : MonoBehaviour
    {
        public GameObject[] objectsToDisable;
        public GameObject[] objectsToEnable;
        public RectTransform[] WingoPositions;
        public GameObject[] BetAmountSelectAnim;

        public Transform[] BetPointBtns;
        public RectTransform textArea;

        public UnityEngine.Canvas canvas;

        public TMP_Text winnerBetAmountTxt;
        public TMP_Text BetWillEndTxt;
        public TMP_Text BetWillStartCount;
        public TMP_Text BetStartStopTxt;
        public TMP_Text playerName;

        [Header(" Record In percentage Text")]
        public TMP_Text[] HistoryRecordTxt;


        public int[] WinNumbersRecord = new int[32];

        public Transform ChipsHome;
        public Transform ChipsParent;
        public List<GameObject> AllCreatedChips = new List<GameObject>();
        public RectTransform ChipPrefab;

        [ShowOnly] public bool isBetting = false;
        [ShowOnly] public bool isPlayerEnteredDuringGame = false;


        [ShowOnly] public BigInteger WingoCurrentBetAmount = 0;
        [ShowOnly] public float RewardMultiplier1_3_7_9_2_4_6_8;
        [ShowOnly] public float RewardMultiplier0_5;
        [ShowOnly] public float RewardMultiplier0_9;
        [ShowOnly] float timeDurationToSpin;

        Transform BetPoint;

        public GameObject BetAmountsParent;

        public Button rebetBtn;
        public Button Button_1000;
        public Button Button_10000;
        public Button Button_50000;
        public Button Button_100000;

        public Image profileImage;

        public RawImage SpinnerBar;
        [ShowOnly]
        public float TargetValue;
        //[Range(0, 360)]
        public float CurrentuvRectValue;
        PlayerInfo MyPlayerInfo;
        UIManager uIManager;
        public UnityEngine.Vector3 touchPos;
        //public Button swtichTable;
        public Button nextHand;

        public BigInteger TotalRewardOnWin = 0;


        public LottaryNumber[] Spinner;
        [ShowOnly]
        public int WinningNumber;

        public Sprite[] NumberBallsPreviousRecord;
        public Transform NumberBallsParent;
        public Transform WingoHistoryRecordPanel;

        [HideInInspector]
        public List<RebetField> ReBetHistory;
        bool isRebetHistoryShouldClear;
        bool isRebettingBtnPressed;
        public BigInteger TotalAmountPlacedOnBet = 0;
        //float screenHeight;
        // Start is called before the first frame update
        #region Creating Instance
        private static WingoManager _instance;
        public static WingoManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<WingoManager>();
                return _instance;
            }
        }
        #endregion


        private void Awake()
        {
            if (!MatchHandler.isWingoLottary())
            {
                gameObject.SetActive(false);
                return;
            }
            if (_instance == null)
                _instance = this;
        }
        void Start()
        {
            if (!MatchHandler.isWingoLottary())
                return;
            //screenHeight = Screen.height;
            LocalSettings.ToggleObjectState(objectsToDisable, false);
            LocalSettings.ToggleObjectState(objectsToEnable, true);
            objectsToDisable[0].transform.parent.GetComponent<Image>().enabled = false;
            LocalSettings.SetPosAndRect(GameStartManager.Instance.GameStartWaitText.transform.parent.gameObject, textArea, GameManager.Instance.PlayerTable);
            LocalSettings.SetPosAndRect(UIManager.Instance.InfoObj, textArea, GameManager.Instance.PlayerTable);
            uIManager = UIManager.Instance;

            nextHand.interactable = false;
            if (LocalSettings.GetTotalChips() >= LocalSettings.MinBetAmount)
                AssignButtonReferenceOfWingowLottery();
            SetRewardMultiplier();
            AdjustNewPositions();
            isBetting = false;
            isBetting = false;
            BetWillEndTxt.transform.parent.gameObject.SetActive(false);
            BetStartStopTxt.gameObject.SetActive(false);
            if (LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.WingoLastRecordsKey) == null)
                FirstTimeSettingRoomRecord();
            else
                ShowLastRecordInUI();

            rebetBtn.gameObject.SetActive(false);

        }
        [ShowOnly] public BigInteger bet1xInt;
        [ShowOnly] public BigInteger bet10xInt;
        [ShowOnly] public BigInteger bet50xInt;
        [ShowOnly] public BigInteger bet100xInt;
        void AssignButtonReferenceOfWingowLottery()
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


        int ConverStringtoint(string betString)
        {
            int betAmont;
            if (int.TryParse(betString, out betAmont))
            {
                betAmont = int.Parse(betString);
            }
            return betAmont;
        }


        //public void UpdateTheWinAmount()
        //{
        //    if (isPlayerPlayedThisHand)
        //        TotalHands++;

        //    TotalWinsAmount += TotalRewardOnWin;
        //    BigInteger currentWinReport = TotalWinsAmount - TotalBetPlacedAmount;

        //    uIManager.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.totalcashWinLossKey, currentWinReport);
        //    uIManager.GetMyPlayerInfo().player.SetCustomData(LocalSettings.TotalHandsKey, TotalHands);
        //    uIManager.GetMyPlayerInfo().player.SetCustomData(LocalSettings.WinHandsKey, TotalWinHands);
        //    uIManager.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());

        //}


        //public void UpdateTexts(string WinAmount, string Hands, string CashAmount, Sprite profileSprite, string namePlayer)
        //{
        //    PlayerTotalCashText.text = CashAmount;
        //    if (WinAmount.Contains("-"))
        //        TotalWinAmountText.color = Color.red;
        //    else
        //        TotalWinAmountText.color = Color.green;

        //    TotalWinAmountText.text = LocalSettings.Rs(WinAmount);
        //    TotalHandsText.text = Hands;
        //    profileImage.sprite = profileSprite;
        //    playerName.text = namePlayer;
        //}


        void FirstTimeSettingRoomRecord()
        {
            for (int i = 0; i < WinNumbersRecord.Length; i++)
            {
                WinNumbersRecord[i] = Random.Range(0, 10);
            }
            LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.WingoLastRecordsKey, WinNumbersRecord);
            // First time history of all points
            int[] HistoryAllPoints = new int[14];

            // Total match increment
            HistoryAllPoints[0] = WinNumbersRecord.Length;
            // Incrementing points based on history
            for (int i = 0; i < WinNumbersRecord.Length; i++)
            {
                if (WinNumbersRecord[i] == 1 || WinNumbersRecord[i] == 3 || WinNumbersRecord[i] == 7 || WinNumbersRecord[i] == 9)
                {
                    HistoryAllPoints[1]++;
                }
                else if (WinNumbersRecord[i] == 0 || WinNumbersRecord[i] == 5)
                {
                    HistoryAllPoints[2]++;
                }
                else if (WinNumbersRecord[i] == 2 || WinNumbersRecord[i] == 4 || WinNumbersRecord[i] == 6 || WinNumbersRecord[i] == 8)
                {
                    HistoryAllPoints[3]++;
                }
                HistoryAllPoints[WinNumbersRecord[i] + 4]++;
            }
            LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.WingoHistoryAllPointsKey, HistoryAllPoints);

            Invoke(nameof(ShowLastRecordInUI), 2f);
        }

        public void ShowLastRecordInUI()
        {
            // Show Previous Winning Numbers On UI 
            // If there is -1 then That Item is Empty You can show "-"

            foreach (var item in LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.WingoLastRecordsKey))
            {
                //Debug.LogError(item);

            }
            int[] tempArr = new int[32];
            tempArr = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.WingoLastRecordsKey);
            for (int i = 0; i < tempArr.Length; i++)
            {
                if (i < NumberBallsParent.childCount)
                    NumberBallsParent.GetChild(i).GetComponent<Image>().sprite = NumberBallsPreviousRecord[tempArr[i]];
                WingoHistoryRecordPanel.GetChild(i).GetComponent<Image>().sprite = NumberBallsPreviousRecord[tempArr[i]];
            }


            if (uIManager.GetMyPlayerInfo() != null)
            {
                //Debug.LogError("Remaining Cash: " + LocalSettings.GetTotalChips());
                uIManager.GetMyPlayerInfo().playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
            }

            // Showing history of wingo record in percentage
            int[] HistoryAllPoints = new int[14];
            HistoryAllPoints = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.WingoHistoryAllPointsKey);

            float totalGames = HistoryAllPoints[0];
            HistoryRecordTxt[0].text = "Games: " + HistoryAllPoints[0];
            HistoryRecordTxt[1].text = "1/3/7/9: " + (((float)HistoryAllPoints[1]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[2].text = "0/5: " + (((float)HistoryAllPoints[2]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[3].text = "2/4/6/8: " + (((float)HistoryAllPoints[3]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[4].text = "0: " + (((float)HistoryAllPoints[4]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[5].text = "1: " + (((float)HistoryAllPoints[5]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[6].text = "2: " + (((float)HistoryAllPoints[6]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[7].text = "3: " + (((float)HistoryAllPoints[7]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[8].text = "4: " + (((float)HistoryAllPoints[8]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[9].text = "5: " + (((float)HistoryAllPoints[9]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[10].text = "6: " + (((float)HistoryAllPoints[10]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[11].text = "7: " + (((float)HistoryAllPoints[11]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[12].text = "8: " + (((float)HistoryAllPoints[12]) * 100 / totalGames).ToString("F2") + "%";
            HistoryRecordTxt[13].text = "9: " + (((float)HistoryAllPoints[13]) * 100 / totalGames).ToString("F2") + "%";
        }

        void PuttingARecord(int val)
        {
            int[] tempArr = new int[32];
            tempArr = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.WingoLastRecordsKey);
            for (int i = tempArr.Length - 1; i > 0; i--)
            {
                tempArr[i] = tempArr[i - 1];
            }
            //Add A New Variable Here
            tempArr[0] = val;
            LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.WingoLastRecordsKey, tempArr);

            // --------------------------------------------------------------------------------------------------------
            // putting last played game history of all points
            int[] HistoryAllPoints = new int[14];
            HistoryAllPoints = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.WingoHistoryAllPointsKey);

            // Total match increment
            HistoryAllPoints[0]++;
            // Incrementing points using history
            {
                if (val == 1 || val == 3 || val == 7 || val == 9)
                {
                    HistoryAllPoints[1]++;
                }
                else if (val == 0 || val == 5)
                {
                    HistoryAllPoints[2]++;
                }
                else if (val == 2 || val == 4 || val == 6 || val == 8)
                {
                    HistoryAllPoints[3]++;
                }
                HistoryAllPoints[val + 4]++;
            }
            LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.WingoHistoryAllPointsKey, HistoryAllPoints);
        }


        //        private void Update()
        //        {
        //#if UNITY_EDITOR
        //            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //#else
        //            GetTouchpos();
        //#endif
        //        }
        //bool isGetTouchPos;
        //public void getTouchPos(bool isDown)
        //{
        //    isGetTouchPos = isDown;

        //}
        void SetRewardMultiplier()
        {
            RewardMultiplier1_3_7_9_2_4_6_8 = 2.4f;
            RewardMultiplier0_5 = 4.8f;
            RewardMultiplier0_9 = 9.6f;
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


        void AdjustNewPositions()
        {
            for (int i = 0; i < 5; i++)
            {
                GameManager.Instance.position_availability[i].Pos.position = WingoPositions[i].position;
                LocalSettings.SetPosAndRect(GameManager.Instance.position_availability[i].Pos.gameObject, WingoPositions[i], WingoPositions[i].transform.parent);
            }
        }

        public void ResetWingoLottary()
        {
            GameStartManager.Instance._1stCurrentChaalBool = true;
            ShowLastRecordInUI();
            TotalRewardOnWin = 0;
            CheckEnoughAmountAvailableToBet();
            foreach (GameObject chip in AllCreatedChips)
            {
                if (chip)
                    Destroy(chip);
            }

            foreach (Transform betPoint in BetPointBtns)
            {
                if (betPoint.GetChild(1).gameObject.activeInHierarchy)
                    betPoint.GetChild(1).gameObject.SetActive(false);
                betPoint.gameObject.GetComponent<Button>().interactable = true;
            }

            uIManager.isPlayerPlayedThisHand = false;
            isBetting = false;
            AllCreatedChips.Clear();
            BetWillEndTxt.transform.parent.gameObject.SetActive(false);
            isDone = false;
            foreach (Transform betPt in BetPointBtns)
                betPt.GetComponent<PointBetAmount>().ResetBetAmounts();
            PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationWingowLottery;
            if (uIManager.GetMyPlayerInfo().photonView.IsMine)
                PositionsManager.Instance.AssignMyLocalPositionWithAllOtherClients();

            if (LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount && uIManager.GetMyPlayerInfo().photonView.IsMine)
            {
                uIManager.quickShop.SetActive(true);
                uIManager.GetMyPlayerInfo().StandUp();
                return;
            }

        }


        public void OnBetAmountsBtnClick(BigInteger BetAmount)
        {
            CheckEnoughAmountAvailableToBet();
            WingoCurrentBetAmount = BetAmount;
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
        public void OnBetPointBtnClick(int PointNumber)
        {
            // Return if the player is out of table
            if (uIManager.GetMyPlayerCurrentState().currentState == PlayerState.STATE.OutOfTable)
                return;

            // if the room state is game is playing
            if (!isBetting && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                if (LocalSettings.GetTotalChips() < WingoCurrentBetAmount)
                {
                    uIManager.quickShop.SetActive(true);
                    return;
                }

                BigInteger currentAmountOnPoint = BetPointBtns[PointNumber - 1].GetComponent<PointBetAmount>().MineBetAmount;
                if (currentAmountOnPoint >= 10000000000)
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
                    RebetField rebetField = new RebetField();
                    rebetField.PointNumber = PointNumber;
                    rebetField.BetAmount = WingoCurrentBetAmount;
                    rebetField.betAmountInString = WingoCurrentBetAmount.ToString();
                    rebetField.CoinPos = touchPos;
                    ReBetHistory.Add(rebetField);
                }

                // Subtract bet amount
                LocalSettings.SetTotalChips(-WingoCurrentBetAmount);
                //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, WingoCurrentBetAmount.ToString());
                TotalAmountPlacedOnBet += WingoCurrentBetAmount;
                if (MyPlayerInfo == null)
                    MyPlayerInfo = uIManager.GetMyPlayerInfo();
                //Debug.LogError("Remaining Cash: " + LocalSettings.GetTotalChips());
                MyPlayerInfo.playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
                CheckEnoughAmountAvailableToBet();
                BetPoint = BetPointBtns[PointNumber - 1];
                int PointNumberInt = PointNumber - 1;
                //#if !UNITY_EDITOR
                //                GetTouchpos();
                //#endif
                //UnityEngine.Vector2 touchPos2 = getMyPositions(BetPoint);
                UnityEngine.Vector2 touchPos2 = UnityEngine.Vector2.one;

                MyPlayerInfo.BetOnWingoLottary(WingoCurrentBetAmount, PointNumberInt, touchPos2, isRebettingBtnPressed);
                uIManager.TotalBetPlacedAmount += WingoCurrentBetAmount;
                uIManager.TotalBetPlaceFor1Game += WingoCurrentBetAmount;
                uIManager.isPlayerPlayedThisHand = true;


                WingoLottaryAPI.Instance.WingoLottarySendBet(PointNumberInt.ToString(), WingoCurrentBetAmount.ToString(), WinningNumber.ToString());
            }
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
        public GameObject CreateChipToShow()
        {
            GameObject chip = Instantiate(ChipPrefab.gameObject);
            chip.transform.parent = ChipsParent.transform;
            AllCreatedChips.Add(chip);
            return chip;
        }

        float currentRctVal = 0;

        Tweener tweener;
        float tempRemTime = 0;
        public void SpinWingoLottary()
        {
            if (!isPlayerEnteredDuringGame)
                timeDurationToSpin = LocalSettings.PlayerTurnDurationWingowLottery + 3f;
            else
            {
                timeDurationToSpin = LocalSettings.GetCurrentRoom.GetCustomFloatRoomData(LocalSettings.WingoCurrentRemainingTime);
                isPlayerEnteredDuringGame = false;
            }
            tempRemTime = 0;
            tweener = DOTween.To(() => CurrentuvRectValue, x => CurrentuvRectValue = x, TargetValue + 5f, timeDurationToSpin)
                .OnUpdate(() =>
                {
                    if (RoomStateManager.Instance.CurrentRoomState != RoomState.STATE.GameIsPlaying)
                    {

                        tweener.Kill();
                        SpinnerBar.uvRect = new Rect(0, 0, 1, 1);
                        return;
                    }
                    SpinnerBar.uvRect = new Rect(CurrentuvRectValue, 0, 1, 1);
                    tempRemTime += Time.deltaTime;
                    float remT = timeDurationToSpin - tempRemTime;
                    if (PhotonNetwork.IsConnectedAndReady)
                        LocalSettings.GetCurrentRoom.SetCustomFloatRoomData(LocalSettings.WingoCurrentRemainingTime, remT);
                }).OnComplete(() => OnCompleteAnimation());
        }
        AudioSource spinningSoundAS;
        public void GenerateLottaryNumber(int LuckyNumber)
        {
            TargetValue = Spinner[LuckyNumber].UVRectVlue;
            WinningNumber = Spinner[LuckyNumber].NumberOnImage;
            if (spinningSoundAS)
            {
                spinningSoundAS.Stop();
                spinningSoundAS = null;
            }
            spinningSoundAS = SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WingoLotteryRotate, true, true);
            SpinWingoLottary();
        }

        void OnCompleteAnimation()
        {
            if (spinningSoundAS)
            {
                spinningSoundAS.Stop();
                spinningSoundAS = null;
            }
            ShowResult();
            CurrentuvRectValue = 0;


        }

        void MakeChildWinAnimActive()
        {
            switch (WinningNumber)
            {
                case 1:
                case 3:
                case 7:
                case 9:
                    BetPointBtns[0].GetChild(1).gameObject.SetActive(true);
                    break;
                case 2:
                case 4:
                case 6:
                case 8:
                    BetPointBtns[2].GetChild(1).gameObject.SetActive(true);
                    break;
                case 0:
                case 5:
                    BetPointBtns[1].GetChild(1).gameObject.SetActive(true);
                    break;
            }
            BetPointBtns[WinningNumber + 3].GetChild(1).gameObject.SetActive(true);
            foreach (Transform btn in BetPointBtns)
            {
                if (!btn.GetChild(1).gameObject.activeInHierarchy)
                    btn.gameObject.GetComponent<Button>().interactable = false;
            }
        }

        public void ShowResult()
        {
            MakeChildWinAnimActive();
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);

            uIManager.GetMyPlayerInfo().SendChipsToEveryOne();
            LocalSettings.SetTotalChips(TotalRewardOnWin);
            //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, TotalRewardOnWin.ToString());
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Reward, false);
            if (TotalRewardOnWin > 1)
            {
                Debug.LogError("Check Total Reward..." + TotalRewardOnWin);
                //winnerBetAmountTxt.text = "Cong! You Win:- " + LocalSettings.Rs(TotalRewardOnWin).ToString();
                uIManager.TotalWinAmountFor1Game = TotalRewardOnWin;
                uIManager.SetTotalWinHandByPlayer();
                GameManager.Instance.AddXPToMyPlayer(true);
            }
            //else
            //    winnerBetAmountTxt.text = "you lose ";


            if (PhotonNetwork.IsMasterClient)
                PuttingARecord(WinningNumber);
            BigInteger totalAmount = LocalSettings.GetTotalChips();
            uIManager.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
            //Debug.LogError("Remaining Cash: " + LocalSettings.GetTotalChips());
            uIManager.GetMyPlayerInfo().playerTotalCash.text = LocalSettings.Rs(totalAmount);
            //Reset the Game After Time
            isRebetHistoryShouldClear = true;
            Invoke(nameof(ResetAfterDelay), 4f);
        }

        private void ResetAfterDelay()
        {
            GameResetManager.Instance.ResetWingoLottary();
        }


        void RoomStateToResult()
        {
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ShowingResults);
        }
        bool isDone;
        public void BetWillEnd(float remainingSeconds)
        {
            if (!BetWillEndTxt.transform.parent.gameObject.activeInHierarchy && !isDone && UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
            {
                //Debug.LogError("betting oepening check 1");
                isDone = true;
                BetWillEndTxt.transform.parent.gameObject.SetActive(true);
                BetStartStopTxt.text = "BETTING OPEN...";
                BetStartStopTxt.gameObject.SetActive(true);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Notification, false);
                //Debug.LogError("Betting open sound is playing");
            }
            BetWillEndTxt.text = "Betting will end in " + (int)remainingSeconds + " seconds";
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
            BigInteger totalRebetAmount = 0;
            foreach (RebetField rebetPoint in ReBetHistory)
                totalRebetAmount += rebetPoint.BetAmount;
            if (totalRebetAmount <= LocalSettings.GetTotalChips())
            {
                foreach (RebetField rebetPoint in ReBetHistory)
                {
                    WingoCurrentBetAmount = rebetPoint.BetAmount;
                    touchPos = rebetPoint.CoinPos;
                    OnBetPointBtnClick(rebetPoint.PointNumber);
                    //yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(0.1f);
            isRebettingBtnPressed = false;
            OnBetAmountsBtnClick((int)WingoCurrentBetAmount);
            BtnsEnableDisable(true);
        }

        void BtnsEnableDisable(bool IsEnable)
        {
            foreach (Transform tr in BetPointBtns)
                tr.GetComponent<Button>().enabled = IsEnable;
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

    }
    [System.Serializable]
    public class LottaryNumber
    {
        public float UVRectVlue;
        public int NumberOnImage;
    }

    [System.Serializable]
    public class RebetField
    {
        public int PointNumber;
        public BigInteger BetAmount;
        public string betAmountInString;
        public UnityEngine.Vector3 CoinPos;
    }
}