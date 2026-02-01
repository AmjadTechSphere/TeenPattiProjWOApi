using com.mani.muzamil.amjad;
using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LWActionPanelScript : MonoBehaviour
{

    public GameObject goToWarPanel;
    //  Buttons
    public Button dealBtn;
    public Button tieBetBtn;
    public Button tieNveBtn;
    public Button tiePveBtn;
    public Button betBetBtn;
    public Button betNveBtn;
    public Button betPveBtn;
    // Texts
    public TMP_Text TieBetAmountBtnTxt;
    public TMP_Text BetBetAmountBtnTxt;
    // Bools
    public bool isTurnPass = false;

    #region Creating Instance
    private static LWActionPanelScript _instance;
    public static LWActionPanelScript Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<LWActionPanelScript>();
            return _instance;
        }
    }
    #endregion
    void Awake()
    {
        if (_instance == null)
            _instance = this;
        //Time.timeScale = 4;
    }
    // Start is called before the first frame update
    UIManager uIManager;
    void Start()
    {

        uIManager = UIManager.Instance;
    }


    //Reset Action Panel Btn
    public void ResetActionBtn()
    {
        isTurnPass = false;
        betNveBtn.interactable = false;
        tieNveBtn.interactable = false;
        EnableDisableBetBtn(true);
        EnableDisableTieBet(false);
    }
    //private void Update()
    //{

    //    if (LuckyWarManager.Instance.LWActionPanel.activeInHierarchy)
    //    {
    //        if (!LuckyWarManager.Instance.checkTieWinner())
    //        {
    //            LuckyWarBetting("4");
    //            LuckyWarBetting("1");
    //            LuckyWarManager.Instance.LWActionPanel.SetActive(false);
    //        }
    //    }

    //}

    #region LuckyWar Btns
    public void LuckyWarBetting(string BtnNumber)
    {
        LuckyWarManager luckyWar = LuckyWarManager.Instance;



        switch (BtnNumber)
        {
            case "1":
                // Tie bet btn
                if (LocalSettings.GetTotalChips() == luckyWar.TieExtraTotalBetAmount)
                    luckyWar.TieTotalAmount = LocalSettings.GetTotalChips();
                uIManager.TotalBetPlacedAmount += luckyWar.TieTotalAmount;
                uIManager.TotalBetPlaceFor1Game += luckyWar.TieTotalAmount;
                PlaceTie();
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
                break;
            case "2":
                // Tie - btn
                tiePveBtn.interactable = true;
                if (LocalSettings.GetTotalChips() != luckyWar.TieExtraTotalBetAmount)
                {
                    // Debug.LogError(uIManager.AndarNveBtn.interactable);
                    BigInteger ANBetAmount = luckyWar.TieTotalAmount / 2;
                    if (LocalSettings.MinBetAmount <= ANBetAmount)
                    {
                        luckyWar.TieTotalAmount = ANBetAmount;
                        if (LocalSettings.MinBetAmount >= ANBetAmount)
                            tieNveBtn.interactable = false;
                    }
                }
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);

                luckyWar.TieExtraTotalBetAmount = 0;
                // Debug.LogError(uIManager.AndarNveBtn.interactable);
                TieBetAmountBtnTxt.text = LocalSettings.Rs(luckyWar.TieTotalAmount);
                break;
            case "3":
                // Tie + btn
                tieNveBtn.interactable = true;
                BigInteger APBetAmount = luckyWar.TieTotalAmount * 2;
                if (LocalSettings.GetTotalChips() >= APBetAmount)
                {

                    // if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 2) >= APBetAmount)
                    // {
                    if ((Pot.instance.maximumTieBetAmount) >= APBetAmount)
                    {
                        luckyWar.TieTotalAmount = APBetAmount;
                        TieBetAmountBtnTxt.text = LocalSettings.Rs(luckyWar.TieTotalAmount);
                        //   if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 2) <= APBetAmount)
                        //  {
                        // Debug.LogError("False Andar button");
                        // uIManager.AndarPveBtn.interactable = false;
                        // }
                        //}
                    }

                }
                else
                {
                    luckyWar.TieExtraTotalBetAmount = LocalSettings.GetTotalChips();
                    TieBetAmountBtnTxt.text = LocalSettings.Rs(luckyWar.TieExtraTotalBetAmount);
                }

                if (LocalSettings.GetTotalChips() <= APBetAmount || (Pot.instance.maximumTieBetAmount) <= APBetAmount)
                {
                    //AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount = LocalSettings.GetTotalChips();
                    // uIManager.AndarBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.AndarExtraTotalBetAmount);
                    tiePveBtn.interactable = false;
                }
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
                break;
            case "4":
                // Bahar bet Btn
                if (LocalSettings.GetTotalChips() == luckyWar.BetExtraTotalBetAmount)
                    luckyWar.BetTotalAmount = LocalSettings.GetTotalChips();

                uIManager.TotalBetPlacedAmount += luckyWar.BetTotalAmount;
                uIManager.TotalBetPlaceFor1Game += luckyWar.BetTotalAmount;
                PlaceBet();
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
                break;
            case "5":
                // Bahar - btn
                betPveBtn.interactable = true;
                if (LocalSettings.GetTotalChips() != luckyWar.BetExtraTotalBetAmount)
                {

                    BigInteger BNBetAmount = luckyWar.BetTotalAmount / 2;
                    if (LocalSettings.MinBetAmount <= BNBetAmount)
                    {
                        luckyWar.BetTotalAmount = BNBetAmount;
                        if (LocalSettings.MinBetAmount >= BNBetAmount)
                            betNveBtn.interactable = false;
                    }
                }

                BetBetAmountBtnTxt.text = LocalSettings.Rs(luckyWar.BetTotalAmount);
                luckyWar.BetExtraTotalBetAmount = 0;
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
                break;
            case "6":
                // bahar + btn
                betNveBtn.interactable = true;
                BigInteger BPBetAmount = luckyWar.BetTotalAmount * 2;
                if (LocalSettings.GetTotalChips() > BPBetAmount)
                {
                    if (Pot.instance.maximumBet >= BPBetAmount)
                    {
                        // if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 2) >= BPBetAmount)
                        // {
                        luckyWar.BetTotalAmount = BPBetAmount;
                        BetBetAmountBtnTxt.text = LocalSettings.Rs(luckyWar.BetTotalAmount);
                    }
                    // if (LocalSettings.MinABBetAmount * (LocalSettings.PotLimitMultiplier * 2) <= BPBetAmount)
                    // uIManager.BaharPveBtn.interactable = false;
                    // }
                }
                else
                {
                    luckyWar.BetExtraTotalBetAmount = LocalSettings.GetTotalChips();
                    BetBetAmountBtnTxt.text = LocalSettings.Rs(luckyWar.BetExtraTotalBetAmount);
                }
                if (LocalSettings.GetTotalChips() <= BPBetAmount || (Pot.instance.maximumBet) <= BPBetAmount)
                {
                    // AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount = LocalSettings.GetTotalChips();
                    // uIManager.BaharBetAmountBtnTxt.text = LocalSettings.Rs(AndarBaharPositionsManager.Instance.BaharExtraTotalBetAmount);
                    betPveBtn.interactable = false;
                }
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
                break;
            default:
                break;
            case "7":
                // Skip bet btn
                Deal_Bet();
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
                break;
        }
    }


    void PlaceTie()
    {
        uIManager.GetMyPlayerInfo().PlaceBetTie();
        DisableBtnsOnBetLW();
        DisableBtnsOnSuperBet();
    }
    void DisableBtnsOnBetLW()
    {
        //if (secondTurnTurnAb)
        // {

        LuckyWarManager.Instance.LWActionPanel.SetActive(false);
        
        // for need in lukcyWar In when Tie is win
        //  Invoke(nameof(ShowSuperABActionPanel), UIManager.Instance.BetAmountToTargetAnim().TimeToCompleteAnim);        


        uIManager.SkipBetBtn.interactable = true;
        //Debug.LogError("Starting camparison");
        LuckyWarManager.Instance.TieTotalAmount = LuckyWarManager.Instance.BetTotalAmount = LocalSettings.MinBetAmount;
        BetBetAmountBtnTxt.text = TieBetAmountBtnTxt.text = LocalSettings.Rs(LocalSettings.MinBetAmount);
        uIManager.AndarNveBtn.interactable = false;
        uIManager.BaharNveBtn.interactable = false;
        //PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.PlayerTurnDurationAndarBahar;
        // secondTurnTurnAb = true;
        // Skip_Bet_TurnManager = true;
    }
    void PlaceBet()
    {
        uIManager.GetMyPlayerInfo().PlaceBetBet();
        StartCoroutine(waitForTieBetActive());
    }

    IEnumerator waitForTieBetActive()
    {

        isTurnPass = true;
        betNveBtn.interactable = false;
        EnableDisableBetBtn(false);
        yield return new WaitForSeconds(uIManager.BetAmountToTargetAnim().TimeToCompleteAnim - 0.5f);
        EnableDisableTieBet(true);
        if (LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount)
        {
            UIManager.Instance.quickShop.SetActive(true);
            DisableBtnsOnSuperBet();
            LuckyWarManager.Instance.LWActionPanel.SetActive(false);

        }
        else
            EnableDisableTieBet(true);


    }

    public void EnableDisableBetBtn(bool istrue)
    {
        betBetBtn.interactable = istrue;
        betPveBtn.interactable = istrue;

    }
    public void EnableDisableTieBet(bool isTrue)
    {
        tieBetBtn.interactable = isTrue;
        tiePveBtn.interactable = isTrue;
        dealBtn.interactable = isTrue;
    }
    public void Deal_Bet()
    {
        DisableBtnsOnSuperBet();
    }
    public void DisableBtnsOnSuperBet()
    {

        if (uIManager.GetMyPlayerInfo() != null)
            uIManager.GetMyPlayerInfo().TurnOnOffLW(false);
        LuckyWarManager.Instance.LWActionPanel.SetActive(false);

        // ToggleABActionPanelBtns(true);

    }


    public void GOToWarBtn()
    {
        goToWarPanel.SetActive(false);
        if (LocalSettings.GetTotalChips() >= uIManager.GetMyPlayerInfo().player.GetCustomBigIntegerData(LocalSettings.LWBetAmountKey)){
            LuckyWarBetting("4");
            LuckyWarManager.Instance.onGotoWarBtnClick();
        }
        else{
            Surrender();
           // Debug.LogError("check Total Hands win...4");
        }
        isTurnPass = false;
        // LuckyWarManager.Instance.StartCardShuffling(false);
        uIManager.GetMyPlayerInfo().TurnOnOffLW(false);
        //LuckyWarManager.Instance.SettingRandomCardsArrayToRoomProperty();
        // LuckyWarManager.Instance.AssignCardToTieBetWinner();

        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
    }





    public void Surrender()
    {
        goToWarPanel.SetActive(false);
        isTurnPass = false;
        if (PlayerStateManager.Instance.PlayingList.Count <= 1)
        {

            // uIManager.GetMyPlayerInfo().TurnOnOffTieWinLW(false);
            uIManager.GetMyPlayerInfo().TurnOnOffLW(false);
            LuckyWarManager.Instance.OnSurrenderBetAmountGone(uIManager.GetMyPlayerInfo());
           // Debug.LogError("check Total Hands win...3");
           // LuckyWarManager.Instance.ResetLWGame();
        }
        else
        {
            uIManager.GetMyPlayerCurrentState().UpdateCurrentPlayerState(PlayerState.STATE.Packed);
        }
        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound, false);
    }



    #endregion
}
