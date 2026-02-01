using com.mani.muzamil.amjad;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviourPunCallbacks
{




    public void ActivateInfoPanel()
    {
        playerinfo(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
        //switch (MatchHandler.CurrentMatch)
        //{
        //    case MatchHandler.MATCH.CLASSIC:
        //        playerinfo(LocalSettings.totalcashWinLossKeyClassic, LocalSettings.TotalHandsKeyClassic, LocalSettings.WinHandsKeyClassic);
        //        break;
        //    case MatchHandler.MATCH.MUFFLIS:
        //        playerinfo(LocalSettings.totalcashWinLossKeyMufflis, LocalSettings.TotalHandsKeyMufflis, LocalSettings.WinHandsKeyMufflis);
        //        break;
        //    case MatchHandler.MATCH.HUKM:
        //        playerinfo(LocalSettings.totalcashWinLossKeyHukm, LocalSettings.TotalHandsKeyHukm, LocalSettings.WinHandsKeyHukm);
        //        break;
        //    case MatchHandler.MATCH.ANDAR_BAHAR:
        //        playerinfo(LocalSettings.totalcashWinLossKeyAB, LocalSettings.TotalHandsKeyAB, LocalSettings.WinHandsKeyAB);
        //        break;
        //    case MatchHandler.MATCH.WINGOLOTTARY:
        //        playerinfo(LocalSettings.totalcashWinLossKeyWL, LocalSettings.TotalHandsKeyWL, LocalSettings.WinHandsKeyWL);
        //        break;
        //    case MatchHandler.MATCH.LuckyWar:
        //        break;
        //    case MatchHandler.MATCH.VrMuflis:
        //        break;
        //    case MatchHandler.MATCH.VrRoyal:
        //        break;
        //    case MatchHandler.MATCH.VrAK47:
        //        break;
        //    case MatchHandler.MATCH.DeluxeClassic:
        //        break;
        //    case MatchHandler.MATCH.DeluxeJoker:
        //        break;
        //    case MatchHandler.MATCH.DeluxeHukam:
        //        break;
        //    case MatchHandler.MATCH.DeluxeMuflis:
        //        break;
        //    case MatchHandler.MATCH.DeluxePotBlind:
        //        break;
        //    case MatchHandler.MATCH.Poker:
        //        break;
        //    case MatchHandler.MATCH.Chatai:
        //        break;
        //    case MatchHandler.MATCH.Rummy:
        //        break;
        //    case MatchHandler.MATCH.DeluxeTable:
        //        break;
        //    case MatchHandler.MATCH.BagumPakad:
        //        break;
        //    case MatchHandler.MATCH.PlauOnHotspot:
        //        break;
        //    case MatchHandler.MATCH.PlayWithFriends:
        //        break;
        //    case MatchHandler.MATCH.None:
        //        break;

        //}


        UIManager.Instance.PlayerInfoPanel.SetActive(true);
    }






    void playerinfo(string totalWinLoseCashkey, string TotalhandsKey, string Winhandskey)
    {
        PlayerInfo pinfo = GetComponent<PlayerInfo>();
        Player this_player = pinfo.player;
        // Sprite profileSprite = GameManager.Instance.PlayerProfileImages.Sprites[this_player.GetCustomData(LocalSettings.ProfilePic)];

        Sprite profileSprite = GetComponent<PlayerInfo>().PlayerAvatorImage.sprite;
        int ProfileFrameSize = this_player.GetCustomData(LocalSettings.ProfileFrame);
        string name = this_player.NickName;
        BigInteger totalWinLoss = this_player.GetCustomBigIntegerData(totalWinLoseCashkey);

        int totalHands = this_player.GetCustomData(TotalhandsKey);
        int winHands = this_player.GetCustomData(Winhandskey);
        BigInteger cashAmount = this_player.GetCustomBigIntegerData(LocalSettings.MyTotalCashKey);

        string totalWinLossT = totalWinLoss > 0 ? "+" + totalWinLoss : "" + totalWinLoss;
        int NumberLevel = (int)this_player.GetCustomData(LocalSettings
            .PlayerTotalLevelKey);
        int player_ID = this_player.GetCustomData(LocalSettings.player_ID_Key);
        int player_Incremeted_ID = this_player.GetCustomData(LocalSettings.player_Incremented_ID_Key);

        UIManager.Instance.addFriendBtn.onClick.RemoveAllListeners();
        UIManager.Instance.addFriendBtn.onClick.AddListener(() => OnClickAddFriendBtn(player_ID, player_Incremeted_ID));
         UIManager.Instance.addFriendBtn.gameObject.SetActive(!pinfo.photonView.IsMine ? true : false);
        UIManager.Instance.fillerLevelImage.SetActive(pinfo.photonView.IsMine ? true : false);

        UIManager.Instance.UpdateTexts(totalWinLossT, (winHands + "/" + totalHands), LocalSettings.Rs(cashAmount), profileSprite, ProfileFrameSize, name, NumberLevel);
    }


    void OnClickAddFriendBtn(int playerID, int IncrementedID)
    {
        Debug.LogError("Show Player ID...." + playerID + "  with Incremented ID..." + IncrementedID);
        FriendListTD.Instance.SearchFriendByPlayerIDInGamePlay(playerID.ToString());
    }




}
