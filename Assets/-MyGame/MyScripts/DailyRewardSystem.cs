using UnityEngine;
using UnityEngine.UI;
using System;

public class DailyRewardSystem : MonoBehaviour
{
    //public int maxDays = 7;  // Number of days for the reward cycle
    //public int rewardAmount = 100;  // Reward amount to give to the user

    //public Button[] rewardButtons;  // Array of UI buttons for rewards

    //private int currentDay = 0;
    //private DateTime lastClaimDate;

    //private void Start()
    //{
    //    LoadData();
    //    UpdateUI(); // Update UI based on current day and claimed rewards
    //    CheckReward();
    //}

    //private void LoadData()
    //{
    //    lastClaimDate = DateTime.Parse(PlayerPrefs.GetString("LastClaimDate", DateTime.Now.ToString()));
    //    currentDay = PlayerPrefs.GetInt("CurrentDay", 0);
    //}

    //private void SaveData()
    //{
    //    PlayerPrefs.SetString("LastClaimDate", lastClaimDate.ToString());
    //    PlayerPrefs.SetInt("CurrentDay", currentDay);
    //}

    //private void UpdateUI()
    //{
    //    for (int i = 0; i < rewardButtons.Length; i++)
    //    {
    //        if (i < currentDay)
    //        {
    //            rewardButtons[i].interactable = false; // Disable already claimed rewards
    //            rewardButtons[i].GetComponentInChildren<Text>().text = "Claimed";
    //        }
    //        else if (i == currentDay)
    //        {
    //            rewardButtons[i].interactable = true; // Enable the next claimable reward
    //            rewardButtons[i].GetComponentInChildren<Text>().text = "Claim";
    //        }
    //        else
    //        {
    //            rewardButtons[i].interactable = false; // Disable future rewards
    //            rewardButtons[i].GetComponentInChildren<Text>().text = "Locked";
    //        }
    //    }
    //}

    //public void ClaimReward(int day)
    //{
    //    lastClaimDate = DateTime.Parse("8/14/2023 12:28:34 PM");
    //    string claimdays = DaysDiff(DateTime.Now.ToString(), lastClaimDate.ToString()).ToString();
    //    Debug.LogError("Day: " + day +"    Last claim day: " + lastClaimDate +"    days: " + claimdays);
    //    //if (day == currentDay && (DateTime.Now - lastClaimDate).TotalDays >= 1)
    //    if (day == currentDay && DaysDiff(DateTime.Now.ToString(), lastClaimDate.ToString()) >= 1)
    //    {
    //        lastClaimDate = DateTime.Now;
    //        currentDay = (currentDay % maxDays) + 1;
    //        rewardButtons[day].interactable = false; // Disable button after claiming
    //        rewardButtons[day].GetComponentInChildren<Text>().text = "Claimed";
    //        SaveData();
    //    }
    //}

    //private void CheckReward()
    //{
    //    if ((DateTime.Now - lastClaimDate).TotalDays >= 1)
    //    {
    //        rewardButtons[currentDay].interactable = true; // Enable claimable reward
    //    }
    //}


    //int DaysDiff(string prevDays, string nowdateTime)
    //{
    //    string dateString1 = nowdateTime;
    //    string dateString2 = prevDays;

    //    // Convert date strings to DateTime objects
    //    DateTime date1 = DateTime.Parse(dateString1);
    //    DateTime date2 = DateTime.Parse(dateString2); ;

    //    // Calculate the difference
    //    TimeSpan timeDifference = date2 - date1;

    //    // Extract days, hours, minutes, and seconds
    //    int totalDays = (int)timeDifference.TotalDays;
    //    //int hours = timeDifference.Hours;
    //    //int minutes = timeDifference.Minutes;
    //    //int seconds = timeDifference.Seconds;

    //    // Print the result
    //    Debug.Log($"Total days: {totalDays}");
    //    //Debug.Log($"Time: {hours} hours, {minutes} minutes, {seconds} seconds");
    //    return totalDays;
    //}
}
