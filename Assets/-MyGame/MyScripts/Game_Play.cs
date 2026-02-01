using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.mani.muzamil.amjad
{
    public class Game_Play : MonoBehaviour
    {
        UIManager uIManager;
        [ShowOnly] public bool secondTurnTurnAb;
        [ShowOnly] public bool Skip_Bet_TurnManager;
        [ShowOnly] public bool StandUpFlag;
        [ShowOnly] public bool MasterClientStandUpFlag;
        [ShowOnly] public bool secondSuperBahar;



        // Start is called before the first frame update
        #region Creating Instance;
        private static Game_Play _instance;
        public static Game_Play Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<Game_Play>();
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }
        #endregion
        void Start()
        {
            uIManager = UIManager.Instance;
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(0);
            }
            LocalSettings.isSwitchRoom = false;
            // Debug.LogError("Check Bool......" + LocalSettings.isSwitchRoom);
            Skip_Bet_TurnManager = false;
            secondTurnTurnAb = false;
            StandUpFlag = true;
            MasterClientStandUpFlag = false;
            secondSuperBahar = false;
        }

        #region 3 patti Game
        public void back()
        {
            PhotonNetwork.LoadLevel("MainMenu");
        }

        public void LeaveRoomAndExitToLobby()
        {
            if (PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.LeaveRoom();
            uIManager.LoadingPanel.SetActive(true);
            StartCoroutine(GoToLobby(1f));
        }

        IEnumerator GoToLobby(float TimeDelay)
        {
            yield return new WaitForSecondsRealtime(TimeDelay);
            PhotonNetwork.LoadLevel(0);

        }

        public void EndMyTurn()
        {
            uIManager.GetMyPlayerInfo().MyChaalsPlayedCounter++;
            uIManager.GetMyPlayerInfo().UpdatePlayerState(PlayerState.STATE.BetPlaced);
            if (!uIManager.GetMyPlayerInfo().IsSeen)
            {
                if (uIManager.GetMyPlayerInfo().MyChaalsPlayedCounter >= uIManager.TotalChals)
                {
                    uIManager.GetMyPlayerInfo().ShowCardsFromBlind();
                }
            }
        }

        public void PackThisPlayer()
        {
            UIManager.Instance.GetMyPlayerInfo().UpdatePlayerState(PlayerState.STATE.Packed);
        }

        public void OnBettingAmoundIncrease()
        {
            Pot.instance.CurrentChalAmount *= 2;
            uIManager.GetMyPlayerInfo().CurrentChalAmountSendToAllPlayers(Pot.instance.CurrentChalAmount);
            uIManager.BetAmountDecreaseBtn.interactable = true;
            uIManager.BetAmountIncreaseBtn.interactable = false;
        }
        public void OnBettingAmoundDecrease()
        {
            if (Pot.instance.CurrentChalAmount > 30)
                Pot.instance.CurrentChalAmount /= 2;
            uIManager.GetMyPlayerInfo().CurrentChalAmountSendToAllPlayers(Pot.instance.CurrentChalAmount);
            uIManager.BetAmountDecreaseBtn.interactable = false;
            uIManager.BetAmountIncreaseBtn.interactable = true;
        }

        public void MyTurnBetAmountLimit()
        {

            uIManager.BetAmountDecreaseBtn.interactable = false;
            uIManager.BetAmountIncreaseBtn.interactable = true;
            BigInteger betAmount = Pot.instance.CurrentChalAmount;

            //if (Pot.instance.ChaalAmountLimit() <= Pot.instance.CurrentChalAmount)
            if (Pot.instance.ChaalAmountLimit() <= betAmount)
                uIManager.BetAmountIncreaseBtn.interactable = false;
            else
                if (uIManager.GetMyPlayerInfo().IsSeen)
                betAmount = Pot.instance.CurrentChalAmount * 2;

            uIManager.UpDateCurrentChalAmountText(betAmount);
        }
        public void GiveTipToGirl()
        {
            if (LocalSettings.GetTotalChips() <= LocalSettings.MinBetAmount)
            {
                uIManager.InfoTxt.text = uIManager.GetMyPlayerInfo().player_name.text + "  Your Table Minimum bet limited is " + LocalSettings.MinBetAmount;
                uIManager.InfoObj.SetActive(true);
                return;
            }
            int dialogueNumber = Random.Range(0, uIManager.GetMyPlayerInfo().tipsDialouges.Count);
            if (uIManager.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfTable)
                UIManager.Instance.GetMyPlayerInfo().ForAllShowTipToGirl(dialogueNumber);
            else
                uIManager.quickShop.SetActive(true);

        }
        public void ShowWinner()
        {
            EndMyTurn();
            GameResultsManager.Instance.CheckWinnerOfThisGame();

            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ShowingResults);
            Debug.Log("Here is your error 1......." + RoomStateManager.Instance.CurrentRoomState);
        }

        public void ShowInfo(string message, float timeDelay)
        {
            uIManager.InfoObj.SetActive(false);
            PerformFunction perf = uIManager.InfoObj.GetComponent<PerformFunction>();
            perf.TimeDelay = timeDelay;
            uIManager.InfoTxt.text = message;
            uIManager.InfoObj.SetActive(true);

        }

        public void OnClickSideShowBtn()
        {
            uIManager.GetMyPlayerInfo().GivesideShowAlertToAll(true);
            uIManager.ActionTable.SetActive(false);

            EndMyTurn();
            // check if all players played
            if (Pot.instance.potSize >= Pot.instance.PotLimit)
            {
                RoomStateManager.Instance.UpdateCurrentState(RoomState.STATE.WaitingForResults, LocalSettings.textStringOfPotLimitReached);
            }
            else
            {

                int prevPlayerInt = PlayerStateManager.Instance.SideShowPrev();
                PlayerStateManager.Instance.PlayingList[prevPlayerInt].getCurrentPlayerState().UpdateCurrentPlayerState(PlayerState.STATE.RecieverSideShow);
                UIManager.Instance.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.SenderSideShow);
            }

            //RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameSideShow);

            //uIManager.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.SenderSideShow);
        }

        public void OnClickAcceptSideShowBtn()
        {
            uIManager.sideShowPanel.SetActive(false);
            GameResultsManager.Instance.DeclareWinningSideShowPlayerOfTeenPatti();
            //uIManager.GetMyPlayerInfo().GivesideShowAlertToAll(false);
        }

        public void OnClickCancelSideShowBtn()
        {
            RoomStateManager.Instance.UpdateCurrentStateOnShowBtn(RoomState.STATE.GameIsPlaying);
            uIManager.sideShowPanel.SetActive(false);
            uIManager.GetMyPlayerInfo().GivesideShowAlertToAll(false);
            if (PlayerStateManager.Instance.PlayingList.Count == 2)
            {
                int a = PlayerStateManager.Instance.SideShowNext();
                if (PlayerStateManager.Instance.PlayingList[a].currentPlayerStateRef.currentState != PlayerState.STATE.ExecutingTurn)
                    uIManager.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
            }


            //int nextPlayerInt = PlayerStateManager.Instance.PlayingList.IndexOf(uIManager.GetMyPlayerInfo());
            //nextPlayerInt++;
            //if (nextPlayerInt >= PlayerStateManager.Instance.PlayingList.Count)
            //    nextPlayerInt = 1;
            //else
            //    nextPlayerInt++;
            //PlayerStateManager.Instance.PlayingList[nextPlayerInt].getCurrentState().UpdateCurrentState(PlayerState.STATE.ExecutingTurn);
        }

        public void OnClickShowBtn()
        {
            if (PlayerStateManager.Instance.PlayingList[PlayerStateManager.Instance.SideShowNext()].getCurrentPlayerState().currentState != PlayerState.STATE.ExecutingTurn)
            {
                uIManager.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
            }
            if (uIManager.GetMyPlayerInfo().photonView.IsMine)
            {
                PlayerTurnManager.Instance.AddChaalAmount();
            }
            RoomStateManager.Instance.UpdateCurrentStateOnShowBtn(RoomState.STATE.WaitingForResults, LocalSettings.textStringOnShowCard);
            uIManager.ActionTable.SetActive(false);

        }


        public void StandUp()
        {
            uIManager.GetMyPlayerInfo().StandUp();
        }
        #endregion

        #region Andar Bahar Btns

        public void AndarBaharBetting(string BtnNumber)
        {


            switch (BtnNumber)
            {
                case "1":
                    // Andar bet btn
                    if (LocalSettings.GetTotalChips() == AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount)
                        AndarBaharPositionsManager.Instance.AndarTotalBetAmount = LocalSettings.GetTotalChips();
                    UIManager.Instance.TotalBetPlacedAmount += AndarBaharPositionsManager.Instance.AndarTotalBetAmount;
                    uIManager.TotalBetPlaceFor1Game += AndarBaharPositionsManager.Instance.AndarTotalBetAmount;
                    PlaceAndar();
                    break;
                case "2":
                    // Andar - btn
                    uIManager.AndarPveBtn.interactable = true;
                    if (LocalSettings.GetTotalChips() != AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount)
                    {
                        // Debug.LogError(uIManager.AndarNveBtn.interactable);
                        BigInteger ANBetAmount = AndarBaharPositionsManager.Instance.AndarTotalBetAmount / 2;
                        if (LocalSettings.MinBetAmount <= ANBetAmount)
                        {
                            AndarBaharPositionsManager.Instance.AndarTotalBetAmount = ANBetAmount;
                            if (LocalSettings.MinBetAmount >= ANBetAmount)
                                uIManager.AndarNveBtn.interactable = false;
                        }
                    }

                    AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount = 0;
                    // Debug.LogError(uIManager.AndarNveBtn.interactable);

                    uIManager.AndarBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.AndarTotalBetAmount);
                    break;
                case "3":
                    // Andar + btn
                    uIManager.AndarNveBtn.interactable = true;
                    BigInteger APBetAmount = AndarBaharPositionsManager.Instance.AndarTotalBetAmount * 2;
                    if (LocalSettings.GetTotalChips() >= APBetAmount)
                    {

                        // if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 2) >= APBetAmount)
                        // {
                        if ((Pot.instance.maximumBet) >= APBetAmount)
                        {
                            AndarBaharPositionsManager.Instance.AndarTotalBetAmount = APBetAmount;
                            uIManager.AndarBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.AndarTotalBetAmount);
                            //   if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 2) <= APBetAmount)
                            //  {
                            // Debug.LogError("False Andar button");
                            // uIManager.AndarPveBtn.interactable = false;
                            // }
                            //}
                        }

                    }
                    else
                    {
                        AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount = LocalSettings.GetTotalChips();
                        uIManager.AndarBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount);
                    }

                    if (LocalSettings.GetTotalChips() <= APBetAmount || (Pot.instance.maximumBet) <= APBetAmount)
                    {
                        //AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount = LocalSettings.GetTotalChips();
                        // uIManager.AndarBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount);
                        uIManager.AndarPveBtn.interactable = false;
                    }

                    break;
                case "4":
                    // Bahar bet Btn
                    if (LocalSettings.GetTotalChips() == AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount)
                        AndarBaharPositionsManager.Instance.BaharTotalBetAmount = LocalSettings.GetTotalChips();
                    UIManager.Instance.TotalBetPlacedAmount += AndarBaharPositionsManager.Instance.BaharTotalBetAmount;
                    uIManager.TotalBetPlaceFor1Game += AndarBaharPositionsManager.Instance.BaharTotalBetAmount;
                    PlaceBahar();
                    break;
                case "5":
                    // Bahar - btn
                    uIManager.BaharPveBtn.interactable = true;
                    if (LocalSettings.GetTotalChips() != AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount)
                    {

                        BigInteger BNBetAmount = AndarBaharPositionsManager.Instance.BaharTotalBetAmount / 2;
                        if (LocalSettings.MinBetAmount <= BNBetAmount)
                        {
                            AndarBaharPositionsManager.Instance.BaharTotalBetAmount = BNBetAmount;
                            if (LocalSettings.MinBetAmount >= BNBetAmount)
                                uIManager.BaharNveBtn.interactable = false;
                        }
                    }

                    uIManager.BaharBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.BaharTotalBetAmount);
                    AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount = 0;
                    break;
                case "6":
                    // bahar + btn
                    uIManager.BaharNveBtn.interactable = true;
                    BigInteger BPBetAmount = AndarBaharPositionsManager.Instance.BaharTotalBetAmount * 2;
                    if (LocalSettings.GetTotalChips() > BPBetAmount)
                    {
                        if ((Pot.instance.maximumBet) >= BPBetAmount)
                        {


                            // if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 2) >= BPBetAmount)
                            // {
                            AndarBaharPositionsManager.Instance.BaharTotalBetAmount = BPBetAmount;
                            uIManager.BaharBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.BaharTotalBetAmount);
                        }
                        // if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 2) <= BPBetAmount)
                        // uIManager.BaharPveBtn.interactable = false;
                        // }
                    }
                    else
                    {
                        AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount = LocalSettings.GetTotalChips();
                        uIManager.BaharBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount);
                    }
                    if (LocalSettings.GetTotalChips() <= BPBetAmount || (Pot.instance.maximumBet) <= BPBetAmount)
                    {
                        // AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount = LocalSettings.GetTotalChips();
                        // uIManager.BaharBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount);
                        uIManager.BaharPveBtn.interactable = false;
                    }

                    break;
                default:
                    break;
                case "7":
                    // Skip bet btn
                    Skip_Bet();
                    break;
            }
        }



        public void Skip_Bet()
        {
            DisableBtnsOnSuperBet();
        }

        void PlaceAndar()
        {
            uIManager.GetMyPlayerInfo().PlaceBetAndar();
            DisableBtnsOnBet();
        }

        void PlaceBahar()
        {
            uIManager.GetMyPlayerInfo().PlaceBetBahar();

            DisableBtnsOnBet();
        }

        void DisableBtnsOnBet()
        {
            //if (secondTurnTurnAb)
            // {

            AndarBaharPositionsManager.Instance.ABActionPanel.SetActive(false);
            uIManager.isPlayerPlayedThisHand = true;
            //    uIManager.GetMyPlayerInfo().TurnOnOffAb(false);
            // }
            //if (!secondTurnTurnAb)
            //{
            if (LocalSettings.Get_Super_Bahar_Status())
                Invoke(nameof(ShowSuperABActionPanel), UIManager.Instance.BetAmountToTargetAnim().TimeToCompleteAnim);
            else
                Skip_Bet();
            // ShowSuperABActionPanel();
            //}


            //
            uIManager.SkipBetBtn.interactable = true;
            //Debug.LogError("Starting camparison");
            AndarBaharPositionsManager.Instance.AndarTotalBetAmount = AndarBaharPositionsManager.Instance.BaharTotalBetAmount = LocalSettings.MinBetAmount;
            uIManager.BaharBetAmountBtnTxt.text = uIManager.AndarBetAmountBtnTxt.text = LocalSettings.Rs(LocalSettings.MinBetAmount);
            uIManager.AndarNveBtn.interactable = false;
            uIManager.BaharNveBtn.interactable = false;
            //PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationAndarBahar;
            secondTurnTurnAb = true;
            Skip_Bet_TurnManager = true;
        }

        /// <summary>
        /// Super andar bahr section
        /// </summary>
        /// <param name="Super andar bahar section"></param>

        //public void SuperAndarBaharBetting(string BtnNumber)
        //{
        //    switch (BtnNumber)
        //    {
        //        case "1":
        //            // Andar bet btn

        //            PlaceSuperAndar();
        //            break;
        //        case "2":
        //            // Andar - btn
        //            uIManager.SuperAndarPveBtn.interactable = true;
        //            BigInteger SuperANBetAmount = AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount / 2;
        //            if (LocalSettings.MinABBetAmount <= SuperANBetAmount)
        //            {
        //                AndarBaharPositionsManager.Instance.AndarTotalBetAmount = SuperANBetAmount;
        //                if (LocalSettings.MinABBetAmount >= SuperANBetAmount)
        //                    uIManager.SuperAndarNveBtn.interactable = false;
        //            }
        //            uIManager.SuperAndarBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount);
        //            break;
        //        case "3":
        //            // Andar + btn
        //            uIManager.SuperAndarNveBtn.interactable = true;
        //            BigInteger SuperAPBetAmount = AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount * 2;
        //            if (LocalSettings.GetTotalChips() >= SuperAPBetAmount)
        //            {
        //                if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 5) >= SuperAPBetAmount)
        //                {
        //                    AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount = SuperAPBetAmount;
        //                    if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 5) <= SuperAPBetAmount)
        //                        uIManager.SuperAndarPveBtn.interactable = false;
        //                }
        //            }
        //            uIManager.SuperAndarBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.SuperAndarTotalBetAmount);
        //            break;
        //        case "4":
        //            // Bahar bet Btn
        //            //PlaceSuperBahar();
        //            break;
        //        case "5":
        //            // Bahar - btn
        //            uIManager.SuperBaharPveBtn.interactable = true;
        //            BigInteger SuperBNBetAmount = AndarBaharPositionsManager.Instance.SuperBaharTotalBetAmount / 2;
        //            if (LocalSettings.MinABBetAmount <= SuperBNBetAmount)
        //            {
        //                AndarBaharPositionsManager.Instance.SuperBaharTotalBetAmount = SuperBNBetAmount;
        //                if (LocalSettings.MinABBetAmount >= SuperBNBetAmount)
        //                    uIManager.SuperBaharNveBtn.interactable = false;
        //            }
        //            uIManager.SuperBaharBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.SuperBaharTotalBetAmount);
        //            break;
        //        case "6":
        //            // bahar + btn
        //            uIManager.SuperBaharNveBtn.interactable = true;
        //            BigInteger SuperBPBetAmount = AndarBaharPositionsManager.Instance.SuperBaharTotalBetAmount * 2;
        //            if (LocalSettings.GetTotalChips() >= SuperBPBetAmount)
        //            {
        //                if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 5) >= SuperBPBetAmount)
        //                {
        //                    AndarBaharPositionsManager.Instance.SuperBaharTotalBetAmount = SuperBPBetAmount;
        //                    if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 5) <= SuperBPBetAmount)
        //                        uIManager.SuperBaharPveBtn.interactable = false;
        //                }
        //            }
        //            uIManager.SuperBaharBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.SuperBaharTotalBetAmount);
        //            break;
        //        default:
        //            break;
        //        case "7":
        //            // Skip bet btn
        //            Skip_Bet();
        //            break;
        //    }
        //}

        public void PlaceSuperAndar()
        {
            uIManager.GetMyPlayerInfo().PlaceBetSuperAndar();
            DisableBtnsOnSuperBet();
            // ShowSuperABActionPanel();
            //
        }

        #region For Future Need Only Superbahar According to KK
        //public void PlaceSuperBahar(int amount)
        //{
        //    if (LocalSettings.GetTotalChips() >= amount)
        //    {
        //        AndarBaharPositionsManager andarBaharPositionsManager = AndarBaharPositionsManager.Instance;

        //        if (secondSuperBahar)
        //        {
        //            andarBaharPositionsManager.secondSuperBaharBet = andarBaharPositionsManager.firstSuperBaharBet + amount;
        //            uIManager.GetMyPlayerInfo().PlaceBetSuperBahar(andarBaharPositionsManager.secondSuperBaharBet);
        //        }
        //        else
        //        {
        //            andarBaharPositionsManager.firstSuperBaharBet = amount;

        //            uIManager.GetMyPlayerInfo().PlaceBetSuperBahar(andarBaharPositionsManager.firstSuperBaharBet);

        //        }

        //        andarBaharPositionsManager.SuperBaharTotalBetAmount = amount;
        //        //uIManager.GetMyPlayerInfo().PlaceBetSuperBahar(amount);
        //        // ShowSuperABActionPanel();
        //        DisableBtnsOnSuperBet();
        //    }

        //    //sdf
        //}
        #endregion




        public void DisableBtnsOnSuperBet()
        {
            if (uIManager.GetMyPlayerInfo() != null)
                uIManager.GetMyPlayerInfo().TurnOnOffAb(false);
            AndarBaharPositionsManager.Instance.SuperABActionPanel.SetActive(false);
            AndarBaharPositionsManager.Instance.ABActionPanel.SetActive(false);
            secondSuperBahar = true;
            // ToggleABActionPanelBtns(true);

        }

        public void ClickOnSkip_Btn()
        {
            if (LocalSettings.Get_Super_Bahar_Status())
                ShowSuperABActionPanel();
            else
                Skip_Bet();
            AndarBaharPositionsManager.Instance.ABActionPanel.SetActive(false);
        }


        void ShowSuperABActionPanel()
        {
            //  if (!AndarBaharPositionsManager.Instance.isSuperABDone)
            //  {
            //ToggleABActionPanelBtns(false);
            if (PlayerTurnManager.Instance.turnManager.RemainingSecondsInTurn > 0.1)
                AndarBaharPositionsManager.Instance.SuperABActionPanel.SetActive(true);
            //StartCoroutine(waitForABFirstTurn());

            //}
            //else
            //{
            //    ToggleABActionPanelBtns(true);
            //    AndarBaharPositionsManager.Instance.ABActionPanel.SetActive(false);
            //   AndarBaharPositionsManager.Instance.SuperABActionPanel.SetActive(false);
            // }
        }

        public void ABActionButtonInteractable()
        {
            if (LocalSettings.GetTotalChips() <= LocalSettings.MinBetAmount)
            {
                ToggleABActionPanelBtns(false);
                uIManager.quickShop.SetActive(true);
            }
            else
                ToggleABActionPanelBtns(true);
        }

        void ToggleABActionPanelBtns(bool isTure)
        {
            // uIManager.AndarNveBtn.interactable = isTure;
            uIManager.AndarPveBtn.interactable = isTure;
            // uIManager.BaharNveBtn.interactable = isTure;
            uIManager.BaharPveBtn.interactable = isTure;
            if (LocalSettings.GetTotalChips() == LocalSettings.MinBetAmount)
            {
                isTure = true;
                uIManager.AndarBetBtn.interactable = isTure;
                uIManager.BaharBetBtn.interactable = isTure;
            }
            else
            {
                uIManager.AndarBetBtn.interactable = isTure;
                uIManager.BaharBetBtn.interactable = isTure;
            }
        }
        #endregion


        public void SwitchToRoom()
        {
            LocalSettings.isSwitchRoom = true;
            Debug.LogError("Check Bool......" + LocalSettings.isSwitchRoom);
            LeaveRoomAndExitToLobby();
            //PhotonNetwork.LeaveRoom();
            //uIManager.LoadingPanel.SetActive(true); // Leave the current room
            //PhotonNetwork.LoadLevel(0);
            //NetworkSettings.Instance.RoomEntranceProperty(MatchHandler.CurrentMatch, (int)LocalSettings.MinBetAmount);
            //PhotonNetwork.JoinOrCreateRoom(NetworkSettings.Instance.RoomName, new RoomOptions(), TypedLobby.Default);


        }


        public void OnClickGoldTranferBtn()
        {
            uIManager.CheckConditionGoldTranfer();
        }

        public void OnClickLevelPanel()
        {
            StartCoroutine(waitForPanelLevelPanelOff());
        }

        IEnumerator waitForPanelLevelPanelOff()
        {
            yield return new WaitForSeconds(1f);
            UIManager.Instance.levelUpPanel.SetActive(false);
        }

    }
}