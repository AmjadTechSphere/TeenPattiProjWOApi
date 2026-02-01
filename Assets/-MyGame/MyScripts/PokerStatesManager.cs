using com.mani.muzamil.amjad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PokerState
{
    DEFAULT,
    NO_COMMUNITY_CARDS,
    THREE_COMMUNITY_CARDS,
    FOUR_COMMUNITY_CARDS,
    FIVE_COMMUNITY_CARDS
}

public class PokerStatesManager : MonoBehaviour
{

    public PokerState currentState;

    public int D_Index;

    #region Creating Instance
    private static PokerStatesManager _instance;
    public static PokerStatesManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<PokerStatesManager>();
            return _instance;
        }
    }
    #endregion


    private void Awake()
    {
        if (!MatchHandler.IsPoker())
        {
            gameObject.SetActive(false);
            return;
        }
        if (_instance == null)
            _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!MatchHandler.IsPoker())
        {
            gameObject.SetActive(false);
            return;
        }
    }


    public void SetDIndexRoomProperty(int val)
    {
        LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.DIndexKey, val);
    }


    public int GetDIndexRoomProperty()
    {
        int currentDIndex = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.DIndexKey);
        if (currentDIndex >= PlayerStateManager.Instance.PlayingList.Count)
        {
            SetDIndexRoomProperty(0);
            currentDIndex = 0;
        }
        return currentDIndex;
    }

    public void UpdateDIndex()
    {
        D_Index += 1;
        if (PlayerStateManager.Instance.PlayingList.Count > 0)
        {
            if (D_Index % PlayerStateManager.Instance.PlayingList.Count == 0)
            {
                D_Index = 0;
            }
        }
        SetDIndexRoomProperty(D_Index);
    }


    public void updatePokerState(PokerState pokerState)
    {
        //return;
        currentState = pokerState;
        UpdateTheCurrentCardsStatus(pokerState);
    }


    public void UpdateTheCurrentCardsStatus(PokerState pokerState)
    {
        if (pokerState == PokerState.NO_COMMUNITY_CARDS)
        {
            PokerCheckWinner.Instance.UpdateRankOnNoCardState();
        }
        else if (pokerState == PokerState.THREE_COMMUNITY_CARDS)
        {
            PokerCheckWinner.Instance.UpdateRankOnThreeCardState();
        }
        else if (pokerState == PokerState.FOUR_COMMUNITY_CARDS)
        {
            PokerCheckWinner.Instance.UpdateRankOnFourCardState();
        }
        else if (pokerState == PokerState.FIVE_COMMUNITY_CARDS)
        {
            for (int i = 0; i < PlayerStateManager.Instance.PlayingList.Count; i++)
            {
                // PokerCheckWinner.Instance.UpdateRankOnFiveCardState(PlayerStateManager.Instance.PlayingList[i]);
                if (PlayerStateManager.Instance.PlayingList[i].photonView.IsMine)
                {
                    PokerCheckWinner.Instance.UpdateRankOnFiveCardState(PlayerStateManager.Instance.PlayingList[i]);
                }
            }
        }
    }


}
