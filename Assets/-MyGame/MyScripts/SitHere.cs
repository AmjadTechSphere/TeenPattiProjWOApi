using com.mani.muzamil.amjad;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SitHere : MonoBehaviour
{
    public int positionToSit;

    private void OnEnable()
    {
        RegisterListener();
    }

    void RegisterListener()
    {
        Button local_button = GetComponent<Button>();
        local_button.onClick.RemoveAllListeners();
        local_button.onClick.AddListener(SetThisPositionByPlayer);
    }

    public void SetThisPositionByPlayer()
    {
        if ((LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount && !MatchHandler.IsPoker()) && (LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount && !MatchHandler.IsTeenPatti()))
        {
            if (UIManager.Instance.GetMyPlayerInfo().photonView.IsMine)
            {
                UIManager.Instance.quickShop.SetActive(true);
                UIManager.Instance.GetMyPlayerInfo().StandUp();
                return;
            }

        }
        if (MatchHandler.isWingoLottary())
        {
            WingoManager.Instance.BetAmountsParent.SetActive(true);
        }
        if (MatchHandler.isDragonTiger())
        {
            DragonTigerManager.Instance.BetAmountsParent.SetActive(true);
        }
        UIManager uIManager = UIManager.Instance;
        if (MatchHandler.IsPoker())
        {
            if (LocalSettings.GetTotalChips() < LocalSettings.GetStartingMinAmountPoker())
            {
                if (UIManager.Instance.GetMyPlayerInfo().photonView.IsMine)
                {
                    UIManager.Instance.quickShop.SetActive(true);
                    UIManager.Instance.GetMyPlayerInfo().StandUp();
                    return;
                }
            }
            if (uIManager.GetMyPlayerInfo().PokerTotalCash < LocalSettings.MinBetAmount)
            {
               PokerTableAmount.Instance.PlayerTotalAmountTxt.text =/*"Gold: " +*/ LocalSettings.Rs(LocalSettings.GetTotalChips());
                PokerManager.Instance.BuyInCashPanel.SetActive(true);
                PokerManager.Instance.sitPosAfterReset = positionToSit;
                return;
            }
        }

        PositionsManager.Instance.SitHere(positionToSit);
        Invoke(nameof(RefreshAfterSomeTime), 0.5f);
        GameManager.Instance.SitHereBtnStatus(false);
    }

    void RefreshAfterSomeTime()
    {
        PositionsManager.Instance.AssignMyLocalPositionWithAllOtherClients();
    }

}
