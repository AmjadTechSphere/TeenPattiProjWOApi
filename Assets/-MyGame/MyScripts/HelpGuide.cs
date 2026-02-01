using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpGuide : MonoBehaviour
{
    public int number;
    public GameObject[] GameHelpPanels;
    GameObject CurrentGameHelpParent;
    int currentPageIndex = 0;

    private void OnEnable()
    {
        AssigningAndActivatingHelpPanel();
    }
    public void AssigningAndActivatingHelpPanel()
    {
        foreach (GameObject go in GameHelpPanels)
            if (go.activeInHierarchy)
                go.SetActive(false);
        // Assigning current game mode help panel
        switch (MatchHandler.CurrentMatch)
        {
            case MatchHandler.MATCH.Classic:
                currentPageIndex = 0;
                CurrentGameHelpParent = GameHelpPanels[0];
                break;
            case MatchHandler.MATCH.Mufflis:
                currentPageIndex = 0;
                CurrentGameHelpParent = GameHelpPanels[1];
                break;
            case MatchHandler.MATCH.HUukm:
                currentPageIndex = 0;
                CurrentGameHelpParent = GameHelpPanels[2];
                break;
            case MatchHandler.MATCH.Andar_Bahar:
                currentPageIndex = 0;
                CurrentGameHelpParent = GameHelpPanels[3];
                break;
            case MatchHandler.MATCH.Wingo_Lottery:
                currentPageIndex = 0;
                CurrentGameHelpParent = GameHelpPanels[4];
                break;
            case MatchHandler.MATCH.Lucky_War:
                currentPageIndex = 0;
                CurrentGameHelpParent = GameHelpPanels[5];
                break;
            case MatchHandler.MATCH.Dragon_Tiger:
                currentPageIndex = 0;
                CurrentGameHelpParent = GameHelpPanels[6];
                break;
            case MatchHandler.MATCH.Poker:
                currentPageIndex = 0;
                CurrentGameHelpParent = GameHelpPanels[7];
                break;
        }
        currentPageIndex = 0;
        GameHelpPanels[0].transform.parent.gameObject.SetActive(true);
        CurrentGameHelpParent.SetActive(true);
       // ActivatePanel(currentPageIndex);
    }
    public void onNextBtnClick(bool isNextPage)
    {
        int temp = isNextPage ? currentPageIndex++ : currentPageIndex--;

        if (currentPageIndex < 0)
            currentPageIndex = 0;
        if (currentPageIndex >= CurrentGameHelpParent.transform.childCount)
            currentPageIndex = CurrentGameHelpParent.transform.childCount - 1;

        ActivatePanel(currentPageIndex);
    }

    void ActivatePanel(int childIndex)
    {
        for (int i = 0; i < CurrentGameHelpParent.transform.childCount; i++)
        {
            if (CurrentGameHelpParent.transform.GetChild(i).gameObject.activeInHierarchy)
                CurrentGameHelpParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        CurrentGameHelpParent.transform.GetChild(childIndex).gameObject.SetActive(true);
    }
}