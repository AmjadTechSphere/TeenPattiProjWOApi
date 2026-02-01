using UnityEngine;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;
using System;

public class GuestAuth : MonoBehaviour
{
    public GameObject Authpanel;
    FirebaseAuth auth;

    #region Creating Instance
    private static GuestAuth _instance;
    public static GuestAuth Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GuestAuth>();
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
            _instance = this;

    }
    #endregion
    //public Action<string, string> returnUserID;
    // Start is called before the first frame update
    void Start()
    {
        //returnUserID = getUserOrTokenID;
    }


    public void getUserOrTokenID(string tokenID, string isAlready)
    {
        //ShowPopUpMessage("Congrates \n " + isAlready + " \nDelegate User ID is: " + userID);
        // After successfully getting toker ID
        Debug.LogError("Token ID: \n" + tokenID + "\n" + isAlready);
        LoginWithAllAuth.Instance.GetTokenIDAndOtherThing(1, tokenID, "", "", null);
        //Authpanel.SetActive(false);
    }
    public void SignInAnonymously()
    {
        LoginWithAllAuth.Instance.WaitPanel.SetActive(true);
        SigninAnnonmouslyy(getUserOrTokenID);
    }

    public bool CheckIfAnonymousUserExists()
    {
        auth = FirebaseAuth.DefaultInstance;

        // Check if the current user is signed in and anonymous
        if (auth.CurrentUser != null && auth.CurrentUser.IsAnonymous)
        {
            // Anonymous user exists
            //string userId = auth.CurrentUser.UserId;
            //ShowPopUpMessage("Anonymous User Exists" + "\nUser ID: " + userId);
            return true;
        }
        else
        {
            // Anonymous user does not exist
            //Debug.Log("Anonymous user does not exist.");
            return false;
        }
    }
    public void SigninAnnonmouslyy(Action<string, string> GetUserIDString)
    {
        auth = FirebaseAuth.DefaultInstance;
        if (CheckIfAnonymousUserExists())
        {
            //ShowPopUpMessage("Anonymous User already Exists" + "\nUser ID: " + auth.CurrentUser.UserId);
            GetUserIDString.Invoke(auth.CurrentUser.UserId, "Your ID Already exists");

            return;
        }

        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                //UpdateText("SignInAnonymouslyAsync was canceled.");
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                LoginWithAllAuth.Instance.ShowError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                //UpdateText("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                LoginWithAllAuth.Instance.ShowError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            AuthResult result = task.Result;
            //UpdateText("New User ID: " + result.User.UserId + "     DisPlayname: " + result.User.DisplayName);
            //ShowPopUpMessage("New User ID: " + result.User.UserId + "\n  DisPlayname: " + result.User.DisplayName);
            GetUserIDString.Invoke(auth.CurrentUser.UserId, "Your ID is newly created");
        });




    }
    public void GuestSingOut()
    {

        DailyReward.Instance.isCalledOnce = false;
        DailyReward.dayToCollect = 1;
        DailyReward.getRewardDayStatusClass = null;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        LoginWithAllAuth.Instance.AuthPanel.SetActive(true);

    }
}
