using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.mani.muzamil.amjad
{
    public class GameResultsManager : MonoBehaviourPunCallBacksWithNSSCallBacks //MonoBehaviour
    {

        bool _isGameCompleted;

        public bool isGameCompleted
        {
            get { return _isGameCompleted; }
            set { _isGameCompleted = value; }
        }

        #region Creating Instance
        private static GameResultsManager _instance;
        public static GameResultsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GameResultsManager>();
                return _instance;
            }
        }
        #endregion
        void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        public void CheckWinnerOfThisGame()
        {
            //MatchHandler.MATCH match = MatchHandler.CurrentMatch;
            // switch (match)
            //  {
            //     case MatchHandler.MATCH.Classic:
            if (MatchHandler.IsTeenPatti())
            {
                PlayerStateManager.Instance.AllPlayersGameCompleted();
                DeclareWinningPlayerOfTeenPatti();
            }
            //         break;
            // }
        }

        #region Calculating and Declare winning player;
        public void DeclareWinningPlayerOfTeenPatti()
        {
            List<PlayerInfo> playersList = new List<PlayerInfo>();
            playersList = PlayerStateManager.Instance.PlayingList;

            // Calculating ranks and scores from every players own cards
            for (int i = 0; i < playersList.Count; i++)
            {
                if (playersList[i])
                {
                    GettingPlayerScoreAndRank(playersList[i]);
                    playersList[i].MyRank = PlayerCardsRankAndScoreCalc.Rank;
                    playersList[i].MyScores = PlayerCardsRankAndScoreCalc.Scores;
                    playersList[i].OrgCardValues = PlayerCardsRankAndScoreCalc.CardValuesArrayForPlayer;
                }
            }

            PlayerDataNew[] playerData;
            playerData = new PlayerDataNew[playersList.Count];
            for (int i = 0; i < playerData.Length; i++)
            {
                playerData[i] = new PlayerDataNew();
                playerData[i].rank = playersList[i].MyRank;
                playerData[i].intCardsArray = playersList[i].OrgCardValues;
                playerData[i].PlayerViewID = playersList[i].photonView.ViewID;
            }
            // Array Sorting w.r.t Player ranks and scores
            Array.Sort(playerData, new PlayerComparer());
            if (MatchHandler.CurrentMatch == MatchHandler.MATCH.Mufflis)
            {
                Array.Reverse(playerData);
            }
            // Debug.LogError(MatchHandler.CurrentMatch);
            // Debug.LogError(NetworkSettings.Instance.currentRoomFilter);

            // Declaring Winner and running RPC 
            if (playerData[0].rank != 3)
            {
                ShowWinnerFromPlayerDataArray(playersList, playerData);
            }
            else
            {
                int NumberofPlayersWithRank3 = 0;
                for (int i = 0; i < playersList.Count; i++)
                {
                    if (playersList[i].MyRank == 3)
                        NumberofPlayersWithRank3++;
                }
                if (NumberofPlayersWithRank3 == 1)
                {
                    ShowWinnerFromPlayerDataArray(playersList, playerData);
                }
                else
                {
                    int TempInt = 0;
                    PlayerDataRanks[] playerDataRanks = new PlayerDataRanks[NumberofPlayersWithRank3];
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        if (playersList[i].MyRank == 3)
                        {
                            playerDataRanks[TempInt] = new PlayerDataRanks();
                            playerDataRanks[TempInt].Rank = playersList[i].MyRank;
                            playerDataRanks[TempInt].Scores = playersList[i].MyScores;
                            playerDataRanks[TempInt].viewID = playersList[i].photonView.ViewID;
                            TempInt++;
                        }
                    }

                    Array.Sort(playerDataRanks, new PlayerDataComparer());
                    if (MatchHandler.CurrentMatch == MatchHandler.MATCH.Mufflis)
                    {
                        Array.Reverse(playerDataRanks);
                    }
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        if (playerDataRanks[0].viewID == playersList[i].photonView.ViewID)
                        {
                            playersList[i].IAmWinner(true);

                            if (playersList[i].photonView.IsMine)
                            {
                                GameManager.Instance.PlayerTotalChipsUpdate(Pot.instance.potSize);
                                GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, Pot.instance.potSize.ToString());
                                UIManager.Instance.TotalWinsAmount += Pot.instance.potSize;
                                UIManager.Instance.TotalWinHands++;
                                GameManager.Instance.AddXPToMyPlayer(true);
                                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                                Debug.LogError("Win sound is playing");
                            }
                        }
                        else
                            playersList[i].IAmWinner(false);
                    }
                }
            }
            if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.Watching)
            {
                UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
            }

        }
        void ShowWinnerFromPlayerDataArray(List<PlayerInfo> playersList, PlayerDataNew[] playerData)
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                if (playerData[0].PlayerViewID == playersList[i].photonView.ViewID)
                {

                    if (playersList[i].photonView.IsMine)
                    {
                        playersList[i].IAmWinner(true);
                        GameManager.Instance.PlayerTotalChipsUpdate(Pot.instance.potSize);
                        GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, Pot.instance.potSize.ToString());
                        UIManager.Instance.TotalWinsAmount += Pot.instance.potSize;
                        UIManager.Instance.TotalWinHands++;
                        GameManager.Instance.AddXPToMyPlayer(true);
                        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                        //Debug.LogError("Win sound is playing");
                    }
                }
                else
                    playersList[i].IAmWinner(false);
            }
        }
        void GettingPlayerScoreAndRank(PlayerInfo playerInfo)
        {

            Transform PlayerCardsParent = playerInfo.PlayerOrignalCardsToShowParent.transform;
            if (PlayerCardsParent.childCount > 3)
                PlayerCardsRankAndScoreCalc.CalculateRankAndScores(PlayerCardsParent.GetChild(3).gameObject.GetComponent<CardProperty>(), PlayerCardsParent.GetChild(4).gameObject.GetComponent<CardProperty>(), PlayerCardsParent.GetChild(5).gameObject.GetComponent<CardProperty>());
        }
        #endregion

        #region Calculating and Declare winning of SideShow player;
        public void DeclareWinningSideShowPlayerOfTeenPatti()
        {
            StartCoroutine(waitforLoadSideShow());
            //PlayerStateManager.Instance.PlayerstateChangeForNextRound();
        }

        IEnumerator waitforLoadSideShow()
        {
            List<PlayerInfo> playersList = new List<PlayerInfo>();
            int nextPlayerInt = PlayerStateManager.Instance.SideShowNext();
            PlayerInfo sideShowPlayerInfo = PlayerStateManager.Instance.PlayingList[nextPlayerInt];

            for (int i = 0; i < 2; i++)
            {
                switch (i)
                {
                    case 0:
                        playersList.Add(UIManager.Instance.GetMyPlayerInfo());
                        break;
                    case 1:
                        playersList.Add(sideShowPlayerInfo);
                        sideShowPlayerInfo.getCurrentPlayerState().UpdateCurrentPlayerState(PlayerState.STATE.Watching);
                        break;
                }
            }

            // Show Cards
            for (int i = 0; i < playersList.Count; i++)
                playersList[i].ShowCardsFromBlind();
            // playersList = PlayerStateManager.Instance.PlayingList;

            // Calculating ranks and scores from every players own cards
            for (int i = 0; i < playersList.Count; i++)
            {
                if (playersList[i])
                {
                    GettingPlayerScoreAndRank(playersList[i]);
                    playersList[i].MyRank = PlayerCardsRankAndScoreCalc.Rank;
                    playersList[i].MyScores = PlayerCardsRankAndScoreCalc.Scores;
                    playersList[i].OrgCardValues = PlayerCardsRankAndScoreCalc.CardValuesArrayForPlayer;
                }
            }

            ///////////////

            //PlayerDataNew[] playerData;
            //playerData = new PlayerDataNew[playersList.Count];
            //for (int i = 0; i < playerData.Length; i++)
            //{
            //    playerData[i] = new PlayerDataNew();
            //    playerData[i].rank = playersList[i].MyRank;
            //    playerData[i].intCardsArray = playersList[i].OrgCardValues;
            //    playerData[i].PlayerViewID = playersList[i].photonView.ViewID;
            //}
            //// Array Sorting w.r.t Player ranks and scores
            //Array.Sort(playerData, new PlayerComparer());

            //// Declaring Winner and running RPC 
            ////for (int i = 0; i < playersList.Count; i++)
            ////{
            ////    if (playerData[0].PlayerViewID == playersList[i].photonView.ViewID)
            ////        playersList[i].IAmWinner(true);
            ////    else
            ////        playersList[i].IAmWinner(false);
            ////}
            //// Declaring Winner and running RPC 
            //yield return new WaitForSeconds(LocalSettings.GameResultWaitingTime);
            //for (int i = 0; i < playersList.Count; i++)
            //{
            //    if (playerData[0].PlayerViewID == playersList[i].photonView.ViewID)
            //    {
            //        // playersList[i].IAmWinner(true);
            //    }
            //    else
            //        playersList[i].getCurrentState().UpdateCurrentState(PlayerState.STATE.Packed);
            //}
            PlayerDataNew[] playerData;
            playerData = new PlayerDataNew[playersList.Count];
            for (int i = 0; i < playerData.Length; i++)
            {
                playerData[i] = new PlayerDataNew();
                playerData[i].rank = playersList[i].MyRank;
                playerData[i].intCardsArray = playersList[i].OrgCardValues;
                playerData[i].PlayerViewID = playersList[i].photonView.ViewID;
            }
            // Array Sorting w.r.t Player ranks and scores
            Array.Sort(playerData, new PlayerComparer());
            if (MatchHandler.CurrentMatch == MatchHandler.MATCH.Mufflis)
            {
                Array.Reverse(playerData);
                //Debug.LogError(MatchHandler.CurrentMatch);
            }

            yield return new WaitForSeconds(LocalSettings.GameResultWaitingTime);
            // Declaring Winner and running RPC 
            if (playerData[0].rank != 3)
            {
                //ShowWinnerFromPlayerDataArray(playersList, playerData);
                for (int i = 0; i < playersList.Count; i++)
                {
                    if (playerData[0].PlayerViewID == playersList[i].photonView.ViewID)
                    {
                        //playersList[i].IAmWinner(true);
                    }
                    else
                        playersList[i].getCurrentPlayerState().UpdateCurrentPlayerState(PlayerState.STATE.Packed);
                }
            }
            else
            {
                int NumberofPlayersWithRank3 = 0;
                for (int i = 0; i < playersList.Count; i++)
                {
                    if (playersList[i].MyRank == 3)
                        NumberofPlayersWithRank3++;
                }
                if (NumberofPlayersWithRank3 == 1)
                {
                    for (int i = 0; i < playersList.Count; i++)
                    {

                        if (playerData[0].PlayerViewID == playersList[i].photonView.ViewID)
                        {
                            //playersList[i].IAmWinner(true);
                        }
                        else
                            playersList[i].getCurrentPlayerState().UpdateCurrentPlayerState(PlayerState.STATE.Packed);
                    }
                }
                else
                {
                    int TempInt = 0;
                    PlayerDataRanks[] playerDataRanks = new PlayerDataRanks[NumberofPlayersWithRank3];
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        if (playersList[i].MyRank == 3)
                        {
                            playerDataRanks[TempInt] = new PlayerDataRanks();
                            playerDataRanks[TempInt].Rank = playersList[i].MyRank;
                            playerDataRanks[TempInt].Scores = playersList[i].MyScores;
                            playerDataRanks[TempInt].viewID = playersList[i].photonView.ViewID;
                            TempInt++;
                        }
                    }

                    Array.Sort(playerDataRanks, new PlayerDataComparer());
                    if (MatchHandler.CurrentMatch == MatchHandler.MATCH.Mufflis)
                    {
                        Array.Reverse(playerDataRanks);
                    }
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        if (playerDataRanks[0].viewID == playersList[i].photonView.ViewID)
                        {
                            //playersList[i].IAmWinner(true);
                        }
                        else
                            playersList[i].getCurrentPlayerState().UpdateCurrentPlayerState(PlayerState.STATE.Packed);
                    }
                }
            }
        }


        #endregion




        public IEnumerator ShowResult(float TimeDelay, string infoText)
        {
            Game_Play.Instance.ShowInfo(infoText, 3f);
            yield return new WaitForSeconds(TimeDelay);
            ShowingResult();
        }
        public void ShowingResult()
        {
            CheckWinnerOfThisGame();
            for (int i = 0; i < PlayerStateManager.Instance.PlayingList.Count; i++)
                PlayerStateManager.Instance.PlayingList[i].ShowCardsFromBlind();


            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ShowingResults);
            Debug.Log("Here is your error 2......." + RoomStateManager.Instance.CurrentRoomState);
        }



        public void ResetAllDataAndNewGamestart()
        {
            if (MatchHandler.IsTeenPatti())
                StartCoroutine(ResetAllDataAndNewGameBegin(LocalSettings.ShowingResultAndResetDelayTime));
            else if(MatchHandler.IsPoker())
                StartCoroutine(ResetAllDataAndNewGameBegin(LocalSettings.ShowingResultAndResetDelayTime - 3));
        }

        IEnumerator ResetAllDataAndNewGameBegin(float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);

            if (MatchHandler.IsTeenPatti())
                GameResetManager.Instance.ResetGameTeenPatti();
            else if (MatchHandler.IsPoker())
                GameResetManager.Instance.ResetGamePoker();

        }
    }
    // Comparison of player ranks and their high Cards
    #region Comparison of player ranks and their high Cards
    public class PlayerDataNew
    {
        public int rank;
        public int[] intCardsArray;
        public int PlayerViewID;
    }
    public class PlayerComparer : IComparer<PlayerDataNew>
    {
        public int Compare(PlayerDataNew p1, PlayerDataNew p2)
        {
            // Compare ranks first
            int rankComparison = p2.rank.CompareTo(p1.rank);
            if (rankComparison != 0)
            {
                return rankComparison;
            }

            // If ranks are the same, compare integer arrays
            for (int i = 0; i < p1.intCardsArray.Length; i++)
            {
                int intComparison = p2.intCardsArray[i].CompareTo(p1.intCardsArray[i]);
                if (intComparison != 0)
                {
                    return intComparison;
                }
            }

            // If everything is equal, return 0
            return 0;
        }
    }
    #endregion

    #region Comparison of player ranks and their high Cards
    [System.Serializable]
    public class PlayerDataRanks
    {
        public int viewID;
        public int Rank;
        public int Scores;
    }
    public class PlayerDataComparer : IComparer<PlayerDataRanks>
    {
        public int Compare(PlayerDataRanks x, PlayerDataRanks y)
        {
            int result = y.Rank.CompareTo(x.Rank);

            if (result == 0)
            {
                result = y.Scores.CompareTo(x.Scores);
            }

            return result;
        }
    }
    #endregion
}