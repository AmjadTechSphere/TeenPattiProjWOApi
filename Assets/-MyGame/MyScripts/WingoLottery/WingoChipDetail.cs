using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class WingoChipDetail : MonoBehaviour
    {
        public int ViewID;
        public int PointNumber;
        public BigInteger BetAmount;
        public bool IsGoToPlayer;

        public Sprite[] BetAmountSprites;
        Image ShipIcon;

        public void ChangeChipSprite()
        {
            //if (BetAmount == 1000)
            //    GetComponent<Image>().sprite = BetAmountSprites[0];
            //else 
            if (MatchHandler.isWingoLottary())
            {
                if (BetAmount == WingoManager.Instance.bet10xInt)
                    GetComponent<Image>().sprite = BetAmountSprites[1];
                else if (BetAmount == WingoManager.Instance.bet50xInt)
                    GetComponent<Image>().sprite = BetAmountSprites[2];
                else if (BetAmount == WingoManager.Instance.bet100xInt)
                    GetComponent<Image>().sprite = BetAmountSprites[3];
            }
            else if (MatchHandler.isDragonTiger())
            {
                if (BetAmount == DragonTigerManager.Instance.bet10xInt)
                    GetComponent<Image>().sprite = BetAmountSprites[1];
                else if (BetAmount == DragonTigerManager.Instance.bet50xInt)
                    GetComponent<Image>().sprite = BetAmountSprites[2];
                else if (BetAmount == DragonTigerManager.Instance.bet100xInt)
                    GetComponent<Image>().sprite = BetAmountSprites[3];
            }
        }


    }
}