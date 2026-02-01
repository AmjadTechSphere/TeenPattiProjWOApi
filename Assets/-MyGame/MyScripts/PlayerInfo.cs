using DG.Tweening;
using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics.Eventing.Reader;
using System.Numerics;
using System;
using TMPro;
//using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using UnityEngine.SceneManagement;
using System.Linq;

namespace com.mani.muzamil.amjad
{

    public class PlayerInfo : MonoBehaviourPunCallbacks
    {
        [ShowOnly] public bool checkApplicationBackground;

        public void playerBackGround(bool isTrue)
        {
            //Toaster.ShowAToast("Check GameManager..." + isTrue);
            photonView.RPC(nameof(playerRunInBackGroundRPC), RpcTarget.All, isTrue);
        }

        [PunRPC]
        void playerRunInBackGroundRPC(bool isTrue)
        {
            checkApplicationBackground = isTrue;

            //Toaster.ShowAToast("Check Player....." + checkApplicationBackground);
        }



        public GameObject PlayerDummyCardsToShowParent;
        public GameObject PlayerOrignalCardsToShowParent;
        public GameObject ShowBtn;
        public GameObject viewResultAB;
        public GameObject SeenIndicator;
        public GameObject BlindIndicator;
        public GameObject WinningIndicator;
        public GameObject BetAmountAnim;
        public GameObject tipAmountAnim;
        public GameObject SideShowIndicatorAnim;
        public GameObject ABBettingSection;
        // Lucky War Game K lye
        public GameObject LWBettingSection;

        public GameObject ChatMessageBox;
        public GameObject EmojiAnim;
        public GameObject FireAnimObj;

        [HideInInspector]
        public PlayerGiftTransfer playerGiftTransfer
        {
            get
            {
                return GetComponent<PlayerGiftTransfer>();
            }
        }



        public RectTransform For3PattiPlayer;

        public Player player;

        public Image FillerImage;
        public Image PlayerAvatorImage;
        public Image playerFrameImage;

        public TMP_Text PicIndexTxt;
        public TMP_Text player_name;
        public TMP_Text BetAmountAnimText;
        public TMP_Text PackedText;
        public TMP_Text PlayerStateText;

        public TMP_Text AndarBetAmoutTxt;
        public TMP_Text BaharBetAmoutTxt;
        public TMP_Text TieBetAmoutTxt;
        public TMP_Text BetBetAmoutTxt;
        public TMP_Text SuperAndarBetAmoutTxt;
        public TMP_Text SuperBaharBetAmoutTxt;


        public TMP_Text playerTotalCash;

        public TMP_Text WingoWinCashTxt;

        public bool seated;
        public bool gamePlaying;
        [ShowOnly] public bool IsSeen;
        [ShowOnly] public bool IsSideShow;
        [ShowOnly] public bool AbTurn;
        [ShowOnly] public bool LWTurn;
        [ShowOnly] public bool firstTurnAB;
        [ShowOnly] public bool cardShuffleBool = false;


        [ShowOnly] public bool TieBetWinLw;


        // For Dragon Tiger
        [ShowOnly] public bool iSDragonTigerStart = false;

        public int TableIndex;
        public int PicIndexInt;
        [ShowOnly] public int MyScores;
        [ShowOnly] public int MyRank;
        [ShowOnly] public int[] OrgCardValues = { 0, 0, 0 };
        [ShowOnly]
        public int MyChaalsPlayedCounter;
        //[ShowOnly]
        //public int PlayerIndexInPlayingList;

        [ShowOnly]
        public int myNetworkSeat;

        [ShowOnly] public RectTransform playerRectTransform;

        public PlayerProperties player_properties;

        #region TransforGift



        #endregion

        #region Poker Variables
        /// <summary>
        /// poker 
        /// </summary>
        [Header("Poker Section")]
        public GameObject PokerParentThings;


        public GameObject DummyCardsParent;

        public RectTransform card_1_RectTr;
        public RectTransform card_2_RectTr;

        [ShowOnly] public CardProperty Hole_Card1;
        [ShowOnly] public CardProperty Hole_Card2;

        [ShowOnly] public CardProperty[] HighRankingCards;
        [ShowOnly] public List<CardProperty> remainingCards;

        [ShowOnly] public int PokerPlayerCurrentRank;
        [ShowOnly] public int PokerScores;
        public TMP_Text HandRankLabelTxt;
        public TMP_Text PokerInfoMessage;
        public GameObject BetStatusObj;
        public GameObject CashAllInIndicator;
        public TMP_Text PokerTotalCashTxt;
        public RectTransform isMinePokerTotalCashTranform;
        public BigInteger PokerTotalCash;


        public GameObject PokerBetAmountAnimPrefab;

        [ShowOnly] public GameObject pokerBetAmountAnim;
        [ShowOnly] public BigInteger pokerTotalBetCash = 0;
        [ShowOnly] public BigInteger PokerTotalWholeBetAmount = 0;
        public Transform FirstTargetPokerBetAmount;

        public GameObject Dealer;
        [ShowOnly]
        public bool isAllInCashBet;
        [ShowOnly]
        public bool isBetPlacedPocker = false;
        [ShowOnly]
        public bool isCircleCheckFlag = false;

        public List<string> tipsDialouges;
        [ShowOnly]
        public bool PokerPlayerCashAllIn;
        // poker end
        #endregion


        #region Lucky War Section
        /// <summary>
        /// Lucky War 
        /// </summary>
        [Header("Lucky War Section")]
        public CardProperty PlayerLWCard;
        public GameObject LWDummyCardPrent;

        #endregion
        public int AdjustGameManagerLocalSeat
        {
            get
            {
                return privateNetworkSeat;
            }
            set
            {
                privateNetworkSeat = value;
                if (photonView.IsMine)
                {
                    GameManager.Instance.myLocalSeat = value;
                }
            }
        }



        private int privateNetworkSeat;

        private PhotonView this_photonView;


        public PlayerCurrentState currentPlayerStateRef;


        private void Awake()
        {



            this_photonView = GetComponent<PhotonView>();
            currentPlayerStateRef = GetComponent<PlayerCurrentState>();
            player_properties = GetComponent<PlayerProperties>();
            player = photonView.Controller;
            player_properties.player = player;
            MyScores = 0;
            MyRank = 0;

            if (!PhotonNetwork.IsMasterClient && photonView.IsMine)
            {
                RoomStateManager.Instance.UpdateLocalRoomState(LocalSettings.GetCurrentRoom.GetRoomStateProperty(LocalSettings.roomState));
            }
            //SetRoomStateFromNetworkCustomProperty();

            if (PhotonNetwork.IsMasterClient)
                PositionsManager.Instance.AssignPositionOfthisPlayer(this);

        }

        public int GetMineIndexInPlayerList()
        {
            int index = GameManager.Instance.playersList.FindIndex(x => x == this);
            return index;
        }

        void EnableDisableThings()
        {
            if (MatchHandler.IsTeenPatti())
            {
                LocalSettings.SetPosAndRect(player_name.transform.parent.gameObject, For3PattiPlayer, this.gameObject.transform);
                DeActivateUIOfAndarBahar(false);
            }
            else if (MatchHandler.IsAndarBahar())
            {

                LWBettingSection.SetActive(false);
            }
            else if (MatchHandler.isWingoLottary())
            {
                DeActivateUIOfAndarBahar(false);
            }
            else if (MatchHandler.IsPoker())
            {
                DeActivateUIOfAndarBahar(false);
            }
            else if (MatchHandler.IsLuckyWar())
            {
                ABBettingSection.SetActive(false);
            }
            else if (MatchHandler.isDragonTiger())
            {
                DeActivateUIOfAndarBahar(false);
            }
        }
        int num = 0;
        void Start()
        {
            //PhotonNetwork.LocalCleanPhotonView(photonView);
            EnableDisableThings();
            Playeramount();
            playerEnterenceState();
            num++;
            // Debug.LogError("Adding My Player Of time: " + num);
            GameManager.Instance.AddingPlayer(this);
            IsSeen = false;
            if (photonView.IsMine)
            {
                UIManager.Instance.MyLocalPlayer = gameObject;
                WinningIndicator.transform.GetChild(0).localScale = UnityEngine.Vector3.one * 1.1f;
                SideShowIndicatorAnim.transform.GetChild(0).localScale = UnityEngine.Vector3.one * 1.1f;
            }

            AssignName(player.NickName);
            BetAmountAnim.SetActive(false);
            SetProfileImage += UpdateProfileImageAfterReceivingFromServer;
            ResetAllPlayerKeys();
            GetProfilePic();
            GameManager.Instance.UpdateWatchingPlayers();

        }

        void ResetAllPlayerKeys()
        {
            if (photonView.IsMine)
            {
                player.SetCustomBigIntegerData(LocalSettings.totalcashWinLossKey, 0);
                player.SetCustomData(LocalSettings.TotalHandsKey, 0);
                player.SetCustomData(LocalSettings.WinHandsKey, 0);
            }
        }

        public void Playeramount()
        {
            if (MatchHandler.isWingoLottary())
            {
                if (photonView.IsMine)
                {
                    player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
                    playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
                    player_name.transform.parent.gameObject.SetActive(false);
                    LocalSettings.SetPosAndRect(playerTotalCash.transform.parent.gameObject, player_name.transform.parent.gameObject.GetComponent<RectTransform>(), this.gameObject.GetComponent<RectTransform>());
                }
                else
                {
                    playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
                }
            }
            else if (MatchHandler.isDragonTiger())
            {
                if (photonView.IsMine)
                {
                    player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
                    playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
                    player_name.transform.parent.gameObject.SetActive(false);
                    LocalSettings.SetPosAndRect(playerTotalCash.transform.parent.gameObject, player_name.transform.parent.gameObject.GetComponent<RectTransform>(), this.gameObject.GetComponent<RectTransform>());

                }
                else
                {
                    playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
                }
            }
            else if (MatchHandler.IsAndarBahar())
            {
                if (photonView.IsMine)
                {
                    player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
                    playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
                    player_name.transform.parent.gameObject.SetActive(false);
                    playerTotalCash.transform.parent.gameObject.SetActive(false);

                }
                else
                {
                    playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
                }
            }
            else if (MatchHandler.IsLuckyWar())
            {
                if (photonView.IsMine)
                {
                    player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
                    playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
                    player_name.transform.parent.gameObject.SetActive(false);
                    playerTotalCash.transform.parent.gameObject.SetActive(false);

                }
                else
                {
                    playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
                    player_name.transform.parent.gameObject.SetActive(true);
                }
            }
            else
            {
                playerTotalCash.transform.parent.gameObject.SetActive(false);
            }
        }



        void playerEnterenceState()
        {
            if ((LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount && !MatchHandler.IsTeenPatti()) || (LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount && !MatchHandler.IsPoker()))
            {
                if (photonView.IsMine)
                {
                    UIManager.Instance.quickShop.SetActive(true);

                    StandUp();
                    if (MatchHandler.IsAndarBahar())
                    {

                        if (player.GetCustomBigIntegerData(LocalSettings.abAndarAmountKey) > 0)
                            AndarBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.abAndarAmountKey));
                        if (player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey) > 0)
                            BaharBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey));
                        if (player.GetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey) > 0)
                            SuperBaharBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey));
                    }
                    else if (MatchHandler.IsLuckyWar())
                    {
                        if (player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) > 0)
                            TieBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey));
                        if (player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey) > 0)
                            BetBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey));

                    }
                }
                // Debug.LogError("Check Return..");
                return;
            }
            else if (MatchHandler.IsPoker())
            {
                if ((LocalSettings.GetTotalChips() < LocalSettings.GetStartingMinAmountPoker() && LocalSettings.GetPokerBuyInChips() < LocalSettings.GetStartingMinAmountPoker()) && currentPlayerStateRef.currentState != PlayerState.STATE.OutOfTable)
                {
                    if (photonView.IsMine)
                    {
                        UIManager.Instance.quickShop.SetActive(true);
                        StandUp();
                        return;
                    }
                }
                PokerParentThings.SetActive(true);
                if (photonView.IsMine)
                    PokerTotalCash = LocalSettings.GetPokerBuyInChips();
                else
                    PokerTotalCash = player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey);

                PokerTotalCashTxt.text = LocalSettings.Rs(PokerTotalCash);
                // Debug.LogError("Here is you Bug     Buyin chios: " + LocalSettings.GetPokerBuyInChips());
                if (photonView.IsMine)
                {
                    LocalSettings.SetPosAndRect(PokerTotalCashTxt.transform.parent.gameObject, isMinePokerTotalCashTranform, isMinePokerTotalCashTranform.parent);
                    player_name.transform.parent.gameObject.SetActive(false);
                }

                PokerTotalCashTxt.transform.parent.gameObject.SetActive(true);


            }



            if (RoomStateManager.Instance.IsStarted())
            {
                //Debug.LogError(currentPlayerStateRef.currentState);
                if (photonView.IsMine)
                {
                    currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.OutOfGame);
                }
                else
                {
                    if (MatchHandler.IsTeenPatti())
                    {
                        currentPlayerStateRef.currentState = (player.GetPlayerStateProperty(LocalSettings.playerState));
                        //Invoke("StartThisCoroutine", 0.5f);
                        StartCoroutine(GiveOrgCardsIfNotInGame(0.5f));
                    }
                    else if (MatchHandler.IsAndarBahar())
                    {
                        currentPlayerStateRef.currentState = (player.GetPlayerStateProperty(LocalSettings.playerState));
                        if (player.GetCustomBigIntegerData(LocalSettings.abAndarAmountKey) > 0)
                            AndarBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.abAndarAmountKey));
                        if (player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey) > 0)
                            BaharBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey));
                        if (player.GetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey) > 0)
                            SuperBaharBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey));
                    }
                    else if (MatchHandler.IsPoker())
                    {
                        currentPlayerStateRef.currentState = (player.GetPlayerStateProperty(LocalSettings.playerState));
                        //Give original Cards to all player
                        StartCoroutine(GiveOrgPokersCardsIfNotInGame(0.5f));
                    }
                    else if (MatchHandler.IsLuckyWar())
                    {
                        currentPlayerStateRef.currentState = (player.GetPlayerStateProperty(LocalSettings.playerState));
                        if (player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey) > 0)
                            TieBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.LWTieAmountKey));
                        if (player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey) > 0)
                            BetBetAmoutTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey));

                    }
                }
            }
            else
            {
                if (MatchHandler.IsTeenPatti())
                {
                    if (photonView.IsMine)
                        currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.AbleToJoin);
                }
                else if (MatchHandler.IsAndarBahar())
                {

                    currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.AbleToJoin);
                }
                else if (MatchHandler.isWingoLottary())
                {

                    currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.AbleToJoin);
                }
                else if (MatchHandler.IsPoker())
                {
                    if (photonView.IsMine)
                        currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.AbleToJoin);
                }
                else if (MatchHandler.IsLuckyWar())
                {
                    //Debug.LogError("Check Player Entrance......");
                    if (photonView.IsMine)
                        currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.AbleToJoin);
                    if (player.GetPlayerStateProperty(LocalSettings.playerState) == PlayerState.STATE.AbleToJoin && !photonView.IsMine)
                    {
                        currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.AbleToJoin);
                    }
                }
                else if (MatchHandler.isDragonTiger())
                {
                    currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.AbleToJoin);
                }

            }
            if (player.GetPlayerStateProperty(LocalSettings.playerState) == PlayerState.STATE.OutOfTable && !photonView.IsMine)
            {
                gameObject.SetActive(false);
            }


        }




        public void SetRoomStateFromNetworkCustomProperty()
        {
            RoomStateManager.Instance.UpdateCurrentRoomState(LocalSettings.GetCurrentRoom.GetRoomStateProperty(LocalSettings.roomState));
        }
        IEnumerator GiveOrgCardsIfNotInGame(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            //Debug.LogError("Aya k ahi");
            if (player.GetPlayerStateProperty(LocalSettings.playerState) != PlayerState.STATE.OutOfGame)
            {
                GameManager gameManager = GameManager.Instance;
                int[] OrgCardsAry = new int[3]; // Initialize OrgCardsAry with a size of 3
                OrgCardsAry[0] = 0; // Assign initial values to array elements if needed
                OrgCardsAry[1] = 0;
                OrgCardsAry[2] = 0;
                //Debug.LogError("Original Card " + player.GetCustomArray(LocalSettings.OrgCardsArray).Length + " NickName is " + player.NickName);

                //Debug.LogError("Original Card " + OrgCardsAry.Length);
                yield return new WaitUntil(() => player.GetCustomArray(LocalSettings.OrgCardsArray).Length > 0);
                OrgCardsAry = player.GetCustomArray(LocalSettings.OrgCardsArray);
                if (OrgCardsAry.Length > 0)
                {
                    for (int i = 0; i < OrgCardsAry.Length; i++)
                    {
                        GameObject card = Instantiate(gameManager.AllCards.Card[OrgCardsAry[i]].gameObject);
                        RectTransform rt = PlayerOrignalCardsToShowParent.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                        Transform parntObj = PlayerOrignalCardsToShowParent.transform;
                        LocalSettings.SetPosAndRect(card, rt, parntObj.transform);
                    }
                }
            }
        }



        void PlayerSeenCardsStatus()
        {
            print(photonView.Controller.NickName + " : " + player.GetCustomBoolData("is_seen"));
            if (RoomStateManager.Instance.GetCurrentRoomState() == RoomState.STATE.GameIsPlaying)
            {
                if (!photonView.IsMine && currentPlayerStateRef.currentState != PlayerState.STATE.OutOfGame)
                {
                    IsSeen = player.GetCustomBoolData("is_seen");
                    BlindIndicator.SetActive(!IsSeen);
                    SeenIndicator.SetActive(IsSeen);
                }
            }
        }

        //public int counter;
        void Update()
        {
            if (!photonView.IsMine)
                return;
            //if (Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    counter++;
            //    GameManager.Instance.TurnTxt.text = counter.ToString();
            //    //photonView.RPC(counter.ToString(), photonView,"asdf");
            //    photonView.RPC("increment", RpcTarget.All, counter);
            //}

        }

        //[PunRPC]
        //public void increment(int counter2)
        //{
        //    counter = counter2;
        //    GameManager.Instance.TurnTxt.text = counter.ToString();
        //}


        public void AssignNetworkSeat(int seat)
        {
            this_photonView.RPC("AssignNetworkSeatToAllInstanceOfThisPlayer", RpcTarget.AllBuffered, seat);
        }

        [PunRPC]
        public void AssignNetworkSeatToAllInstanceOfThisPlayer(int seat)
        {
            myNetworkSeat = seat;
            AdjustGameManagerLocalSeat = seat;
        }


        public void ReAssignNetworkSeat()
        {
            this_photonView.RPC("ReAssignNetworkSeatToAllInstanceOfThisPlayer", RpcTarget.AllBuffered, player.GetCustomData(LocalSettings.networkPosition));
        }

        [PunRPC]
        public void ReAssignNetworkSeatToAllInstanceOfThisPlayer(int seat)
        {
            myNetworkSeat = seat;
            AdjustGameManagerLocalSeat = seat;
        }



        public void StandUp()
        {

            PlayerStateManager.Instance.taptoSitHere.SetActive(true);

            PlayerStateManager.Instance.waitForNextRound.SetActive(false);
            PlayerStateManager.Instance.Amountobject.SetActive(false);

            if (MatchHandler.IsTeenPatti())
            {
                if (getCurrentPlayerState().currentState == PlayerState.STATE.OutOfTable)
                    return;
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                {
                    if (getCurrentPlayerState().currentState == PlayerState.STATE.ExecutingTurn)
                    {
                        int a = PlayerStateManager.Instance.SideShowNext();
                        if (PlayerStateManager.Instance.PlayingList[a].getCurrentPlayerState().currentState != PlayerState.STATE.RecieverSideShow)
                        {
                            // Debug.Log("Player Left Room Called " + PlayingList[a].player.NickName + PlayingList[a].getCurrentPlayerState().currentState);
                            getCurrentPlayerState().giveTurnToNext = true;
                        }

                    }
                    else
                    {
                        if (getCurrentPlayerState().currentState == PlayerState.STATE.RecieverSideShow)
                        {
                            Game_Play.Instance.OnClickCancelSideShowBtn();
                            getCurrentPlayerState().giveTurnToNext = false;
                        }
                        else
                        {
                            if (getCurrentPlayerState().currentState == PlayerState.STATE.SenderSideShow)
                            {
                                Game_Play.Instance.OnClickCancelSideShowBtn();
                                AllSideShowPanelsFalse(false);
                                getCurrentPlayerState().giveTurnToNext = false;
                            }
                            else
                            {
                                getCurrentPlayerState().giveTurnToNext = false;
                                photonView.RPC("SetgiveTurnToNextBool", RpcTarget.All);
                            }
                        }

                    }

                }

            }
            else if (MatchHandler.IsAndarBahar())
            {
                Game_Play.Instance.Skip_Bet();
            }
            else if (MatchHandler.IsPoker())
            {
                //Debug.LogError("Check player Status about Player Standup.....1");
                if (getCurrentPlayerState().currentState == PlayerState.STATE.OutOfTable)
                {
                    return;
                }
                //else
                //{
                //    if (currentPlayerStateRef.currentState != PlayerState.STATE.Packed)
                //    {
                //        currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.Packed);
                //        // Invoke(nameof(Game_Play.Instance.StandUp), 1f);
                //        Debug.LogError("ata hai ya nahi");
                //        return;
                //    }
                //}
            }
            else if (MatchHandler.IsLuckyWar())
            {
                LWActionPanelScript.Instance.Deal_Bet();
            }

            GameManager.Instance.position_availability[0].is_reserved = null;
            if (PhotonNetwork.IsMasterClient)
                PositionsManager.Instance.ReleasePosition(GameManager.Instance.myLocalSeat);

            currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.OutOfTable);
            GameManager.Instance.SitHereBtnStatus(true);
        }




        [PunRPC]
        public void SetgiveTurnToNextBool()
        {
            getCurrentPlayerState().giveTurnToNext = false;
        }

        public void ActivatePlayerAgainOnNetwork()
        {

            playerEnterenceState();

            this_photonView.RPC("ActivatePlayerAgain", RpcTarget.All);
        }

        [PunRPC]
        public void ActivatePlayerAgain()
        {
            if (MatchHandler.IsLuckyWar())
            {
                gameObject.SetActive(true);
                PackedText.gameObject.SetActive(false);
                if (getCurrentPlayerState().currentState != PlayerState.STATE.ExecutingTurn && getCurrentPlayerState().currentState != PlayerState.STATE.WaitingForTurn)
                {
                    if (PlayerLWCard != null)
                        PlayerLWCard.gameObject.SetActive(false);
                }


            }
            else
            {
                gameObject.SetActive(true);
                PackedText.gameObject.SetActive(false);
                if (playerGiftTransfer.giftObject != null)
                    Destroy(playerGiftTransfer.giftObject);
                if (getCurrentPlayerState().currentState != PlayerState.STATE.ExecutingTurn && getCurrentPlayerState().currentState != PlayerState.STATE.WaitingForTurn)
                {
                    PlayerOrignalCardsToShowParent.gameObject.SetActive(false);
                }
            }
        }



        public PlayerCurrentState getCurrentPlayerState()
        {
            return currentPlayerStateRef;
        }


        public void AssignName(string name)
        {
            player_name.text = name;
            gameObject.name = name;
        }


        private void UpdateProfilePic()
        {
            if (photonView.IsMine)
            {
                Debug.Log(player.NickName + " With Actor " + player.ActorNumber + " is UpdatingProfilePic");
                player.SetCustomData(LocalSettings.ProfilePic, LocalSettings.GetprofilePic());
                player.SetCustomData(LocalSettings.ProfileFrame, LocalSettings.GetprofileFrame());
            }
        }


        public void ForAllShowTipToGirl(int dialogueNumber)
        {

            GameManager.Instance.PlayerTotalChipsUpdate(-Pot.instance.potTip);
            player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());


            photonView.RPC(nameof(TipToGirl), RpcTarget.All, dialogueNumber);
            GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.tip, Pot.instance.potTip.ToString());
        }



        [ShowOnly]
        public float timeDelay = 1.5f;
        [PunRPC]
        public void TipToGirl(int dialogueNumber)
        {
            Pot potInstance = Pot.instance;

            GameObject tipCollect = Instantiate(tipAmountAnim, this.transform);
            tipCollect.GetComponent<BetAmountToTargetAnim>().targetPos = potInstance.targeToTipGirl;
            tipCollect.SetActive(false);
            tipCollect.GetComponentInChildren<TMP_Text>().text = LocalSettings.Rs(potInstance.potTip);
            //potInstance.tipFromPlayerNameText.text = "";
            //potInstance.tipFromPlayerNameText.text = "Thanks you, " + player_name.text + ". May the good hands be with you!";


            if (!potInstance.tipFromPlayerNameText.transform.parent.gameObject.activeSelf)
            {
                potInstance.tipFromPlayerNameText.text = "Thanks you, " + player_name.text + ". " + tipsDialouges[dialogueNumber] + "!";
                potInstance.NextTipOfPlayer(timeDelay, this);
            }




            potInstance.tipDialogueTextObject.Add("Thanks you, " + player_name.text + ". " + tipsDialouges[dialogueNumber] + "!");
            potInstance.tipFromPlayerNameText.transform.parent.gameObject.SetActive(true);

            //GameManager.Instance.PlayerTotalChipsUpdate(-potInstance.potTip);





            MyTotalCashTextUpdate();
            tipCollect.SetActive(true);

            Destroy(tipCollect, 4f);

        }



        public void AddToPot(BigInteger amount)
        {

            if (photonView.IsMine)
            {
                if (MatchHandler.IsTeenPatti())
                {
                    Pot.instance.potSize += amount;
                    photonView.RPC(nameof(UpdatePotSize), RpcTarget.All, Pot.instance.potSize.ToString());

                    LocalSettings.GetCurrentRoom.SetTableCollectedCash(LocalSettings.TableCashKey, Pot.instance.potSize);
                    /* in Game_Play where click on End_my_turn
                    if (!IsSeen)
                    {

                        //if (PlayerStateManager.Instance.CheckIfAllPlayersPlayedMaxChaals())
                        if (MyChaalsPlayedCounter > UIManager.Instance.TotalChals)
                        {

                            // PlayerStateManager.Instance.ShowCardsOfAllPlayers();
                            ShowCardsFromBlind();
                        }
                    }
                    */
                }


            }
        }
        bool isShowAllCards;
        [PunRPC]
        public void UpdatePotSize(string newPotSizeString)
        {
            BigInteger newPotSize = BigInteger.Parse(newPotSizeString);
            Pot.instance.potSize = newPotSize;
            Pot.instance.SetCashText(Pot.instance.potSize.ToString());
            BigInteger ChalAmount = Pot.instance.CurrentChalAmount;
            if (IsSeen)
                ChalAmount = Pot.instance.CurrentChalAmount * 2;
            BetAmountAnimText.text = LocalSettings.Rs(ChalAmount);
            if (currentPlayerStateRef.currentState != PlayerState.STATE.OutOfGame)
            {
                BetAmountAnim.GetComponent<BetAmountToTargetAnim>().targetPos = Pot.instance.PotPanel.gameObject;
                if (!Pot.instance.PotPanel.gameObject.activeInHierarchy)
                    Pot.instance.PotPanel.gameObject.SetActive(true);
                BetAmountAnim.SetActive(true);
            }

            // MyChaalsPlayedCounter++;


            string blindOrChal = "";
            if (IsSeen)
                blindOrChal = " Chaal of ";
            else
                blindOrChal = " Blind of ";
            if (RoomStateManager.Instance.CurrentRoomState != RoomState.STATE.CardDistributing)
                Game_Play.Instance.ShowInfo(photonView.Controller.NickName + " played a" + blindOrChal + " Rs." + ChalAmount, 1f);

            // check if all players played
            if (Pot.instance.potSize >= Pot.instance.PotLimit)
            {
                RoomStateManager.Instance.UpdateCurrentState(RoomState.STATE.WaitingForResults, LocalSettings.textStringOfPotLimitReached);
            }
        }



        [PunRPC]
        public void GiveChaalAmountOnNetoworkAtStart()
        {

            Game_Play.Instance.ShowInfo(LocalSettings.textStringOnStartBetAmount, 1f);
            BetAmountAnimText.text = LocalSettings.Rs(Pot.instance.CurrentChalAmount);
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            if (!PhotonNetwork.IsMasterClient)
            {
                if (currentPlayerStateRef.currentState != PlayerState.STATE.OutOfTable && currentPlayerStateRef.currentState != PlayerState.STATE.OutOfGame)
                {
                    if (photonView.IsMine)
                    {
                        if (LocalSettings.GetTotalChips() >= Pot.instance.startPotAmount)
                        {
                            GameManager.Instance.PlayerTotalChipsUpdate(-Pot.instance.startPotAmount);
                            GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, Pot.instance.startPotAmount.ToString());
                            UIManager.Instance.TotalBetPlacedAmount += Pot.instance.startPotAmount;
                        }
                        else
                        {
                            StandUp();
                            UIManager.Instance.quickShop.SetActive(true);
                        }

                    }


                    //Debug.LogError("starting Amount  :   - " + Pot.instance.startPotAmount);
                }


            }
            BetAmountAnim.GetComponent<BetAmountToTargetAnim>().targetPos = Pot.instance.PotPanel.gameObject;
            BetAmountAnim.SetActive(true);
        }

        public void GiveChaalAmountOnGameStart()
        {
            //Debug.Log("Start Pot Amount Deduct" + -Pot.instance.startPotAmount);
            photonView.RPC(nameof(GiveChaalAmountOnNetoworkAtStart), RpcTarget.All);
        }



        /// <summary>
        /// Player cards indexes sending and receiving
        /// </summary>
        /// <param name="ModifiedChalAmount"></param>
        #region Sending And Getting Player cards array
        public void SendPlayerCardsArray(int[] RandomCardsArray)
        {
            photonView.RPC("UpDatePlayerCardsArray", RpcTarget.All, RandomCardsArray);
        }

        [PunRPC]
        public void UpDatePlayerCardsArray(int[] PlayerCardsArray)
        {
            OrignalCardsSetting(PlayerCardsArray);
        }

        public void OrignalCardsSetting(int[] cardsArray)
        {
            int CardPosNumber;
            GameManager gameManager = GameManager.Instance;

            PlayerStateManager psm = PlayerStateManager.Instance;
            GameObject card = null;
            RectTransform rt;
            Transform parntObj;
            int PlayerNumber = 0;
            for (int i = 0; i < psm.PlayingList.Count; i++)
            {
                if (psm.PlayingList[i].photonView.ViewID == photonView.ViewID)
                {
                    PlayerNumber = i; break;
                }
            }

            for (int j = 0; j < psm.PlayingList.Count; j++)
            {
                card = Instantiate(gameManager.AllCards.Card[cardsArray[(j * 3) + 0]].gameObject);
                rt = psm.PlayingList[j].PlayerOrignalCardsToShowParent.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
                parntObj = psm.PlayingList[j].PlayerOrignalCardsToShowParent.transform;
                LocalSettings.SetPosAndRect(card, rt, parntObj.transform);

                card = Instantiate(gameManager.AllCards.Card[cardsArray[(j * 3) + 1]].gameObject);
                rt = psm.PlayingList[j].PlayerOrignalCardsToShowParent.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
                parntObj = psm.PlayingList[j].PlayerOrignalCardsToShowParent.transform;
                LocalSettings.SetPosAndRect(card, rt, parntObj.transform);

                card = Instantiate(gameManager.AllCards.Card[cardsArray[(j * 3) + 2]].gameObject);
                rt = psm.PlayingList[j].PlayerOrignalCardsToShowParent.transform.GetChild(2).gameObject.GetComponent<RectTransform>();
                parntObj = psm.PlayingList[j].PlayerOrignalCardsToShowParent.transform;
                LocalSettings.SetPosAndRect(card, rt, parntObj.transform);

            }


            if (PhotonNetwork.IsMasterClient)
            {
                int[] orgCardsAry = new int[3];

                for (int i = 0; i < PlayerStateManager.Instance.PlayingList.Count; i++)
                {
                    orgCardsAry[0] = psm.PlayingList[i].PlayerOrignalCardsToShowParent.transform.GetChild(3).gameObject.GetComponent<CardProperty>().CardIndexInArray;
                    orgCardsAry[1] = psm.PlayingList[i].PlayerOrignalCardsToShowParent.transform.GetChild(4).gameObject.GetComponent<CardProperty>().CardIndexInArray;
                    orgCardsAry[2] = psm.PlayingList[i].PlayerOrignalCardsToShowParent.transform.GetChild(5).gameObject.GetComponent<CardProperty>().CardIndexInArray;
                    psm.PlayingList[i].player.SetCustomArray(LocalSettings.OrgCardsArray, orgCardsAry);
                    string koibhi = "";
                    for (int j = 0; j < orgCardsAry.Length; j++)
                    {
                        koibhi += orgCardsAry[j] + ", ";
                    }
                    // Debug.LogError("Player Orignal Cards : " + koibhi);
                }
            }

            if (MatchHandler.CurrentMatch == MatchHandler.MATCH.HUukm)
            {
                //Debug.LogError("Hukm is working");
                GameObject supCard = Instantiate(gameManager.AllCards.Card[cardsArray[cardsArray.Length - 1]].gameObject, gameManager.PlayerTable);
                gameManager.SupportingCard = supCard.GetComponent<CardProperty>();
                LocalSettings.SetPosAndRect(gameManager.SupportingCard.gameObject, gameManager.SupportingCardPos, gameManager.PlayerTable);
                gameManager.SupportingCard.gameObject.SetActive(false);


                for (int i = 0; i < psm.PlayingList.Count; i++)
                {
                    PlayerInfo pInfo = psm.PlayingList[i];
                    if (pInfo.PlayerOrignalCardsToShowParent.transform.childCount > 2)
                    {
                        CardProperty card0 = pInfo.PlayerOrignalCardsToShowParent.transform.GetChild(3).gameObject.GetComponent<CardProperty>();
                        CardProperty card1 = pInfo.PlayerOrignalCardsToShowParent.transform.GetChild(4).gameObject.GetComponent<CardProperty>();
                        CardProperty card2 = pInfo.PlayerOrignalCardsToShowParent.transform.GetChild(5).gameObject.GetComponent<CardProperty>();
                        HukmGameLogic.GetAlternateCard(card0, card1, card2, gameManager.SupportingCard);
                        CardProperty cardToReplaceWithPlayerCard = HukmGameLogic.cardToReplace;
                        if (cardToReplaceWithPlayerCard)
                        {
                            GameObject newCardForPlayer = Instantiate(gameManager.SupportingCard.gameObject);
                            if (cardToReplaceWithPlayerCard.Power == card0.Power && cardToReplaceWithPlayerCard.Suit == card0.Suit)
                            {
                                LocalSettings.SetPosAndRect(newCardForPlayer, card0.gameObject.GetComponent<RectTransform>(), pInfo.PlayerOrignalCardsToShowParent.transform);
                                card0.transform.parent = newCardForPlayer.transform;
                                newCardForPlayer.transform.SetSiblingIndex(newCardForPlayer.transform.GetSiblingIndex() - 2);
                                card0.gameObject.AddComponent<ColorChangeAnim>();
                            }
                            else if (cardToReplaceWithPlayerCard.Power == card1.Power && cardToReplaceWithPlayerCard.Suit == card1.Suit)
                            {
                                LocalSettings.SetPosAndRect(newCardForPlayer, card1.gameObject.GetComponent<RectTransform>(), pInfo.PlayerOrignalCardsToShowParent.transform);
                                card1.transform.parent = newCardForPlayer.transform;
                                newCardForPlayer.transform.SetSiblingIndex(newCardForPlayer.transform.GetSiblingIndex() - 1);
                                card1.gameObject.AddComponent<ColorChangeAnim>();
                            }
                            else if (cardToReplaceWithPlayerCard.Power == card2.Power && cardToReplaceWithPlayerCard.Suit == card2.Suit)
                            {
                                LocalSettings.SetPosAndRect(newCardForPlayer, card2.gameObject.GetComponent<RectTransform>(), pInfo.PlayerOrignalCardsToShowParent.transform);
                                card2.transform.parent = newCardForPlayer.transform;
                                card2.gameObject.AddComponent<ColorChangeAnim>();
                            }
                        }
                    }
                }
            }
            //ShowCardsFromBlind();
        }

        public void ShowCardsFromBlind()
        {
            PlayerDummyCardsToShowParent.gameObject.SetActive(false);
            PlayerOrignalCardsToShowParent.SetActive(true);
            for (int i = 0; i < PlayerOrignalCardsToShowParent.transform.childCount; i++)
            {
                if (!PlayerOrignalCardsToShowParent.transform.GetChild(i).gameObject.activeInHierarchy)
                    PlayerOrignalCardsToShowParent.transform.GetChild(i).gameObject.SetActive(true);

                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
            }
            IsSeen = true;
            player.SetCustomBoolData("is_seen", true);
            UIManager.Instance.ChaalTypeText.text = "Chaal";
            photonView.RPC(nameof(SeenAlert), RpcTarget.All, IsSeen);
            if (photonView.IsMine)
            {
                PlayerStateManager.Instance.SideShowAndShowbtn();
            }

        }

        public void GiveSeenAlertToAll()
        {
            photonView.RPC("SeenAlert", RpcTarget.All, IsSeen);
        }


        [PunRPC]
        public void SeenAlert(bool seen)
        {
            if (seen)
            {
                IsSeen = seen;
                BlindIndicator.SetActive(false);
                SeenIndicator.SetActive(true);
                if (Pot.instance.ChaalAmountLimit() != Pot.instance.CurrentChalAmount && photonView.IsMine)
                    UIManager.Instance.UpDateCurrentChalAmountText(Pot.instance.CurrentChalAmount * 2);
            }
            if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                int PrevPlayer = PlayerStateManager.Instance.SideShowPrev();
                // Debug.LogError("Player Current state:-----  " + PrevPlayer);
                if (PrevPlayer >= 0)
                {
                    if (PlayerStateManager.Instance.PlayingList[PrevPlayer].IsSeen)
                    {
                        if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.ExecutingTurn)
                        {
                            UIManager.Instance.sideShowBtn.interactable = true;
                        }
                    }
                }
            }
        }

        public void GivesideShowAlertToAll(bool sideShow)
        {
            IsSideShow = sideShow;
            //photonView.RPC("SideShowAlert", RpcTarget.AllBuffered, IsSideShow);
            photonView.RPC("SideShowAlert", RpcTarget.All, IsSideShow);
        }


        [PunRPC]
        public void SideShowAlert(bool setbool)
        {
            IsSideShow = setbool;
            foreach (var item in PlayerStateManager.Instance.PlayingList)
            {
                item.IsSideShow = setbool;
                if (!setbool)
                    item.SideShowIndicatorAnim.SetActive(setbool);
            }
        }
        public void AllSideShowPanelsFalse(bool sideShow)
        {
            //photonView.RPC("SideShowAlert", RpcTarget.AllBuffered, IsSideShow);
            photonView.RPC("SideShowPanelsFalse", RpcTarget.All, sideShow);
        }


        [PunRPC]
        public void SideShowPanelsFalse(bool setbool)
        {

            UIManager.Instance.sideShowPanel.SetActive(setbool);
        }

        #endregion

        public void CurrentChalAmountSendToAllPlayers(BigInteger ModifiedChalAmount)
        {
            if (photonView.IsMine)
            {
                Pot.instance.CurrentChalAmount = ModifiedChalAmount;
                photonView.RPC("UpdateAllPlayersCurrentChallAmount", RpcTarget.All, Pot.instance.CurrentChalAmount.ToString());
            }
        }
        [PunRPC]
        public void UpdateAllPlayersCurrentChallAmount(string ChalAmountUpdatedString)
        {
            BigInteger ChalAmountUpdated = BigInteger.Parse(ChalAmountUpdatedString);
            Pot.instance.CurrentChalAmount = ChalAmountUpdated;
            BigInteger betAmount = Pot.instance.CurrentChalAmount;
            if (IsSeen)
                betAmount = Pot.instance.CurrentChalAmount * 2;
            UIManager.Instance.UpDateCurrentChalAmountText(betAmount);
        }

        //public void AddToPotForNewPlayer(int amount)
        //{
        //    if (photonView.IsMine)
        //    {
        //        Pot.instance.potSize += amount;
        //        photonView.RPC("UpdatePotSizeForNewPlayer", RpcTarget.All, Pot.instance.potSize);
        //    }
        //}
        //[PunRPC]
        //public void UpdatePotSizeForNewPlayer(int newPotSize)
        //{

        //    Pot.instance.potSize = newPotSize;
        //    Pot.instance.PotTxt.text = "Rs." + Pot.instance.potSize;



        //    //Debug.Log("New pot size: " + Pot.instance.potSize);
        //}



        public void StartPotAmount()
        {
            //AddToPot(Pot.instance.startPotAmount * PhotonNetwork.CurrentRoom.PlayerCount);
            AddToPot(Pot.instance.startPotAmount * PlayerStateManager.Instance.PlayingList.Count);
            if (currentPlayerStateRef.currentState != PlayerState.STATE.OutOfTable && currentPlayerStateRef.currentState != PlayerState.STATE.OutOfGame && photonView.IsMine)
            {
                GameManager.Instance.PlayerTotalChipsUpdate(-Pot.instance.startPotAmount);
                UIManager.Instance.TotalBetPlacedAmount += Pot.instance.startPotAmount;
                GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, Pot.instance.startPotAmount.ToString());
            }

        }



        public void GetProfilePic()
        {
            // int index = player.GetCustomData(LocalSettings.ProfilePic);

            int indexFrame = player.GetCustomData(LocalSettings.ProfileFrame);
            // PlayerAvatorImage.sprite = GameManager.Instance.PlayerProfileImages.Sprites[index];

            //  playerFrameImage.sprite = GameManager.Instance.playerProfileFrameImage.Sprites[indexFrame].Sprites;
            LocalSettings.CheckFrameNumber(playerFrameImage, GameManager.Instance.playerProfileFrameImage, indexFrame);
            if (photonView.IsMine)
                PlayerAvatorImage.sprite = LocalSettings.ServerSideImge;
            else
                if (this.gameObject.activeSelf)
            {
                StartCoroutine(waitforLoadPlayerAvatar());
            }
        }

        IEnumerator waitforLoadPlayerAvatar()
        {
            //yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => !string.IsNullOrEmpty(player.GetCustomString(LocalSettings.ProfilePicNameKey)));
            string profileImgName = player.GetCustomString(LocalSettings.ProfilePicNameKey);
            // Debug.LogError("Check Name of image"  + "   " + player.GetCustomString(LocalSettings.ProfilePicNameKey));
            RetrieveImageFromDB(profileImgName);

            //if (ProfilePic == null)
            //{
            //    StartCoroutine(waitforLoadPlayerAvatar());
            //    yield return null;
            //}
            yield return new WaitUntil(() => ProfilePic != null);
            PlayerAvatorImage.sprite = ProfilePic;
        }


        public void UpdateScore()
        {
            if (photonView.IsMine)
                player.SetCustomData(LocalSettings.Score, UnityEngine.Random.Range(0, 100));
        }

        public void GetScore()
        {
            PicIndexTxt.text = player.GetCustomData(LocalSettings.Score).ToString();
        }

        public void IAmWinner(bool winner)
        {
            photonView.RPC(nameof(RPCWinner), RpcTarget.All, winner);

        }

        public void PlayerPacked()
        {
            currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.Packed);
        }

        public void UpdatePlayerState(PlayerState.STATE state)
        {
            currentPlayerStateRef.UpdateCurrentPlayerState(state);
        }

        bool isWinner
        {
            get { return isWinner; }
            set
            {
                if (value == true)
                {

                    if (!GameManager.Instance.isRunINBackGround || !MatchHandler.IsPoker())
                    {
                        // Toaster.ShowAToast("Winner Indicator..." + GameManager.Instance.isRunINBackGround);
                        WinningIndicator.SetActive(true);
                        Pot pot = Pot.instance;
                        if (!MatchHandler.IsPoker())
                        {
                            pot.winAmountText.text = LocalSettings.Rs(pot.potSize);

                        }
                        else if (MatchHandler.IsPoker())
                        {
                            pot.PotPanel.SetActive(false);
                            pot.potLimitTxt.text = "";
                            pot.winAmountText.text = LocalSettings.Rs(PokerActionPanel.Instance.TotalPotAmount());
                            LocalSettings.SetPosAndRect(pot.winAmountAnim, PokerManager.Instance.FinalPokerBetAmountPoint.GetComponent<RectTransform>(), pot.winAmountAnim.transform.parent);


                            foreach (var item in GameManager.Instance.playersList)
                            {
                                if (item.pokerBetAmountAnim != null)
                                    Destroy(item.pokerBetAmountAnim);
                            }
                            //Debug.LogError("Check here...." + LocalSettings.Rs(PokerActionPanel.Instance.TotalPotAmount()));

                        }
                        //Debug.LogError("Check here...." + LocalSettings.Rs(PokerActionPanel.Instance.TotalPotAmount()));
                        pot.winAmountAnim.GetComponent<BetAmountToTargetAnim>().targetPos = WinningIndicator;
                        pot.winAmountAnim.SetActive(true);
                        if (photonView.IsMine)
                        {
                            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                        }
                    }
                }
                else
                {
                    WinningIndicator.SetActive(false);
                }

            }

        }

        [PunRPC]
        public void RPCWinner(bool net_winner)
        {
            isWinner = net_winner;
        }


        private void OnDisable()
        {

            if (PhotonNetwork.IsMasterClient)
            {
                // Release the position if the player was the master client
                PositionsManager.Instance.ReleasePosition(myNetworkSeat);
            }
            if (GameStartManager.Instance != null)
            {
                GameStartManager.Instance.AddOrRemovePlayer(-1);
            }
            ShowBtn.SetActive(false);

            PlayerOrignalCardsToShowParent.SetActive(false);
            PlayerDummyCardsToShowParent.SetActive(false);
            SeenIndicator.SetActive(false);
            BlindIndicator.SetActive(false);
        }


        private void OnEnable()
        {

            GetProfilePic();
            if (MatchHandler.IsPoker())
            {
                StartCoroutine(GetPropertyBuyInChips());
            }
            GameManager.Instance.UpdateWatchingPlayers();
            GameStartManager.Instance.AddOrRemovePlayer(1);

        }

        IEnumerator GetPropertyBuyInChips()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                yield return new WaitUntil(() => player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey) > 0);
                if (!photonView.IsMine)
                    PokerTotalCashTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey));
                //Debug.LogError("3.....Check Here For Poker Cash....." + PokerTotalCashTxt.text);
            }
        }

        public void sendGoldTranferAndUpdateChips()
        {
            photonView.RPC(nameof(sendGoldTranferAndUpdateChipsRPC), RpcTarget.All);
        }

        [PunRPC]
        public void sendGoldTranferAndUpdateChipsRPC()
        {
            StartCoroutine(GetPropertyBuyInChipsDuringGoldTransfer());
        }

        IEnumerator GetPropertyBuyInChipsDuringGoldTransfer()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                yield return new WaitForSeconds(1f);
                if (!photonView.IsMine)
                    playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
                //Debug.LogError("3.....Check Here For Poker Cash....." + PokerTotalCashTxt.text);
            }
        }

        #region AndarBaharSection


        public void ViewResultAB()
        {
            viewResultAB.SetActive(false);
            AndarBaharPositionsManager.Instance.CalculationOfWinnerindicatorOfAB();
        }

        public void DeActivateUIOfAndarBahar(bool istrue)
        {
            ABBettingSection.SetActive(istrue);
            LWBettingSection.SetActive(istrue);

        }



        public void PlaceBetAndar()
        {
            Pot.instance.AddAndarAmount(AndarBaharPositionsManager.Instance.AndarTotalBetAmount);
            player.SetCustomBigIntegerData(LocalSettings.abAndarAmountKey, Pot.instance.AndarTotalBetPlaced);
            AndarBaharPositionsManager.Instance.OnclickAndarBetAPI();
            photonView.RPC(nameof(AndarBetOnNetwork), RpcTarget.All, Pot.instance.AndarTotalBetPlaced.ToString());
            GameManager.Instance.PlayerTotalChipsUpdate(-AndarBaharPositionsManager.Instance.AndarTotalBetAmount);
            player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            UpdateTextForOtherPlayer();
            // GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, AndarBaharPositionsManager.Instance.AndarTotalBetAmount.ToString());
        }

        public void PlaceBetBahar()
        {
            Pot.instance.AddBaharAmount(AndarBaharPositionsManager.Instance.BaharTotalBetAmount);
            player.SetCustomBigIntegerData(LocalSettings.abBaharAmountKey, Pot.instance.BaharTotalBetPlaced);
            if (!Game_Play.Instance.secondTurnTurnAb)
                AndarBaharPositionsManager.Instance.OnclickBahar1BetAPI();
            else
                AndarBaharPositionsManager.Instance.OnclickBahar2BetAPI();

            photonView.RPC(nameof(BaharBetOnNetwork), RpcTarget.All, Pot.instance.BaharTotalBetPlaced.ToString());
            GameManager.Instance.PlayerTotalChipsUpdate(-AndarBaharPositionsManager.Instance.BaharTotalBetAmount);
            player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            UpdateTextForOtherPlayer();
            // GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, AndarBaharPositionsManager.Instance.BaharTotalBetAmount.ToString());
        }

        public void TurnOnOffAb(bool turn)
        {

            photonView.RPC(nameof(TurnForAllPlayerOfAB), RpcTarget.All, turn);
        }

        public void FirstAbOnMasterClientLeft(bool turn)
        {
            photonView.RPC(nameof(firstTurnFromMaster), RpcTarget.All, turn);
        }
        public void SecondTurnAB(bool turn)
        {
            photonView.RPC(nameof(SecondTurnAndarBahar), RpcTarget.All, turn);
        }

        [PunRPC]
        void AndarBetOnNetwork(string andarAmount)
        {
            BetAmountAnimText.text = LocalSettings.Rs(andarAmount);
            BetAmountAnim.GetComponent<BetAmountToTargetAnim>().targetPos = ABBettingSection.transform.GetChild(1).gameObject;
            BetAmountAnim.SetActive(true);
            StartCoroutine(LoadTextForBet(AndarBetAmoutTxt, andarAmount));
            // AndarBetAmoutTxt.text = andarAmount;
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            // Debug.LogError("Chip Adding sound is playing");

        }


        public void UpdateTextForOtherPlayer()
        {
            photonView.RPC(nameof(UpdateAndarBaharTextTotalCash), RpcTarget.All);
        }


        [PunRPC]
        void UpdateAndarBaharTextTotalCash()
        {
            StartCoroutine(SetTextOfOtherPlayers());
        }

        IEnumerator SetTextOfOtherPlayers()
        {
            yield return new WaitForSeconds(1f);
            playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
        }

        [PunRPC]
        void BaharBetOnNetwork(string baharAmmount)
        {
            BetAmountAnimText.text = LocalSettings.Rs(baharAmmount);
            BetAmountAnim.GetComponent<BetAmountToTargetAnim>().targetPos = ABBettingSection.transform.GetChild(2).gameObject;
            BetAmountAnim.SetActive(true);
            StartCoroutine(LoadTextForBet(BaharBetAmoutTxt, baharAmmount));
            //BaharBetAmoutTxt.text = baharAmmount;

            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            //Debug.LogError("Chip adding sound is playing");
        }
        [PunRPC]
        void firstTurnFromMaster(bool turn)
        {
            if (GameManager.Instance != null && GameManager.Instance.playersList != null)
            {
                foreach (PlayerInfo item in GameManager.Instance.playersList)
                {
                    item.firstTurnAB = turn;
                }
            }
        }
        [PunRPC]
        void TurnForAllPlayerOfAB(bool turn)
        {

            AbTurn = turn;
            player.SetCustomBoolData(LocalSettings.AbturnKey, turn);
            FillerImage.gameObject.SetActive(turn);
        }
        [PunRPC]
        void SecondTurnAndarBahar(bool turn)
        {
            Game_Play.Instance.MasterClientStandUpFlag = true;

            PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationAndarBahar;
            PlayerTurnManager.Instance.turnManager.BeginTurn();
            if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
            {
                Game_Play.Instance.ABActionButtonInteractable();
                LocalSettings.Vibrate();
                AndarBaharPositionsManager.Instance.ABActionPanel.SetActive(true);


            }
            AbTurn = turn;

            foreach (PlayerInfo item in PlayerStateManager.Instance.PlayingList)
            {
                item.AbTurn = true;
                item.player.SetCustomBoolData(LocalSettings.AbturnKey, item.AbTurn);

            }

        }



        /// <summary>
        ///  Super andar bahar section
        /// </summary>
        public void PlaceBetSuperAndar()
        {
            Pot.instance.AddSuperAndarAmount(AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount);
            player.SetCustomBigIntegerData(LocalSettings.abSuperAndarAmountKey, Pot.instance.SuperAndarTotalBetPlaced);
            photonView.RPC(nameof(SuperAndarBetOnNetwork), RpcTarget.All, Pot.instance.SuperAndarTotalBetPlaced);
            GameManager.Instance.PlayerTotalChipsUpdate(-AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount);
            // GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount.ToString());
        }

        public void PlaceBetSuperBahar(BigInteger SuperBaharAmount)
        {
            //Pot.instance.AddSuperBaharAmount(AndarBaharPositionsManager.Instance.SuperBaharTotalBetAmount);

            player.SetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey, SuperBaharAmount);

            photonView.RPC(nameof(SuperBaharBetOnNetwork), RpcTarget.All, SuperBaharAmount.ToString() /*LocalSettings.Rs(SuperBaharAmount)*/);
            GameManager.Instance.PlayerTotalChipsUpdate(-SuperBaharAmount);
            player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            UpdateTextForOtherPlayer();
            //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, SuperBaharAmount.ToString());
        }

        [PunRPC]
        void SuperAndarBetOnNetwork(string andarAmount)
        {
            SuperAndarBetAmoutTxt.transform.parent.gameObject.SetActive(true);
            SuperAndarBetAmoutTxt.text = LocalSettings.Rs(andarAmount);
        }

        [PunRPC]
        void SuperBaharBetOnNetwork(string baharAmmount)
        {
            BetAmountAnimText.text = LocalSettings.Rs(baharAmmount);
            BetAmountAnim.GetComponent<BetAmountToTargetAnim>().targetPos = ABBettingSection.transform.GetChild(0).gameObject;
            BetAmountAnim.SetActive(true);
            SuperBaharBetAmoutTxt.transform.parent.gameObject.SetActive(true);
            if (AndarBaharPositionsManager.Instance.secondSuperBaharBet > 0)
            {
                BigInteger baharAmmountBigInt = BigInteger.Parse(baharAmmount);
                BigInteger secBet = baharAmmountBigInt + AndarBaharPositionsManager.Instance.firstSuperBaharBet;
                StartCoroutine(LoadTextForBet(SuperBaharBetAmoutTxt, secBet.ToString()));


            }
            else
            {

                StartCoroutine(LoadTextForBet(SuperBaharBetAmoutTxt, baharAmmount));
            }
            //SuperBaharBetAmoutTxt.text = baharAmmount;

            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            //Debug.LogError("Chips adding sound is playing");
        }

        IEnumerator LoadTextForBet(TMP_Text BetAmountText, string Amount)
        {
            //Debug.LogError("kia bana");
            yield return new WaitUntil(() => !this.BetAmountAnim.gameObject.activeSelf);

            if (TieBetWinLw)
            {
                //Debug.LogError("betting amount: " + Amount);
                BigInteger newAmount = BigInteger.Parse(Amount);
                newAmount *= 2;
                BetAmountText.text = LocalSettings.Rs(newAmount);
            }
            else
                BetAmountText.text = LocalSettings.Rs(Amount);
        }
        //public void TurnOnOffAb(bool turn)
        //{
        //    photonView.RPC(nameof(TurnForAllPlayerOfAB), RpcTarget.All, turn);
        //}
        //public void SecondTurnAB(bool turn)
        //{
        //    photonView.RPC(nameof(SecondTurnAndarBahar), RpcTarget.All, turn);
        //}
        #endregion

        #region Wingo Lottary Section

        public void BetOnWingoLottary(BigInteger BetAmount, int TargetPointInt, UnityEngine.Vector2 TouchPosition, bool isMute)
        {
            photonView.RPC(nameof(BetOnWingoLottaryRPC), RpcTarget.All, BetAmount.ToString(), TargetPointInt, TouchPosition, isMute);
        }
        Transform[] BetPointsArray = new Transform[13];
        [PunRPC]
        public void BetOnWingoLottaryRPC(string BetAmountString, int TargetPoint, UnityEngine.Vector2 TouchPosition, bool isMute)
        {
            BigInteger BetAmount = BigInteger.Parse(BetAmountString);
            GameObject chip = WingoManager.Instance.CreateChipToShow();
            //LocalSettings.SetPosAndRect(chip, gameObject.GetComponent<RectTransform>(), WingoManager.Instance.ChipsParent);
            if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
            {

                LocalSettings.SetPosAndRect(chip, GameManager.Instance.shakeAnimationWhenPositionAvailFul, GameManager.Instance.shakeAnimationWhenPositionAvailFul.parent);
                chip.transform.localScale *= 0.55f;
            }
            else
            {
                LocalSettings.SetPosAndRect(chip, gameObject.GetComponent<RectTransform>(), WingoManager.Instance.ChipsParent);
                if (photonView.IsMine)
                    chip.transform.localScale *= 0.23f;
                else
                    chip.transform.localScale *= 0.38f;
            }
            chip.SetActive(true);
            WingoChipDetail wcd = chip.GetComponent<WingoChipDetail>();
            wcd.ViewID = photonView.ViewID;
            wcd.PointNumber = TargetPoint;
            wcd.BetAmount = BetAmount;
            wcd.ChangeChipSprite();
            TouchPosition = WingoManager.Instance.getMyPositions(WingoManager.Instance.BetPointBtns[TargetPoint]);
            PlayAnimation(chip.transform, TouchPosition, 0.25f);
            //StartCoroutine(PlayAnimationCoRoutine(chip.transform, TouchPosition, 0.25f));

            // Show bet amount on bet point
            BetPointsArray = WingoManager.Instance.BetPointBtns;
            PointBetAmount pba = BetPointsArray[TargetPoint].gameObject.GetComponent<PointBetAmount>();
            pba.SetBetAmount(BetAmount, photonView.IsMine);

            // GetComponent<RectTransform>().position = UnityEngine.Vector2.zero;
            GetComponent<PlayerCurrentAnim>().PlayShakeAnimation();

            Playeramount();
            if (!isMute)
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            //Debug.LogError("Chips adding sound is playing");
        }

        void PlayAnimation(Transform ObjToAnimate, UnityEngine.Vector2 TouchPos, float TimeToReach)
        {
            ObjToAnimate.DOMove(TouchPos, TimeToReach, false);
        }

        //IEnumerator PlayAnimationCoRoutine(Transform ObjToAnimate, UnityEngine.Vector2 TouchPos, float TimeToReach)
        //{
        //    yield return new WaitForSeconds(0.01f);
        //    ObjToAnimate.DOMove(TouchPos, TimeToReach, false);
        //}


        // When result is called
        public void SendChipsToEveryOne()
        {
            double mul1 = 2.4f;
            double mul2 = 4.8f;
            double mul3 = 9.6f;
            WingoManager wm = WingoManager.Instance;
            GameManager gm = GameManager.Instance;
            UIManager uIManager = UIManager.Instance;
            int LuckyNumber = wm.WinningNumber;
            for (int i = 0; i < gm.playersList.Count; i++)
            {
                for (int j = 0; j < wm.AllCreatedChips.Count; j++)
                {
                    WingoChipDetail wcd = wm.AllCreatedChips[j].GetComponent<WingoChipDetail>();
                    if (gm.playersList[i].photonView.ViewID == wcd.ViewID)
                    {

                        if (LuckyNumber == 0 || LuckyNumber == 5)
                        {
                            if (wcd.PointNumber == 1)
                            {

                                if (uIManager.GetMyPlayerInfo().photonView.ViewID == wcd.ViewID)
                                {
                                    double amount = (double)(wcd.BetAmount) * mul2;
                                    wm.TotalRewardOnWin += (BigInteger)(amount);
                                    //wm.TotalRewardOnWin += (BigInteger)(wcd.BetAmount * mul2);
                                }
                                wcd.IsGoToPlayer = true;
                                //Before when 5 players
                                //PlayAnimation(wm.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);

                                //After When Enter more than 5 Players
                                if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
                                    PlayAnimation(wm.AllCreatedChips[j].transform, gm.shakeAnimationWhenPositionAvailFul.position, 0.75f);
                                else
                                    PlayAnimation(wm.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);
                            }
                        }
                        else
                        {
                            if (wcd.PointNumber == 0)
                            {
                                if (LuckyNumber == 1 || LuckyNumber == 3 || LuckyNumber == 7 || LuckyNumber == 9)
                                {
                                    if (uIManager.GetMyPlayerInfo().photonView.ViewID == wcd.ViewID)
                                    {
                                        double amount = (double)(wcd.BetAmount) * mul1;
                                        wm.TotalRewardOnWin += (BigInteger)(amount);
                                        //wm.TotalRewardOnWin += (BigInteger)(wcd.BetAmount * mul1);
                                    }
                                    wcd.IsGoToPlayer = true;
                                    //Before when 5 players
                                    //PlayAnimation(wm.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);

                                    //After When Enter more than 5 Players
                                    if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
                                        PlayAnimation(wm.AllCreatedChips[j].transform, gm.shakeAnimationWhenPositionAvailFul.position, 0.75f);
                                    else
                                        PlayAnimation(wm.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);
                                }
                            }
                            else if (wcd.PointNumber == 2)
                            {
                                if (LuckyNumber == 2 || LuckyNumber == 4 || LuckyNumber == 6 || LuckyNumber == 8)
                                {
                                    if (uIManager.GetMyPlayerInfo().photonView.ViewID == wcd.ViewID)
                                    {
                                        double amount = (double)(wcd.BetAmount) * mul1;
                                        wm.TotalRewardOnWin += (BigInteger)(amount);
                                        //wm.TotalRewardOnWin += (BigInteger)(wcd.BetAmount * mul1);
                                    }

                                    wcd.IsGoToPlayer = true;
                                    //Before when 5 players
                                    //PlayAnimation(wm.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);

                                    //After When Enter more than 5 Players
                                    if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
                                        PlayAnimation(wm.AllCreatedChips[j].transform, gm.shakeAnimationWhenPositionAvailFul.position, 0.75f);
                                    else
                                        PlayAnimation(wm.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);
                                }
                            }
                        }

                        if (LuckyNumber == wcd.PointNumber - 3)
                        {
                            if (uIManager.GetMyPlayerInfo().photonView.ViewID == wcd.ViewID)
                            {
                                double amount = (double)(wcd.BetAmount) * mul3;
                                wm.TotalRewardOnWin += (BigInteger)(amount);
                                //wm.TotalRewardOnWin += (BigInteger)(wcd.BetAmount * mul3);
                            }

                            wcd.IsGoToPlayer = true;
                            //Before when 5 players
                            //PlayAnimation(wm.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);

                            //After When Enter more than 5 Players
                            if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
                                PlayAnimation(wm.AllCreatedChips[j].transform, gm.shakeAnimationWhenPositionAvailFul.position, 0.75f);
                            else
                                PlayAnimation(wm.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);
                        }
                    }
                }
            }
            for (int i = 0; i < wm.AllCreatedChips.Count; i++)
            {
                if (!wm.AllCreatedChips[i].GetComponent<WingoChipDetail>().IsGoToPlayer)
                    PlayAnimation(wm.AllCreatedChips[i].transform, wm.ChipsHome.position, 0.75f);
            }
            if (WingoManager.Instance.TotalRewardOnWin > 0)
                photonView.RPC(nameof(ShowWinAmountRPC), RpcTarget.All, WingoManager.Instance.TotalRewardOnWin.ToString());
        }

        [PunRPC]
        public void ShowWinAmountRPC(string TotalWinAmountString)
        {
            BigInteger TotalWinAmount = BigInteger.Parse(TotalWinAmountString);
            WinningIndicator.SetActive(true);
            WingoWinCashTxt.text = "+" + LocalSettings.Rs(TotalWinAmount);
            WingoWinCashTxt.transform.parent.gameObject.SetActive(true);
            //Debug.LogError("here is your Player:....." + TotalWinAmount); ;
        }
        #endregion

        #region Chat messages Section

        public void ShowChatMessage(string message)
        {
            int ProfilePicIndex = player.GetCustomData(LocalSettings.ProfilePic);
            int profileFrameIndex = player.GetCustomData(LocalSettings.ProfileFrame);
            photonView.RPC(nameof(ShowTxtMessgeRPC), RpcTarget.All, message, ProfilePicIndex, profileFrameIndex);
        }

        [PunRPC]
        void ShowTxtMessgeRPC(string message, int ProfilePicIndex, int profileFrameIndex)
        {
            ChatMessageBox.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
            if (ChatMessageBox.activeInHierarchy)
                ChatMessageBox.SetActive(false);
            ChatMessageBox.SetActive(true);
            Chating.Instance.ProfilePicIndex = ProfilePicIndex;
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Notification, false);
            //Debug.LogError("Notification sound is playing");
        }

        public void ShowEmojiOnPlayer(int SpriteIndex)
        {
            photonView.RPC(nameof(ShowEmojiOnPlayerRPC), RpcTarget.All, SpriteIndex);
        }

        [PunRPC]
        void ShowEmojiOnPlayerRPC(int spriteIndex)
        {
            Sprite emojiImg = Chating.Instance.Emojies.Sprites[spriteIndex - 1];
            EmojiAnim.GetComponent<Image>().sprite = emojiImg;
            if (EmojiAnim.activeInHierarchy)
                EmojiAnim.SetActive(false);
            EmojiAnim.SetActive(true);
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Notification, false);
            //Debug.LogError("Notification sound is playing");
        }
        #endregion


        #region LuckyWarSection
        //public void TieBetWin(bool tieTrue)
        //{
        //    photonView.RPC(nameof(TieBetOnNetwork), RpcTarget.All, tieTrue);
        //}

        //[PunRPC]
        //public void TieBetWinnerNetwork(bool tieTrue)
        //{
        //    TieBetWinLw = tieTrue;

        //}

        public void TurnOnOffTieWinLW(bool turn)
        {

            photonView.RPC(nameof(TieWinPlayerOfLW), RpcTarget.All, turn);
        }
        [PunRPC]
        void TieWinPlayerOfLW(bool turn)
        {
            TieBetWinLw = turn;
            //Debug.LogError("Check Bool Variable" + TieBetWinLw);
            if (PhotonNetwork.IsConnectedAndReady)
                player.SetCustomBoolData(LocalSettings.LWTieWinKey, turn);
        }
        public void TurnOnOffLW(bool turn)
        {

            photonView.RPC(nameof(TurnForAllPlayerOfLW), RpcTarget.All, turn);
        }
        [PunRPC]
        public void TurnForAllPlayerOfLW(bool turn)
        {

            LWTurn = turn;
            if (PhotonNetwork.IsConnectedAndReady)
                player.SetCustomBoolData(LocalSettings.LWturnKey, turn);
            FillerImage.gameObject.SetActive(turn);
        }

        public void PlaceBetTie()
        {
            Pot.instance.AddTieAmount(LuckyWarManager.Instance.TieTotalAmount);
            GameManager.Instance.PlayerTotalChipsUpdate(-LuckyWarManager.Instance.TieTotalAmount);
            //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, LuckyWarManager.Instance.TieTotalAmount.ToString());
            player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            player.SetCustomBigIntegerData(LocalSettings.LWTieAmountKey, Pot.instance.TieTotalBetPlaced);
            LuckyWarManager.Instance.onTieBtnClick(LuckyWarManager.Instance.TieTotalAmount.ToString());
            Debug.LogError("tie btn pressed");
            photonView.RPC(nameof(TieBetOnNetwork), RpcTarget.All, Pot.instance.TieTotalBetPlaced.ToString());
        }
        [PunRPC]
        void TieBetOnNetwork(string tieAmount)
        {
            BetAmountAnimText.text = LocalSettings.Rs(tieAmount);
            BetAmountAnim.GetComponent<BetAmountToTargetAnim>().targetPos = LWBettingSection.transform.GetChild(1).gameObject;
            BetAmountAnim.SetActive(true);
            StartCoroutine(LoadTextForBet(TieBetAmoutTxt, tieAmount));
            // AndarBetAmoutTxt.text = andarAmount;
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            // Debug.LogError("Chip Adding sound is playing");
            if (!photonView.IsMine)
                playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
        }
        public void PlaceBetBet()
        {
            if (!TieBetWinLw)
            {
                Pot.instance.AddBetAmount(LuckyWarManager.Instance.BetTotalAmount);
            }
            GameManager.Instance.PlayerTotalChipsUpdate(-LuckyWarManager.Instance.BetTotalAmount);
            //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, LuckyWarManager.Instance.BetTotalAmount.ToString());
            player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            player.SetCustomBigIntegerData(LocalSettings.LWBetAmountKey, Pot.instance.BetTotalBetPlaced);
            photonView.RPC(nameof(BetBetOnNetwork), RpcTarget.All, Pot.instance.BetTotalBetPlaced.ToString());

            LuckyWarManager.Instance.onBetBtnClick(Pot.instance.BetTotalBetPlaced.ToString());
        }
        [PunRPC]
        void BetBetOnNetwork(string baharAmmount)
        {
            BetAmountAnimText.text = LocalSettings.Rs(baharAmmount);
            //Debug.LogError("here is You Update Amount..." + baharAmmount);
            BetAmountAnim.GetComponent<BetAmountToTargetAnim>().targetPos = LWBettingSection.transform.GetChild(0).gameObject;
            BetAmountAnim.SetActive(true);
            StartCoroutine(LoadTextForBet(BetBetAmoutTxt, baharAmmount));
            //BaharBetAmoutTxt.text = baharAmmount;

            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);

            if (!photonView.IsMine)
                playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
            //Debug.LogError("Chip adding sound is playing");
        }
        public void UpdateLWPlayerCash(string CurrentCash)
        {
            photonView.RPC(nameof(UpdateLWPlayerCashRPC), RpcTarget.All, CurrentCash);
        }

        [PunRPC]
        public void UpdateLWPlayerCashRPC(string cashString)
        {
            if (!photonView.IsMine)
                playerTotalCash.text = cashString;
        }
        public void CardshuffleboolForMasterCleint(bool isTrue)
        {
            photonView.RPC(nameof(cardshuffleBoolOnNetwork), RpcTarget.All, isTrue);
        }

        [PunRPC]
        public void cardshuffleBoolOnNetwork(bool isTrue)
        {
            foreach (PlayerInfo item in GameManager.Instance.playersList)
            {
                item.cardShuffleBool = isTrue;
            }
        }

        #endregion

        #region DragonTiger
        public void BetOnDragonTiger(BigInteger BetAmount, int TargetPointInt, UnityEngine.Vector2 TouchPosition, bool isMute)
        {
            photonView.RPC(nameof(BetOnDragonTigerRPC), RpcTarget.All, BetAmount.ToString(), TargetPointInt, TouchPosition, isMute);
        }
        Transform[] BetPointsArrayDT = new Transform[13];
        [PunRPC]
        public void BetOnDragonTigerRPC(string BetAmountString, int TargetPoint, UnityEngine.Vector2 TouchPosition, bool isMute)
        {
            BigInteger BetAmount = BigInteger.Parse(BetAmountString);
            GameObject chip = DragonTigerManager.Instance.CreateChipToShow();
            //LocalSettings.SetPosAndRect(chip, gameObject.GetComponent<RectTransform>(), WingoManager.Instance.ChipsParent);
            if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
            {

                LocalSettings.SetPosAndRect(chip, GameManager.Instance.shakeAnimationWhenPositionAvailFul, GameManager.Instance.shakeAnimationWhenPositionAvailFul.parent);
                chip.transform.localScale *= 0.55f;
            }
            else
            {
                LocalSettings.SetPosAndRect(chip, gameObject.GetComponent<RectTransform>(), DragonTigerManager.Instance.ChipsParent);
                if (photonView.IsMine)
                    chip.transform.localScale *= 0.23f;
                else
                    chip.transform.localScale *= 0.38f;
            }
            chip.SetActive(true);
            WingoChipDetail wcd = chip.GetComponent<WingoChipDetail>();
            wcd.ViewID = photonView.ViewID;
            wcd.PointNumber = TargetPoint;
            wcd.BetAmount = BetAmount;
            wcd.ChangeChipSprite();
            TouchPosition = DragonTigerManager.Instance.getMyPositions(DragonTigerManager.Instance.BetPointBtnsDT[TargetPoint]);
            PlayAnimation(chip.transform, TouchPosition, 0.25f);
            //StartCoroutine(PlayAnimationCoRoutine(chip.transform, TouchPosition, 0.25f));

            // Show bet amount on bet point
            BetPointsArrayDT = DragonTigerManager.Instance.BetPointBtnsDT;
            PointBetAmount pba = BetPointsArrayDT[TargetPoint].gameObject.GetComponent<PointBetAmount>();
            pba.SetBetAmount(BetAmount, photonView.IsMine);

            // GetComponent<RectTransform>().position = UnityEngine.Vector2.zero;
            GetComponent<PlayerCurrentAnim>().PlayShakeAnimation();

            Playeramount();
            if (!isMute)
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
            //Debug.LogError("Chips adding sound is playing");
        }
        public void SendChipsToEveryOneDT()
        {
            BigInteger mul1 = 2;
            BigInteger mul2 = 12;
            DragonTigerManager dT = DragonTigerManager.Instance;
            GameManager gm = GameManager.Instance;
            UIManager uIManager = UIManager.Instance;
            int LuckyNumber = dT.WinningNumber;
            for (int i = 0; i < gm.playersList.Count; i++)
            {
                for (int j = 0; j < dT.AllCreatedChips.Count; j++)
                {
                    WingoChipDetail wcd = dT.AllCreatedChips[j].GetComponent<WingoChipDetail>();
                    if (gm.playersList[i].photonView.ViewID == wcd.ViewID)
                    {

                        if (LuckyNumber == 0)
                        {
                            if (wcd.PointNumber == 0)
                            {

                                if (uIManager.GetMyPlayerInfo().photonView.ViewID == wcd.ViewID)
                                {
                                    BigInteger amount = wcd.BetAmount * mul1;
                                    dT.TotalRewardOnWin += (BigInteger)(amount);
                                    //wm.TotalRewardOnWin += (BigInteger)(wcd.BetAmount * mul2);
                                }
                                wcd.IsGoToPlayer = true;

                                //After When Enter more than 5 Players
                                if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
                                    PlayAnimation(dT.AllCreatedChips[j].transform, gm.shakeAnimationWhenPositionAvailFul.position, 0.75f);
                                else
                                    PlayAnimation(dT.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);
                            }
                        }
                        else if (wcd.PointNumber == 1)
                        {
                            if (LuckyNumber == 1)
                            {
                                if (uIManager.GetMyPlayerInfo().photonView.ViewID == wcd.ViewID)
                                {
                                    BigInteger amount = wcd.BetAmount * mul2;
                                    dT.TotalRewardOnWin += (BigInteger)(amount);
                                    //wm.TotalRewardOnWin += (BigInteger)(wcd.BetAmount * mul1);
                                }
                                wcd.IsGoToPlayer = true;

                                if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
                                    PlayAnimation(dT.AllCreatedChips[j].transform, gm.shakeAnimationWhenPositionAvailFul.position, 0.75f);
                                else
                                    PlayAnimation(dT.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);
                            }
                        }
                        else if (LuckyNumber == 2)
                        {
                            if (wcd.PointNumber == 2)
                            {

                                if (uIManager.GetMyPlayerInfo().photonView.ViewID == wcd.ViewID)
                                {
                                    BigInteger amount = wcd.BetAmount * mul1;
                                    dT.TotalRewardOnWin += (BigInteger)(amount);
                                }
                                wcd.IsGoToPlayer = true;

                                //After When Enter more than 5 Players
                                if (GetMineIndexInPlayerList() >= GameManager.Instance.position_availability.Length)
                                    PlayAnimation(dT.AllCreatedChips[j].transform, gm.shakeAnimationWhenPositionAvailFul.position, 0.75f);
                                else
                                    PlayAnimation(dT.AllCreatedChips[j].transform, gm.playersList[i].transform.position, 0.75f);
                            }
                        }
                    }
                }
            }

            if (DragonTigerManager.Instance.TotalRewardOnWin > 0)
            {
                //Debug.LogError("Check Winner Lucky Number..." + LuckyNumber);
                photonView.RPC(nameof(ShowWinAmountRPCDT), RpcTarget.All, DragonTigerManager.Instance.TotalRewardOnWin.ToString());
            }
            StartCoroutine(waitForLoseChipGoneToDealerDT(dT));
        }



        IEnumerator waitForLoseChipGoneToDealerDT(DragonTigerManager dT)
        {
            dT = DragonTigerManager.Instance;
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < dT.AllCreatedChips.Count; i++)
            {
                if (!dT.AllCreatedChips[i].GetComponent<WingoChipDetail>().IsGoToPlayer)
                    PlayAnimation(dT.AllCreatedChips[i].transform, dT.ChipsHome.position, 0.75f);
            }
        }

        [PunRPC]
        public void ShowWinAmountRPCDT(string TotalWinAmountString)
        {
            BigInteger TotalWinAmount = BigInteger.Parse(TotalWinAmountString);
            WinningIndicator.SetActive(true);
            WingoWinCashTxt.text = "+" + LocalSettings.Rs(TotalWinAmount);
            WingoWinCashTxt.transform.parent.gameObject.SetActive(true);

            //Debug.LogError("here is your Player:....." + TotalWinAmount); ;
        }



        public void MyTotalCashTextUpdate()
        {
            photonView.RPC(nameof(cashCheckOfAllPlayers), RpcTarget.All);
        }
        [PunRPC]
        void cashCheckOfAllPlayers()
        {
            StartCoroutine(GetTotalCash());
            //playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));
        }

        IEnumerator GetTotalCash()
        {
            yield return new WaitForSeconds(1f);
            playerTotalCash.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey));


            //  Debug.LogError(LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey)));
        }

        #endregion

        #region Poker Section



        public void UpdateMessage(string message)
        {
            PokerInfoMessage.text = message;
            BetStatusObj.SetActive(true);
            Invoke(nameof(StatusToFalse), 2f);
        }

        public void UpdateHandRankLabel(string message, int rank, int scores)
        {
            photonView.RPC(nameof(UpdatingHandRankLabelOnNextwork), RpcTarget.All, message, rank, scores);
        }

        [PunRPC]
        void UpdatingHandRankLabelOnNextwork(string message, int rank, int scores)
        {
            HandRankLabelTxt.text = message;
            PokerPlayerCurrentRank = rank;
            PokerScores = scores;
        }

        void StatusToFalse()
        {
            BetStatusObj.SetActive(false);
        }

        public void BetStartAmountBet(BigInteger pokerCurrentBetAmount, string viewID)
        {
            photonView.RPC(nameof(BetStartAmountBetRPC), RpcTarget.All, LocalSettings.BigIntegerToString(pokerCurrentBetAmount), viewID);
        }


        [PunRPC]
        public void BetStartAmountBetRPC(string pokerCurrentBetAmount, string ViewID)
        {
            BigInteger betAmount = LocalSettings.StringToBigInteger(pokerCurrentBetAmount);
            int viewID = int.Parse(ViewID);
            //Debug.LogError("Check View ID...1..." + viewID + "...Check AMount...." + betAmount);

            if (viewID == UIManager.Instance.GetMyPlayerInfo().photonView.ViewID)
            {
                //Debug.LogError("Check View ID...2..." + viewID + "...Check AMount...." + betAmount);
                LocalSettings.SetPokerBuyInChips(-betAmount);
                PokerTotalCash = LocalSettings.GetPokerBuyInChips();
                PokerTotalCashTxt.text = LocalSettings.Rs(PokerTotalCash);
                // Debug.LogError("4.....Check Here For Poker Cash....." + PokerTotalCashTxt.text);
                player.SetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey, LocalSettings.GetPokerBuyInChips());

            }

            //foreach (var item in PlayerStateManager.Instance.PlayingList)
            //{
            //    if (item.photonView.ViewID == viewID)
            //    {

            //        if (player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey) != null)
            //        {
            //            PokerTotalCash = player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey);
            //            PokerTotalCashTxt.text = LocalSettings.Rs(PokerTotalCash);
            //        }
            //    }
            //}
            StartCoroutine(UpdateCash(viewID));

        }

        IEnumerator UpdateCash(int viewID)
        {
            yield return new WaitForSeconds(0.5f);
            foreach (var item in PlayerStateManager.Instance.PlayingList)
            {
                if (item.photonView.ViewID == viewID)
                {

                    if (player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey) != null)
                    {
                        PokerTotalCash = player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey);
                        PokerTotalCashTxt.text = LocalSettings.Rs(PokerTotalCash);
                        // Debug.LogError("6.....Check Here For Poker Cash....." + PokerTotalCashTxt.text);
                    }
                }
            }
        }

        public void SendPokerBet(BigInteger pokerCurrentBetAmount, bool isAllIn)
        {
            photonView.RPC(nameof(SendPokerBetRPC), RpcTarget.All, LocalSettings.BigIntegerToString(pokerCurrentBetAmount), isAllIn);
        }
        [PunRPC]
        public void SendPokerBetRPC(string pokerCurrentBetAmountString, bool isAllIn)
        {

            BigInteger pokerCurrentBetAmount = LocalSettings.StringToBigInteger(pokerCurrentBetAmountString);
            pokerTotalBetCash += pokerCurrentBetAmount;
            if (photonView.IsMine)
                UIManager.Instance.TotalBetPlacedAmount += pokerCurrentBetAmount;
            PokerActionPanel.Instance.BetPlacedAmount(pokerCurrentBetAmount);
            PokerTotalWholeBetAmount += pokerCurrentBetAmount;
            PokerHistory.Instance.SetHistoryBetAmountForEachPlayer(photonView.ViewID, PokerTotalWholeBetAmount);
            if (pokerCurrentBetAmount > 0)
            {

                GameObject betAnimation = Instantiate(PokerBetAmountAnimPrefab);
                betAnimation.SetActive(true);
                LocalSettings.SetPosAndRect(betAnimation, PokerBetAmountAnimPrefab.GetComponent<RectTransform>(), PokerBetAmountAnimPrefab.transform.parent);
                if (pokerBetAmountAnim == null)
                {
                    pokerBetAmountAnim = betAnimation;

                    betAnimation.GetComponent<PokerBetAmountAnim>().PlayAnimation(FirstTargetPokerBetAmount, false, pokerCurrentBetAmount);
                }
                else
                {
                    betAnimation.GetComponent<PokerBetAmountAnim>().PlayAnimation(pokerBetAmountAnim.transform, false, pokerCurrentBetAmount);
                }
            }

            if (photonView.IsMine)
            {
                PokerTotalCash = LocalSettings.GetPokerBuyInChips();
                //Debug.LogError("Check Animation on Start.....1");
            }
            else
            {
                //Debug.LogError("Check Animation on Start.....2");
                PokerTotalCash = player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey);
            }

            PokerTotalCashTxt.text = LocalSettings.Rs(PokerTotalCash);
            // Debug.LogError("7.....Check Here For Poker Cash....." + PokerTotalCashTxt.text);
            // Debug.LogError("first check -------------- 1");
            SetCurrentTargetBetAmount();
            CheckAllIncash(isAllIn);
            if (PokerActionPanel.Instance.checkPockeBetPlaced())//isCircleCheckFlag)
            {
                if ((checkForAllPlayersbetAreEqual() || PokerActionPanel.Instance.CheckBoolAllIN()) && PokerActionPanel.Instance.checkPockeBetPlaced())
                    StartCoroutine(BetsGoToFinalPoints(1.5f));
            }
            else
                CheckIfNoOneHasCircleFlag();

        }




        public bool checkForAllPlayersbetAreEqual()
        {
            for (int i = 0; i < PlayerStateManager.Instance.PlayingList.Count; i++)
            {
                //if (pokerTotalBetCash != PlayerStateManager.Instance.PlayingList[i].pokerTotalBetCash)
                //    return false;

                //if (CheckBoolAllIN())
                //    return true;
                if (currentPlayerStateRef.currentState == PlayerState.STATE.Packed || currentPlayerStateRef.currentState == PlayerState.STATE.OutOfTable)
                {
                    if (pokerTotalBetCash < PlayerStateManager.Instance.PlayingList[i].pokerTotalBetCash)
                        return true;
                }


                if (pokerTotalBetCash != PlayerStateManager.Instance.PlayingList[i].pokerTotalBetCash)
                    return false;


                // There will be check if all in bet placed
            }
            return true;
        }
        void CheckIfNoOneHasCircleFlag()
        {
            bool isThereNoCircleFlag = true;
            foreach (PlayerInfo pinfo in PlayerStateManager.Instance.PlayingList)
            {
                if (pinfo.isCircleCheckFlag)
                {
                    isThereNoCircleFlag = false;
                }
            }
            if (isThereNoCircleFlag)
                isCircleCheckFlag = true;
        }



        public void SendBetToThePot()
        {

            // photonView.RPC(nameof(BetGoToThePot), RpcTarget.All);
        }


        //[PunRPC]

        public void PokerBetGoToThePot()
        {
            StartCoroutine(BetsGoToFinalPoints(0.2f));

        }

        public void BetGoToFinalPointsForPacked(float delay)
        {
            StartCoroutine(BetsGoToFinalPoints(delay));
        }

        /// <summary>
        /// Just Checking List of Amjad
        /// </summary>
        [ShowOnly]
        public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

        IEnumerator BetsGoToFinalPoints(float DelayTime)
        {
            // Check bets for Alla players
            PlayerStateManager PSM = PlayerStateManager.Instance;

            if (isAllInCashBet)
            {

                BigInteger pokerAllInTotalBetCash = PSM.PlayingList
             .Min(info => info.pokerTotalBetCash);

                for (int i = 0; i < PSM.PlayingList.Count; i++)
                {
                    if (pokerAllInTotalBetCash < PSM.PlayingList[i].pokerTotalBetCash)
                    {
                        BigInteger RemainingAmount = PSM.PlayingList[i].pokerTotalBetCash - pokerAllInTotalBetCash;
                        PSM.PlayingList[i].pokerTotalBetCash = pokerAllInTotalBetCash;
                        PokerActionPanel.Instance.BetPlacedAmount(-RemainingAmount);
                        Pot.instance.PotTxt.text = LocalSettings.Rs(PokerActionPanel.Instance.TotalPotAmount());
                        if (PSM.PlayingList[i].photonView.IsMine)
                        {
                            LocalSettings.SetPokerBuyInChips(RemainingAmount);

                            PSM.PlayingList[i].PokerTotalCash = LocalSettings.GetPokerBuyInChips();
                            PSM.PlayingList[i].player.SetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey, LocalSettings.GetPokerBuyInChips());
                            PSM.PlayingList[i].PokerTotalCashTxt.text = LocalSettings.Rs(LocalSettings.GetPokerBuyInChips());

                            Debug.LogError("Check Here player Status   " + LocalSettings.Rs(LocalSettings.GetPokerBuyInChips()));
                        }
                        else
                        {
                            StartCoroutine(GetPropertyBuyInChips());
                        }

                    }
                }
            }

            //IEnumerator GetPropertyBuyInChipsForReturnCash()
            //{
            //    if (PhotonNetwork.IsConnectedAndReady)
            //    {
            //        yield return new WaitUntil(() => player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey) > 0);
            //        if (!photonView.IsMine)
            //            PokerTotalCashTxt.text = LocalSettings.Rs(player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey));
            //        //Debug.LogError("3.....Check Here For Poker Cash....." + PokerTotalCashTxt.text);
            //    }
            //}

            // Disable the poker action panel for everyone and show the cards
            foreach (PlayerInfo pinfo in PlayerStateManager.Instance.PlayingList)
            {
                pinfo.pokerTotalBetCash = 0;

            }
            yield return new WaitForSeconds(DelayTime);

            PokerActionPanel.Instance.AllPokerBetsGoToFinalPoint();

            //Debug.LogError("packed State....3...");

            // Debug.LogError("Second check -------------- 2");
            SetCurrentTargetBetAmount();
        }

        public void SetCircleFlatPoker()
        {
            photonView.RPC(nameof(SetCircleFlagPokerRPC), RpcTarget.All);
        }
        [PunRPC]
        public void SetCircleFlagPokerRPC()
        {
            isCircleCheckFlag = true;
        }
        void SetCurrentTargetBetAmount()
        {
            if (PokerActionPanel.Instance.CurrentTargetBetAmount < pokerTotalBetCash)
                PokerActionPanel.Instance.CurrentTargetBetAmount = pokerTotalBetCash;
            //Debug.LogError("packed State....4...");
            PokerActionPanel.Instance.SetMinBetAmount();



        }

        public void GiveTurnToNextPlayerPocker()
        {
            photonView.RPC(nameof(GiveTurnToNextPlayer), RpcTarget.All);
        }

        [PunRPC]
        void GiveTurnToNextPlayer()
        {
            if (photonView.IsMine)
            {
                int myIndex = PlayerStateManager.Instance.SideShowNext();
                //if (checkPockeBetPlaced())
                //    photonView.RPC(nameof(resetBoolValue), RpcTarget.All);
                PlayerStateManager.Instance.PlayingList[myIndex].currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
            }
        }
        void CheckAllIncash(bool isAllIn)
        {
            if (isAllIn)
            {
                isAllInCashBet = true;
                CashAllInIndicator.SetActive(true);
            }
        }




        public void myPockerTurnComplete(bool isTrue)
        {
            photonView.RPC(nameof(placedMyBet), RpcTarget.All, isTrue);
        }

        [PunRPC]
        void placedMyBet(bool istrue)
        {
            isBetPlacedPocker = istrue;
        }
        public void ResetIsBetPlacedPocker()
        {
            photonView.RPC(nameof(resetBoolValue), RpcTarget.All);
        }
        [PunRPC]
        public void resetBoolValue()
        {
            foreach (PlayerInfo item in PlayerStateManager.Instance.PlayingList)
            {
                item.isBetPlacedPocker = false;
                //PokerActionPanel.Instance.callBetPockerBtn.gameObject.SetActive()
            }

        }

        IEnumerator GiveOrgPokersCardsIfNotInGame(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (player.GetPlayerStateProperty(LocalSettings.playerState) != PlayerState.STATE.OutOfGame)
            {
                GameManager gameManager = GameManager.Instance;

                int card1Index = player.GetCustomData(LocalSettings.pokerHoleCard1ForPlayer);
                int card2Index = player.GetCustomData(LocalSettings.pokerHoleCard2ForPlayer);

                yield return new WaitForSeconds(0.2f);
                GameObject hole_card1 = Instantiate(GameManager.Instance.AllCards.Card[card1Index].gameObject);
                GameObject hole_card2 = Instantiate(GameManager.Instance.AllCards.Card[card2Index].gameObject);
                PokerCheckWinner.Instance.HoleCardsToDestroy.Add(hole_card1);
                PokerCheckWinner.Instance.HoleCardsToDestroy.Add(hole_card2);
                LocalSettings.SetPosAndRect(hole_card1, card_1_RectTr, card_1_RectTr.parent);
                LocalSettings.SetPosAndRect(hole_card2, card_2_RectTr, card_2_RectTr.parent);

                Hole_Card1 = hole_card1.GetComponent<CardProperty>();
                Hole_Card2 = hole_card2.GetComponent<CardProperty>();
                if (RoomStateManager.Instance.GetCurrentRoomState() == RoomState.STATE.WaitingForResults || RoomStateManager.Instance.GetCurrentRoomState() == RoomState.STATE.ShowingResults)
                {
                    Hole_Card1.gameObject.SetActive(true);
                    Hole_Card2.gameObject.SetActive(true);
                }
                else
                {
                    Hole_Card1.gameObject.SetActive(false);
                    Hole_Card2.gameObject.SetActive(false);
                    DummyCardsParent.transform.GetChild(0).gameObject.SetActive(true);
                    DummyCardsParent.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }

        public void updateHighRankCardsPoker(int[] cardIndexArray)
        {
            string aa = "";
            foreach (int item in cardIndexArray)
            {
                aa = aa + item + " -- ";
            }
            // Debug.LogError(photonView.Controller.NickName + ":    " + aa);
            photonView.RPC(nameof(updateHighRankCardsPokerRPC), RpcTarget.All, cardIndexArray);
        }
        [PunRPC]
        public void updateHighRankCardsPokerRPC(int[] cardIndexArray)
        {
            if (HighRankingCards.Length == 0)
            {
                HighRankingCards = new CardProperty[5];
            }
            for (int i = 0; i < cardIndexArray.Length; i++)
            {
                HighRankingCards[i] = GetCard(cardIndexArray[i]);
            }
        }

        CardProperty GetCard(int cardIndex)
        {
            if (Hole_Card1.CardIndexInArray == cardIndex)
                return Hole_Card1;
            if (Hole_Card2.CardIndexInArray == cardIndex)
                return Hole_Card2;
            PokerCheckWinner pcw = PokerCheckWinner.Instance;
            for (int i = 0; i < pcw.Community_Cards.Length; i++)
            {
                if (pcw.Community_Cards[i].CardIndexInArray == cardIndex)
                    return pcw.Community_Cards[i];
            }
            return null;
        }
        #endregion



        #region API For Profile Image
        Action<Sprite> SetProfileImage;
        Sprite ProfilePic;
        public void RetrieveImageFromDB(string imagePath)
        {
            StartCoroutine(DownloadImageAndConvertToSprite(APIStrings.ImageURLAPI + imagePath, SetProfileImage));
        }


        private IEnumerator DownloadImageAndConvertToSprite(string imageUrl, Action<Sprite> onCompleteMethod)
        {
            WWW www = new WWW(imageUrl); // Start downloading the image

            yield return www; // Wait for the download to complete

            if (!string.IsNullOrEmpty(www.error))
            {
                StartCoroutine(waitforLoadPlayerAvatar());
                Debug.LogError("Error downloading image: " + www.error);
                yield break;
            }

            Texture2D texture = www.texture; // Get the downloaded texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), UnityEngine.Vector2.one * 0.5f);

            //imageDisplay.sprite = sprite; // Set the sprite on the Image component

            onCompleteMethod.Invoke(sprite);
        }

        public void UpdateProfileImageAfterReceivingFromServer(Sprite sprite)
        {

            ProfilePic = sprite;

        }
        #endregion
    }
}