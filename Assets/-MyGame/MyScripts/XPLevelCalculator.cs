using Photon.Pun;
using System;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPLevelCalculator : MonoBehaviour
{
    public TMP_Text CurrentLevelLabel;
    public TMP_Text nextLevelRewardTxt;
    public int currentXP = 0;
    int xpRequiredForLevel1 = 20;
    float xpIncreaseFactor = 1.3f;

    public Image XPFiller;


    [ShowOnly]
    public int level;
    public int requireXpToIncreaseLevel;
    int oldxp;


    public int xpForNextLevel;

    // instance Creating of Rest API Script
    #region Creating Instance;
    private static XPLevelCalculator _instance;
    public static XPLevelCalculator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<XPLevelCalculator>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    #endregion

    private void Start()
    {
        xpRequiredForLevel1 = 130;
        xpIncreaseFactor = 1.5f;

        UpDateXpFromServer(LocalSettings.TotalXpMyPlayer.ToString());
    }

    public int CurrentLevel
    {
        get
        {
            level = CalculateLevel();


           // Debug.LogError("check calculation");

            if (LocalSettings.IsMenuScene())
            {
                CurrentLevelLabel.text = "Level " + level;
                BigInteger getRewardNext = LocalSettings.levelUpRewardAmount * (level + 1);
                nextLevelRewardTxt.text = "Get " + LocalSettings.Rs(getRewardNext);
                PhotonNetwork.LocalPlayer.SetCustomData(LocalSettings.PlayerTotalLevelKey, level);
            }
            return level;
        }
    }

    private void Update()
    {
        if (currentXP != oldxp)
        {
            oldxp = currentXP;
            int aa = CurrentLevel;
        }
    }

    float divider = 1;
    float oldValueFornextLevel;
    private int CalculateLevel()
    {
        int level = 0;
        xpForNextLevel = xpRequiredForLevel1;

        while (currentXP >= xpForNextLevel)
        {
            level++;
            oldValueFornextLevel = xpForNextLevel;
            xpForNextLevel = Mathf.RoundToInt(xpRequiredForLevel1 + (xpForNextLevel * xpIncreaseFactor));
            divider = xpForNextLevel - oldValueFornextLevel;
        }
        requireXpToIncreaseLevel = xpForNextLevel - currentXP;

        float diff = (float)xpForNextLevel - (float)oldValueFornextLevel;
        float newVal = requireXpToIncreaseLevel / diff;

        XPFiller.fillAmount = 1 - newVal;

        return level;
    }

    public void UpDateXpFromServer(string xpString)
    {
        currentXP = int.Parse(xpString);
        int lvl = CurrentLevel;


    }
}
