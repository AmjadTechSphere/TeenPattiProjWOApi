using Photon.Pun;
using Photon.Realtime;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace com.mani.muzamil.amjad
{
    public class PokerActionPanel : MonoBehaviour
    {
        //public int NumberOfSteps = 20;
        public Slider BetAmountSlider;
        [ShowOnly] public BigInteger MinBetAmount;
        [ShowOnly] public BigInteger MaxBetAmount;
        BigInteger OffSet;

        public GameObject ActionPanelPoker;
        public GameObject BetRaiseSliderBtn;
        public GameObject BetRaiseLabelObj;
        public GameObject BetRaiseSlider;
        public GameObject CallAllInCheckBtnObj;
        public GameObject callBetPockerBtn;
        public GameObject checkPokerBtn;
        public Button pokerPlusBtn;
        public Button pokerMinusBtn;

        public TMP_Text CallCheckAllInTxt;
        public TMP_Text SliderRaiseAmountTxt;
        BigInteger currentSliderAmount;
        public PlayerInfo MyLocalPlayerInfo;

        public BigInteger CurrentTargetBetAmount;
        [ShowOnly] public bool isAllIn;
        public GameObject HelpBtn;
        public GameObject friendsRankingBtn;
        public RectTransform helpPokerTransform;
        public RectTransform FriendRankingPokerTransform;



        #region Creating Instance
        private static PokerActionPanel _instance;
        public static PokerActionPanel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<PokerActionPanel>();
                return _instance;
            }
        }
        private void Awake()
        {
            if (!MatchHandler.IsPoker())
            {
                gameObject.SetActive(false);
                return;
            }
            if (_instance == null)
                _instance = this;
        }
        #endregion

        private void Start()
        {
            if (!MatchHandler.IsPoker())
                return;
            StartForSelectAmount();
            LocalSettings.SetPosAndRect(HelpBtn, helpPokerTransform, helpPokerTransform.transform.parent);
            LocalSettings.SetPosAndRect(friendsRankingBtn, FriendRankingPokerTransform, FriendRankingPokerTransform.transform.parent);
        }
        public void StartForSelectAmount()
        {
            if (ActionPanelPoker.activeInHierarchy)
                ActionPanelPoker.SetActive(false);
            MaxBetAmount = LocalSettings.GetPokerBuyInChips();
            // MinBetAmount = LocalSettings.GetBlindAmountPoker();
            MinBetAmount = LocalSettings.MinBetAmount;
            OffSet = MinBetAmount;
            SliderRaiseAmountTxt.text = MinBetAmount.ToString();
            BetRaiseLabelObj.SetActive(true);
            BetRaiseSlider.SetActive(false);
            BetRaiseSliderBtn.SetActive(true);
            onSliderValueChange();
            CallCheckAllInTxt.text = "Call\n" + LocalSettings.Rs(MinBetAmount);
        }

        public void onSliderValueChange()
        {
            //double CurrentSliderVal = BetAmountSlider.value / NumberOfSteps;
            //MaxBetAmount = LocalSettings.GetPokerBuyInChips();
            //BigInteger finalSliderVal = MaxBetAmount - OffSet;

            //currentSliderAmount = ((BigInteger)(CurrentSliderVal * (double)finalSliderVal)) + OffSet;
            int increment = (Mathf.RoundToInt(BetAmountSlider.value));
            if (MinBetAmount == LocalSettings.MinBetAmount / 2)
            {

                currentSliderAmount = (increment * LocalSettings.MinBetAmount) + MinBetAmount;
                if (increment == BetAmountSlider.maxValue)
                {
                    SliderRaiseAmountTxt.text = "ALL IN";//\n" + LocalSettings.Rs((currentSliderAmount));
                    isAllIn = true;
                  //  Debug.LogError("Check All In Bet...1" + isAllIn);
                    currentSliderAmount = LocalSettings.GetPokerBuyInChips();
                }
                else
                {
                    isAllIn = false;
                    SliderRaiseAmountTxt.text = LocalSettings.Rs((currentSliderAmount));
                }
            }
            else
            {
                // Debug.LogError("slider value....." + OffSet);
                if (checkPokerBtn.gameObject.activeInHierarchy)
                {
                    currentSliderAmount = (increment * LocalSettings.MinBetAmount) + LocalSettings.MinBetAmount;
                }
                else
                {
                    currentSliderAmount = (increment * LocalSettings.MinBetAmount) + MinBetAmount;
                   // Debug.LogError("check Current AMount..." + currentSliderAmount + "....Slider value..." + increment + "......Minimum Bet....." + LocalSettings.MinBetAmount + "Check Poker Min Bet" + MinBetAmount);
                }
                if (increment == BetAmountSlider.maxValue)
                {
                    //SliderRaiseAmountTxt.text = "ALL IN\n" + LocalSettings.Rs((currentSliderAmount));
                    SliderRaiseAmountTxt.text = "ALL IN";//\n" + LocalSettings.Rs((LocalSettings.GetPokerBuyInChips()));
                    isAllIn = true;

                    currentSliderAmount = LocalSettings.GetPokerBuyInChips();
                }
                else
                {
                    isAllIn = false;
                    SliderRaiseAmountTxt.text = LocalSettings.Rs((currentSliderAmount));
                }
            }
            //if (LocalSettings.MinBetAmount >= LocalSettings.GetPokerBuyInChips())
            //{
            //    currentSliderAmount = LocalSettings.GetPokerBuyInChips();
            //    SliderRaiseAmountTxt.text = "ALL IN";
            //    isAllIn = true;
            //    BetAmountSlider.interactable = false;
            //    pokerMinusBtn.interactable = false;
            //    pokerPlusBtn.interactable = false;
            //}
        }
        public void SetSliderAmount(string sign)
        {
            //BetAmountSlider.value = sign == "+" ? BetAmountSlider.value + 0.05f : BetAmountSlider.value - 0.05f;
            BetAmountSlider.value = sign == "+" ? BetAmountSlider.value + 1 : BetAmountSlider.value - 1;
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
        }



        public void SetBetAmountOnDealer(bool isHalf, PlayerInfo pInfo)
        {
            //BigInteger BootAmount = LocalSettings.GetBlindAmountPoker();
            //if (isHalf)
            //{
            //    BootAmount = LocalSettings.GetBlindAmountPoker() / 2;
            //}
            BigInteger BootAmount = LocalSettings.MinBetAmount;
            if (isHalf)
            {
                BootAmount = LocalSettings.MinBetAmount / 2;
            }
            int viewID = (int)pInfo.photonView.ViewID;
            if (pInfo.photonView.IsMine)
            {
                LocalSettings.SetPokerBuyInChips(-BootAmount);
                pInfo.player.SetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey, LocalSettings.GetPokerBuyInChips());

                //Debug.LogError("Check Total player Amount..." + LocalSettings.GetPokerBuyInChips() + "...Check Bet Cash from Player Payment....." + BootAmount);
            }
            else
            {
                //Debug.LogError("Check Total player Amount..." + LocalSettings.GetPokerBuyInChips() + "...Check Bet Cash from Player Payment....." + BootAmount);
                pInfo.BetStartAmountBet(BootAmount, viewID.ToString());
                //LocalSettings.SetPokerBuyInChips(-BootAmount);


            }

            pInfo.SendPokerBet(BootAmount, false);

        }




        public void OnRaiseSliderBtnClick()
        {
            if (BetRaiseSliderBtn.transform.GetChild(0).gameObject.activeInHierarchy)
            {
                BetRaiseLabelObj.SetActive(false);
                BetRaiseSlider.SetActive(true);
                BetAmountSlider.value = 0;
                onSliderValueChange();
            }
            else
            {
                if (LocalSettings.GetPokerBuyInChips() >= currentSliderAmount)
                {
                    LocalSettings.SetPokerBuyInChips(-currentSliderAmount);
                    PokerManager.Instance.SetStartingAmount();
                    bool isAllInBet = LocalSettings.GetPokerBuyInChips() > 0 ? false : true;
                    isAllIn = isAllInBet;
                   // Debug.LogError("Check All In Bet...1" + isAllIn);
                    GetMyPlayerInfo().SendPokerBet(currentSliderAmount, isAllInBet);
                    BetRaiseLabelObj.SetActive(true);
                    BetRaiseSlider.SetActive(false);
                }
                MaxBetAmount = LocalSettings.GetPokerBuyInChips();
                onSliderValueChange();
            }

            if (MinBetAmount > MaxBetAmount)
            {
                BetRaiseSliderBtn.SetActive(false);
                BetRaiseSlider.SetActive(false);
            }
            else
            {
                BetRaiseSliderBtn.SetActive(true);
                BetRaiseSlider.SetActive(false);
            }
        }



        void OnFolding()
        {
            UIManager.Instance.GetMyPlayerInfo().UpdatePlayerState(PlayerState.STATE.Packed);
        }

        [HideInInspector]
        public bool isSliderBtnClick;
        public void CallButtonClick(bool isSliderBtn)
        {
            UIManager uIManager = UIManager.Instance;
            isSliderBtnClick = isSliderBtn;
            if (isSliderBtnClick)
            {

                if (BetRaiseSliderBtn.transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    BetRaiseLabelObj.SetActive(false);
                    BetRaiseSlider.SetActive(true);
                    BetAmountSlider.minValue = 1;
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
                    if (checkPokerBtn.gameObject.activeInHierarchy || MinBetAmount == LocalSettings.MinBetAmount / 2)
                    {
                        BetAmountSlider.maxValue = (int)(LocalSettings.GetPokerBuyInChips() / LocalSettings.MinBetAmount); // Set this to the maximum number of increments based on total amount

                    }
                    else
                    {

                        BigInteger remainingAmountPokerBuyIN = LocalSettings.GetPokerBuyInChips() - MinBetAmount;
                        BetAmountSlider.maxValue = (int)(remainingAmountPokerBuyIN / LocalSettings.MinBetAmount);
                    }
                    BetAmountSlider.value = 1; // Default starting value

                    if (MinBetAmount == LocalSettings.MinBetAmount / 2)
                    {

                        currentSliderAmount = (BigInteger)(Mathf.RoundToInt(BetAmountSlider.value)) * LocalSettings.MinBetAmount + MinBetAmount;
                    }
                    else
                    {
                        if (LocalSettings.MinBetAmount >= LocalSettings.GetPokerBuyInChips())
                        {
                            currentSliderAmount = LocalSettings.GetPokerBuyInChips();
                            SliderRaiseAmountTxt.text = "ALL IN";
                            isAllIn = true;
                            BetAmountSlider.interactable = false;
                            pokerMinusBtn.interactable = false;
                            pokerPlusBtn.interactable = false;
                        }
                        else
                        {
                            if (checkPokerBtn.gameObject.activeInHierarchy)
                            {
                                currentSliderAmount = ((BigInteger)(Mathf.RoundToInt(BetAmountSlider.value) * LocalSettings.MinBetAmount)) + LocalSettings.MinBetAmount;
                            }
                            else
                            {
                                currentSliderAmount = ((BigInteger)(Mathf.RoundToInt(BetAmountSlider.value) * LocalSettings.MinBetAmount)) + MinBetAmount;
                            }
                        }
                    }
                    if (LocalSettings.MinBetAmount < LocalSettings.GetPokerBuyInChips())
                        SliderRaiseAmountTxt.text = LocalSettings.Rs((currentSliderAmount));
                    return;
                }
                if (!CheckBoolAllIN())
                    uIManager.GetMyPlayerInfo().ResetIsBetPlacedPocker();
            }

            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            uIManager.GetMyPlayerInfo().myPockerTurnComplete(true);
            uIManager.GetMyPlayerInfo().UpdatePlayerState(PlayerState.STATE.BetPlaced);
        }


        public void OnClickCheckBtn()
        {
            ActionPanelPoker.SetActive(false);
            UIManager uIManager = UIManager.Instance;
            if (!checkPockeBetPlaced())
            {
                uIManager.GetMyPlayerInfo().myPockerTurnComplete(true);
                // Debug.LogError("Value Of IsBetPlacedPocker......" + uIManager.GetMyPlayerInfo().isBetPlacedPocker);
            }

            uIManager.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.WaitingForTurn);
            GiveTurnToNextPlayerPocker();
            if (checkPockeBetPlaced())
            {
                //  Debug.LogError("changing state to AB First turn");
                RoomStateManager.Instance.UpdateCurrentStateOnShowBtn(RoomState.STATE.ABFirstTurn);
            }
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
        }

        public bool CheckBoolAllIN()
        {
            foreach (var item in PlayerStateManager.Instance.PlayingList)
            {
                if (item.isAllInCashBet)
                    return true;
            }
            return false;
        }
        public bool checkPockeBetPlaced()
        {
            foreach (PlayerInfo item in PlayerStateManager.Instance.PlayingList)
            {
                if (item.isBetPlacedPocker == false)
                    return false;
            }
            return true;
        }
        public void GiveTurnToNextPlayerPocker()
        {
            int myIndex = PlayerStateManager.Instance.SideShowNext();
            PlayerStateManager.Instance.PlayingList[myIndex].currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
        }

        public void OnCallCheckAllInBtnClick()
        {
            if (LocalSettings.GetPokerBuyInChips() >= MinBetAmount)
            {
                LocalSettings.SetPokerBuyInChips(-MinBetAmount);
                PokerManager.Instance.SetStartingAmount();
                bool isAllInBet = LocalSettings.GetPokerBuyInChips() > 0 ? false : true;
                isAllIn = isAllInBet;
              //  Debug.LogError("Check All In Bet...1" + isAllIn);
                GetMyPlayerInfo().SendPokerBet(MinBetAmount, isAllInBet);
                MaxBetAmount = LocalSettings.GetPokerBuyInChips();
                BetAmountSlider.value = 0;
                onSliderValueChange();
            }
            if (MinBetAmount > MaxBetAmount)
            {
                BetRaiseSliderBtn.SetActive(false);
                BetRaiseSlider.SetActive(false);
            }
            else
            {
                BetRaiseSliderBtn.SetActive(true);
                BetRaiseSlider.SetActive(false);
                BetRaiseLabelObj.SetActive(true);
            }
        }

        public void SetMinBetAmount()
        {
            //BigInteger BetDifference = CurrentTargetBetAmount - GetMyPlayerInfo().PokerTotalWholeBetAmount;
            BigInteger BetDifference = CurrentTargetBetAmount - GetMyPlayerInfo().pokerTotalBetCash;
            OffSet = BetDifference;
            //Debug.LogError("Bet Diff: " + BetDifference + "     CurrentTargetBetAmount: " + CurrentTargetBetAmount + "       Totalcash: " + GetMyPlayerInfo().pokerTotalBetCash);
            if (BetDifference == 0 && !CheckBoolAllIN())
            {
                checkPokerBtn.SetActive(true);
                callBetPockerBtn.SetActive(false);
                if (LocalSettings.GetPokerBuyInChips() > 0)
                    BetRaiseSliderBtn.SetActive(true);
                else
                    BetRaiseSliderBtn.SetActive(false);


                MinBetAmount = 0;
            }
            else if (BetDifference < LocalSettings.GetPokerBuyInChips())
            {
                MinBetAmount = BetDifference;
                CallCheckAllInTxt.text = "Call\n" + LocalSettings.Rs(MinBetAmount);
            }
            else
            {
                if (LocalSettings.GetPokerBuyInChips() != 0)
                {
                    CallCheckAllInTxt.text = "ALL IN";/*\n" + LocalSettings.Rs(LocalSettings.GetPokerBuyInChips());*/
                    //Debug.LogError("Check All In Bet...1" + isAllIn);
                    MinBetAmount = LocalSettings.GetPokerBuyInChips();
                    isAllIn = true;
                }
                else
                {
                    CallCheckAllInTxt.text = "CHECK";
                }
                BetRaiseSliderBtn.SetActive(false);
                BetRaiseSlider.SetActive(false);
            }
          //  Debug.LogError("Min bet:  " + MinBetAmount + "     Max Bet Amount: " + MaxBetAmount + "    Is all in: " + isAllIn + "      Bet difference: " + BetDifference + "     CurrentTargetBetAmount: " + CurrentTargetBetAmount + "       Totalcash: " + GetMyPlayerInfo().pokerTotalBetCash);
            if (isAllIn)
            {
                checkPokerBtn.SetActive(true);
                callBetPockerBtn.SetActive(false);

            }
            else
            {

            }
            BetRaiseSlider.SetActive(false);
            BetAmountSlider.value = 0;
            onSliderValueChange();

        }
        public void OnFoldBtnClick()
        {
            OnFolding();
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
        }

        public void SetAllInStatus()
        {
            GetMyPlayerInfo().PokerPlayerCashAllIn = true;
        }

        public void AllPokerBetsGoToFinalPoint()
        {
            GameManager gameManager = GameManager.Instance;
            foreach (PlayerInfo pInfo in gameManager.playersList)
            {
               // Debug.LogError("packed State....2...1");
                if (pInfo.pokerBetAmountAnim != null)
                {
                   // Debug.LogError("packed State....2...2");
                    pInfo.pokerBetAmountAnim.GetComponent<PokerBetAmountAnim>().PlayAnimation(PokerManager.Instance.FinalPokerBetAmountPoint, true, 0);
                   // Debug.LogError("packed State....2...2");
                    Pot.instance.PotTxt.text = LocalSettings.Rs(TotalPotAmount());
                    Pot.instance.PotPanel.SetActive(true);
                }
            }
            CurrentTargetBetAmount = 0;
            // PokerManager.Instance.NowDropCommunityCard(0.5f);
            PokerManager.Instance.DropCommunityCard();
        }

        public BigInteger TotalPotAmount()
        {
            BigInteger totalPotPockerAmount = 0;
            //foreach (PlayerInfo item in PlayerStateManager.Instance.PlayingList)
            //{
            //totalPotPockerAmount += item.PokerTotalWholeBetAmount;
            //}
            totalPotPockerAmount = amountPlacedOnBet;
            // Debug.LogError("Amount : " + amountPlacedOnBet);
            //amountPlacedOnBet = 0;
            return totalPotPockerAmount;
        }
        public BigInteger amountPlacedOnBet;
        public void BetPlacedAmount(BigInteger amount)
        {
            amountPlacedOnBet += amount;
        }

        PlayerInfo GetMyPlayerInfo()
        {
            if (MyLocalPlayerInfo == null)
                MyLocalPlayerInfo = UIManager.Instance.GetMyPlayerInfo();
            return MyLocalPlayerInfo;
        }
    }
}