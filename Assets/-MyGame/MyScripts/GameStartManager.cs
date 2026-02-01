using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using ES3Types;
using System.Numerics;
//using static UnityEditor.PlayerSettings;

namespace com.mani.muzamil.amjad
{
    public class GameStartManager : MonoBehaviourPunCallBacksWithNSSCallBacks
    {
        [ShowOnly]
        public float startWaitTime;
        public float waitTimeBeforeGame;
        [ShowOnly]
        public float sideShowCount;

        public GameObject GameStarWaitTextGameObject;


        public TMP_Text GameStartWaitText;
        public TMP_Text sideShowTimeText;

        [ShowOnly]
        public bool MinimumPlayerSatisfied;
        [ShowOnly]
        public bool GameIsGoingToStart;
        [ShowOnly]
        public bool IsGameStartingState;
        [ShowOnly]
        public bool _1stCurrentChaalBool = true;

        public PlayerTurnManager playerTurnManager;

        [ShowOnly]
        public int _currentNumberOfPlayers;

        #region Creating Instance
        private static GameStartManager _instance;
        public static GameStartManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GameStartManager>();
                return _instance;
            }
        }
        #endregion
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        #region Add_Or_Remove_Player
        public void AddOrRemovePlayer(int plyr)
        {
            _currentNumberOfPlayers += plyr;
            if (_currentNumberOfPlayers <= 1)
            {
                if (MatchHandler.IsTeenPatti())
                {
                    if (RoomStateManager.Instance.GetIsNotInStartedState())
                        GameResetManager.Instance.ResetGameTeenPatti();
                }
                else if (MatchHandler.IsAndarBahar() && _currentNumberOfPlayers <= 0)
                {
                    GameResetManager.Instance.ResetABGame();
                }
                else if (MatchHandler.IsPoker())
                {
                    if (RoomStateManager.Instance.GetIsNotInStartedState())
                        GameResetManager.Instance.ResetGamePoker();
                }
                else if (MatchHandler.IsLuckyWar() && _currentNumberOfPlayers <= 0)
                {
                    GameResetManager.Instance.ResetLWGame();

                }

                if (PhotonNetwork.IsMasterClient)
                {
                    if (MatchHandler.IsTeenPatti())
                        startWaitTime = LocalSettings.GameStartWaitTime;
                    else if (MatchHandler.IsAndarBahar())
                        startWaitTime = LocalSettings.GameStartWaitTimeAndarBahar;
                    else if (MatchHandler.isWingoLottary())
                        startWaitTime = LocalSettings.GameStartWaitTimeWingoLottery;
                    else if (MatchHandler.IsPoker())
                        startWaitTime = LocalSettings.GameStartWaitTimePoker;
                    else if (MatchHandler.IsLuckyWar())
                        startWaitTime = LocalSettings.GameStartWaitTimePoker;
                    else if (MatchHandler.isDragonTiger())
                        startWaitTime = LocalSettings.GameStartWaitTimeDragonTiger;
                }

            }
            else
            {
                if (RoomStateManager.Instance.CurrentRoomState != RoomState.STATE.GameIsPlaying)
                    ResetTxt();
            }
        }
        void ResetTxt()
        {
            if (_currentNumberOfPlayers <= 1)
            {
                if (!MatchHandler.isWingoLottary() && !MatchHandler.isDragonTiger())
                {
                    GameStarWaitTextGameObject.SetActive(true);

                    GameStartWaitText.text = "Waiting For Other Players";
                    Pot.instance.PotTxt.text = "";
                }
            }
            else
            {
                GameStartWaitText.text = "";

                //Pot.instance.SetCashText("");
            }

        }


        #endregion


        private void Start()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (_currentNumberOfPlayers < LocalSettings.GetMinPlayers())
                {
                    RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.WaitingForPlayers);
                }
            }
        }

        private void Update()
        {

            LocalSettings.GamePlayTimeCount += Time.deltaTime;


            if (PhotonNetwork.IsMasterClient && !MinimumPlayerSatisfied)
            {
                CheckingMinimumPlayersRequired();
            }

            if (PhotonNetwork.IsMasterClient && GameIsGoingToStart)
            {
                if (!IsGameStartingState)
                {
                    IsGameStartingState = true;
                    if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.WaitingForPlayers)
                        RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsStarting);

                }
                OtherPlayersWait();
            }
            sideShowTimeCounter();
        }

        void sideShowTimeCounter()
        {
            if (UIManager.Instance.GetMyPlayerInfo() == null)
                return;
            if (UIManager.Instance.GetMyPlayerInfo().getCurrentPlayerState().currentState == PlayerState.STATE.RecieverSideShow)
            {
                Debug.Log("Check It Brother");
                sideShowCount -= Time.deltaTime;
                sideShowTimeText.text = Mathf.RoundToInt(sideShowCount) + "S".ToString();
                if (sideShowCount <= 0)
                {
                    UIManager.Instance.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.WaitingForTurn);
                    Game_Play.Instance.OnClickCancelSideShowBtn();
                }

            }

        }


        public void OtherPlayersWait()
        {
            if (_currentNumberOfPlayers < LocalSettings.GetMinPlayers())
            {
                MinimumPlayerSatisfied = false;
                GameIsGoingToStart = false;
                return;
            }
            startWaitTime -= Time.deltaTime;
            photonView.RPC(nameof(WaitForOtherPlayersToJoinBeforeStart), RpcTarget.All, startWaitTime);
        }

        void CheckingMinimumPlayersRequired()
        {

            if (_currentNumberOfPlayers < LocalSettings.GetMinPlayers())
            {
                GameStarWaitTextGameObject.SetActive(true);
                GameStartWaitText.text = "Waiting For Other Players";
                Pot.instance.SetCashText("");
            }
            else
            {

                MinimumPlayerSatisfied = true;
                GameIsGoingToStart = true;
            }
        }

        //  int Tempnumber = 3; //network_time / 2;
        // int Tempnumber2 = -1; //network_time / 2;

        [PunRPC]
        private void WaitForOtherPlayersToJoinBeforeStart(float network_time)
        {
            if (GameIsGoingToStart)
                startWaitTime = network_time;
            else
                startWaitTime = waitTimeBeforeGame;

            if (UIManager.Instance.GetMyPlayerCurrentState() == null)
                return;

            if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
            {
                //Debug.LogError("check for other player entry .... 1");
                if (!MatchHandler.isWingoLottary() && !MatchHandler.isDragonTiger())
                    GameStartWaitText.text = "Game Starting In " + network_time.ToString("0") + " seconds";
                else
                {
                    if (MatchHandler.isWingoLottary())
                    {
                        //Debug.LogError("check for other player entry .... 2");
                        WingoManager wm = WingoManager.Instance;
                        GameStartWaitText.transform.parent.gameObject.SetActive(false);
                        if (network_time > 0.5f)
                        {
                            //if (network_time <= 4 && network_time >= 5)
                            //    Tempnumber = 3;
                            //else if (network_time <= 4 && network_time >= 3)
                            //    Tempnumber = 2;
                            //else if (network_time <= 2 && network_time >= 0)
                            //    Tempnumber = 1;
                            int Tempnumber = Mathf.RoundToInt(network_time) - 1;

                            wm.BetWillStartCount.gameObject.SetActive(true);
                            //wm.BetWillStartCount.text = Tempnumber.ToString("0");
                            wm.BetWillStartCount.text = Tempnumber.ToString("0");
                            GameStartWaitText.text = network_time.ToString("0");
                            // Debug.LogError("value of int...." + network_time, "     ")
                            //  if (Mathf.RoundToInt(network_time) != network_time)
                            //  {
                            //      Tempnumber2 = Tempnumber;
                            // SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ToonSound, false);
                            //  }
                            wm.BetWillEndTxt.text = "Next Hand Starts Only";
                            wm.BetWillEndTxt.transform.parent.gameObject.SetActive(true);
                        }
                        else
                        {
                            //Debug.LogError("betting oepening check...2");

                            wm.BetWillEndTxt.transform.parent.gameObject.SetActive(false);
                            //   Tempnumber2 = -1;
                            wm.BetWillStartCount.gameObject.SetActive(false);
                            wm.BetStartStopTxt.text = "BETTING OPEN...";
                            wm.BetStartStopTxt.gameObject.SetActive(true);
                            // Tempnumber = 3;
                            if (wm.ReBetHistory.Count == 0)
                                wm.rebetBtn.gameObject.SetActive(false);
                            else
                            {
                                if (wm.TotalAmountPlacedOnBet <= LocalSettings.GetTotalChips())
                                    wm.rebetBtn.gameObject.SetActive(true);
                                else
                                    wm.rebetBtn.gameObject.SetActive(false);

                            }
                            wm.TotalAmountPlacedOnBet = 0;
                        }
                    }
                    else if (MatchHandler.isDragonTiger())
                    {
                        //Debug.LogError("check for other player entry .... 3");
                        DragonTigerManager DT = DragonTigerManager.Instance;
                        GameStartWaitText.transform.parent.gameObject.SetActive(false);
                        if (network_time > 0.5f && !UIManager.Instance.GetMyPlayerInfo().iSDragonTigerStart)
                        {
                            //if (network_time <= 6 && network_time >= 5)
                            //    Tempnumber = 3;
                            //else if (network_time <= 4 && network_time >= 3)
                            //    Tempnumber = 2;
                            //else if (network_time <= 2 && network_time >= 0)
                            //    Tempnumber = 1;
                            int Tempnumber = Mathf.RoundToInt(network_time) - 1;

                            DT.BetWillStartCount.gameObject.SetActive(true);
                            //  DT.BetWillStartCount.text = Tempnumber.ToString("0");
                            DT.BetWillStartCount.text = Tempnumber.ToString("0");
                            GameStartWaitText.text = network_time.ToString("0");
                            //  if (Tempnumber != Tempnumber2)
                            //  {
                            //Tempnumber2 = Tempnumber;
                            // SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ToonSound, false);
                            //  }
                            DT.BetWillEndTxt.text = "Next Hand Starts Only";
                            DT.BetWillEndTxt.transform.parent.gameObject.SetActive(true);
                        }
                        else
                        {
                            GameIsGoingToStart = false;
                            DT.BetWillEndTxt.transform.parent.gameObject.SetActive(false);
                            if (!UIManager.Instance.GetMyPlayerInfo().iSDragonTigerStart)
                                DragonTigerManager.Instance.Playanim();
                            //Tempnumber2 = -1;
                            //DT.BetWillStartCount.gameObject.SetActive(false);
                            //DT.BetStartStopTxt.text = "BETTING OPEN...";
                            //DT.BetStartStopTxt.gameObject.SetActive(true);
                            //Tempnumber = 3;
                            //if (DT.ReBetHistory.Count == 0)
                            //    DT.rebetBtn.gameObject.SetActive(false);
                            //else
                            //    DT.rebetBtn.gameObject.SetActive(true);
                        }
                    }
                }
            }

            else
                ResetTxt();

            if (!MatchHandler.isDragonTiger())
                if (network_time < 0 && GameIsGoingToStart)
                {
                    GameIsGoingToStart = false;
                    GameStarted();

                }
        }


        public void CountingSoundForWingow()
        {
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ToonSound, false);
        }


        public void OpenBettingForDT()
        {

            DragonTigerManager DT = DragonTigerManager.Instance;
            //  Tempnumber2 = -1;
            DT.BetWillStartCount.gameObject.SetActive(false);
            // DT.BetStartStopTxt.text = "BETTING OPEN...";
            //DT.BetStartStopTxt.gameObject.SetActive(true);
            //  Tempnumber = 3;
            if (DT.ReBetHistory.Count == 0)
                DT.rebetBtn.gameObject.SetActive(false);
            else
            {
                if (DT.TotalAmountPlacedOnBet <= LocalSettings.GetTotalChips())
                    DT.rebetBtn.gameObject.SetActive(true);
                else
                    DT.rebetBtn.gameObject.SetActive(false);
            }
            DT.TotalAmountPlacedOnBet = 0;
        }

        void GameStarted()
        {
            // if (MatchHandler.IsLuckyWar())
            //    RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsPlaying);
            // else
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.CardDistributing);
        }



        public void ResetWaiting()
        {
            if (PhotonNetwork.IsMasterClient && GameIsGoingToStart)
            {
                MinimumPlayerSatisfied = false;
                GameIsGoingToStart = false;
                if (_currentNumberOfPlayers < LocalSettings.GetMinPlayers())
                {
                    RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.WaitingForPlayers);
                }
            }
        }



        #region Room_Changes_ Call_Backs

        #region Room_Waiting_For_Player_State

        public override void OnRoomStateChangeToWaitingForPlayers()
        {

            GameStartWaitText.text = "Waiting For Other Players";
            if (MatchHandler.IsTeenPatti())
                startWaitTime = LocalSettings.GameStartWaitTime;
            else if (MatchHandler.IsAndarBahar())
                startWaitTime = LocalSettings.GameStartWaitTimeAndarBahar;
            else if (MatchHandler.isWingoLottary())
                startWaitTime = LocalSettings.GameStartWaitTimeWingoLottery;
            else if (MatchHandler.isDragonTiger())
                startWaitTime = LocalSettings.GameStartWaitTimeDragonTiger;
            else if (MatchHandler.IsPoker())
                startWaitTime = LocalSettings.GameStartWaitTimePoker;
            else if (MatchHandler.IsLuckyWar())
                startWaitTime = LocalSettings.GameStartWaitTimeLukyWar;

        }
        #endregion

        #region Room_Card_Distribution_State
        public override void OnRoomStateChangeToCardDistributing()
        {
            GameStarWaitTextGameObject.SetActive(false);
            if (UIManager.Instance.MyLocalPlayer == null)
                return;

            if (MatchHandler.IsTeenPatti())
            {
                CardDistributionDelay();
            }
            else if (MatchHandler.IsAndarBahar())
            {
                AndarBaharPositionsManager.Instance.RandomArrayCards = LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.ab_card_listKey);
                if (PhotonNetwork.IsConnectedAndReady)
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ResetAbleRoom, 1);
                //Debug.LogError("First Card Key: " + LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.abPlayCardKey));
                //AndarBaharPositionsManager.Instance.PlaceFirstCardOverall(LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.abPlayCardKey));
                AndarBaharPositionsManager.Instance.PlaceFirstCardOverall(AndarBaharPositionsManager.Instance.RandomArrayCards[0]);
                if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
                {
                    UIManager uIManager = UIManager.Instance;


                    //Reset the Custom Property keys of Players
                    if (PhotonNetwork.IsConnectedAndReady)
                    {
                        uIManager.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.abAndarAmountKey, 0);
                        uIManager.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.abBaharAmountKey, 0);
                        uIManager.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey, 0);
                    }

                    uIManager.AndarNveBtn.interactable = false;
                    uIManager.AndarPveBtn.interactable = true;
                    uIManager.BaharNveBtn.interactable = false;
                    uIManager.BaharPveBtn.interactable = true;
                    uIManager.AndarBetBtn.interactable = true;
                    uIManager.BaharBetBtn.interactable = true;


                    AndarBaharPositionsManager.Instance.AndarTotalBetAmount = LocalSettings.MinBetAmount;
                    AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount = LocalSettings.MinBetAmount;
                    uIManager.AndarBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.AndarTotalBetAmount);

                    AndarBaharPositionsManager.Instance.BaharTotalBetAmount = LocalSettings.MinBetAmount;
                    uIManager.BaharBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.BaharTotalBetAmount);

                    Game_Play.Instance.ABActionButtonInteractable();
                    AndarBaharPositionsManager.Instance.ABActionPanel.SetActive(true);
                    LocalSettings.Vibrate();
                    PlayerTurnManager.Instance.turnManager.BeginTurn();
                }
                PlayerStateManager.Instance.UpdatePlayingList();
                AndarBaharPositionsManager.Instance.CardDistributingAB();
            }
            else if (MatchHandler.isWingoLottary())
            {
                PlayerTurnManager.Instance.turnManager.BeginTurn();
                RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsPlaying);
                UIManager.Instance.GetMyPlayerInfo().FillerImage.gameObject.SetActive(true);
                playerTurnManager.alarmTime = LocalSettings.RemainingTikTimer;
                if (PhotonNetwork.IsMasterClient)
                {
                    int luckyNumber = Random.Range(0, WingoManager.Instance.Spinner.Length);
                    photonView.RPC(nameof(StartSpinWingoLottary), RpcTarget.All, luckyNumber);
                    if (PhotonNetwork.IsConnectedAndReady)
                        LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.WingoluckyNumberSave, luckyNumber);
                }
                PlayerStateManager.Instance.UpdatePlayingList();
            }
            else if (MatchHandler.IsPoker())
            {
                PlayerStateManager.Instance.UpdatePlayingList();

                foreach (PlayerInfo item in PlayerStateManager.Instance.PlayingList)
                {
                    item.isBetPlacedPocker = false;
                }
                //  Debug.LogError("entering in poker");
                if (PhotonNetwork.IsMasterClient)
                {
                    PokerStatesManager.Instance.UpdateDIndex();
                    PokerManager.Instance.SettingRandomCardsArrayToRoomProperty();
                }


            }
            else if (MatchHandler.IsLuckyWar())
            {

                PlayerStateManager.Instance.UpdatePlayingList();

                if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
                {
                    UIManager.Instance.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.LWTieAmountKey, 0);
                    UIManager.Instance.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.LWBetAmountKey, 0);
                    LuckyWarManager.Instance.BetTotalAmount = LuckyWarManager.Instance.TieTotalAmount = LocalSettings.MinBetAmount;
                    LWActionPanelScript.Instance.TieBetAmountBtnTxt.text = LocalSettings.Rs(LuckyWarManager.Instance.TieTotalAmount);
                    LWActionPanelScript.Instance.BetBetAmountBtnTxt.text = LocalSettings.Rs(LuckyWarManager.Instance.BetTotalAmount);

                    LWActionPanelScript.Instance.betNveBtn.interactable = false;
                    LWActionPanelScript.Instance.EnableDisableBetBtn(true);
                    LWActionPanelScript.Instance.tieNveBtn.interactable = false;
                    LWActionPanelScript.Instance.EnableDisableTieBet(false);
                    if (LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount)
                    {
                        LuckyWarManager.Instance.LWActionPanel.SetActive(false);
                        UIManager.Instance.quickShop.SetActive(true);
                        Game_Play.Instance.StandUp();
                    }
                    else
                    {
                        LocalSettings.Vibrate();
                        LuckyWarManager.Instance.LWActionPanel.SetActive(true);

                    }
                }
                RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsPlaying);

                PlayerTurnManager.Instance.turnManager.BeginTurn();
                LuckyWarManager.Instance.SettingRandomCardsArrayToRoomProperty();


            }
            else if (MatchHandler.isDragonTiger())
            {
                if (_currentNumberOfPlayers < 1)
                {
                    // Call Reset Function
                    GameResetManager.Instance.ResetDragonTigerGame();
                    return;

                }
                PlayerTurnManager.Instance.turnManager.BeginTurn();
                UIManager.Instance.GetMyPlayerInfo().FillerImage.gameObject.SetActive(true);
                playerTurnManager.alarmTime = LocalSettings.RemainingTikTimer;
                PlayerStateManager.Instance.UpdatePlayingList();

                RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsPlaying);
                //if (PhotonNetwork.IsMasterClient)
                //{
                //    int luckyNumber = Random.Range(0, WingoManager.Instance.Spinner.Length);
                //    photonView.RPC(nameof(StartSpinWingoLottary), RpcTarget.All, luckyNumber);
                //    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.WingoluckyNumberSave, luckyNumber);
                //}
            }


            MinimumPlayerSatisfied = true;
            GameIsGoingToStart = false;
            IsGameStartingState = true;
        }

        [PunRPC]
        public void StartSpinWingoLottary(int LuckyNumber)
        {
            WingoManager.Instance.GenerateLottaryNumber(LuckyNumber);
        }

        void CardDistributionDelay()
        {
            PlayerStateManager.Instance.UpdatePlayingList();
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Instance.AssignCardsToAllPlayers();
            }

            GameManager.Instance.InstantiateDummyPlayerCard(_currentNumberOfPlayers);
            _1stCurrentChaalBool = true;

        }

        #endregion

        #region Room_Game_Is_Playing_State
        public override void OnRoomStateChangeToGameIsPlaying()
        {
            if (_1stCurrentChaalBool)
            {
                _1stCurrentChaalBool = false;
                StartTurnManager();
            }
        }

        void StartTurnManager()
        {
            if (UIManager.Instance.GetMyPlayerCurrentState() == null)
                return;


            if (MatchHandler.IsTeenPatti())
                playerTurnManager.turnManager.TurnDuration = LocalSettings.PlayerTurnDuration;
            else if (MatchHandler.IsAndarBahar())
            {
                playerTurnManager.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationAndarBahar;

            }
            else if (MatchHandler.isWingoLottary() && UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
            {
                playerTurnManager.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationWingowLottery;
                LocalSettings.Vibrate();
            }
            else if (MatchHandler.IsPoker() && UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
            {
                playerTurnManager.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationPoker;
            }
            else if (MatchHandler.IsLuckyWar())
            {
                playerTurnManager.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationLuckyWar;
                UIManager.Instance.isPlayerPlayedThisHand = true;

            }
            else if (MatchHandler.isDragonTiger() && UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
            {
                if (_currentNumberOfPlayers < 1)
                {
                    // Call Reset Function
                    GameResetManager.Instance.ResetDragonTigerGame();
                    return;

                }
                playerTurnManager.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationDragonTiger;
            }


            if (MatchHandler.IsTeenPatti())
            {
                if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
                {
                    UIManager.Instance.isPlayerPlayedThisHand = true;
                    UIManager.Instance.GetMyPlayerInfo().GiveChaalAmountOnGameStart();
                    PlayerStateManager.Instance.GetPlayerCardStatus();
                    Pot.instance.PotPanel.SetActive(true);

                }
                StartingChaalAmount();
                if (PhotonNetwork.IsMasterClient)
                {
                    if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.OutOfTable || UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.OutOfGame)
                        PlayerStateManager.Instance.PlayingList[0].currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
                    else
                        UIManager.Instance.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
                }
            }
            else if (MatchHandler.IsPoker())
            {
                // Debug.LogError("I am in Poker");
                if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
                {
                    UIManager.Instance.isPlayerPlayedThisHand = true;
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    if (PokerStatesManager.Instance.GetDIndexRoomProperty() < PlayerStateManager.Instance.PlayingList.Count)
                        DealerToNext();
                    else
                        DealerToNext();
                    // Yahan per hum ny circle men baari deni hy.

                }
                else
                {

                    PlayerStateManager.Instance.PlayingList[PokerStatesManager.Instance.GetDIndexRoomProperty()].Dealer.SetActive(true);
                }
            }

        }
        public void StartingChaalAmount()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                UIManager.Instance.GetMyPlayerInfo().StartPotAmount();
            }
        }
        void DealerToNext()
        {
            PlayerStateManager Psm = PlayerStateManager.Instance;
            int dealerIndex = PokerStatesManager.Instance.GetDIndexRoomProperty();
            Psm.PlayingList[dealerIndex].Dealer.SetActive(true);


            dealerIndex = DealerToNextPlayerTurn(dealerIndex);

            //  SetBetAmountOnDealerpoker(Psm.PlayingList.Count == 2 ? false : true, Psm.PlayingList[dealerIndex].photonView.ViewID);
            //  dealerIndex = DealerToNextPlayerTurn(dealerIndex);
            //  SetBetAmountOnDealerpoker(Psm.PlayingList.Count == 2 ? true : false, Psm.PlayingList[dealerIndex].photonView.ViewID);
            // Old Is Gold F
            PokerActionPanel.Instance.SetBetAmountOnDealer(Psm.PlayingList.Count == 2 ? false : true, Psm.PlayingList[dealerIndex]);
            dealerIndex = DealerToNextPlayerTurn(dealerIndex);
            PokerActionPanel.Instance.SetBetAmountOnDealer(Psm.PlayingList.Count == 2 ? true : false, Psm.PlayingList[dealerIndex]);


            dealerIndex = Psm.PlayingList.Count == 2 ? dealerIndex : DealerToNextPlayerTurn(dealerIndex);

            Psm.PlayingList[dealerIndex].currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
            Psm.PlayingList[dealerIndex].SetCircleFlatPoker();


        }
        int DealerToNextPlayerTurn(int Number)
        {
            Number++;
            if (Number >= PlayerStateManager.Instance.PlayingList.Count)
            {
                Number = 0;
            }
            return Number;
        }

        public void SetBetAmountOnDealerpoker(bool isHalf, int viewID)
        {

            photonView.RPC(nameof(SetBetAmountOnDealerPokerRPC), RpcTarget.All, isHalf, viewID);
        }
        [PunRPC]
        void SetBetAmountOnDealerPokerRPC(bool isHalf, int viewID)
        {

            Debug.LogError("Check View ID...1..." + viewID);
            for (int i = 0; i < PlayerStateManager.Instance.PlayingList.Count; i++)
            {
                PlayerInfo pInfo = PlayerStateManager.Instance.PlayingList[i];
                if (pInfo.photonView.ViewID == viewID)
                {
                    Debug.LogError("Check View ID...2..." + viewID);
                    PokerActionPanel.Instance.SetBetAmountOnDealer(isHalf, pInfo);

                }
            }
            //if (UIManager.Instance.GetMyPlayerInfo().photonView.ViewID == viewID)
            //{
            //}
        }
        #endregion

        #region Room_Waiting_For_Result_State
        public override void OnRoomStateChangeToWaitingForResults(string infoText)
        {
            if (MatchHandler.IsTeenPatti())
            {
                UIManager.Instance.ActionTable.SetActive(false);
                StartCoroutine(GameResultsManager.Instance.ShowResult(LocalSettings.GameResultWaitingTime, infoText));
            }
            else if (MatchHandler.IsPoker())
            {
                if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame && UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
                    UIManager.Instance.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.Watching);

                StartCoroutine(waitForGameShowReseltAndReset(5f));
            }


        }

        IEnumerator waitForGameShowReseltAndReset(float delay)
        {
            yield return new WaitForSeconds(1);

            PokerCheckWinner.Instance.DeclareWinnerOfPoker();
            yield return new WaitForSeconds(1);
            // Debug.LogError("Pocker Check BetAmount" + UIManager.Instance.GetMyPlayerInfo().player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey));
            foreach (var item in PlayerStateManager.Instance.PlayingList)
            {
                item.PokerTotalCashTxt.text = LocalSettings.Rs(item.player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey));
                // Debug.LogError("2.....Check Here For Poker Cash....."  + item.PokerTotalCashTxt.text);

            }

            yield return new WaitForSeconds(delay - 2);
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ShowingResults);
        }
        #endregion

        #region Room_Showing_Result
        public override void OnRoomStateChangeToShowingResults()
        {
            if (MatchHandler.IsTeenPatti())
            {
                GameResultsManager.Instance.ResetAllDataAndNewGamestart();
            }
            else if (MatchHandler.IsAndarBahar())
            {

                AndarBaharPositionsManager.Instance.ResetAllDataAndNewGamestartAB();
            }
            else if (MatchHandler.IsPoker())
            {


                if (PlayerStateManager.Instance.PlayingList.Count < 1)
                {
                    if (GameStartManager.Instance._currentNumberOfPlayers < LocalSettings.GetMinPlayers())
                        RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.WaitingForPlayers);
                    else
                        RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsStarting);
                }
                GameResultsManager.Instance.ResetAllDataAndNewGamestart();
            }
        }

        #endregion

        #region Room_Change_AB_First_Turn_State
        public override void OnRoomStateChangeToABFirstTurn()
        {
            if (MatchHandler.IsLuckyWar())
                LuckyWarManager.Instance.SettingRandomCardsArrayToRoomProperty();
            else if (MatchHandler.IsPoker() && (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame && UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable))
            {
                // Debug.LogError("Check Bool " + CheckPlayerExistOrNot());
                UIManager.Instance.GetMyPlayerInfo().PokerBetGoToThePot();
            }
            else
            {
                PokerActionPanel.Instance.AllPokerBetsGoToFinalPoint();
            }
            //if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame && UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)

        }
        bool CheckPlayerExistOrNot()
        {
            return PlayerStateManager.Instance.PlayingList.Exists(player => player == photonView.IsMine);
        }


        #endregion

        #region Room_change_AB_2nd_Turn_state
        public override void OnRoomStateChangeToABSecondTurn()
        {

        }

        #endregion

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        }
        #endregion



        #region Adding_Or_Subtraction_Event_System_Of_RoomProperty
        private void OnEnable()
        {
            RegisterRoomEvents();
        }

        private void OnDisable()
        {
            UnregisterRoomEvents();
            CancelInvoke("ResetTxt");
        }
        #endregion
    }
}