
using TMPro;
using UnityEngine;

public class EmailFields : MonoBehaviour
{
    public TMP_Text DateOfEmail;
    int idToOpenEmail;
    [ShowOnly]
    public int indexOfEmail;
 

    

    

    // _______________________________________________________________________
    // History fields
    int idToOpenEmailHistory;
    [ShowOnly]
    public int indexOfEmailHistory;
    public void EmailHistoryListFieldsFill(EmailHistoryList emailHistory, int index)
    {
        indexOfEmailHistory = index;
        DateOfEmail.text = emailHistory.players[indexOfEmailHistory].updated_at.ToString();
        idToOpenEmailHistory = emailHistory.players[indexOfEmailHistory].id;
    }

    public void OnEmailHistoryBtnClick()
    {
        //EmailHistory
        //EmailHistory.Instance.OpenhistoryEmail(int playerid, dateupdated);
        EmailHistroy.Instance.OpenhistoryEmail(indexOfEmailHistory);

    }
}
