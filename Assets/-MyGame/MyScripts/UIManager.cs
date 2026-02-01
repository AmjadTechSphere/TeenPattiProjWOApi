using Photon.Pun;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace com.mani.muzamil.amjad
{
    public class UIManager : MonoBehaviour
    {

        public GameObject[] panelReference;

        public Collections BGs;
        public GameObject DisconnectedPanel;
        public GameObject LoadingPanel;
        public GameObject ActionTable;
        public GameObject sideShowPanel;
        public GameObject InfoObj;
        public GameObject quickShop;
        private GameObject LocalPlayer;
        public GameObject ChatSystemPanel;
        public GameObject GiftTranferpanel;
        public GameObject GoldTranferpanel;

        // Level up Panel
        public GameObject levelUpPanel;
        public TMP_Text levelUpText;
        public TMP_Text levelUpRewardText;

        public Image MainBG;
        public Image FillerImage;
        public TMP_Text watchingGamePlayer;

        public TMP_Text TurnText;
        public TMP_Text TimeText;
        public TMP_Text CurrentChalAmoundText;
        public TMP_Text ActorNumber;
        public TMP_Text PlayerTotalChipsTxt;
        public TMP_Text ChaalTypeText;
        public TMP_Text InfoTxt;
        public TMP_Text playerName;
        //public TMP_Text modeText;

        public TMP_Text AndarBetAmountBtnTxt;
        public TMP_Text BaharBetAmountBtnTxt;

        //public TMP_Text SuperAndarBetAmountBtnTxt;
        //public TMP_Text SuperBaharBetAmountBtnTxt;

        // PlayerInformation
        public GameObject PlayerInfoPanel;
        public TMP_Text TotalWinAmountText;
        public TMP_Text TotalHandsText;
        public TMP_Text PlayerTotalCashText;
        public TMP_Text playerInfoName;
        public TMP_Text playerLevelText;
        public Image profileImage;
        public Image profileFrameImage;
        public Button addFriendBtn;
        public GameObject fillerLevelImage;

        [ShowOnly]
        public int TotalHands;
        [ShowOnly]
        public int TotalWinHands;
        [ShowOnly]
        public bool isPlayerPlayedThisHand;

        public BigInteger TotalWinsAmount;

        public BigInteger TotalBetPlacedAmount;

        public BigInteger TotalBetPlaceFor1Game;
        public BigInteger TotalWinAmountFor1Game;


        private PlayerInfo myPlayerInfo;
        private PlayerCurrentState myPlayerState;
        private BetAmountToTargetAnim betAmountAnim;


        public Button SwithcTableBtn;

        public Button BetAmountIncreaseBtn;
        public Button BetAmountDecreaseBtn;
        public Button showBtn;
        public Button sideShowBtn;
        public Button AcceptSideShowBtn;
        public Button cancelSideShowBtn;
        // Andar Bahar Btns
        public Button SkipBetBtn;
        public Button AndarBetBtn;
        public Button AndarNveBtn;
        public Button AndarPveBtn;
        public Button BaharBetBtn;
        public Button BaharNveBtn;
        public Button BaharPveBtn;
        // Super Andar bahar btns
        //public Button SuperAndarBetBtn;
        //public Button SuperAndarNveBtn;
        //public Button SuperAndarPveBtn;
        //public Button SuperBaharBetBtn;
        //public Button SuperBaharNveBtn;
        //public Button SuperBaharPveBtn;



        public int TotalChals;
        int ChallsDone;
        private static UIManager _instance;
        public GameObject MyLocalPlayer
        {
            set
            {
                LocalPlayer = value;
                AssignReferences();
            }
            get
            {
                return LocalPlayer;
            }
        }

        public int TotalCircleChals()
        {
            return ChallsDone;
        }

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<UIManager>();
                return _instance;
            }
        }
        void Awake()
        {
            if (_instance == null)
                _instance = this;
        }


        public PlayerInfo GetMyPlayerInfo()
        {
            return myPlayerInfo;
        }

        public PlayerCurrentState GetMyPlayerCurrentState()
        {
            return myPlayerState;
        }



        public BetAmountToTargetAnim BetAmountToTargetAnim()
        {
            return betAmountAnim;
        }


        void AssignReferences()
        {
            myPlayerInfo = LocalPlayer.GetComponent<PlayerInfo>();
            myPlayerState = LocalPlayer.GetComponent<PlayerCurrentState>();

            betAmountAnim = myPlayerInfo.BetAmountAnim.transform.GetComponent<BetAmountToTargetAnim>();
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadingPanel.SetActive(false);
            if (MatchHandler.isDragonTiger() || MatchHandler.isWingoLottary())
                SwithcTableBtn.interactable = false;
            TotalWinAmountText.text = "0";
            TotalHandsText.text = "0/0";
            isPlayerPlayedThisHand = false;
            ChatSystemPanel.SetActive(true);
            profileFrameImage = profileImage.transform.GetChild(0).GetComponent<Image>();

            SetbackGrounds();


        }

        void SetbackGrounds()
        {
            if (MatchHandler.IsTeenPatti())
                MainBG.sprite = BGs.Sprites[0];
            else if (MatchHandler.IsAndarBahar())
                MainBG.sprite = BGs.Sprites[1];
            else if (MatchHandler.isWingoLottary())
                MainBG.sprite = BGs.Sprites[2];
            else if (MatchHandler.IsLuckyWar())
                MainBG.sprite = BGs.Sprites[3];
            else if (MatchHandler.isDragonTiger())
                MainBG.sprite = BGs.Sprites[4];
            else if (MatchHandler.IsPoker())
                MainBG.sprite = BGs.Sprites[5];

        }

        public void UpDateCurrentChalAmountText(BigInteger BetAmount)
        {
            CurrentChalAmoundText.text = LocalSettings.Rs(BetAmount);
        }

        public void UpdateTexts(string WinAmount, string Hands, string CashAmount, Sprite profileSprite, int size, string namePlayer, int playerLevel)
        {
            PlayerTotalCashText.text = CashAmount;
            string totalAmount = WinAmount;
            if (WinAmount.Contains("-"))
            {
                TotalWinAmountText.color = Color.red;
                string aa = "-";
                WinAmount = totalAmount.Replace(aa, "");
                TotalWinAmountText.text = "-" + LocalSettings.Rs(WinAmount);
            }
            else
            {
                TotalWinAmountText.color = Color.green;
                TotalWinAmountText.text = LocalSettings.Rs(WinAmount);
            }
            TotalHandsText.text = Hands;
            profileImage.sprite = profileSprite;
            // profileFrameImage.sprite = profileFrameSprite;
            LocalSettings.CheckFrameNumber(profileFrameImage, GameManager.Instance.playerProfileFrameImage, size);
            playerInfoName.text = namePlayer;
            playerLevelText.text = "Level " + playerLevel;
        }
        public void UpdateTheWinAmount(string totalcashWinLossKey, string TotalHandsKey, string WinHandsKey)
        {
            if (isPlayerPlayedThisHand && GetMyPlayerInfo().photonView.IsMine)
            {
                TotalHands++;
                ///Debug.LogError("check Total Hands win...2");                                
                if (!GetMyPlayerInfo().WinningIndicator.activeInHierarchy)
                    GameManager.Instance.AddXPToMyPlayer(false);
            }

            if (MatchHandler.isWingoLottary())
            {
                TotalWinsAmount += WingoManager.Instance.TotalRewardOnWin;
            }
            else if (MatchHandler.isDragonTiger())
            {
                TotalWinsAmount += DragonTigerManager.Instance.TotalRewardOnWin;
            }
            BigInteger currentWinReport = TotalWinsAmount - TotalBetPlacedAmount;
            // When Room left
            LocalSettings.winOrLoseAmount = currentWinReport;
            //Debug.LogError("Win or Lose Amont  " + LocalSettings.winOrLoseAmount);
            GetMyPlayerInfo().player.SetCustomBigIntegerData(totalcashWinLossKey, currentWinReport);
            GetMyPlayerInfo().player.SetCustomData(TotalHandsKey, TotalHands);
            GetMyPlayerInfo().player.SetCustomData(WinHandsKey, TotalWinHands);
            GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());


            //TotalWinAmountText.text = currentWinReport.ToString();
            //TotalHandsText.text = TotalWinHands + "/" + TotalHands;            

        }

        // Transfer Gift To single Player

        public void CheckConditionGoldTranfer()
        {
            //if (GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
            //{
            //    GoldTransfer.Instance.showMessage("Stand Up Before Gold Transfer");
            //    GoldTransfer.Instance.MessagePanel.SetActive(true);
            //    return;
            //}
            GoldTranferpanel.SetActive(true);
        }


        public void SetTotalWinHandByPlayer()
        {
            if (TotalBetPlaceFor1Game < TotalWinAmountFor1Game)
            {
                TotalWinHands++;
            }
        }

    }
}