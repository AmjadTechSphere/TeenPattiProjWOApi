using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuickShopManager : MonoBehaviour
{
    public TMP_Text chipText;


    private void OnEnable()
    {
        chipText.text= LocalSettings.FormatNumber(LocalSettings.GetTotalChips().ToString());
    }
}
