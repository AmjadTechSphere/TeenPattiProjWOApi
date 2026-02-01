using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatchHandler
{
    public enum MATCH
    {
        Classic = 0,
        Mufflis = 1,
        HUukm = 2,
        Andar_Bahar = 3,
        Wingo_Lottery = 4,
        Poker = 5,
        Lucky_War = 6,
        Dragon_Tiger = 7,
        VrAK47 = 8,
        DeluxeClassic = 9,
        DeluxeJoker = 10,
        DeluxeHukam = 11,
        DeluxeMuflis = 12,
        DeluxePotBlind = 13,
        VrMuflis = 14,
        Chatai = 15,
        Rummy = 16,
        DeluxeTable = 17,
        BagumPakad = 18,
        PlauOnHotspot = 19,
        PlayWithFriends = 20,
        None = 21
    };

    public static MATCH CurrentMatch;

    public static bool IsTeenPatti()
    {
        return CurrentMatch == MATCH.Classic || CurrentMatch == MATCH.Mufflis || CurrentMatch == MATCH.HUukm;
    }

    public static bool IsPoker()
    {
        return CurrentMatch == MATCH.Poker;
    }

    public static bool IsAndarBahar()
    {
        return CurrentMatch == MATCH.Andar_Bahar;
    }
    public static bool IsLuckyWar()
    {
        return CurrentMatch == MATCH.Lucky_War;
    }

    public static bool isWingoLottary()
    {
        return CurrentMatch == MATCH.Wingo_Lottery;
    }

    public static bool isDragonTiger()
    {
        return CurrentMatch == MATCH.Dragon_Tiger;
    }

}
