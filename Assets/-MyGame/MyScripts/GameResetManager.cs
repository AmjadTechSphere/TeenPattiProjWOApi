using DG.Tweening;
using Photon.Pun;
using System.Net.NetworkInformation;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class GameResetManager : MonoBehaviour
    {
        public static GameResetManager Instance;
        GameManager gameManager;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
        }

        public void ResetGameTeenPatti()
        {
            SetGameStartBooleans();
            ResetPlayerDummyAndOriginalCards();
            ResetSeenBlindStatesAndImages();
            ResetTurnManagerValues();
            ResetActionPanelAndPot();

            UpDateAllPlayersStates();

            RoomStateUpdate();
            AdjustSideShowButtonsOfAllPlayers();
            // SetTableCashReset();
            UIManager.Instance.isPlayerPlayedThisHand = false;
        }


        public void ResetGamePoker()
        {
            //Debug.LogError("Reseting Poker Game");
            SetGameStartBooleans();
            //ResetPlayerDummyAndOriginalCards();
            //ResetSeenBlindStatesAndImages();
            ResetTurnManagerValues();
            //ResetActionPanelAndPot();

            UpDateAllPlayersStates();

            RoomStateUpdate();
            PokerManager.Instance.ResetPokerGame();
            //AdjustSideShowButtonsOfAllPlayers();
            // SetTableCashReset();
            UIManager.Instance.isPlayerPlayedThisHand = false;
        }

        public void ResetABGame()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.TotalBetPlaceFor1Game = 0;
                UIManager.Instance.TotalBetPlaceFor1Game = 0;
            }
            RoomStateUpdateAB();
            SetGameStartBooleans();
            UpDateAllPlayersStates();
            ResetActionPanelAndPotAB();
            DestroyABCardsList();
            ResetTurnManagerValuesAB();
            ResetExtraBoolsAB();
            ResetABTxtFields();
            UIManager.Instance.isPlayerPlayedThisHand = false;
        }

        public void ResetLWGame()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.TotalBetPlaceFor1Game = 0;
                UIManager.Instance.TotalBetPlaceFor1Game = 0;
            }
            RoomStateUpdateAB();
            SetGameStartBooleans();
            UpDateAllPlayersStates();
            ResetTurnManagerValuesLW();
            LuckyWarManager.Instance.ResetLuckywarGame();
        }

        public void ResetWingoLottary()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.TotalBetPlaceFor1Game = 0;
                UIManager.Instance.TotalBetPlaceFor1Game = 0;
            }
            RoomStateUpdateAB();
            UpDateAllPlayersStatesWingo();
            GameStartManager gameStartManager = GameStartManager.Instance;
            gameStartManager.MinimumPlayerSatisfied = false;
            gameStartManager.GameIsGoingToStart = false;
            gameStartManager.IsGameStartingState = false;
            gameStartManager.GameStarWaitTextGameObject.gameObject.SetActive(true);
            gameStartManager.startWaitTime = LocalSettings.WingoGameStartAfterReset;
            WingoManager.Instance.ResetWingoLottary();

        }

        public void ResetDragonTigerGame()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.TotalBetPlaceFor1Game = 0;
                UIManager.Instance.TotalBetPlaceFor1Game = 0;
            }
            RoomStateUpdateAB();
            UpDateAllPlayersStatesDT();
            GameStartManager gameStartManager = GameStartManager.Instance;
            gameStartManager.MinimumPlayerSatisfied = false;
            gameStartManager.GameIsGoingToStart = false;
            gameStartManager.IsGameStartingState = false;
            gameStartManager.GameStarWaitTextGameObject.gameObject.SetActive(true);
            gameStartManager.startWaitTime = LocalSettings.GameStartWaitTimeDragonTiger;
            DragonTigerManager.Instance.ResetDTForGameResetMamager();
        }

        void ResetABTxtFields()
        {
            if (GameManager.Instance != null)
            {
                GameManager gameManager = GameManager.Instance;
                for (int i = 0; i < gameManager.playersList.Count; i++)
                {
                    gameManager.playersList[i].AndarBetAmoutTxt.text = "";
                    gameManager.playersList[i].BaharBetAmoutTxt.text = "";
                    gameManager.playersList[i].SuperBaharBetAmoutTxt.text = "";
                    gameManager.playersList[i].SuperAndarBetAmoutTxt.text = "";
                    // gameManager.playersList[i].SuperAndarBetAmoutTxt.transform.parent.gameObject.SetActive(false);
                    // gameManager.playersList[i].SuperBaharBetAmoutTxt.transform.parent.gameObject.SetActive(false);
                    gameManager.playersList[i].WinningIndicator.SetActive(false);
                }
            }
        }
        void ResetABPlayerListCustomProperties()
        {
            if (GameManager.Instance == null)
                return;
            GameManager gm = GameManager.Instance;
            foreach (PlayerInfo pInfo in gm.playersList)
            {
                pInfo.player.SetCustomBigIntegerData(LocalSettings.abAndarAmountKey, 0);
                pInfo.player.SetCustomBigIntegerData(LocalSettings.abBaharAmountKey, 0);
                pInfo.player.SetCustomBigIntegerData(LocalSettings.abSuperAndarAmountKey, 0);
                pInfo.player.SetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey, 0);

            }
        }

        void ResetExtraBoolsAB()
        {
            if (Game_Play.Instance == null && UIManager.Instance == null)
                return;

            Game_Play.Instance.secondSuperBahar = false;
            Game_Play.Instance.secondTurnTurnAb = false;
            Game_Play.Instance.StandUpFlag = true;
            Game_Play.Instance.MasterClientStandUpFlag = false;
            UIManager.Instance.SkipBetBtn.interactable = false;
            if (UIManager.Instance.GetMyPlayerInfo() != null)
                UIManager.Instance.GetMyPlayerInfo().FirstAbOnMasterClientLeft(false);

        }

        void DestroyABCardsList()
        {
            if (AndarBaharPositionsManager.Instance != null)
                AndarBaharPositionsManager.Instance.DestroyCardsList();

        }

        void RoomStateUpdate()
        {
            if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying && PlayerStateManager.Instance.PlayingList.Count > 1)
                return;
            if (GameStartManager.Instance._currentNumberOfPlayers < 1)
                return;
            if (RoomStateManager.Instance != null)
            {
                if (GameStartManager.Instance._currentNumberOfPlayers < LocalSettings.GetMinPlayers())
                    RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.WaitingForPlayers);
                else
                    RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsStarting);
            }
        }

        void RoomStateUpdateAB()
        {
            //if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying && PlayerStateManager.Instance.PlayingList.Count > 1)
            //    return;

            if (RoomStateManager.Instance != null)
            {
                if (GameStartManager.Instance._currentNumberOfPlayers < LocalSettings.GetMinPlayers())
                    RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.WaitingForPlayers);
                else
                    RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsStarting);
            }
        }

        void SetGameStartBooleans()
        {
            if (GameStartManager.Instance == null)
                return;
            GameStartManager gameStartManager = GameStartManager.Instance;
            gameStartManager.MinimumPlayerSatisfied = false;
            gameStartManager.GameIsGoingToStart = false;
            gameStartManager.IsGameStartingState = false;
            gameStartManager._1stCurrentChaalBool = true;
            gameStartManager.GameStarWaitTextGameObject.gameObject.SetActive(true);
            if (MatchHandler.IsTeenPatti())
                gameStartManager.startWaitTime = LocalSettings.GameStartAfterReset;
            else if (MatchHandler.IsAndarBahar())
                gameStartManager.startWaitTime = LocalSettings.GameStartWaitTimeAndarBahar - 2;
            else if (MatchHandler.IsPoker())
                gameStartManager.startWaitTime = LocalSettings.PokerGameStartAfterReset;
            else if (MatchHandler.IsLuckyWar())
                gameStartManager.startWaitTime = LocalSettings.LuckyWarGameStartAfterReset;
        }

        void UpDateAllPlayersStates()
        {
            if (gameManager != null)
                for (int i = 0; i < gameManager.playersList.Count; i++)
                {
                    if (gameManager.playersList[i].getCurrentPlayerState().currentState != PlayerState.STATE.OutOfTable)
                    {
                        if (PhotonNetwork.IsMasterClient)
                            gameManager.playersList[i].UpdatePlayerState(PlayerState.STATE.AbleToJoin);

                        if (i < gameManager.playersList.Count)
                        {
                            if (gameManager.playersList[i] != null)
                            {
                                gameManager.playersList[i].gameObject.GetComponent<Button>().interactable = true;

                                gameManager.playersList[i].transform.DOScale(new UnityEngine.Vector3(1f, 1f, 1), 1);
                            }
                        }
                    }
                }
        }

        void UpDateAllPlayersStatesWingo()
        {
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                if (gameManager.playersList[i].getCurrentPlayerState().currentState != PlayerState.STATE.OutOfTable)
                {
                    if (PhotonNetwork.IsMasterClient)
                        gameManager.playersList[i].UpdatePlayerState(PlayerState.STATE.AbleToJoin);

                    //gameManager.playersList[i].gameObject.GetComponent<Button>().interactable = true;
                    gameManager.playersList[i].transform.DOScale(new UnityEngine.Vector3(1f, 1f, 1), 1);

                }
            }
        }
        void UpDateAllPlayersStatesDT()
        {
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                if (gameManager.playersList[i].getCurrentPlayerState().currentState != PlayerState.STATE.OutOfTable)
                {
                    if (PhotonNetwork.IsMasterClient)
                        gameManager.playersList[i].UpdatePlayerState(PlayerState.STATE.AbleToJoin);

                    //gameManager.playersList[i].gameObject.GetComponent<Button>().interactable = true;
                    gameManager.playersList[i].transform.DOScale(new UnityEngine.Vector3(1f, 1f, 1), 1);

                }
            }
        }



        void ResetPlayerDummyAndOriginalCards()
        {
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                // Disable dummy cards
                if (gameManager.playersList[i].PlayerDummyCardsToShowParent == null)
                    return;
                Transform dummycardsParent = gameManager.playersList[i].PlayerDummyCardsToShowParent.transform;
                dummycardsParent.gameObject.SetActive(true);
                dummycardsParent.GetChild(0).gameObject.SetActive(false);
                dummycardsParent.GetChild(1).gameObject.SetActive(false);
                dummycardsParent.GetChild(2).gameObject.SetActive(false);
                dummycardsParent.GetChild(3).gameObject.SetActive(false);


                // Destroy original cards if exists                
                Transform orgcardsParent = gameManager.playersList[i].PlayerOrignalCardsToShowParent.transform;
                orgcardsParent.gameObject.SetActive(false);
                if (orgcardsParent.childCount > 5)
                    Destroy(orgcardsParent.GetChild(5).gameObject);
                if (orgcardsParent.childCount > 4)
                    Destroy(orgcardsParent.GetChild(4).gameObject);
                if (orgcardsParent.childCount > 3)
                    Destroy(orgcardsParent.GetChild(3).gameObject);

            }
            if (gameManager.SupportingCard)
                Destroy(gameManager.SupportingCard.gameObject);
        }

        void ResetSeenBlindStatesAndImages()
        {
            if (PlayerStateManager.Instance != null)
            {
                if (PlayerStateManager.Instance.PlayingList == null)
                    return;
                for (int i = 0; i < gameManager.playersList.Count; i++)
                {
                    PlayerInfo plyrInfo = gameManager.playersList[i];
                    if (plyrInfo != null)
                    {
                        plyrInfo.IsSeen = false;
                        plyrInfo.ShowBtn.SetActive(false);
                        plyrInfo.SeenIndicator.SetActive(false);
                        plyrInfo.BlindIndicator.SetActive(false);
                        plyrInfo.MyChaalsPlayedCounter = 0;
                        plyrInfo.WinningIndicator.SetActive(false);
                        plyrInfo.FillerImage.gameObject.SetActive(false);
                        plyrInfo.PackedText.gameObject.SetActive(false);
                        plyrInfo.GivesideShowAlertToAll(false);
                        if (PhotonNetwork.IsConnectedAndReady)
                            plyrInfo.player.SetCustomBoolData("is_seen", false);

                    }
                }
            }
        }

        void ResetTurnManagerValues()
        {
            if (PlayerStateManager.Instance != null)
            {
                if (PlayerStateManager.Instance.PlayingList.Count > 0)
                    if (PhotonNetwork.IsConnectedAndReady)
                        PlayerTurnManager.Instance.turnManager.ResetTurn();
            }
        }

        void ResetTurnManagerValuesAB()
        {
            if (PlayerStateManager.Instance != null)
            {
                if (PlayerStateManager.Instance.PlayingList.Count > 0)
                    if (PhotonNetwork.IsConnectedAndReady)
                        PlayerTurnManager.Instance.turnManager.ResetTurnAB();
            }
        }
        void ResetTurnManagerValuesLW()
        {
            if (PlayerStateManager.Instance != null)
            {
                if (PlayerStateManager.Instance.PlayingList.Count > 0)
                    if (PhotonNetwork.IsConnectedAndReady)
                        PlayerTurnManager.Instance.turnManager.ResetTrunLW();
            }
        }

        void ResetActionPanelAndPot()
        {
            if (Pot.instance == null)
                return;
            if (UIManager.Instance == null)
                return;

            Pot.instance.ResetPot();

            UIManager.Instance.ActionTable.SetActive(false);
            BigInteger betAmount = Pot.instance.CurrentChalAmount;
            UIManager.Instance.UpDateCurrentChalAmountText(betAmount);
            UIManager.Instance.ChaalTypeText.text = "Blind";
            UIManager.Instance.sideShowBtn.interactable = false;
            UIManager.Instance.showBtn.interactable = false;



            if (UIManager.Instance.GetMyPlayerInfo() != null)
                if (UIManager.Instance.GetMyPlayerInfo().IsSeen)
                    betAmount = Pot.instance.CurrentChalAmount * 2;

        }


        void ResetActionPanelAndPotAB()
        {
            if (Pot.instance == null)
                return;
            if (UIManager.Instance == null)
                return;
            Pot.instance.ResetPotAB();


        }

        void AdjustSideShowButtonsOfAllPlayers()
        {
            UIManager.Instance.sideShowBtn.interactable = false;
            UIManager.Instance.showBtn.interactable = false;
        }

    }
}