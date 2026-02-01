using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace com.mani.muzamil.amjad
{
    public class PlayerCurrentState : MonoBehaviourPunCallbacks
    {
        public PlayerState.STATE currentState;

        PlayerInfo playerInfo;
        PlayerCurrentAnim playerAnim;
        UIManager uiManager;
        public bool giveTurnToNext;

        private void OnEnable()
        {
            playerInfo = GetComponent<PlayerInfo>();
            playerAnim = GetComponent<PlayerCurrentAnim>();
            uiManager = UIManager.Instance;
            giveTurnToNext = true;
        }


        int counter = 0;
        void OnUpdateCurrentState(PlayerState.STATE state)
        {
            switch (state)
            {
                case PlayerState.STATE.OutOfGame:
                    TriggerStateOutofGame();
                    break;

                case PlayerState.STATE.Watching:
                    TriggerStateWatching();
                    break;

                case PlayerState.STATE.AbleToJoin:
                    TriggerAbleToJoin();
                    break;

                case PlayerState.STATE.WaitingForTurn:
                    //Debug.LogError("  ex  ------------------- " + counter++);
                    TriggerWaitingForTurn();

                    break;

                case PlayerState.STATE.ExecutingTurn:
                    TriggerStateExecutingTurn();
                    break;

                case PlayerState.STATE.BetPlaced:
                    TriggerStateBetPlaced();
                    break;

                case PlayerState.STATE.Packed:
                    TriggerStatePacked();
                    break;

                case PlayerState.STATE.RecieverSideShow:
                    TriggerStateRecieverSideShow();
                    break;
                case PlayerState.STATE.SenderSideShow:
                    //Debug.LogError("  ex  ------------------- " + counter++);
                    TriggerStateSenderSideShow();

                    break;

                case PlayerState.STATE.OutOfTable:
                    TriggerStateOutOfTable();
                    break;
            }
        }

        public PlayerState.STATE GetCurrentState()
        {
            return currentState;
        }

        public void UpdateCurrentPlayerState(PlayerState.STATE state)
        {
            if (PhotonNetwork.IsConnectedAndReady)
                playerInfo.player.SetPlayerStateProperty(LocalSettings.playerState, state);
            photonView.RPC("UpdateCurrentPlayerStateOnNetwork", RpcTarget.All, state);
        }


        [PunRPC]
        public void UpdateCurrentPlayerStateOnNetwork(PlayerState.STATE state)
        {
            currentState = state;
            OnUpdateCurrentState(state);
        }


        public void TriggerStateOutofGame()
        {
            if (playerInfo == null)
                playerInfo = GetComponent<PlayerInfo>();

            // PlayerStateManager.Instance.PlayingList.Clear();
            GameManager.Instance.UpdateWatchingPlayers();
            playerInfo.PlayerStateText.text = "Out Of Game";
            if (photonView.IsMine)
            {
                if (!MatchHandler.isWingoLottary() && !MatchHandler.isDragonTiger())
                {
                    if (MatchHandler.IsTeenPatti())
                    {
                        BigInteger cash = LocalSettings.GetCurrentRoom.GetTableCollectedCash(LocalSettings.TableCashKey);
                        Pot.instance.SetCashText(cash.ToString());
                        // playerInfo.AddToPotForNewPlayer(0);
                        Pot.instance.PotPanel.SetActive(true);
                        playerInfo.ShowBtn.SetActive(false);

                        playerInfo.SeenIndicator.SetActive(false);
                        playerInfo.BlindIndicator.SetActive(false);
                    }
                    else if (MatchHandler.IsAndarBahar())
                    {
                        AndarBaharPositionsManager.Instance.isNotFirstTurn = LocalSettings.GetCurrentRoom.GetCustomRoomBoolData(LocalSettings.IsNotCardDistribute);
                        if (AndarBaharPositionsManager.Instance.FirstCardRef != null)
                            Destroy(AndarBaharPositionsManager.Instance.FirstCardRef);
                        if (AndarBaharPositionsManager.Instance.andarCardRef != null)
                            Destroy(AndarBaharPositionsManager.Instance.andarCardRef);
                        if (AndarBaharPositionsManager.Instance.BaharCardRef != null)
                            Destroy(AndarBaharPositionsManager.Instance.BaharCardRef);
                        if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                        {

                            //AndarBaharPositionsManager.Instance.PlaceFirstCard();
                            AndarBaharPositionsManager.Instance.RandomArrayCards = LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.ab_card_listKey);
                            AndarBaharPositionsManager.Instance.PlaceFirstCardOverallForOutPlayer(LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.abPlayCardKey));
                            if (AndarBaharPositionsManager.Instance.isNotFirstTurn)
                            {
                                AndarBaharPositionsManager.Instance.PlaceAndarBaharCardForOutPlayer(LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.abBaharCardkey), AndarBaharPositionsManager.Instance.BaharCardPos, LocalSettings.abBaharCardkey);
                                AndarBaharPositionsManager.Instance.PlaceAndarBaharCardForOutPlayer(LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.abAndarCardkey), AndarBaharPositionsManager.Instance.AndarCardPos, LocalSettings.abAndarCardkey);
                            }
                        }
                        AndarBaharPositionsManager.Instance.ABActionPanel.SetActive(false);
                        playerInfo.ABBettingSection.SetActive(false);

                    }
                    else if (MatchHandler.IsLuckyWar())
                    {
                        LuckyWarManager luckyWar = LuckyWarManager.Instance;
                        luckyWar.LWActionPanel.SetActive(false);
                        playerInfo.LWBettingSection.SetActive(false);
                    }
                    else if (MatchHandler.IsPoker())
                    {
                        PokerManager.Instance.PokerRandomArrayCards = LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.Poker_card_listKey);

                        playerInfo.HandRankLabelTxt.transform.parent.gameObject.SetActive(false);

                        if (LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.pokerCommunityCardsArray) != null)
                            PokerManager.Instance.GenerateCommunityCardsForOutOfGame();

                        if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.pockerNumberofdropCummunityCardsKey) > 0 && (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn))
                        {
                            int index = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.pockerNumberofdropCummunityCardsKey);
                            PokerManager.Instance.AssignCummintyCardToOutOFGame(index);
                        }
                    }

                    if (RoomStateManager.Instance.CurrentRoomState != RoomState.STATE.ShowingResults)
                        GameStartManager.Instance.GameStarWaitTextGameObject.SetActive(false);

                    PlayerStateManager.Instance.Amountobject.SetActive(false);
                    PlayerStateManager.Instance.taptoSitHere.SetActive(false);
                    PlayerStateManager.Instance.waitForNextRound.SetActive(true);

                }
                else if (MatchHandler.isWingoLottary())
                {
                    if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                    {
                        PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.WingoTunDurationSave);

                        //WingoManager.Instance.CurrentuvRectValue = LocalSettings.GetCurrentRoom.GetCustomFloatRoomData(LocalSettings.WingoCurrentRemainingTime);
                        WingoManager.Instance.isPlayerEnteredDuringGame = true;

                        WingoManager.Instance.GenerateLottaryNumber(LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.WingoluckyNumberSave));
                    }


                }
                else if (MatchHandler.isDragonTiger())
                {
                    DragonTigerManager dT = DragonTigerManager.Instance;
                    if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                    {
                        PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DragonTigerTurnDurationSave);
                        if ((dT.DTOrgCard0 == null && dT.DTOrgCard1 == null) && LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.DT_card_listKey) != null)
                        {
                            dT.AssingDummyCardPlayerOutOfGame();
                        }

                        //WingoManager.Instance.CurrentuvRectValue = LocalSettings.GetCurrentRoom.GetCustomFloatRoomData(LocalSettings.WingoCurrentRemainingTime);
                        // WingoManager.Instance.isPlayerEnteredDuringGame = true;

                        //WingoManager.Instance.GenerateLottaryNumber(LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.WingoluckyNumberSave));
                    }


                }

            }



            //GameManager gm = GameManager.Instance;
            //for (int i = 0; i < gm.playersList.Count; i++)
            //{
            //    if (gm.playersList[i].GetComponent<PlayerCurrentState>().currentState != PlayerState.STATE.OutOfGame)
            //    {
            //        PlayerStateManager.Instance.PlayingList.Add(gm.playersList[i]);
            //    }
            //}
            if (MatchHandler.IsTeenPatti())
            {
                PlayerStateManager.Instance.GetPlayerCardStatus();
                Debug.Log("-------- Out Of Game State");
            }
            if (MatchHandler.isWingoLottary())
            {
                playerInfo.LWBettingSection.SetActive(false);
            }
        }





        public void TriggerStateWatching()
        {

            if (playerInfo.IsSideShow && photonView.IsMine)
            {
                int nextPlayerInt = PlayerStateManager.Instance.SideShowPrev();

                PlayerStateManager.Instance.PlayingList[nextPlayerInt].ShowCardsFromBlind();
            }
            playerInfo.PlayerStateText.text = "Watching";
            Debug.Log("-------- Watching State");
            //Play 
        }

        public void TriggerAbleToJoin()
        {
            if (playerInfo == null)
                playerInfo = GetComponent<PlayerInfo>();
            if (!MatchHandler.isWingoLottary() && !MatchHandler.isDragonTiger())
            {
                if (!PlayerStateManager.Instance)
                    return;

                PlayerStateManager.Instance.waitForNextRound.SetActive(false);
                PlayerStateManager.Instance.taptoSitHere.SetActive(false);
                if (!MatchHandler.IsPoker())
                    PlayerStateManager.Instance.Amountobject.SetActive(true);
                else
                    playerInfo.FillerImage.gameObject.SetActive(false);
                playerInfo.PlayerStateText.text = "Able To Join";
                if (MatchHandler.IsAndarBahar())
                {
                    playerInfo.AbTurn = true;
                    if (PhotonNetwork.IsConnectedAndReady)
                        playerInfo.player.SetCustomBoolData(LocalSettings.AbturnKey, playerInfo.AbTurn);
                    playerInfo.ABBettingSection.SetActive(true);
                }
                else if (MatchHandler.IsLuckyWar())
                {
                    //Debug.LogError("-------- Able To Join State");
                    playerInfo.LWTurn = true;
                    // playerInfo.TurnOnOffLW(true);
                    playerInfo.player.SetCustomBoolData(LocalSettings.LWturnKey, playerInfo.LWTurn);
                    playerInfo.LWBettingSection.SetActive(true);
                    LuckyWarManager.Instance.DestroyObjectsOnReset();

                }
            }
            GameManager.Instance.UpdateWatchingPlayers();
        }

        public void TriggerWaitingForTurn()
        {
            //if (photonView.IsMine)
            //{
            //    if (uiManager.ActionTable.activeSelf)
            //    {
            //        Debug.LogError("Action panel trouble  1 ----------------------------------------");
            //        uiManager.ActionTable.SetActive(false);
            //    }
            //}
            playerInfo.FillerImage.gameObject.SetActive(false);
            playerInfo.FillerImage.fillAmount = 0;
            Debug.Log("-------- Waiting For Turn State");
            playerInfo.PlayerStateText.text = "Waiting For Turn";

        }


        public void TriggerStateExecutingTurn()
        {
            if (!MatchHandler.IsPoker())
            {
                if (!playerInfo.IsSideShow)
                {
                    StartCoroutine(MethodOfExecutingTurn());
                    if (photonView.IsMine)
                        Debug.Log("-------- Executing Turn State");

                }
                else
                    StartCoroutine(waitSideShowoff());
            }
            else
            {
                if (playerInfo != null)
                {
                    StartCoroutine(MethodOfExecutingTurn());
                    if (photonView.IsMine)
                        Debug.Log("-------- Executing Turn State");

                }
            }

        }
        IEnumerator MethodOfExecutingTurn()
        {
            if (PlayerStateManager.Instance.PlayingList.Count <= 1)
                yield return null;

            if (photonView.IsMine && (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn))
            {

                if (MatchHandler.IsTeenPatti())
                {
                    if (!uiManager.ActionTable.activeSelf)
                    {
                        LocalSettings.Vibrate();
                        uiManager.ActionTable.SetActive(true);
                        PlayerStateManager.Instance.SideShowAndShowbtn();
                        Game_Play.Instance.MyTurnBetAmountLimit();
                        //Debug.LogError("-------- Executing Turn State");
                    }
                }
                else if (MatchHandler.IsPoker())
                {

                    if (PokerActionPanel.Instance.checkPockeBetPlaced())
                    {

                        yield return new WaitUntil(() => PlayerStateManager.Instance.PlayingList.All(x => x.isBetPlacedPocker == false));
                    }

                    //yield return new WaitUntil(() => RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying);


                    if (!PokerActionPanel.Instance.ActionPanelPoker.activeSelf && currentState == PlayerState.STATE.ExecutingTurn && PlayerStateManager.Instance.PlayingList.Count > 1)
                    {

                        PokerActionPanel.Instance.callBetPockerBtn.SetActive(playerInfo.checkForAllPlayersbetAreEqual() ? false : true);
                        PokerActionPanel.Instance.checkPokerBtn.SetActive(!PokerActionPanel.Instance.callBetPockerBtn.activeSelf);


                        PokerActionPanel.Instance.ActionPanelPoker.SetActive(true);
                    }
                    else
                        yield return null;
                }
            }
            else
            {
                if (MatchHandler.IsTeenPatti())
                {
                    if (uiManager.ActionTable.activeSelf)
                    {
                        uiManager.ActionTable.SetActive(false);
                    }
                }
                else if (MatchHandler.IsPoker())
                {

                    if (PokerActionPanel.Instance.ActionPanelPoker.activeSelf)
                    {
                        PokerActionPanel.Instance.ActionPanelPoker.SetActive(false);
                    }
                }
            }

            playerInfo.FillerImage.gameObject.SetActive(true);
            PlayerTurnManager.Instance.alarmTime = LocalSettings.RemainingTikTimer;
            PlayerTurnManager.Instance.turnManager.BeginTurn();
            playerInfo.PlayerStateText.text = "Executing Turn";
        }

        IEnumerator CheckForContinueTurn()
        {
            if (PokerActionPanel.Instance.checkPockeBetPlaced())
                yield return new WaitUntil(() => PlayerStateManager.Instance.PlayingList.All(x => x.isBetPlacedPocker == false));
            //if (!PokerActionPanel.Instance.ActionPanelPoker.activeSelf)
            //{
            //    PokerActionPanel.Instance.ActionPanelPoker.SetActive(true);
            //}
        }

        //public bool checkPockeBetPlaced()
        //{
        //    foreach (PlayerInfo item in PlayerStateManager.Instance.PlayingList)
        //    {
        //        if (item.isBetPlacedPocker == false)
        //            return false;
        //    }
        //    return true;
        //}
        IEnumerator waitSideShowoff()
        {
            while (playerInfo.IsSideShow)
            {
                playerInfo.FillerImage.gameObject.SetActive(false);
                if (uiManager.ActionTable.activeSelf)
                {
                    uiManager.ActionTable.SetActive(false);
                }
                yield return new WaitUntil(() => playerInfo.IsSideShow);
                StartCoroutine(MethodOfExecutingTurn());
            }
        }

        bool checkisSideShow()
        {
            return playerInfo.IsSideShow;
        }

        public void TriggerStateBetPlaced()
        {
            if (MatchHandler.IsTeenPatti())
            {
                Debug.Log("-------- Bet Placed State");
                playerInfo.PlayerStateText.text = "Bet Placed";
                BigInteger chalAmount = Pot.instance.CurrentChalAmount;
                if (playerInfo.IsSeen)
                    chalAmount = Pot.instance.CurrentChalAmount * 2;



                if (LocalSettings.GetTotalChips() >= chalAmount)
                {
                    if (photonView.IsMine)
                    {
                        PlayerTurnManager.Instance.AddChaalAmount();
                    }
                    PlayerTurnManager.Instance.GoToNextTurn();
                    playerInfo.FillerImage.gameObject.SetActive(true);
                    //Debug.LogError("Changing wait for turn ______________");
                    //if (playerInfo.IsSideShow)
                    //    UpdateCurrentPlayerState(PlayerState.STATE.SenderSideShow);
                    //else
                    if (!GameManager.Instance.isRunINBackGround)
                    {
                        UpdateCurrentPlayerState(PlayerState.STATE.WaitingForTurn);
                        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
                    }
                    //Debug.LogError("Win sound is playing");
                }
                else
                {

                    if (photonView.IsMine)
                    {
                        //   UpdateCurrentPlayerState(PlayerState.STATE.Packed);
                        uiManager.quickShop.SetActive(true);
                        Game_Play.Instance.StandUp();
                    }
                }
                if (playerInfo.player.IsLocal)
                    SetNextPlayerToExecutingTurn();
            }
            else if (MatchHandler.IsPoker())
            {
                Debug.Log("-------- Bet Placed State Poker");
                playerInfo.PlayerStateText.text = "Bet Placed Poker";
                //                BigInteger chalAmount = Pot.instance.CurrentChalAmount;
                //                if (playerInfo.IsSeen)
                //                    chalAmount = Pot.instance.CurrentChalAmount * 2;



                if (LocalSettings.GetPokerBuyInChips() >= PokerActionPanel.Instance.MinBetAmount)
                {
                    if (photonView.IsMine)
                    {
                        if (PokerActionPanel.Instance.isSliderBtnClick)
                            PokerActionPanel.Instance.OnRaiseSliderBtnClick();
                        else
                            PokerActionPanel.Instance.OnCallCheckAllInBtnClick();
                    }
                    PlayerTurnManager.Instance.GoToNextTurn();
                    playerInfo.FillerImage.gameObject.SetActive(true);
                    //Debug.LogError("Changing wait for turn ______________");
                    //if (playerInfo.IsSideShow)
                    //    UpdateCurrentPlayerState(PlayerState.STATE.SenderSideShow);
                    //else
                    if (!GameManager.Instance.isRunINBackGround)
                    {
                        UpdateCurrentPlayerState(PlayerState.STATE.WaitingForTurn);
                        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipAdding, false);
                    }
                    //Debug.LogError("Win sound is playing");
                }
                else
                {

                    if (photonView.IsMine)
                    {
                        //   UpdateCurrentPlayerState(PlayerState.STATE.Packed);
                        uiManager.quickShop.SetActive(false);
                        Game_Play.Instance.StandUp();
                    }
                }
                if (playerInfo.player.IsLocal)
                    SetNextPlayerToExecutingTurn();
            }

        }



        //int nextPlayerInt;
        void SetNextPlayerToExecutingTurn()
        {

            int nextPlayerInt = PlayerStateManager.Instance.PlayingList.IndexOf(playerInfo);
            nextPlayerInt++;
            if (nextPlayerInt >= PlayerStateManager.Instance.PlayingList.Count)
                nextPlayerInt = 0;

            PlayerStateManager.Instance.PlayingList[nextPlayerInt].currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
        }

        public void TriggerStatePacked()
        {
            Debug.Log("-------- Packed State");

            if (MatchHandler.IsTeenPatti())
            {
                if (UIManager.Instance.ActionTable.activeSelf)
                    UIManager.Instance.ActionTable.SetActive(false);

                playerInfo.FillerImage.gameObject.SetActive(false);
                playerAnim.PackAnim();
                playerInfo.PackedText.gameObject.SetActive(true);
                updaePlayerProperties(playerInfo);
                playerInfo.PlayerStateText.text = "Packed";
                playerInfo.PackedText.text = "PACKED";
                if (!playerInfo.IsSideShow)
                    PlayerStateManager.Instance.UpdateListOnPlayerPack();
                if (PlayerStateManager.Instance.PlayingList.Count > 1)
                {
                    if (playerInfo.player.IsLocal && !playerInfo.IsSideShow)
                        SetNextPlayerToExecutingTurn();


                    if (playerInfo.IsSideShow)
                        RoomStateManager.Instance.UpdateCurrentStateOnShowBtn(RoomState.STATE.GameIsPlaying);
                    PlayerStateManager.Instance.UpdateListOnPlayerPack();
                }

                if (PlayerStateManager.Instance.PlayingList.Count <= 1)
                {
                    PlayerStateManager.Instance.RemainingPlayerWonGameAutomatic();


                }




                if (playerInfo.IsSideShow)
                {
                    playerInfo.SideShowIndicatorAnim.SetActive(false);
                    playerInfo.GivesideShowAlertToAll(false);

                }
            }
            else if (MatchHandler.IsPoker())
            {
                if (PokerActionPanel.Instance.ActionPanelPoker.activeSelf)
                    PokerActionPanel.Instance.ActionPanelPoker.SetActive(false);

                playerInfo.FillerImage.gameObject.SetActive(false);
                playerAnim.PackAnim();
                playerInfo.PackedText.gameObject.SetActive(true);
                updaePlayerProperties(playerInfo);
                //playerInfo.PlayerStateText.text = "FOLD";
                playerInfo.PackedText.text = "FOLD";
                if (PlayerStateManager.Instance.PlayingList.Count > 1)
                {
                    if (playerInfo.player.IsLocal && !playerInfo.IsSideShow)
                        SetNextPlayerToExecutingTurn();

                    PlayerStateManager.Instance.UpdateListOnPlayerPack();
                }
                if (PokerActionPanel.Instance.checkPockeBetPlaced() && PlayerStateManager.Instance.PlayingList.Count > 1)//isCircleCheckFlag)
                {
                    Debug.LogError("packed State....1...");
                    if (playerInfo.checkForAllPlayersbetAreEqual() || PokerActionPanel.Instance.CheckBoolAllIN())
                    {
                        Debug.LogError("packed State....2...");
                        playerInfo.BetGoToFinalPointsForPacked(1.5f);
                    }
                }

                if (PlayerStateManager.Instance.PlayingList.Count <= 1)
                {
                    PlayerStateManager.Instance.RemainingPlayerWonGameAutomatic();
                    PokerHistory.Instance.SetHistoryBetAmountForEachPlayer(PlayerStateManager.Instance.PlayingList[0].photonView.ViewID, PokerActionPanel.Instance.TotalPotAmount());
                    PokerManager.Instance.HistoryFinalCall();
                    PokerHistory.Instance.SortPokerRecord();
                }

                PokerManager.Instance.DestroyHoleCards(playerInfo);
                playerInfo.HandRankLabelTxt.transform.parent.gameObject.SetActive(false);
            }
            else if (MatchHandler.IsLuckyWar())
            {
                playerInfo.TurnOnOffLW(false);
                PlayerStateManager.Instance.UpdateListOnPlayerPack();


            }

        }
        public void TriggerStateSenderSideShow()
        {
            //if (photonView.IsMine)
            //{
            //    if (uiManager.ActionTable.activeSelf)
            //    {
            //        Debug.LogError("Action panel trouble  1 ----------------------------------------");
            //        uiManager.ActionTable.SetActive(false);
            //    }
            //}

            playerInfo.SideShowIndicatorAnim.SetActive(true);
            playerInfo.FillerImage.gameObject.SetActive(false);
            Debug.Log("-------- Waiting For Turn State");
            playerInfo.PlayerStateText.text = "Waiting For Turn";

        }
        public void TriggerStateRecieverSideShow()
        {
            Debug.Log("-------- Side Show State");

            if (photonView.IsMine)
            {
                int requestFromSideShowInt = PlayerStateManager.Instance.SideShowNext();
                uiManager.playerName.text = PlayerStateManager.Instance.PlayingList[requestFromSideShowInt].name.ToString();
                GameStartManager.Instance.sideShowCount = LocalSettings.SideShowCountDownTime;
                LocalSettings.Vibrate();
                uiManager.sideShowPanel.SetActive(true);

            }
            playerInfo.SideShowIndicatorAnim.SetActive(true);
            playerInfo.PlayerStateText.text = "Side Show";

        }

        public void TriggerStateOutOfTable()
        {
            if (!MatchHandler.IsPoker())
                gameObject.SetActive(false);
            else
            {
                if (photonView.IsMine)
                {
                    GameManager.Instance.position_availability[0].is_reserved = null;
                    if (playerInfo.player.IsMasterClient)
                        PositionsManager.Instance.ReleasePosition(GameManager.Instance.myLocalSeat);
                    GameManager.Instance.SitHereBtnStatus(true);
                }
            }



            OutOfTableThings();

            playerInfo.PlayerStateText.text = "";
            if (photonView.IsMine)
            {

                PositionsManager.Instance.ReArrangePlayerSeatsAccordingToNetworkPositions();
            }
            if (MatchHandler.IsPoker())
                Invoke(nameof(GameObjecSetActiveFalse), 1f);
            GameManager.Instance.UpdateWatchingPlayers();

        }
        void GameObjecSetActiveFalse()
        {
            gameObject.SetActive(false);
        }

        void OutOfTableThings()
        {
            playerInfo.FillerImage.gameObject.SetActive(false);

            if (MatchHandler.IsTeenPatti())
            {
                if (giveTurnToNext && photonView.IsMine)
                {
                    if (UIManager.Instance.ActionTable.activeSelf)
                        UIManager.Instance.ActionTable.SetActive(false);
                }


                PlayerStateManager.Instance.UpdateListOnPlayerPack();
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                {
                    if (PlayerStateManager.Instance.PlayingList.Count > 1)
                    {

                        if (playerInfo.player.IsLocal && !playerInfo.IsSideShow && giveTurnToNext)
                        {
                            SetNextPlayerToExecutingTurn();
                        }

                        giveTurnToNext = true;
                    }
                }



                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                    if (PlayerStateManager.Instance.PlayingList.Count <= 1)
                        PlayerStateManager.Instance.RemainingPlayerWonGameAutomatic();


                playerInfo.SideShowIndicatorAnim.SetActive(false);

            }
            else if (MatchHandler.IsAndarBahar())
            {
                playerInfo.AndarBetAmoutTxt.text = "";
                playerInfo.BaharBetAmoutTxt.text = "";
                PlayerStateManager.Instance.UpdateListOnPlayerPack();
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                {
                    if (PlayerStateManager.Instance.PlayingList.Count < 1)
                    {
                        // Call Reset Function
                        GameResetManager.Instance.ResetABGame();
                    }
                }
            }
            else if (MatchHandler.isWingoLottary())
            {
                //playerInfo.AndarBetAmoutTxt.text = "";
                //playerInfo.BaharBetAmoutTxt.text = "";
                PlayerStateManager.Instance.UpdateListOnPlayerPack();
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                {
                    //if (PlayerStateManager.Instance.PlayingList.Count < 1)
                    if (GameStartManager.Instance._currentNumberOfPlayers < 1)
                    {
                        // Call Reset Function
                        GameResetManager.Instance.ResetWingoLottary();
                    }
                }
                else if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsStarting)
                {
                    if (GameStartManager.Instance._currentNumberOfPlayers < 1)
                        WingoManager.Instance.BetWillEndTxt.transform.parent.gameObject.SetActive(false);
                }
                if (photonView.IsMine)
                    WingoManager.Instance.BetAmountsParent.SetActive(false);
            }
            else if (MatchHandler.IsPoker())
            {


                if (photonView.IsMine)
                {

                    if (PokerActionPanel.Instance.ActionPanelPoker.activeSelf)
                        PokerActionPanel.Instance.ActionPanelPoker.SetActive(false);
                    LocalSettings.SetTotalChips(LocalSettings.GetPokerBuyInChips());
                    GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, LocalSettings.GetPokerBuyInChips().ToString());
                    LocalSettings.SetPokerBuyInChips(-LocalSettings.GetPokerBuyInChips());
                    playerInfo.PokerTotalCash = LocalSettings.GetPokerBuyInChips();
                    playerInfo.player.SetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey, playerInfo.PokerTotalCash);
                    if (playerInfo.Hole_Card1)
                    {
                        Destroy(playerInfo.Hole_Card1.gameObject);
                        Destroy(playerInfo.Hole_Card2.gameObject);
                        playerInfo.DummyCardsParent.transform.GetChild(0).gameObject.SetActive(false);
                        playerInfo.DummyCardsParent.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }



                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
                {
                    if (PlayerStateManager.Instance.PlayingList.Count > 1)
                    {

                        if (playerInfo.player.IsLocal && checkPlayerBool() && !GameManager.Instance.isRunINBackGround)
                            SetNextPlayerToExecutingTurn();

                        if (checkPlayerIsRunInBackGround() && checkPlayerBool() && !GameManager.Instance.isRunINBackGround && PlayerStateManager.Instance.PlayingList.Count > 2)
                            SetNextPlayerToExecutingTurn();



                    }
                }
                
                PlayerStateManager.Instance.UpdateListOnPlayerPack();
                giveTurnToNext = true;

                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
                {

                    if (PokerActionPanel.Instance.checkPockeBetPlaced() && PlayerStateManager.Instance.PlayingList.Count > 1)//isCircleCheckFlag)
                    {
                        // Debug.LogError("packed State....1...");
                        if (playerInfo.checkForAllPlayersbetAreEqual() || PokerActionPanel.Instance.CheckBoolAllIN())
                        {

                            playerInfo.BetGoToFinalPointsForPacked(0f);
                        }
                    }

                }
                updaePlayerProperties(playerInfo);


                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
                    if (PlayerStateManager.Instance.PlayingList.Count <= 1)
                    {
                        if (!GameManager.Instance.isRunINBackGround)
                            PlayerStateManager.Instance.RemainingPlayerWonGameAutomatic();
                        PokerHistory.Instance.SetHistoryBetAmountForEachPlayer(PlayerStateManager.Instance.PlayingList[0].photonView.ViewID, PokerActionPanel.Instance.TotalPotAmount());
                        PokerManager.Instance.HistoryFinalCall();
                        PokerHistory.Instance.SortPokerRecord();
                    }
                PokerManager.Instance.DestroyHoleCards(playerInfo);
                playerInfo.SideShowIndicatorAnim.SetActive(false);
                playerInfo.HandRankLabelTxt.transform.parent.gameObject.SetActive(false);
                // GameManager.Instance.isRunINBackGround = false;
            }
            else if (MatchHandler.IsLuckyWar())
            {
                playerInfo.TieBetAmoutTxt.text = "";
                playerInfo.BetBetAmoutTxt.text = "";
                playerInfo.TurnOnOffTieWinLW(false);
                playerInfo.TurnOnOffLW(false);
                PlayerStateManager.Instance.UpdateListOnPlayerPack();
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
                {
                    if (PlayerStateManager.Instance.PlayingList.Count < 1)
                    {
                        // Call Reset Function
                        GameResetManager.Instance.ResetLWGame();
                    }
                }
            }
            else if (MatchHandler.isDragonTiger())
            {

                //playerInfo.AndarBetAmoutTxt.text = "";
                //playerInfo.BaharBetAmoutTxt.text = "";
                PlayerStateManager.Instance.UpdateListOnPlayerPack();
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                {
                    //if (PlayerStateManager.Instance.PlayingList.Count < 1)
                    if (GameStartManager.Instance._currentNumberOfPlayers < 1)
                    {
                        // Call Reset Function
                        GameResetManager.Instance.ResetDragonTigerGame();
                    }
                }
                else if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsStarting)
                {
                    if (GameStartManager.Instance._currentNumberOfPlayers < 1)
                        DragonTigerManager.Instance.BetWillEndTxt.transform.parent.gameObject.SetActive(false);
                }
                if (photonView.IsMine)
                    DragonTigerManager.Instance.BetAmountsParent.SetActive(false);

            }




        }


        bool checkPlayerIsRunInBackGround()
        {
            for (int i = 0; i < PlayerStateManager.Instance.PlayingList.Count; i++)
            {
                if (PlayerStateManager.Instance.PlayingList[i].checkApplicationBackground)
                    return true;
            }
            return false;
        }

        bool checkPlayerBool()
        {
            foreach (var item in PlayerStateManager.Instance.PlayingList)
            {
                if (playerInfo.photonView.IsMine)
                    if (item.photonView.ViewID == playerInfo.photonView.ViewID)
                        return true;
            }
            return false;
        }

        void updaePlayerProperties(PlayerInfo playerInfo)
        {
            if (playerInfo.photonView.IsMine && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {

                UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
            }
        }
    }
}