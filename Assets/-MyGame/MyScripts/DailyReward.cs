using com.mani.muzamil.amjad;
using Newtonsoft.Json;
using Photon.Pun;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static GoldWinLoose;

public class DailyReward : ES3Cloud
{
    public GameObject DailyRewardPanel;
    public GameObject DailyRewardSuccessPanel;
    public GameObject DailyRewardLoadingPanel;
    public TMP_Text dailyRewardsAmount;
    [SerializeField] TMP_Text ClaimText;
    [SerializeField] Transform[] DaysBtns;
    public bool isCalledOnce;
    public static int dayToCollect;

    public static DailyReward Instance;
    protected DailyReward(string url, string apiKey) : base(url, apiKey)
    {

    }

    private void Start()
    {
        Instance = this;
        DailyRewardPanel.SetActive(false);
        DailyRewardSuccessPanel.SetActive(false);
        DailyRewardLoadingPanel.SetActive(false);
        GetTodayDailyRewardStatusNow(1);
    }

    public void GetTodayDailyRewardStatusNow(float delay)
    {
        if (LocalSettings.GetPlayerID() != 0 && !isCalledOnce)
        {
            if (getRewardDayStatusClass == null)
                GetTodayRewardStatus(delay);
            else
                SetButtonsOfDailyReward();
        }
    }
    #region Offline daily Reward
    //void addCash()
    //{
    //    Debug.LogError("Cash Added     9999999999999");
    //    GoldWinLoose.Instance.SendGold("collecting_gold_Main_Menu", "Daily_Reward", "Daily_reward_table", Trans.win, "9999999999999");
    //}
    // Start is called before the first frame update
    //private void OnEnable()
    //{
    //    UpDateReceivedBtnsStatus();
    //}
    //string CurrentDate()
    //{
    //    DateTime dt = DateTime.Now;
    //    return dt.ToString("yyyy-MM-dd");
    //}

    //void CheckDayExceeding(int day)
    //{
    //    if (LocalSettings.GetRewardDate() == "")
    //    {
    //        LocalSettings.SetRewardDate(CurrentDate());
    //    }

    //    int daysCount = DaysDiff(LocalSettings.GetRewardDate(), CurrentDate());

    //    if (daysCount == 0 && LocalSettings.GetRewardDay() == 0)
    //    {
    //        LocalSettings.SetRewardDay(1);
    //        dayToCollect = LocalSettings.GetRewardDay();
    //    }
    //    else if (daysCount == 1)
    //    {
    //        dayToCollect = LocalSettings.GetRewardDay();
    //        if (dayToCollect > 7)
    //        {
    //            LocalSettings.SetRewardDay(1);
    //            dayToCollect = LocalSettings.GetRewardDay();
    //        }
    //    }
    //    else if (daysCount == 0)
    //    {
    //        dayToCollect = LocalSettings.GetRewardDay();
    //    }
    //    else
    //    {
    //        LocalSettings.SetRewardDay(1);
    //        dayToCollect = LocalSettings.GetRewardDay();
    //    }

    //}
    //IEnumerator SetRemainingTime()
    //{

    //    DateTime dt = DateTime.Now;
    //    // DateTime EndTime = new DateTime(dt.Year, dt.Month, dt.Day + 1, 00, 00, 00);
    //    DateTime EndTime = dt.AddDays(1).Date;

    //    while (true)
    //    {
    //        TimeSpan timeDifference = EndTime - DateTime.Now;
    //        if (timeDifference.Seconds < 0)
    //        {
    //            StopCoroutine(SetRemainingTime());
    //            UpDateReceivedBtnsStatus();
    //            ClaimText.gameObject.SetActive(false);
    //            break;
    //        }
    //        if (!DailyRewardPanel.activeInHierarchy)
    //            StopCoroutine(SetRemainingTime());
    //        ClaimText.text = timeDifference.Hours.ToString("00") + ":" + timeDifference.Minutes.ToString("00") + ":" + timeDifference.Seconds.ToString("00");
    //        yield return new WaitForSecondsRealtime(1);
    //    }
    //}
    //void UpDateReceivedBtnsStatus()
    //{
    //    CheckDayExceeding(dayToCollect);


    //    if (LocalSettings.GetRewardDate() == CurrentDate())
    //    {
    //        StartCoroutine(SetRemainingTime());
    //        ClaimText.gameObject.SetActive(true);
    //    }
    //    foreach (Transform obj in DaysBtns)
    //    {
    //        obj.GetChild(2).gameObject.SetActive(false);
    //        obj.GetChild(0).gameObject.SetActive(false);
    //    }
    //    for (int i = 0; i < dayToCollect; i++)
    //    {
    //        if (dayToCollect == i + 1 && CurrentDate() == LocalSettings.GetRewardCollectedDate())
    //        {
    //            DaysBtns[i].GetChild(2).gameObject.SetActive(false);
    //        }
    //        else if (dayToCollect > i + 1)
    //        {
    //            DaysBtns[i].GetChild(2).gameObject.SetActive(true);
    //        }
    //        if (dayToCollect == i + 1 && CurrentDate() != LocalSettings.GetRewardCollectedDate())
    //        {
    //            DaysBtns[i].GetChild(0).gameObject.SetActive(true);
    //        }
    //    }

    //}
    //public void ClaimBtn(int day)
    //{
    //    if (dayToCollect == day)
    //    {
    //        if (CurrentDate() == LocalSettings.GetRewardCollectedDate())
    //        {
    //            // come on next day
    //            if (day < dayToCollect)
    //            {
    //                Toaster.ShowAToast("Reward already collected");
    //            }
    //            else
    //            {
    //                Toaster.ShowAToast("Come tomarrow to collect this reward");
    //            }
    //        }
    //        else
    //        {
    //            SetDayReward(day);
    //            LocalSettings.SetRewardCollectedDate(CurrentDate());
    //            LocalSettings.SetRewardDay(LocalSettings.GetRewardDay() + 1);
    //            LocalSettings.SetRewardDate(CurrentDate());
    //            dailyRewardsAmount.transform.parent.parent.parent.gameObject.SetActive(true);
    //            //Toaster.ShowAToast("Reward collected");

    //        }
    //    }
    //    else
    //    {
    //        if (day < dayToCollect)
    //        {
    //            Toaster.ShowAToast("Reward already collected");
    //        }
    //        else
    //        {
    //            Toaster.ShowAToast("Come tomarrow to collect this reward");
    //        }
    //    }

    //    UpDateReceivedBtnsStatus();

    //}

    //int DaysDiff(string prevDays, string nowdateTime)
    //{
    //    string dateString1 = nowdateTime;
    //    string dateString2 = prevDays;

    //    DateTime date1 = DateTime.Parse(dateString1);
    //    DateTime date2 = DateTime.Parse(dateString2);

    //    TimeSpan timeDifference = date2 - date1;

    //    int totalDays = ((int)timeDifference.TotalDays);
    //    //int hours = timeDifference.Hours;
    //    //int minutes = timeDifference.Minutes;
    //    //int seconds = timeDifference.Seconds;

    //    // Print the result
    //    //Debug.Log($"Total days: {totalDays}");
    //    //Debug.Log($"Time: {hours} hours, {minutes} minutes, {seconds} seconds");

    //    int days = Mathf.Abs(totalDays);
    //    return days;
    //}
    #endregion
    public void ClaimBtn(int day)
    {
        if (day == dayToCollect)
        {
            if (getRewardDayStatusClass.get == "no")
            {
                // collect today reward
                SetDayReward(dayToCollect);
                //Toaster.ShowAToast("Still working on it");
                //LocalSettings.IsTodayRewardCollected = false;
            }
            else
            {
                Toaster.ShowAToast("Reward already collected");
                //LocalSettings.IsTodayRewardCollected = true;
            }
        }
        else if (dayToCollect < day)
        {
            Toaster.ShowAToast("Come tomarrow to collect this reward");
        }
        else if (dayToCollect > day)
        {
            Toaster.ShowAToast("Reward already collected");
        }
    }
    int chipsAmount;
    void SetDayReward(int dayCollect)
    {
        chipsAmount = 10000;
        switch (dayCollect)
        {
            case 1: chipsAmount = 10000; break;
            case 2: chipsAmount = 30000; break;
            case 3: chipsAmount = 40000; break;
            case 4: chipsAmount = 60000; break;
            case 5: chipsAmount = 80000; break;
            case 6: chipsAmount = 100000; break;
            case 7: chipsAmount = 150000; break;
        }
        CollectReward(chipsAmount.ToString());
    }



    #region Geting Daily Raward Status 
    void GetTodayRewardStatus(float delay)
    {
        StartCoroutine(GetDailyRewardGetStatusAPI(delay));
    }
    public static GetRewardDayStatusClass getRewardDayStatusClass;
    IEnumerator GetDailyRewardGetStatusAPI(float delay)
    {
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
        yield return new WaitForSecondsRealtime(delay);
        string url = APIStrings.GetdailyReardStatusAPIURL + LocalSettings.GetPlayerID();
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error is: " + webRequest.result);

            }
            else
            {
                Debug.Log("success is: " + webRequest.result);
            }
            string jsonString = webRequest.downloadHandler.text;
            //Debug.LogError("DailyReward : " + jsonString);
            getRewardDayStatusClass = new GetRewardDayStatusClass();
            getRewardDayStatusClass = JsonConvert.DeserializeObject<GetRewardDayStatusClass>(jsonString);
            GetStatusOfDailyReward();
        }
    }
    #endregion
    void GetStatusOfDailyReward()
    {
        int day = getRewardDayStatusClass.day;
        if (getRewardDayStatusClass.get == "no")
        {
            DailyRewardPanel.SetActive(true);
        }
        //else
        //{
        //    DailyRewardPanel.SetActive(false);
        //}
        dayToCollect = day;
        SetButtonsOfDailyReward();
    }
    void SetButtonsOfDailyReward()
    {
        for (int i = 0; i < DaysBtns.Length; i++)
        {
            if ((dayToCollect - 1) > i)
            {
                DaysBtns[i].GetChild(2).gameObject.SetActive(true);
                DaysBtns[i].GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                DaysBtns[i].GetChild(0).gameObject.SetActive(false);
                DaysBtns[i].GetChild(2).gameObject.SetActive(false);
            }
        }
        if (getRewardDayStatusClass.get == "no")
        {
            DailyRewardPanel.SetActive(true);
            DaysBtns[dayToCollect - 1].GetChild(0).gameObject.SetActive(true);
            ClaimText.gameObject.SetActive(false);
        }
        else
        {
            ClaimText.gameObject.SetActive(true);
            DaysBtns[dayToCollect - 1].GetChild(0).gameObject.SetActive(true);
            DaysBtns[dayToCollect - 1].GetChild(2).gameObject.SetActive(true);
            StartCoroutine(SetRemainingTime());
        }
        isCalledOnce = true;
    }

    IEnumerator SetRemainingTime()
    {

        DateTime dt = DateTime.Now;
        // DateTime EndTime = new DateTime(dt.Year, dt.Month, dt.Day + 1, 00, 00, 00);
        DateTime EndTime = dt.AddDays(1).Date;

        while (true)
        {
            TimeSpan timeDifference = EndTime - DateTime.Now;
            if (timeDifference.Seconds < 0)
            {
                StopCoroutine(SetRemainingTime());
                //UpDateReceivedBtnsStatus();
                ClaimText.gameObject.SetActive(false);
                break;
            }
            if (!DailyRewardPanel.activeInHierarchy)
                StopCoroutine(SetRemainingTime());
            ClaimText.text = timeDifference.Hours.ToString("00") + ":" + timeDifference.Minutes.ToString("00") + ":" + timeDifference.Seconds.ToString("00");
            yield return new WaitForSecondsRealtime(1);
        }
    }

    #region Collecting reward api

    public void CollectReward(string RewardAmount)
    {

        StartCoroutine(CollectingRewardAPI(RewardAmount));
    }
    IEnumerator CollectingRewardAPI(string RewardAmount)
    {
        string url = APIStrings.SenddailyReardStatusAPIURL + LocalSettings.GetPlayerID() + "&day=" + dayToCollect + "&reward=" + RewardAmount;
        DailyRewardLoadingPanel.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        formData = new List<KeyValuePair<string, string>>();

        WWWForm form = CreateWWWForm();

        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 12;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);

            int responseCode = (int)webRequest.responseCode;
            string responseText = webRequest.downloadHandler.text;
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Result: " + webRequest.result);
                Toaster.ShowAToast("Unable to Collect Reward\nTry Again Later");
            }
            else
            {
                GetTodayRewardStatus(0.05f);
                dailyRewardsAmount.text = LocalSettings.Rs(RewardAmount);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.DailyReward, false);
                DailyRewardSuccessPanel.SetActive(true);
                // give reward of the day from 0 to 6
                //GoldWinLoose.Instance.SendGold("collecting_gold_Main_Menu", "Daily_Reward", "Daily_reward_table", Trans.win, chipsAmount.ToString());
            }
            DailyRewardLoadingPanel.SetActive(false);

        }

    }
    #endregion


    #region Json to c# 

    public class GetRewardDayStatusClass
    {
        public int day { get; set; }
        public string get { get; set; }
    }

    #endregion
}
