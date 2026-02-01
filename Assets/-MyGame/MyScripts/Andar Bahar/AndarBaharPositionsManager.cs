using com.mani.muzamil.amjad;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class AndarBaharPositionsManager : MonoBehaviourPunCallbacks
    {

        public GameObject[] objectsToDisable;
        public GameObject[] objectsToEnable;
        public RectTransform[] ABposition;
        public GameObject ABTable;
        public GameObject ABActionPanel;
        public GameObject SuperABActionPanel;
        // public GameObject buyPanel;
        public GameObject SuperBaharlWinPanel;
        public GameObject GirlsTeenPatti;



        public TMP_Text SuperBaharWinTxt;
        public TMP_Text TotalHandsAndSB;
        public TMP_Text TotalABWins;


        public RectTransform AllCardsPos;
        public RectTransform PlayCardPos;
        public RectTransform AndarCardPos;
        public RectTransform BaharCardPos;


        [ShowOnly]
        public BigInteger AndarTotalBetAmount;
        [ShowOnly]
        public BigInteger BaharTotalBetAmount;
        [ShowOnly]
        public BigInteger AndarExtraTotalBetAmount;
        [ShowOnly]
        public BigInteger BaharExtraTotalBetAmount;
        [ShowOnly]
        public BigInteger SuperAndarTotalBetAmount;
        [ShowOnly]
        public BigInteger SuperBaharTotalBetAmount;
        [ShowOnly]
        public BigInteger firstSuperBaharBet;
        [ShowOnly]
        public BigInteger secondSuperBaharBet;


        [ShowOnly]
        public bool isCardMatched = false;
        [ShowOnly]
        public bool isNumberMatched = false;
        [ShowOnly]
        public bool superBaharWin = false;
        [ShowOnly]
        public bool isNotFirstTurn = false;

        public Button SuperBahar1xBtn;
        public Button SuperBahar5xBtn;
        public Button SuperBahar10xBtn;
        public Button SuperBahar20xBtn;
        public Button SuperBahar50xBtn;


        public Sprite[] ABResultSprites;
        public Transform SuperCardParent;

        public GameObject AndarBaharHistoryPanel;

        #region Creating Instance
        private static AndarBaharPositionsManager _instance;
        public static AndarBaharPositionsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<AndarBaharPositionsManager>();
                return _instance;
            }
        }
        #endregion

        private void Awake()
        {
            if (_instance == null)
                _instance = this;


            ABActionPanel.SetActive(false);

            if (!MatchHandler.IsAndarBahar())
            {
                GirlsTeenPatti.SetActive(true);
                //buyPanel.SetActive(true);
                DeactivateAllABThings();
                gameObject.SetActive(false);
            }
            else
            {
                LocalSettings.ToggleObjectState(objectsToDisable, false);
                LocalSettings.ToggleObjectState(objectsToEnable, true);
            }
        }

        public List<CardProperty> AllCards = new List<CardProperty>();
        private void Start()
        {
            AllCards.Clear();
            AllCards = GameManager.Instance.AllCards.Card.ToList();
            AssignButtonReferenceOfAndar_Bahar();

            if (LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.ABCardLastRecordsKey) == null)
                FirstTimeSettingRoomRecord();
            else
                ShowLastRecordInUI();
        }

        void DeactivateAllABThings()
        {
            ABTable.SetActive(false);
            ABActionPanel.SetActive(false);
        }


        public void AdjustABTableThings()
        {
            AdjustNewPositions();
            ABTableUpgrade();
            //Changeeeeeeeeeeeee
            {
                PlaceFirstCard();
            }
            //if (LocalSettings.GetTotalChips() <= 0)
            //   LocalSettings.SetTotalChips(500000);
            AndarTotalBetAmount = LocalSettings.MinBetAmount;
            BaharTotalBetAmount = LocalSettings.MinBetAmount;
            //  SuperAndarTotalBetAmount = LocalSettings.MinABBetAmount;
            //  SuperBaharTotalBetAmount = LocalSettings.MinABBetAmount;
        }


        void AdjustNewPositions()
        {
            for (int i = 1; i < 5; i++)
            {
                //GameManager.Instance.position_availability[i].Pos.position = ABposition[i].position;
                LocalSettings.SetPosAndRect(GameManager.Instance.position_availability[i].Pos.gameObject, ABposition[i], GameManager.Instance.PlayerTable);
                Debug.Log("Positions Are " + ABposition[i]);
                //GameManager.Instance.SetPosAndRect()
            }
        }

        void ABTableUpgrade()
        {
            ABTable.SetActive(true);
        }


        public void PlaceFirstCard()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SettingArrayToRoomProperty();
                if (PhotonNetwork.IsConnectedAndReady)
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.abPlayCardKey, RandomArrayCards[0]);

            }
        }





        public void PlaceBetAndar(PlayerInfo info)
        {
            info.AndarBetAmoutTxt.text = LocalSettings.Rs(AndarTotalBetAmount.ToString());
        }

        int FirstCardPower = 0;
        public GameObject FirstCardRef;
        public GameObject andarCardRef;
        public GameObject BaharCardRef;
        public int[] RandomArrayCards = new int[52];
        public void SettingArrayToRoomProperty()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < GameManager.Instance.AllCards.Card.Length; i++)
                {
                    RandomArrayCards[i] = i;
                }
                Shuffle(RandomArrayCards);
                if (PhotonNetwork.IsConnectedAndReady)
                    LocalSettings.GetCurrentRoom.SetCardsList(LocalSettings.ab_card_listKey, RandomArrayCards);
            }
        }
        private System.Random _random = new System.Random();
        void Shuffle(int[] array)
        {
            int p = array.Length;
            for (int n = p - 1; n > 0; n--)
            {
                int r = _random.Next(0, n);
                int t = array[r];
                array[r] = array[n];
                array[n] = t;
            }
        }
        int FirstCardIndexForRecordSave;
        public void PlaceFirstCardOverall(int val)
        {

            RandomArrayCards = LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.ab_card_listKey);

            FirstCardIndexForRecordSave = val;
            GameObject obj = Instantiate(AllCards[val].gameObject);
            if (FirstCardRef)
                Destroy(FirstCardRef);
            FirstCardRef = obj;
            //AllCards.RemoveAt(val);
            FirstCardPower = obj.GetComponent<CardProperty>().Power;

            LocalSettings.SetPosAndRect(obj, AllCardsPos, ABTable.transform);
            if (UIManager.Instance.GetMyPlayerCurrentState().currentState != PlayerState.STATE.OutOfGame)
            {
                PlayAnimation(obj.transform, PlayCardPos, 2.8f, true, 0.5f, null);
                ChageRoomStateToGamePlaying();
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
                //Debug.LogError("Card flip first card sound is playing");
            }


            RandomArrayCards = LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.ab_card_listKey);
        }


        public void PlaceFirstCardOverallForOutPlayer(int val)
        {
            RandomArrayCards = LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.ab_card_listKey);
            GameObject obj = Instantiate(AllCards[val].gameObject);
            FirstCardRef = obj;
            FirstCardPower = obj.GetComponent<CardProperty>().Power;
            LocalSettings.SetPosAndRect(obj, PlayCardPos, ABTable.transform);
            RandomArrayCards = LocalSettings.GetCurrentRoom.GetCardsList(LocalSettings.ab_card_listKey);
        }
        public void PlaceAndarBaharCardForOutPlayer(int val, RectTransform CardPos, string hashKey)
        {
            GameObject obj = Instantiate(AllCards[val].gameObject);
            if (hashKey == LocalSettings.abAndarCardkey)
                andarCardRef = obj;
            else
                BaharCardRef = obj;
            CurrentCardPower = obj.GetComponent<CardProperty>().Power;
            LocalSettings.SetPosAndRect(obj, CardPos, ABTable.transform);
        }

        public void CardDistributingAB()
        {
            if (!UIManager.Instance.GetMyPlayerInfo().firstTurnAB)
                StartCoroutine(waitForABFirstTurn());

        }



        IEnumerator waitForABFirstTurn()
        {
            UIManager.Instance.GetMyPlayerInfo().FirstAbOnMasterClientLeft(true);
            yield return new WaitUntil(() => PlayerStateManager.Instance.PlayingList.All(x => x.AbTurn == false));
            // Debug.LogError("Starting camparison");
            if (PlayerStateManager.Instance.PlayingList.Count > 0)
                if (PhotonNetwork.IsMasterClient)
                {
                    CardDistRoutine = StartCoroutine(StartCardsDistribution());

                }



        }
        bool isAndar = false;
        int CurrentCardPower = 0;
        List<GameObject> CardsToDestroy = new List<GameObject>();



        public void DestroyCardsList()
        {
            StopCardDistRoutine();
            isAndar = false;
            if (CardsToDestroy.Count > 0)
            {
                for (int i = 0; i < CardsToDestroy.Count; i++)
                {
                    if (CardsToDestroy[i] != null)
                        Destroy(CardsToDestroy[i]);
                }
            }
            CardsToDestroy.Clear();



            ThisAndarbaharScriptRestValues();
            //StopCardDistRoutine();
            PlaceFirstCard();
        }

        void ThisAndarbaharScriptRestValues()
        {
            if (FirstCardRef != null)
                Destroy(FirstCardRef);
            if (andarCardRef != null)
                Destroy(andarCardRef);
            if (BaharCardRef != null)
                Destroy(BaharCardRef);
            SuperBaharTotalBetAmount = 0;
            secondSuperBaharBet = 0;
            firstSuperBaharBet = 0;
            SuperBaharCount = 0;
            isCardMatched = false;
            superBaharWin = false;
            SuperBaharlWinPanel.SetActive(false);
            SuperBaharWinTxt.text = "";
            ABActionPanel.SetActive(false);
            CardWinAnimation(AndarCardPos, PlayCardPos, false);
            CardWinAnimation(BaharCardPos, PlayCardPos, false);
            if (UIManager.Instance.GetMyPlayerInfo() != null)
                if (LocalSettings.GetTotalChips() < LocalSettings.MinBetAmount && UIManager.Instance.GetMyPlayerInfo().photonView.IsMine)
                {
                    UIManager.Instance.quickShop.SetActive(true);
                    UIManager.Instance.GetMyPlayerInfo().StandUp();
                    return;
                }

        }

        void StopCardDistRoutine()
        {
            if (CardDistRoutine != null)
                StopCoroutine(CardDistRoutine);
        }

        Coroutine CardDistRoutine;
        //int CurrentDustributed;
        IEnumerator StartCardsDistribution()
        {
            if (CardsToDestroy.Count > 0)
            {
                for (int i = 0; i < CardsToDestroy.Count; i++)
                {
                    if (CardsToDestroy[i] != null)
                        Destroy(CardsToDestroy[i]);
                }
                CardsToDestroy.Clear();
            }
            for (int i = 1; i < RandomArrayCards.Length; i++)
            {
                if (i == 1)
                {
                    isNotFirstTurn = true;
                    LocalSettings.GetCurrentRoom.SetCustomRoomBoolData(LocalSettings.IsNotCardDistribute, isNotFirstTurn);
                }

                if (i == 1 && UIManager.Instance.GetMyPlayerInfo().firstTurnAB == true)
                    UIManager.Instance.GetMyPlayerInfo().FirstAbOnMasterClientLeft(false);
                //Debug.Log("All Cards Count are " + AllCards.Count);
                if (i == 3 && !Game_Play.Instance.MasterClientStandUpFlag)
                {
                    if (!Game_Play.Instance.secondTurnTurnAb)
                        Game_Play.Instance.secondTurnTurnAb = true;
                    if (Game_Play.Instance.secondTurnTurnAb)
                    {
                        //Debug.LogError("Yahan sy ho kr aa rha hy");
                        UIManager.Instance.GetMyPlayerInfo().SecondTurnAB(true);
                    }
                }
                yield return new WaitUntil(() => PlayerStateManager.Instance.PlayingList.All(x => x.AbTurn == false));

                if (i % 2 == 0)
                {
                    if (i == 2)
                    {
                        int a = RandomArrayCards[i];
                        if (PhotonNetwork.IsConnectedAndReady)
                            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.abAndarCardkey, a);
                        Debug.Log("Andar AYa" + a);
                    }
                    else
                    {
                        int a = RandomArrayCards[i - 2];
                        if (PhotonNetwork.IsConnectedAndReady)
                            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.abAndarCardkey, a);
                        Debug.Log("Andar AYa" + a);

                    }
                }
                else
                {
                    if (i == 1)
                    {
                        int b = RandomArrayCards[i];
                        if (PhotonNetwork.IsConnectedAndReady)
                            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.abBaharCardkey, b);
                        Debug.Log("Bahar AYa" + b);
                    }
                    else
                    {
                        int b = RandomArrayCards[i - 1];
                        if (PhotonNetwork.IsConnectedAndReady)
                            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.abBaharCardkey, b);
                        Debug.Log("Bahar AYa" + b);
                    }

                }

                yield return new WaitForSeconds(0.6f);

                //int randomCardIndex = UnityEngine.Random.Range(0, AllCards.Count);

                int SeqCardIndex = RandomArrayCards[i];


                if (isCardMatched)
                {

                    //Debug.LogError("RandomCardIndex 1st calling:---------" + SeqCardIndex);
                    break;
                }
                else
                    photonView.RPC(nameof(showCardAndarbarRPC), RpcTarget.All, SeqCardIndex, i % 2 == 0 ? true : false);

            }
        }
        int Counter;
        int SuperBaharCount;
        [PunRPC]
        void showCardAndarbarRPC(int randomCardIndex, bool isTrue)
        {
            SuperBaharCount++;
            // Debug.LogError("RandomCardIndex 2nd calling:---------" + randomCardIndex);
            GameObject card = Instantiate(AllCards[randomCardIndex].gameObject);
            CardsToDestroy.Add(card);
            CurrentCardPower = card.GetComponent<CardProperty>().Power;

            if (SuperBaharCount == 4 && UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
                UIManager.Instance.GetMyPlayerInfo().viewResultAB.SetActive(true);

            isAndar = isTrue;
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
            //Debug.LogError("Card flip first card sound is playing");
            if (isAndar)
            {
                LocalSettings.SetPosAndRect(card, AllCardsPos, ABTable.transform);
                PlayAnimation(card.transform, AndarCardPos, 2.8f, true, 0.5f, null);
                //  GameManager.Instance.SetPosAndRect(card, AndarCardPos, AndarCardPos.transform.parent);

            }
            else
            {
                LocalSettings.SetPosAndRect(card, AllCardsPos, ABTable.transform);
                PlayAnimation(card.transform, BaharCardPos, 2.8f, true, 0.5f, null);
                // GameManager.Instance.SetPosAndRect(card, BaharCardPos, BaharCardPos.transform.parent);

            }
            if (CurrentCardPower == FirstCardPower)
            {
                StartCoroutine(waitForStartAnim());
                isCardMatched = true;
                if (isCardMatched)
                    UIManager.Instance.GetMyPlayerInfo().viewResultAB.SetActive(false);

            }
        }

        IEnumerator waitForStartAnim()
        {
            yield return new WaitForSeconds(1);
            if (isAndar)
            {

                //Debug.LogError("Result decleared Andar Wins --------------");
                CardWinAnimation(AndarCardPos, PlayCardPos, true);
                ShowAndAddReward(true);
                SavingRecordInRoomProperty(FirstCardIndexForRecordSave, 0);
                int totalwins = 0;
                if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalAndarWinKey) != null)
                    totalwins = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalAndarWinKey);
                totalwins++;
                LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ABTotalAndarWinKey, totalwins);
            }
            else
            {

                CardWinAnimation(BaharCardPos, PlayCardPos, true);
                if (SuperBaharCount == 1)
                {
                    if (firstSuperBaharBet > 0)
                        superBaharWin = true;
                    SavingRecordInRoomProperty(FirstCardIndexForRecordSave, 2);
                    int totalwins = 0;
                    if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey) != null)
                        totalwins = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey);
                    totalwins++;
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey, totalwins);
                }
                else if (SuperBaharCount == 3)
                {
                    if (secondSuperBaharBet > 0)
                        superBaharWin = true;
                    SavingRecordInRoomProperty(FirstCardIndexForRecordSave, 2);
                    int totalwins = 0;
                    if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey) != null)
                        totalwins = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey);
                    totalwins++;
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey, totalwins);
                }
                else
                {
                    SavingRecordInRoomProperty(FirstCardIndexForRecordSave, 1);
                    int totalwins = 0;
                    if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalBaharWinKey) != null)
                        totalwins = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalBaharWinKey);
                    totalwins++;
                    LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ABTotalBaharWinKey, totalwins);
                }

                ShowAndAddReward(false);
            }
        }



        void CardWinAnimation(Transform Wincard, Transform MidCard, bool isTrue)
        {
            Wincard.gameObject.transform.GetChild(0).gameObject.SetActive(isTrue);
            MidCard.gameObject.transform.GetChild(0).gameObject.SetActive(isTrue);

        }


        void ShowAndAddReward(bool isAndar)
        {
            ABActionPanel.SetActive(false);
            if (isAndar)
            {
                PlayerStateManager psm = PlayerStateManager.Instance;
                foreach (PlayerInfo pInfo in psm.PlayingList)
                {
                    if (pInfo.player.GetCustomBigIntegerData(LocalSettings.abAndarAmountKey) > 0)
                    {
                        BigInteger rewardAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.abAndarAmountKey) * 2;
                        
                        GameObject andarTxtParent = pInfo.AndarBetAmoutTxt.transform.parent.gameObject;
                        GameObject rewardBox = Instantiate(andarTxtParent);
                        LocalSettings.SetPosAndRect(rewardBox, andarTxtParent.GetComponent<RectTransform>(), andarTxtParent.transform.parent.gameObject.transform);
                        rewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(rewardAmount);
                        if (UIManager.Instance.GetMyPlayerInfo().photonView.ViewID == pInfo.photonView.ViewID)
                        {
                            UIManager.Instance.TotalWinsAmount += rewardAmount;
                            UIManager.Instance.TotalWinAmountFor1Game = rewardAmount;
                            if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
                            {
                               // UIManager.Instance.TotalWinHands++;
                                GameManager.Instance.AddXPToMyPlayer(true);
                            }
                            PlayAnimation(rewardBox.transform, UIManager.Instance.PlayerTotalChipsTxt.transform, 1, false, 2f, rewardBox);
                            pInfo.WinningIndicator.SetActive(true);
                            if (pInfo.photonView.IsMine)
                            {
                                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                            }
                            StartCoroutine(AddingABWinAmount(rewardBox, rewardAmount));
                            AbBetAmountTxt(pInfo);



                        }
                        else
                        {
                            pInfo.WinningIndicator.SetActive(true);
                            PlayAnimation(rewardBox.transform, pInfo.gameObject.transform, 1, false, 2f, rewardBox);
                            AbBetAmountTxt(pInfo);
                        }
                    }

                    if (!superBaharWin && pInfo.player.GetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey) > 0)
                    {
                        superBaharLose(pInfo);
                    }
                    if (pInfo.player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey) > 0)
                    {

                        BigInteger rewardAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey);
                        GameObject baharTxtParent = pInfo.BaharBetAmoutTxt.transform.parent.gameObject;
                        GameObject rewardBox = Instantiate(baharTxtParent);
                        LocalSettings.SetPosAndRect(rewardBox, baharTxtParent.GetComponent<RectTransform>(), baharTxtParent.transform.parent.gameObject.transform);
                        rewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(rewardAmount);
                        PlayAnimation(rewardBox.transform, GameStartManager.Instance.GameStartWaitText.gameObject.transform, 1, false, 2f, rewardBox);
                        AbBetAmountTxt(pInfo);
                    }
                }
            }
            else
            {
                PlayerStateManager psm = PlayerStateManager.Instance;
                foreach (PlayerInfo pInfo in psm.PlayingList)
                {
                    if (superBaharWin)
                    {
                        BigInteger SuperWinReward = SuperBaharTotalBetAmount * 10;

                        GameObject SuperBaharTxtParent = pInfo.SuperBaharBetAmoutTxt.transform.parent.gameObject;
                        GameObject SuperRewardBox = Instantiate(SuperBaharTxtParent);
                        LocalSettings.SetPosAndRect(SuperRewardBox, SuperBaharTxtParent.GetComponent<RectTransform>(), SuperBaharTxtParent.transform.parent.gameObject.transform);
                        if (secondSuperBaharBet > 0)
                            SuperRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(SuperWinReward + secondSuperBaharBet - firstSuperBaharBet);
                        else
                            SuperRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(SuperWinReward + firstSuperBaharBet);

                        if (UIManager.Instance.GetMyPlayerInfo().photonView.ViewID == pInfo.photonView.ViewID)
                        {
                            if (secondSuperBaharBet > 0)
                            {
                                UIManager.Instance.TotalWinsAmount += (SuperWinReward + secondSuperBaharBet - firstSuperBaharBet);
                                UIManager.Instance.TotalWinAmountFor1Game += (SuperWinReward + secondSuperBaharBet - firstSuperBaharBet);
                            }
                            else
                            {
                                UIManager.Instance.TotalWinsAmount += (SuperWinReward + firstSuperBaharBet);
                                UIManager.Instance.TotalWinAmountFor1Game += (SuperWinReward + firstSuperBaharBet);
                            }
                            //UIManager.Instance.TotalWinsAmount += SuperWinReward;
                            PlayAnimation(SuperRewardBox.transform, UIManager.Instance.PlayerTotalChipsTxt.transform, 1, false, 2f, SuperRewardBox);
                            if (secondSuperBaharBet > 0)
                                StartCoroutine(AddingABWinAmount(SuperRewardBox, SuperWinReward + secondSuperBaharBet - firstSuperBaharBet));
                            else
                                StartCoroutine(AddingABWinAmount(SuperRewardBox, SuperWinReward + firstSuperBaharBet));
                            pInfo.WinningIndicator.SetActive(true);
                            if (pInfo.photonView.IsMine)
                            {
                                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                            }
                            SuperBaharWinTxt.text = LocalSettings.Rs(SuperWinReward);
                            SuperBaharlWinPanel.SetActive(true);
                            AbBetAmountTxt(pInfo);
                        }
                        else
                        {
                            PlayAnimation(SuperRewardBox.transform, pInfo.gameObject.transform, 1, false, 2f, SuperRewardBox);
                            AbBetAmountTxt(pInfo);
                            pInfo.WinningIndicator.SetActive(true);
                        }
                    }
                    if (pInfo.player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey) > 0)
                    {

                        BigInteger rewardAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey);
                        //if (SuperBaharCount == 1))
                        //    pInfo.player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey) * ;
                        if (SuperBaharCount == 1)
                            rewardAmount += (pInfo.player.GetCustomBigIntegerData(LocalSettings.abBaharAmountKey) / 4);
                        else
                            rewardAmount = rewardAmount * 2;

                        GameObject baharTxtParent = pInfo.BaharBetAmoutTxt.transform.parent.gameObject;
                        GameObject rewardBox = Instantiate(baharTxtParent);
                        LocalSettings.SetPosAndRect(rewardBox, baharTxtParent.GetComponent<RectTransform>(), baharTxtParent.transform.parent.gameObject.transform);
                        rewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(rewardAmount);


                        if (UIManager.Instance.GetMyPlayerInfo().photonView.ViewID == pInfo.photonView.ViewID)
                        {
                            UIManager.Instance.TotalWinsAmount += rewardAmount;
                            UIManager.Instance.TotalWinAmountFor1Game += rewardAmount;
                            if (UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.AbleToJoin)
                            {

                               // UIManager.Instance.TotalWinHands++;
                                GameManager.Instance.AddXPToMyPlayer(true);
                            }
                            PlayAnimation(rewardBox.transform, UIManager.Instance.PlayerTotalChipsTxt.transform, 1, false, 2f, rewardBox);
                            if (pInfo.photonView.IsMine)
                            {
                                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                            }
                            StartCoroutine(AddingABWinAmount(rewardBox, rewardAmount));
                            pInfo.WinningIndicator.SetActive(true);
                            AbBetAmountTxt(pInfo);

                        }
                        else
                        {
                            PlayAnimation(rewardBox.transform, pInfo.gameObject.transform, 1, false, 2f, rewardBox);
                            AbBetAmountTxt(pInfo);
                            pInfo.WinningIndicator.SetActive(true);
                        }
                    }


                    if (!superBaharWin && pInfo.player.GetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey) > 0)
                    {
                        superBaharLose(pInfo);
                    }

                    if (pInfo.player.GetCustomBigIntegerData(LocalSettings.abAndarAmountKey) > 0)
                    {
                        if (!superBaharWin)
                        {
                            superBaharLose(pInfo);
                        }
                        BigInteger rewardAmount = pInfo.player.GetCustomBigIntegerData(LocalSettings.abAndarAmountKey);
                        GameObject andarTxtParent = pInfo.AndarBetAmoutTxt.transform.parent.gameObject;
                        GameObject rewardBox = Instantiate(andarTxtParent);
                        LocalSettings.SetPosAndRect(rewardBox, andarTxtParent.GetComponent<RectTransform>(), andarTxtParent.transform.parent.gameObject.transform);
                        rewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(rewardAmount);
                        PlayAnimation(rewardBox.transform, GameStartManager.Instance.GameStartWaitText.gameObject.transform, 1, false, 2f, rewardBox);
                        AbBetAmountTxt(pInfo);

                    }

                    pInfo.UpdateTextForOtherPlayer();
                }
            }

            foreach (PlayerInfo pInfo in PlayerStateManager.Instance.PlayingList)
            {
                if (UIManager.Instance.GetMyPlayerInfo().photonView.ViewID == pInfo.photonView.ViewID)
                {
                    if (pInfo.photonView.IsMine)
                    {
                        UIManager.Instance.SetTotalWinHandByPlayer();
                    }
                }
            }


            UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ShowingResults);
        }

        // AndarBahar Game Reset All Data
        public void ResetAllDataAndNewGamestartAB()
        {
            StartCoroutine(ResetAllDataAndNewGameBeginofAB(LocalSettings.ShowingResultAndResetDelayTimeOfAB));
        }
        IEnumerator ResetAllDataAndNewGameBeginofAB(float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);

            GameResetManager.Instance.ResetABGame();
        }


        void superBaharLose(PlayerInfo pInfo)
        {
            BigInteger SuperWinReward = pInfo.player.GetCustomBigIntegerData(LocalSettings.abSuperBaharAmountKey);
            GameObject SuperBaharTxtParent = pInfo.SuperBaharBetAmoutTxt.transform.parent.gameObject;
            GameObject SuperRewardBox = Instantiate(SuperBaharTxtParent);
            LocalSettings.SetPosAndRect(SuperRewardBox, SuperBaharTxtParent.GetComponent<RectTransform>(), SuperBaharTxtParent.transform.parent.gameObject.transform);
            SuperRewardBox.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = LocalSettings.Rs(SuperWinReward);
            PlayAnimation(SuperRewardBox.transform, GameStartManager.Instance.GameStartWaitText.gameObject.transform, 1, false, 2f, SuperRewardBox);
            AbBetAmountTxt(pInfo);
        }

        void ResetAfterWait()
        {
            if (MatchHandler.IsAndarBahar())
                GameResetManager.Instance.ResetABGame();
        }

        void AbBetAmountTxt(PlayerInfo pInfo)
        {
            pInfo.AndarBetAmoutTxt.text = "";
            pInfo.BaharBetAmoutTxt.text = "";
            pInfo.SuperBaharBetAmoutTxt.text = "";
        }

        void ChageRoomStateToGamePlaying()
        {
            RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsPlaying);
            if (PhotonNetwork.IsMasterClient)
            {
                isNotFirstTurn = false;
                if (PhotonNetwork.IsConnectedAndReady)
                    LocalSettings.GetCurrentRoom.SetCustomRoomBoolData(LocalSettings.IsNotCardDistribute, isNotFirstTurn);
            }
        }



        void PlayAnimation(Transform ObjToAnimate, Transform targetPosition, float ScaleMultiplier, bool isRotate, float TimeToReach, GameObject ObjToDestroy)
        {
            if (targetPosition != null)
                ObjToAnimate.DOMove(targetPosition.position, TimeToReach, false).OnComplete(() => ObjDestroy(ObjToDestroy));
            if (isRotate)
            {
                float RotationOffSet = targetPosition.eulerAngles.z;
                ObjToAnimate.DOLocalRotate(new UnityEngine.Vector3(0, 0, 360 + RotationOffSet), TimeToReach, RotateMode.FastBeyond360);
            }
            RectTransform rt = targetPosition.gameObject.GetComponent<RectTransform>();
            ObjToAnimate.DOScale(UnityEngine.Vector3.one * ScaleMultiplier, TimeToReach).OnComplete(() => cardSetAnchor(ObjToAnimate));

        }

        void cardSetAnchor(Transform card)
        {

            if (MatchHandler.IsAndarBahar())
            {
                if (SuperBaharCount > 0)
                {
                    if (isAndar)
                        LocalSettings.SetPosAndRect(card.gameObject, AndarCardPos, AndarCardPos.parent);
                    else
                        LocalSettings.SetPosAndRect(card.gameObject, BaharCardPos, BaharCardPos.parent);
                }
                else
                {
                    LocalSettings.SetPosAndRect(card.gameObject, PlayCardPos, PlayCardPos.parent);
                }
            }


        }


        IEnumerator AddingABWinAmount(GameObject obj, BigInteger WinAmount)
        {
            yield return new WaitUntil(() => obj == null);
            LocalSettings.SetTotalChips(WinAmount);
            //  GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, WinAmount.ToString());
            // SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Reward, false);
            UIManager.Instance.PlayerTotalChipsTxt.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
            UIManager.Instance.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            UIManager.Instance.GetMyPlayerInfo().UpdateTextForOtherPlayer();
        }




        void ObjDestroy(GameObject obj)
        {
            if (obj != null)
            {
                Destroy(obj);
                if (photonView.IsMine)
                {

                }
            }


        }

        // View WinRecord of Andar bahar





        public void CalculationOfWinnerindicatorOfAB()
        {

            for (int i = 1; i < RandomArrayCards.Length; i++)
            {
                if (i == 1)
                    isNumberMatched = false;
                int SeqCardNumber = RandomArrayCards[i];
                if (isNumberMatched)
                {
                    //Debug.LogError("RandomCardIndex 1st calling:---------" + SeqCardIndex);
                    break;
                }
                else
                {

                    bool istrueOrFalse = (i % 2 == 0) ? true : false;
                    showWinnerCardAB(SeqCardNumber, istrueOrFalse);
                }

            }
        }
        void showWinnerCardAB(int randomCardIndex, bool isTrue)
        {
            GameObject card = Instantiate(AllCards[randomCardIndex].gameObject);
            int CardPower = card.GetComponent<CardProperty>().Power;
            if (CardPower == FirstCardPower)
            {
                if (isTrue)
                {
                    //Debug.LogError("Result decleared Andar Wins --------------");
                    CardWinAnimation(AndarCardPos, PlayCardPos, true);
                }
                else
                {
                    //Debug.LogError("Result decleared Bahar Wins --------------");
                    CardWinAnimation(BaharCardPos, PlayCardPos, true);
                }
                isNumberMatched = true;
            }
            DestroyImmediate(card);
        }

        // SuperBahar According to Bet amount
        [ShowOnly] public BigInteger bet1xInt;
        [ShowOnly] public BigInteger bet5xInt;
        [ShowOnly] public BigInteger bet10xInt;
        [ShowOnly] public BigInteger bet20xInt;
        [ShowOnly] public BigInteger bet50xInt;

        void AssignButtonReferenceOfAndar_Bahar()
        {
            bet1xInt = LocalSettings.MinBetAmount;
            bet5xInt = LocalSettings.MinBetAmount * 5;
            bet10xInt = LocalSettings.MinBetAmount * 10;
            bet20xInt = LocalSettings.MinBetAmount * 20;
            bet50xInt = LocalSettings.MinBetAmount * 50;

            string Bet1x = LocalSettings.Rs(bet1xInt);
            string Bet5x = LocalSettings.Rs(bet5xInt);
            string Bet10x = LocalSettings.Rs(bet10xInt);
            string Bet20x = LocalSettings.Rs(bet20xInt.ToString());
            string Bet50x = LocalSettings.Rs(bet50xInt);


            SuperBahar1xBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet1x;
            SuperBahar5xBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet5x;
            SuperBahar10xBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet10x;
            SuperBahar20xBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet20x;
            SuperBahar50xBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = Bet50x;

            SuperBahar1xBtn.onClick.AddListener(() => PlaceSuperBahar(bet1xInt));
            SuperBahar5xBtn.onClick.AddListener(() => PlaceSuperBahar(bet5xInt));
            SuperBahar10xBtn.onClick.AddListener(() => PlaceSuperBahar(bet10xInt));
            SuperBahar20xBtn.onClick.AddListener(() => PlaceSuperBahar(bet20xInt));
            SuperBahar50xBtn.onClick.AddListener(() => PlaceSuperBahar(bet50xInt));

        }

        public void PlaceSuperBahar(BigInteger amount)
        {
            Game_Play game_Play = Game_Play.Instance;
            if (LocalSettings.GetTotalChips() >= amount)
            {

                UIManager uIManager = UIManager.Instance;
                if (game_Play.secondSuperBahar)
                {
                    secondSuperBaharBet = firstSuperBaharBet + amount;
                    uIManager.GetMyPlayerInfo().PlaceBetSuperBahar(amount);
                    OnclickSuperBahar2BetAPI(amount.ToString());
                }
                else
                {
                    firstSuperBaharBet = amount;
                    uIManager.GetMyPlayerInfo().PlaceBetSuperBahar(firstSuperBaharBet);
                    OnclickSuperBahar1BetAPI(amount.ToString());
                }

                SuperBaharTotalBetAmount = amount;
                UIManager.Instance.TotalBetPlacedAmount += amount;
                uIManager.TotalBetPlaceFor1Game += amount;
            }
            else
            {

                UIManager.Instance.quickShop.SetActive(true);
            }

            game_Play.DisableBtnsOnSuperBet();

        }

        #region Results And History

        void FirstTimeSettingRoomRecord()
        {
            int[] WinNumbersRecordCard = new int[25];
            int[] AndarBaharWinIndex = new int[25];
            for (int i = 0; i < WinNumbersRecordCard.Length; i++)
            {
                WinNumbersRecordCard[i] = Random.Range(0, 52);
                int Probability = Random.Range(0, 20);
                AndarBaharWinIndex[i] = Probability == 3 ? 2 : Random.Range(0, 2);
            }
            LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.ABCardLastRecordsKey, WinNumbersRecordCard);
            LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.ABWinIndexRecordsKey, AndarBaharWinIndex);
            FirstTimeTotalGamesPlayed();
            Invoke(nameof(ShowLastRecordInUI), 2f);
        }

        void FirstTimeTotalGamesPlayed()
        {
            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ABTotalAndarWinKey, Random.Range(105, 160));
            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ABTotalBaharWinKey, Random.Range(105, 160));
            LocalSettings.GetCurrentRoom.SetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey, Random.Range(6, 14));
        }

        void SavingRecordInRoomProperty(int val, int WinIndex)
        {
            int[] tempArrForCard = new int[25];
            int[] tempArrForWinIndex = new int[25];
            if (PhotonNetwork.IsConnectedAndReady)
            {
                tempArrForCard = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.ABCardLastRecordsKey);
                tempArrForWinIndex = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.ABWinIndexRecordsKey);
            }
            for (int i = tempArrForCard.Length - 1; i > 0; i--)
            {
                tempArrForCard[i] = tempArrForCard[i - 1];
                tempArrForWinIndex[i] = tempArrForWinIndex[i - 1];
            }
            //Add A New Variable Here
            tempArrForCard[0] = val;
            tempArrForWinIndex[0] = WinIndex;
            if (PhotonNetwork.IsConnectedAndReady)
            {
                LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.ABCardLastRecordsKey, tempArrForCard);
                LocalSettings.GetCurrentRoom.SetLastRecord(LocalSettings.ABWinIndexRecordsKey, tempArrForWinIndex);
            }

            Invoke(nameof(ShowLastRecordInUI), 1f);
        }


        public void ShowLastRecordInUI()
        {
            // Show Previous Winning Numbers On UI 
            // If there is -1 then That Item is Empty You can show "-"
            if (LocalSettings.GetCurrentRoom != null)
                if (LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.ABCardLastRecordsKey) != null)
                {
                    foreach (var item in LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.ABCardLastRecordsKey))
                    {
                        //Debug.LogError(item);

                    }
                }
            int[] tempArrForCards = new int[25];
            int[] tempArrForWinIndex = new int[25];
            if (PhotonNetwork.IsConnectedAndReady)
            {
                tempArrForCards = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.ABCardLastRecordsKey);
                tempArrForWinIndex = LocalSettings.GetCurrentRoom.GetLastRecord(LocalSettings.ABWinIndexRecordsKey);
            }
            CardProperty[] allcards = GameManager.Instance.AllCards.Card;
            for (int i = 0; i < tempArrForCards.Length; i++)
            {
                //SuperCardParent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = SuperCardPreviousRecord[tempArr[i]];
                SuperCardParent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = allcards[tempArrForCards[i]].GetComponent<Image>().sprite;
                SuperCardParent.GetChild(i).GetChild(1).GetComponent<Image>().sprite = ABResultSprites[tempArrForWinIndex[i]];
            }


            if (UIManager.Instance.GetMyPlayerInfo() != null)
            {
                //Debug.LogError("Remaining Cash: " + LocalSettings.GetTotalChips());
                UIManager.Instance.GetMyPlayerInfo().playerTotalCash.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
            }

            // Showing total andar wins
            float totalwinsAndar = 0;
            if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalAndarWinKey) != null)
                totalwinsAndar = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalAndarWinKey);

            // Showing total Bahar wins
            float totalwinsbahar = 0;
            if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalBaharWinKey) != null)
                totalwinsbahar = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalBaharWinKey);

            // Showing total super bahar wins
            float totalwinsSB = 0;
            if (LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey) != null)
                totalwinsSB = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTotalSuperBaharWinKey);

            float totalGames = totalwinsAndar + totalwinsbahar + totalwinsSB;

            TotalHandsAndSB.text = "Hands: " + totalGames + "\nSuper Bahar: " + totalwinsSB + " (" + ((totalGames == 0) ? 0 : ((totalwinsSB / totalGames) * 100).ToString("N2")) + "%)";
            TotalABWins.text = "Andar: " + totalwinsAndar + " (" + ((totalGames == 0) ? 0 : ((totalwinsAndar / totalGames) * 100).ToString("N2")) + "%)" + "\nBahar: " + totalwinsbahar + " (" + ((totalGames == 0) ? 0 : ((totalwinsbahar / totalGames) * 100).ToString("N2")) + "%)";

        }

        public void ActivateAndarBaharHistoryPanel(bool val)
        {
            AndarBaharHistoryPanel.SetActive(val);
        }

        #endregion

        #region PunCall backs

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (MatchHandler.IsAndarBahar())
            {
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying && PlayerStateManager.Instance.PlayingList.Count <= 1)
                {
                    GameResetManager.Instance.ResetABGame();
                }

                if (!UIManager.Instance.GetMyPlayerInfo().firstTurnAB && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                {
                    CardDistributingAB();
                }
            }
        }
        #endregion


        #region Andar,Bahar & SuperBahar API Functionality 

        #region bahar1 API
        public void OnclickBahar1BetAPI()
        {

            for (int i = 1; i < RandomArrayCards.Length; i++)
            {
                if (i == 1)
                    isNumberMatched = false;
                int SeqCardNumber = RandomArrayCards[i];
                if (isNumberMatched)
                {
                    //Debug.LogError("RandomCardIndex 1st calling:---------" + SeqCardIndex);
                    break;
                }
                else
                {

                    bool istrueOrFalse = (i % 2 == 0) ? true : false;
                    showWinnerBahar1API(SeqCardNumber, istrueOrFalse, i);
                }

            }
        }
        void showWinnerBahar1API(int randomCardIndex, bool isTrue, int i)
        {
            int CardPower = AllCards[randomCardIndex].Power;
            if (CardPower == FirstCardPower)
            {
                if (isTrue)
                {
                    //Debug.LogError("Result decleared Andar Wins --------------");
                    // Debug.LogError("Result decleared Bahar 1 lose --------------" + LocalSettings.Rs(BaharTotalBetAmount));
                    AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.bahr1, BaharTotalBetAmount.ToString(), AndarBaharAPI.BetType.lose);
                }
                else
                {
                    //Debug.LogError("Result decleared Bahar Wins --------------");

                    if (i == 1)
                    {
                        // Debug.LogError("Check Bahar 1.....1st..");
                        //  Debug.LogError("Result decleared Bahar 1 win --------------" + LocalSettings.Rs(BaharTotalBetAmount));
                        AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.bahr1, BaharTotalBetAmount.ToString(), AndarBaharAPI.BetType.win);
                    }
                    else
                    {
                        // Debug.LogError("Check Bahar 1......2nd....");
                        // Debug.LogError("Result decleared Bahar 1 or Bahar 2 win --------------" + LocalSettings.Rs(BaharTotalBetAmount));
                        AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.bahr2, BaharTotalBetAmount.ToString(), AndarBaharAPI.BetType.win);
                    }


                }
                isNumberMatched = true;
            }
        }
        #endregion

        #region Bahar 2 API

        public void OnclickBahar2BetAPI()
        {

            for (int i = 1; i < RandomArrayCards.Length; i++)
            {
                if (i == 1)
                    isNumberMatched = false;
                int SeqCardNumber = RandomArrayCards[i];
                if (isNumberMatched)
                {
                    //Debug.LogError("RandomCardIndex 1st calling:---------" + SeqCardIndex);
                    break;
                }
                else
                {

                    bool istrueOrFalse = (i % 2 == 0) ? true : false;
                    showWinnerBahar2API(SeqCardNumber, istrueOrFalse);
                }

            }
        }
        void showWinnerBahar2API(int randomCardIndex, bool isTrue)
        {
            int CardPower = AllCards[randomCardIndex].Power;
            if (CardPower == FirstCardPower)
            {
                if (isTrue)
                {
                    //Debug.LogError("Result decleared Andar Wins --------------");
                    // Debug.LogError("Result decleared Bahar 2 lose --------------" + LocalSettings.Rs(BaharTotalBetAmount));
                    AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.bahr2, BaharTotalBetAmount.ToString(), AndarBaharAPI.BetType.lose);
                }
                else
                {
                    //Debug.LogError("Result decleared Bahar Wins --------------");
                    // Debug.LogError("Result decleared Bahar 2 Wins --------------" + LocalSettings.Rs(BaharTotalBetAmount));
                    AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.bahr2, BaharTotalBetAmount.ToString(), AndarBaharAPI.BetType.win);
                }
                isNumberMatched = true;
            }
        }

        #endregion

        #region Andar API
        public void OnclickAndarBetAPI()
        {

            for (int i = 1; i < RandomArrayCards.Length; i++)
            {
                if (i == 1)
                    isNumberMatched = false;
                int SeqCardNumber = RandomArrayCards[i];
                if (isNumberMatched)
                {
                    //Debug.LogError("RandomCardIndex 1st calling:---------" + SeqCardIndex);
                    break;
                }
                else
                {

                    bool istrueOrFalse = (i % 2 == 0) ? true : false;
                    showWinnerAndarAPI(SeqCardNumber, istrueOrFalse);
                }

            }
        }
        void showWinnerAndarAPI(int randomCardIndex, bool isTrue)
        {
            int CardPower = AllCards[randomCardIndex].Power;
            if (CardPower == FirstCardPower)
            {
                if (isTrue)
                {
                    //Debug.LogError("Result decleared Andar Wins --------------");
                    AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.andar, AndarTotalBetAmount.ToString(), AndarBaharAPI.BetType.win);
                }
                else
                {
                    //Debug.LogError("Result decleared Bahar Wins --------------");

                    AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.andar, AndarTotalBetAmount.ToString(), AndarBaharAPI.BetType.lose);
                }
                isNumberMatched = true;
            }
        }
        #endregion

        #region SuperBahar API
        public void OnclickSuperBahar1BetAPI(string superBaharAmount)
        {

            for (int i = 1; i < RandomArrayCards.Length; i++)
            {
                if (i == 1)
                    isNumberMatched = false;
                int SeqCardNumber = RandomArrayCards[i];
                if (isNumberMatched)
                {
                    //Debug.LogError("RandomCardIndex 1st calling:---------" + SeqCardIndex);
                    break;
                }
                else
                {

                    bool istrueOrFalse = (i % 2 == 0) ? true : false;
                    showWinnerSuperBahar1BetAPI(SeqCardNumber, istrueOrFalse, i, superBaharAmount);
                }

            }
        }
        void showWinnerSuperBahar1BetAPI(int randomCardIndex, bool isTrue, int i, string SuperBaharAmount)
        {
            int CardPower = AllCards[randomCardIndex].Power;
            if (CardPower == FirstCardPower)
            {
                if (isTrue)
                {
                    //Debug.LogError("Result decleared Andar Wins --------------");
                    AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.superBahr, SuperBaharAmount, AndarBaharAPI.BetType.lose);
                }
                else
                {
                    //Debug.LogError("Result decleared Bahar Wins --------------");
                    if (i == 1)
                        AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.superBahr, SuperBaharAmount, AndarBaharAPI.BetType.win);
                    else
                        AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.superBahr, SuperBaharAmount, AndarBaharAPI.BetType.lose);
                }
                isNumberMatched = true;
            }
        }

        public void OnclickSuperBahar2BetAPI(string superBaharAmount)
        {

            for (int i = 1; i < RandomArrayCards.Length; i++)
            {
                if (i == 1)
                    isNumberMatched = false;
                int SeqCardNumber = RandomArrayCards[i];
                if (isNumberMatched)
                {
                    //Debug.LogError("RandomCardIndex 1st calling:---------" + SeqCardIndex);
                    break;
                }
                else
                {

                    bool istrueOrFalse = (i % 2 == 0) ? true : false;
                    showWinnerSuperBahar2BetAPI(SeqCardNumber, istrueOrFalse, i, superBaharAmount);
                }

            }
        }
        void showWinnerSuperBahar2BetAPI(int randomCardIndex, bool isTrue, int i, string SuperBaharAmount)
        {
            int CardPower = AllCards[randomCardIndex].Power;
            if (CardPower == FirstCardPower)
            {
                if (isTrue)
                {
                    //Debug.LogError("Result decleared Andar Wins --------------");
                    AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.superBahr, SuperBaharAmount, AndarBaharAPI.BetType.lose);
                }
                else
                {
                    //Debug.LogError("Result decleared Bahar Wins --------------");                  
                    if (i == 3)
                        AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.superBahr, SuperBaharAmount, AndarBaharAPI.BetType.win);
                    else
                        AndarBaharAPI.Instance.AndarBaharSendBet(AndarBaharAPI.BetType.superBahr, SuperBaharAmount, AndarBaharAPI.BetType.lose);
                }
                isNumberMatched = true;
            }
        }

        #endregion

        #endregion
    }
}