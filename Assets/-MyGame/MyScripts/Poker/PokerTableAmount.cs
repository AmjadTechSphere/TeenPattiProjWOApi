using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace com.mani.muzamil.amjad
{
    public class PokerTableAmount : MonoBehaviour
    {
        public Slider BuyInAmountSlider;
        public TMP_Text SlideBuyInAmountTxt;
        public TMP_Text BuyInMinAmountTxt;
        public TMP_Text BuyInMaxAmountTxt;
        public TMP_Text BlindAmountTxt;
        public TMP_Text PlayerTotalAmountTxt;
        public BigInteger OffSet;
        public BigInteger CurrentBuyInAmount;


        private static PokerTableAmount _instance;

        public static PokerTableAmount Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<PokerTableAmount>();
                return _instance;
            }
        }


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
        private void Start()
        {
            if (!MatchHandler.IsPoker())
                return;
            if (LocalSettings.GetTotalChips() >= LocalSettings.GetStartingMinAmountPoker())
                AssignStartSliderValues();
            //PokerAddAmount();


        }

        void PokerAddAmount()
        {
            if (LocalSettings.GetPokerBuyInChips() == 0)
            {
                BigInteger amount = LocalSettings.GetStartingMinAmountPoker();
                Debug.LogError("poker entry amount......" + amount);
                GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, amount.ToString());
                LocalSettings.SetPokerBuyInChips(amount);
                PokerManager.Instance.BuyInCashPanel.SetActive(false);
                OnJoinNowBtnClick();
            }
        }
        public void AssignStartSliderValues()
        {
            BuyInMinAmountTxt.text = /*"Min Buy-in \n" + */ LocalSettings.Rs(LocalSettings.GetStartingMinAmountPoker());
            BuyInMaxAmountTxt.text = /*"Max Buy-in \n" +*/ LocalSettings.Rs(LocalSettings.GetStartingMaxAmountPoker());
            OffSet = LocalSettings.GetStartingMinAmountPoker();
            BlindAmountTxt.text = "Blinds: " + LocalSettings.Rs(LocalSettings.GetBlindAmountPoker() / 2) + "/" + LocalSettings.Rs(LocalSettings.GetBlindAmountPoker());
            OnSliderValueChange();
            PlayerTotalAmountTxt.text =/*"Gold: " +*/ LocalSettings.Rs(LocalSettings.GetTotalChips());
            BuyInAmountSlider.minValue = 0;
            BigInteger divider = (LocalSettings.GetBlindAmountPoker() / 2) * 10;
            BuyInAmountSlider.maxValue = (int)((LocalSettings.GetStartingMaxAmountPoker() - OffSet) / divider);
        }

        public void OnSliderValueChange()
        {
            //double CurrentSliderVal = BuyInAmountSlider.value;
            //BigInteger finalSliderVal = LocalSettings.GetStartingMaxAmountPoker() - OffSet;
            //BigInteger currentAmount = (BigInteger)(CurrentSliderVal * (double)finalSliderVal);
            //CurrentBuyInAmount = currentAmount + OffSet;
            
            int increment = (Mathf.RoundToInt(BuyInAmountSlider.value));
            BigInteger multiplayer = (LocalSettings.GetBlindAmountPoker() / 2) * 10;
            CurrentBuyInAmount = (increment * multiplayer) + OffSet;
            Debug.Log(" Checek Current Amount" + CurrentBuyInAmount + "....." + LocalSettings.Rs((CurrentBuyInAmount)) + "  Increment.." + " Muliplyaer.." + multiplayer + "  Offset.." + OffSet);
            SlideBuyInAmountTxt.text = LocalSettings.Rs((CurrentBuyInAmount));
            WhenSliderAmountExceedsTotalAmount();
        }

        void WhenSliderAmountExceedsTotalAmount()
        {
            if (CurrentBuyInAmount > LocalSettings.GetTotalChips())
            {
                BuyInAmountSlider.value -= 1f;
                OnSliderValueChange();
            }
        }
        public void SetSliderAmount(string sign)
        {
            BuyInAmountSlider.value = sign == "+" ? BuyInAmountSlider.value + 1 : BuyInAmountSlider.value - 1;
        }

        public void OnJoinNowBtnClick()
        {
            if (LocalSettings.GetTotalChips() >= CurrentBuyInAmount)
            //if (LocalSettings.GetTotalChips() >= LocalSettings.GetStartingMinAmountPoker())
            {


                LocalSettings.SetTotalChips(-CurrentBuyInAmount);
                GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.bet, CurrentBuyInAmount.ToString());
                LocalSettings.SetPokerBuyInChips(CurrentBuyInAmount);

                GameManager.Instance.StartingThings();
                PokerManager.Instance.SetStartingAmount();
                PokerActionPanel.Instance.StartForSelectAmount();
            }
            else
            {
                UIManager.Instance.InfoTxt.text = "Not Enough Cash";
                UIManager.Instance.InfoObj.SetActive(true);
            }
        }
    }
}
