using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Numerics;
using UnityEngine;

namespace com.mani.muzamil.amjad
{
    public class PlayerTurnManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
    {
        [ShowOnly] public PunTurnManager turnManager;
        PlayerStateManager playerStateManagerInstance;
        GameManager gameManagerInstance;

        public static PlayerTurnManager Instance;
        UIManager uIManager;
        bool IsShowingResults;

        bool isMyTurn = false;
        bool isMyTurnClickAb = true;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            uIManager = UIManager.Instance;
        }
        void Start()
        {

            this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
            this.turnManager.TurnManagerListener = this;
            //if (MatchHandler.isWingoLottary())
            //{
            //    gameObject.SetActive(false);
            //    return;
            //}
            gameManagerInstance = GameManager.Instance;
            playerStateManagerInstance = PlayerStateManager.Instance;
            //this.turnManager.TurnDuration = 8f;
        }

        public void GoToNextTurn()
        {
            if (isMyTurn)
            {
                Debug.Log("On Packed, new begin next turn");
                isMyTurn = false;
                this.turnManager.BeginTurn();
            }
        }

        public void AddChaalAmount()
        {
            BigInteger chalAmount = Pot.instance.CurrentChalAmount;
            if (UIManager.Instance.GetMyPlayerInfo().IsSeen)
                chalAmount = Pot.instance.CurrentChalAmount * 2;
            gameManagerInstance.PlayerTotalChipsUpdate(-chalAmount);
            GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, chalAmount.ToString());
            UIManager.Instance.TotalBetPlacedAmount += chalAmount;
            // Debug.LogError("TotatlbetPlaceAMount   " + UIManager.Instance.TotalBetPlacedAmount);
            UIManager.Instance.GetMyPlayerInfo().AddToPot(chalAmount);
        }


        PlayerInfo GetCurrentPlayingPlayer()
        {
            foreach (PlayerInfo plyrInfo in playerStateManagerInstance.PlayingList)
            {
                if (plyrInfo.getCurrentPlayerState().currentState == PlayerState.STATE.ExecutingTurn)
                {
                    if (!plyrInfo.FillerImage.gameObject.activeInHierarchy)
                    {
                        plyrInfo.FillerImage.gameObject.SetActive(true);
                        PlayerTurnManager.Instance.alarmTime = LocalSettings.RemainingTikTimer;
                    }
                    return plyrInfo;
                }
            }
            return null;
        }
        PlayerInfo currentPlayer;
        public int alarmTime = 6;
        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.CurrentRoom == null)
                return;
            if (MatchHandler.isWingoLottary() || MatchHandler.isDragonTiger())
            {

                if (uIManager.GetMyPlayerInfo() == null)
                    return;
                //else if (uIManager.GetMyPlayerCurrentState().currentState != PlayerState.STATE.AbleToJoin)
                //    return;
            }





            if (RoomStateManager.Instance.CurrentRoomState != RoomState.STATE.GameIsPlaying && RoomStateManager.Instance.CurrentRoomState != RoomState.STATE.ABFirstTurn)
            {
                turnManager.TurnDuration += Time.deltaTime;
                return;
            }


            if (MatchHandler.IsPoker() && PokerActionPanel.Instance.checkPockeBetPlaced())//RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
            {
                // Debug.LogError("Check Here status");
                return;
            }


            //Debug.Log("Turn Time Get" + turnManager.RemainingSecondsInTurn);
            if (MatchHandler.IsAndarBahar())
            {
                if (AndarBaharPositionsManager.Instance.isCardMatched)
                    return;
            }
            else if (MatchHandler.IsTeenPatti() || MatchHandler.IsPoker())
            {
                if (playerStateManagerInstance.PlayingList.Count <= 1)
                    return;
            }

            //if (MatchHandler.IsAndarBahar() && !uIManager.GetMyPlayerInfo().AbTurn)
            //{
            //    turnManager.TurnDuration += Time.deltaTime;
            //    return;
            //}


            if (MatchHandler.IsTeenPatti() || MatchHandler.IsPoker())
            {
                {
                    if (PhotonNetwork.IsConnected && uIManager.DisconnectedPanel.activeSelf == true)
                        uIManager.DisconnectedPanel.SetActive(false);
                    else if (!PhotonNetwork.IsConnected && !uIManager.DisconnectedPanel.activeSelf != true)
                        uIManager.DisconnectedPanel.SetActive(true);

                    if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
                    {
                        if (currentPlayer != null)
                        {
                            if (currentPlayer.getCurrentPlayerState().currentState == PlayerState.STATE.ExecutingTurn)
                            {
                                currentPlayer.FillerImage.fillAmount = turnManager.RemainingSecondsInTurn / turnManager.TurnDuration;
                                PlayTikTimerSoundOnLessTime();
                            }
                            else
                                currentPlayer = GetCurrentPlayingPlayer();
                        }
                        else
                            currentPlayer = GetCurrentPlayingPlayer();


                        if (checkPlayerRunInBackGround(currentPlayer))
                        {
                            if (playerStateManagerInstance.PlayingList.Count > 2)
                            {

                                if (PhotonNetwork.IsMasterClient && !isturnTrue)
                                {
                                    isturnTrue = true;
                                    SetNextPlayerToExecutingTurnPokeRunInBackGround(currentPlayer);
                                    Invoke(nameof(waitForNextTurnBoolOnceTime), 0.3f);
                                }
                            }
                            currentPlayer.currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.OutOfTable);
                        }
                        else
                        {
                            if (turnManager.RemainingSecondsInTurn > 0.11f && MatchHandler.IsPoker())
                            {
                                GameManager.Instance.isRunINBackGround = false;
                            }
                        }

                    }
                }
            }
            else if (MatchHandler.IsAndarBahar())
            {
                if (turnManager.RemainingSecondsInTurn > 0)
                {
                    //Debug.LogError("Aghr ap Out of Game hoty ho to kam krty ho");
                    for (int i = 0; i < playerStateManagerInstance.PlayingList.Count; i++)
                    {

                        if (playerStateManagerInstance.PlayingList[i].AbTurn == false)
                        {
                            //Debug.LogError("Here is your Error");
                            playerStateManagerInstance.PlayingList[i].FillerImage.gameObject.SetActive(false);
                        }
                        else
                        {
                            playerStateManagerInstance.PlayingList[i].FillerImage.fillAmount = turnManager.RemainingSecondsInTurn / turnManager.TurnDuration;
                            if (!playerStateManagerInstance.PlayingList[i].FillerImage.gameObject.activeInHierarchy)
                            {
                                playerStateManagerInstance.PlayingList[i].FillerImage.gameObject.SetActive(true);
                                PlayerTurnManager.Instance.alarmTime = LocalSettings.RemainingTikTimer;
                            }
                            PlayTikTimerSoundOnLessTime(playerStateManagerInstance.PlayingList[i]);
                        }
                        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient)
                            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ABTunDurationSave, Mathf.RoundToInt(turnManager.TurnDuration));
                    }



                }

                StandUpOnTimeEndsForAB();
            }
            else if (MatchHandler.isWingoLottary())
            {
                if (turnManager.RemainingSecondsInTurn > 0)
                {
                    for (int i = 0; i < gameManagerInstance.playersList.Count; i++)
                    {
                        if (gameManagerInstance.playersList[i].photonView.IsMine)
                        {
                            gameManagerInstance.playersList[i].FillerImage.fillAmount = turnManager.RemainingSecondsInTurn / turnManager.TurnDuration;

                            //gameManagerInstance.playersList[i].FillerImage.gameObject.SetActive(true);
                            //PlayerTurnManager.Instance.alarmTime = LocalSettings.RemainingTikTimer;
                            //PlayTikTimerSoundOnLessTime();

                            if (!gameManagerInstance.playersList[i].FillerImage.gameObject.activeInHierarchy)
                            {
                                gameManagerInstance.playersList[i].FillerImage.gameObject.SetActive(true);
                                PlayerTurnManager.Instance.alarmTime = LocalSettings.RemainingTikTimer;
                            }
                            PlayTikTimerSoundOnLessTime(gameManagerInstance.playersList[i]);
                        }
                        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient)
                            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.WingoTunDurationSave, Mathf.RoundToInt(turnManager.TurnDuration));
                        // UIManager.Instance.GetMyPlayerInfo().FillerImage.fillAmount = turnManager.RemainingSecondsInTurn / turnManager.TurnDuration;

                    }

                    WingoManager.Instance.BetWillEnd(turnManager.RemainingSecondsInTurn);
                    WingowLotteryBetStart();
                }

            }
            else if (MatchHandler.IsLuckyWar())
            {
                if (turnManager.RemainingSecondsInTurn > 0)
                {
                    //Debug.LogError("Aghr ap Out of Game hoty ho to kam krty ho");
                    for (int i = 0; i < playerStateManagerInstance.PlayingList.Count; i++)
                    {

                        if (playerStateManagerInstance.PlayingList[i].LWTurn == false)
                        {
                            //Debug.LogError("Here is your Error");
                            playerStateManagerInstance.PlayingList[i].FillerImage.gameObject.SetActive(false);
                        }
                        else
                        {
                            playerStateManagerInstance.PlayingList[i].FillerImage.fillAmount = turnManager.RemainingSecondsInTurn / turnManager.TurnDuration;
                            //playerStateManagerInstance.PlayingList[i].FillerImage.gameObject.SetActive(true);
                            //PlayerTurnManager.Instance.alarmTime = LocalSettings.RemainingTikTimer;

                            if (!playerStateManagerInstance.PlayingList[i].FillerImage.gameObject.activeInHierarchy)
                            {
                                playerStateManagerInstance.PlayingList[i].FillerImage.gameObject.SetActive(true);
                                PlayerTurnManager.Instance.alarmTime = LocalSettings.RemainingTikTimer;
                            }
                            PlayTikTimerSoundOnLessTime(playerStateManagerInstance.PlayingList[i]);

                        }

                        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient)
                            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.LWTunDurationSave, Mathf.RoundToInt(turnManager.TurnDuration));
                    }
                    StandUpOnTimeEndsForLW();
                }
                // StandUp Ontime EndsFor Luky War
            }
            else if (MatchHandler.isDragonTiger())
            {
                if (turnManager.RemainingSecondsInTurn > 0)
                {
                    for (int i = 0; i < gameManagerInstance.playersList.Count; i++)
                    {
                        if (gameManagerInstance.playersList[i].photonView.IsMine)
                        {
                            gameManagerInstance.playersList[i].FillerImage.fillAmount = turnManager.RemainingSecondsInTurn / turnManager.TurnDuration;

                            gameManagerInstance.playersList[i].FillerImage.gameObject.SetActive(true);
                        }
                        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient)
                            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.DragonTigerTurnDurationSave, Mathf.RoundToInt(turnManager.TurnDuration));
                        // UIManager.Instance.GetMyPlayerInfo().FillerImage.fillAmount = turnManager.RemainingSecondsInTurn / turnManager.TurnDuration;

                    }

                    DragonTigerManager.Instance.BetWillEnd(turnManager.RemainingSecondsInTurn);
                    DragonTigerBetStart();
                }

            }


        }

        void PlayTikTimerSoundOnLessTime()
        {
            int remainingMin = (int)(turnManager.RemainingSecondsInTurn);

            if (currentPlayer.photonView.IsMine)
            {
                if (remainingMin < alarmTime && alarmTime > 0)
                {
                    if (remainingMin <= 0)
                        return;
                    // Debug.LogError("playing tik sound    Retime: " + remainingMin + "    Alarm time: " + alarmTime);
                    alarmTime--;
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ClockTikSound, false);
                }
            }
        }
        void PlayTikTimerSoundOnLessTime(PlayerInfo info)
        {
            int remainingMin = (int)(turnManager.RemainingSecondsInTurn);

            if (info.photonView.IsMine)
            {
                if (remainingMin < alarmTime && alarmTime > 0)
                {
                    if (remainingMin <= 0)
                        return;
                    // Debug.LogError("playing tik sound    Retime: " + remainingMin + "    Alarm time: " + alarmTime);
                    alarmTime--;
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ClockTikSound, false);
                }
            }
        }
        void WingowLotteryBetStart()
        {

            if (turnManager.RemainingSecondsInTurn < 0.1 && !WingoManager.Instance.isBetting && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                WingoManager wm = WingoManager.Instance;
                wm.isBetting = true;
                wm.BetWillEndTxt.transform.parent.gameObject.SetActive(false);
                wm.BetStartStopTxt.text = "BETTING ENDED...";
                wm.BetStartStopTxt.gameObject.SetActive(true);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Notification, false);
                wm.rebetBtn.gameObject.SetActive(false);
            }

        }
        void DragonTigerBetStart()
        {

            if (turnManager.RemainingSecondsInTurn < 0.1 && !DragonTigerManager.Instance.isBetting && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                DragonTigerManager dT = DragonTigerManager.Instance;
                dT.isBetting = true;
                dT.BetWillEndTxt.transform.parent.gameObject.SetActive(false);
                dT.BetStartStopTxt.text = "BETTING ENDED...";
                dT.BetStartStopTxt.gameObject.SetActive(true);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Notification, false);
                dT.rebetBtn.gameObject.SetActive(false);
                dT.ShowResultDragonTiger();

            }

        }

        void StandUpOnTimeEndsForLW()
        {

            if (turnManager.RemainingSecondsInTurn < 0.1f && !LWActionPanelScript.Instance.isTurnPass && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying && UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame)
            {

                Game_Play.Instance.StandUp();
                LWActionPanelScript.Instance.isTurnPass = true;
            }
            //else if (turnManager.RemainingSecondsInTurn > 13 && turnManager.RemainingSecondsInTurn < 14.5f && Game_Play.Instance.StandUpFlag && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            //{

            //    Game_Play.Instance.StandUpFlag = false;
            //}

            //if (turnManager.RemainingSecondsInTurn > 5f && turnManager.RemainingSecondsInTurn < 7f)
            //{
            //    Game_Play.Instance.Skip_Bet_TurnManager = true;
            //}
            else
            {
                if (turnManager.RemainingSecondsInTurn < 0.1 && LWActionPanelScript.Instance.isTurnPass && UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame)
                {
                    if (uIManager.GetMyPlayerInfo().TieBetWinLw && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
                    {
                        LWActionPanelScript.Instance.Surrender();


                        Debug.LogError("CheckStandUP of MasterClient Swtich");
                    }
                    else
                    {
                        if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                            LWActionPanelScript.Instance.Deal_Bet();
                    }


                }
            }



        }


        void StandUpOnTimeEndsForAB()
        {

            if (turnManager.RemainingSecondsInTurn < 0.1f && !Game_Play.Instance.StandUpFlag && !Game_Play.Instance.secondTurnTurnAb && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying && UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame)
            {
                Game_Play.Instance.StandUp();
                Game_Play.Instance.StandUpFlag = true;
            }
            else if (turnManager.RemainingSecondsInTurn > 13 && turnManager.RemainingSecondsInTurn < 14.5f && Game_Play.Instance.StandUpFlag && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {

                Game_Play.Instance.StandUpFlag = false;
            }

            if (turnManager.RemainingSecondsInTurn > 5f && turnManager.RemainingSecondsInTurn < 7f)
            {
                Game_Play.Instance.Skip_Bet_TurnManager = true;
            }
            else
            {
                if (turnManager.RemainingSecondsInTurn <= 0 && Game_Play.Instance.secondTurnTurnAb && Game_Play.Instance.Skip_Bet_TurnManager)
                {
                    Game_Play.Instance.Skip_Bet_TurnManager = false;
                    // Debug.LogError("Skipping Turn");
                    Game_Play.Instance.Skip_Bet();
                }
            }


            //if(turnManager.RemainingSecondsInTurn < 0.1f && Game_Play.Instance.secondTurnTurnAb && !firstTurnEndsFlag)
            //{
            //    Debug.LogError("Skipped From Other Side Update");
            //    firstTurnEndsFlag = true;
            //    Game_Play.Instance.Skip_Bet();
            //}

        }

        bool firstTurnEndsFlag;

        #region TurnManager Callbacks
        /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
        public void OnTurnBegins(int turn)
        {
            Debug.Log("OnTurnBegins() turn: " + turn);

            IsShowingResults = false;
        }

        public void OnTurnCompleted(int obj)
        {
            Debug.Log("OnTurnCompleted: " + obj);

            //this.UpdateScores();
            this.OnEndTurn();
        }

        // when a player moved (but did not finish the turn)
        public void OnPlayerMove(Player photonPlayer, int turn, object move)
        {
            Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);
            throw new NotImplementedException();
        }

        // when a player made the last/final move in a turn
        public void OnPlayerFinished(Player photonPlayer, int turn, object move)
        {
            Debug.Log("OnTurnFinished: " + photonPlayer + " turn: " + turn + " action: " + move);

            if (photonPlayer.IsLocal)
            {
                Debug.Log("My Player Ends the Turn");
                //this.localSelection = (Hand)(byte)move;
            }
            else
            {
                Debug.Log("Other Player Ends the Turn");
                //this.remoteSelection = (Hand)(byte)move;
            }
        }
        public void OnTurnTimeEnds(int obj)
        {
            if (RoomStateManager.Instance.GetCurrentRoomState() != RoomState.STATE.GameIsPlaying)
                return;
            if (MatchHandler.IsTeenPatti() || MatchHandler.IsPoker())
            {
                if (currentPlayer != null)
                {
                    if (currentPlayer.photonView.IsMine)
                    {
                        OnTurnCompleted(-1);
                        if (turnManager.Turn > 1)
                        {
                            if (MatchHandler.IsPoker())
                            {
                                if (PokerActionPanel.Instance.checkPokerBtn.activeInHierarchy)
                                {
                                    if (!GameManager.Instance.isRunINBackGround)
                                        PokerActionPanel.Instance.OnClickCheckBtn();
                                    return;
                                }
                            }
                            //    else
                            //    {
                            //        PokerActionPanel.Instance.OnFoldBtnClick();
                            //        Invoke(nameof(Game_Play.Instance.StandUp), 2f);
                            //    }
                            //    Debug.LogError("Sameer Yaki bugat rahe");
                            //}

                            //if (MatchHandler.IsTeenPatti())
                            Game_Play.Instance.StandUp();

                        }

                    }
                }
            }
            else if (MatchHandler.IsAndarBahar())
            {
                //if (Game_Play.Instance.secondTurnTurnAb && UIManager.Instance.GetMyPlayerInfo().AbTurn)
                //{
                //    //Debug.LogError("Skipping Turn");
                //    Game_Play.Instance.Skip_Bet();
                //}
                //turnManager.ResetTurn();
                // Andar bahar turn Time ends
                //if (!Game_Play.Instance.secondTurnTurnAb)
                //{


                //    if (UIManager.Instance.GetMyPlayerInfo().photonView.IsMine)
                //    {
                //        OnTurnCompleted(-1);
                //        Debug.LogError("FaidaKuchNahiHowa");
                //        //if (!Game_Play.Instance.StandUpFlag)
                //        if(uIManager.GetMyPlayerInfo().AbTurn)
                //        {
                //         //   Game_Play.Instance.StandUp();
                //            Game_Play.Instance.StandUpFlag = true;
                //        }

                //        //if (turnManager.Turn > 1)
                //        //{
                //        //}

                //    }

            }


        }
        //private void UpdateScores()
        //{

        //}

        #endregion

        #region Core Gameplay Methods


        /// <summary>Call to start the turn (only the Master Client will send this).</summary>
        public void StartTurn()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                this.turnManager.BeginTurn();
            }
        }

        /*    public void MakeTurn(Hand selection)
            {
                this.turnManager.SendMove((byte)selection, true);
            }*/

        public void OnEndTurn()
        {
            //this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
        }
        //public IEnumerator ShowResultsBeginNextTurnCoroutine()
        //{
        //    IsShowingResults = true;

        //    yield return new WaitForSeconds(1.0f);

        //    this.StartTurn();
        //}
        public void EndGame()
        {
            Debug.Log("EndGame");
        }

        //private void UpdatePlayerTexts()
        //{
        //    Player remote = PhotonNetwork.LocalPlayer.GetNext();
        //    Player local = PhotonNetwork.LocalPlayer;

        //    if (remote != null)
        //    {
        //        // should be this format: "name        00"
        //        //this.RemotePlayerText.text = remote.NickName;
        //        //this.RemotePlayerValueText.text = remote.GetScore().ToString("D2");
        //    }
        //    else
        //    {

        //        //TimerFillImage.anchorMax = new Vector2(0f, 1f);
        //        uIManager.TimeText.text = "";
        //        //this.RemotePlayerText.text = "A espera de um Oponente...";
        //        //this.RemotePlayerValueText.text = "00";
        //    }

        //    if (local != null)
        //    {
        //        // should be this format: "YOU   00"
        //        //this.LocalPlayerText.text = local.GetScore().ToString("D2");
        //    }
        //}
        public void OnClickConnect()
        {
            PhotonNetwork.ConnectUsingSettings();
            // PhotonHandler.StopFallbackSendAckThread();  // this is used in the demo to timeout in background!
        }

        public void OnClickReConnectAndRejoin()
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.ReconnectAndRejoin();
            //PhotonHandler.StopFallbackSendAckThread();  // this is used in the demo to timeout in background!
        }
        #endregion

        void RefreshUIViews()
        {
            uIManager.FillerImage.fillAmount = 1;
            //ConnectUiView.gameObject.SetActive(!PhotonNetwork.InRoom);
            //GameUiView.gameObject.SetActive(PhotonNetwork.InRoom);
        }
        public override void OnLeftRoom()
        {
            //Debug.Log("You Left the Room");

            //RefreshUIViews();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("Other Player named " + otherPlayer.NickName + " left this Room");
            base.OnPlayerLeftRoom(otherPlayer);
        }



        public override void OnJoinedRoom()
        {
            RefreshUIViews();

            if (PhotonNetwork.CurrentRoom.Players.Count == 2)
            {
                if (this.turnManager.Turn == 0)
                {
                    // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                    this.StartTurn();
                }
            }
            else
            {
                Debug.Log("Waiting for another player");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.CurrentRoom.Players.Count == 2)
            {
                if (this.turnManager.Turn == 0)
                {
                    // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                    this.StartTurn();
                }
            }
        }



        void SetNextPlayerToExecutingTurnPokeRunInBackGround(PlayerInfo pInfo)
        {

            int nextPlayerInt = PlayerStateManager.Instance.PlayingList.IndexOf(pInfo);
            nextPlayerInt++;
            if (nextPlayerInt >= PlayerStateManager.Instance.PlayingList.Count)
                nextPlayerInt = 0;

            PlayerStateManager.Instance.PlayingList[nextPlayerInt].currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
        }

        bool checkPlayerRunInBackGround(PlayerInfo pInfo)
        {
            return MatchHandler.IsPoker() && turnManager.RemainingSecondsInTurn > 0.05f && turnManager.RemainingSecondsInTurn < 0.1f && pInfo.checkApplicationBackground && pInfo.currentPlayerStateRef.currentState == PlayerState.STATE.ExecutingTurn;
        }

        [ShowOnly] public bool isturnTrue = false;
        void waitForNextTurnBoolOnceTime()
        {
            isturnTrue = false;
        }
    }
}