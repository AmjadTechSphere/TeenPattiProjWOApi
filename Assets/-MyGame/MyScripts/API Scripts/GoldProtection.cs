using com.mani.muzamil.amjad;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GoldProtection : ES3Cloud
{
    [Header("Panels")]
    public GameObject MainParentGoldProtectionPanel;
    public GameObject GoldProtectionPanel;
    public GameObject GoldProtectionInstructionPanel;
    public GameObject GoldProtectionSetPasswordPanel;
    public GameObject GoldProtectionRemovePasswordPanel;
    public GameObject GoldAccessPanel;

    [Header("Creating Password")]
    public Transform GoldProtectionOnBtn;
    public Transform AccountSecurityStatusBtn;
    public TMP_InputField Password1;
    public TMP_InputField Password2;

    [Header("Removing Password")]
    public TMP_InputField RemovePasswordInputField;

    [Header("Enter Password to Continue")]
    public TMP_InputField PasswordToContinueInputField;



    const string Password = "password";
    const string ProtectionStatus = "protection_status";

    #region Creating Instance;
    private static GoldProtection _instance;
    static bool isOneTimeDone;
    public static GoldProtection Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GoldProtection>();
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    protected GoldProtection(string url, string apiKey) : base(url, apiKey) { }

    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        if (LocalSettings.GetPlayerID() != 0)
            StartCoroutine(GetGoldProtectionDetailAPI(1));
        MainParentGoldProtectionPanel.SetActive(false);
        SetBtnStatus();
        Application.runInBackground = true;
    }

    public void GetGoldProtectionDetail()
    {
        StartCoroutine(GetGoldProtectionDetailAPI(0.1f));
    }
    #region General Methods
    void SetBtnStatus()
    {
        bool isProtectionEnabled = LocalSettings.GetGoldProtectionStatus();
        ToggleBtnOnOff(GoldProtectionOnBtn, isProtectionEnabled);
        ToggleBtnOnOff(AccountSecurityStatusBtn, isProtectionEnabled);
    }



    void OnGetGoldProtectionDetail(GoldProtectionPlayerDetail gppd)
    {
        if (gppd.protection_enable == "no")
        {
            LocalSettings.isPasswordRequired = false;
            LocalSettings.SetGoldProtectionStatus(false);
        }
        else
        {
            if (!isOneTimeDone)
            {
                LocalSettings.isPasswordRequired = true;
                isOneTimeDone = true;
            }
            LocalSettings.SetGoldProtectionStatus(true);
        }
        SetBtnStatus();
    }
    public void ShowSetPasswordPanelOrRemovePasswordPanel()
    {
        MainParentGoldProtectionPanel.SetActive(true);
        GoldProtectionPanel.SetActive(true);
        if (LocalSettings.GetGoldProtectionStatus())
        {
            // Show remove password panel
            GoldProtectionRemovePasswordPanel.SetActive(true);
            GoldProtectionSetPasswordPanel.SetActive(false);
        }
        else
        {
            // Show set password panel
            GoldProtectionRemovePasswordPanel.SetActive(false);
            GoldProtectionSetPasswordPanel.SetActive(true);
        }
    }
    void ToggleBtnOnOff(Transform btn, bool isOn)
    {
        btn.GetChild(0).gameObject.SetActive(isOn);
        btn.GetChild(1).gameObject.SetActive(!isOn);
    }
    #endregion

    #region Get PasswordToContinue the game
    public void ConfirmPasswordToContinue()
    {
        // To authenticate the user to continue the game
        //PasswordToContinueInputField
        string pass = PasswordToContinueInputField.text;
        if (pass == "")
        {
            Toaster.ShowAToast("Password can't be empty. \nPlease enter the password");
            return;
        }
        if (pass.Length < 6)
        {
            Toaster.ShowAToast("Password is too short \nPlease renter the password");
            return;
        }
        StartCoroutine(VerifyUserPasswordAPI(0, pass, GetPasswordVerificationStatus));
    }
    #endregion

    #region Remove password from gold protection region
    public void ConfirmToRemovePassword()
    {
        string password = RemovePasswordInputField.text;
        if (password == "")
        {
            Toaster.ShowAToast("Password can't be empty. \nPlease enter the password");
            return;
        }
        if (password.Length < 6)
        {
            Toaster.ShowAToast("Password is too short \nPlease renter the password");
            return;
        }
        StartCoroutine(RemovePasswordAPI(password));
    }


    void OnRemovePasswordComplate(string jsonString)
    {
        if (jsonString.Contains("Remove Password Successfully!"))
        {
            bool isPasswordRemoved = false;
            LocalSettings.isPasswordRequired = false;
            LocalSettings.SetGoldProtectionStatus(isPasswordRemoved);
            ToggleBtnOnOff(GoldProtectionOnBtn, isPasswordRemoved);
            ToggleBtnOnOff(AccountSecurityStatusBtn, isPasswordRemoved);
            GoldProtectionSetPasswordPanel.SetActive(false);
            GoldProtectionRemovePasswordPanel.SetActive(false);
            GoldProtectionInstructionPanel.SetActive(true);
            Toaster.ShowAToast("Password Removed Successfully");
            Debug.LogError("Password Removed Successfully");
        }
        else
        {
            Toaster.ShowAToast("Incorrect Password\nRetry Again");
            RemovePasswordInputField.text = "";
        }
    }

    #endregion

    #region Set new Password for gold protection
    public void ConfirmPasswordToSetPassword()
    {
        if (Password1.text != Password2.text)
        {
            Toaster.ShowAToast("Password does not match. \nPlease renter the password");
            return;
        }
        if (Password1.text == "" || Password2.text == "")
        {
            Toaster.ShowAToast("Password can't be empty. \nPlease enter the password");
            return;
        }
        if (Password1.text.Length < 6)
        {
            Toaster.ShowAToast("Password can't be less then 6 characters \nPlease renter the password");
            return;
        }
        bool isValid = ValidateString(Password1.text);
        if (!isValid)
        {
            Toaster.ShowAToast("Password should be alpha numeric. \nPlease renter the password");
            //Debug.LogError("String is not valid");
            return;
        }

        //Debug.LogError("<color:green>Everything is alright</color>");
        StartCoroutine(SetGoldProtectionPasswordAPI(Password1.text));
    }


    void OnSetPasswordComplate()
    {
        bool isPasswordAppliedSuccessfully = true;
        LocalSettings.isPasswordRequired = true;
        LocalSettings.SetGoldProtectionStatus(isPasswordAppliedSuccessfully);
        bool isProtectionEnabled = LocalSettings.GetGoldProtectionStatus();
        ToggleBtnOnOff(GoldProtectionOnBtn, isProtectionEnabled);
        ToggleBtnOnOff(AccountSecurityStatusBtn, isProtectionEnabled);
        GoldProtectionSetPasswordPanel.SetActive(false);
        GoldProtectionInstructionPanel.SetActive(true);
        GetGoldProtectionDetail();
    }



    public bool ValidateString(string inputPassword)
    {
        // Define a regular expression pattern to match at least one alphabet and one number.
        string pattern = @"^(?=.*[A-Za-z])(?=.*\d).+$";

        // Use Regex.IsMatch to check if the inputString matches the pattern.
        return Regex.IsMatch(inputPassword, pattern);
    }

    #endregion

    #region API Calling Gold Protection Set Password

    IEnumerator SetGoldProtectionPasswordAPI(string password)
    {
        LoginWithAllAuth.Instance.ProgressTxt.text = "Password Setting ...";
        NetworkSettings.Instance.loadingPanel.SetActive(true);
        string url = APIStrings.SetPasswordAPIURL + LocalSettings.GetPlayerID();// + "&password=" + password + "&protection_status" + "yes";
        formData = new List<KeyValuePair<string, string>>();

        AddPOSTField(Password, password);
        AddPOSTField(ProtectionStatus, "yes");

        WWWForm form = CreateWWWForm();
        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Result: " + webRequest.result);

                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    Toaster.ShowAToast("Unable to set Password\nTry again");
                }
            }
            else
            {
                Debug.LogError("Password Set successfully, ");
                Toaster.ShowAToast("Password Set Successful");
                OnSetPasswordComplate();
            }

        }
        NetworkSettings.Instance.loadingPanel.SetActive(false);
    }

    #endregion

    #region API Calling Gold Protection Get Detail Player
    GoldProtectionPlayerDetail GPPD;
    IEnumerator GetGoldProtectionDetailAPI(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoginWithAllAuth.Instance.ProgressTxt.text = "Gold Protection getting detail ...";
        NetworkSettings.Instance.loadingPanel.SetActive(true);
        string url = APIStrings.GetGoldProtectionDetailAPIURL + LocalSettings.GetPlayerID();
        using (var webRequest = UnityWebRequest.Get(url))
        {
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Result: " + webRequest.result);
                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    Toaster.ShowAToast("Unable to set Password\nTry again");
                }
            }
            //else
            //{
            //    Toaster.ShowAToast(" Gold protection detail got  Successfully");

            //}
            string jsonString = webRequest.downloadHandler.text;
            //Debug.LogError("Joson string: " + jsonString);
            GPPD = new GoldProtectionPlayerDetail();
            GPPD = JsonConvert.DeserializeObject<GoldProtectionPlayerDetail>(jsonString);
            OnGetGoldProtectionDetail(GPPD);

        }
        NetworkSettings.Instance.loadingPanel.SetActive(false);
    }

    #endregion


    #region Verify user password
    public Action<string> levelLoaderMethod;
    string levelString;
    public void AuthenticateWithPassword(Action<string> LevelLoader, string lvlString)
    {
        levelString = lvlString;
        levelLoaderMethod = null;
        levelLoaderMethod += LevelLoader;
        MainParentGoldProtectionPanel.SetActive(true);
        GoldAccessPanel.SetActive(true);
    }
    void GetPasswordVerificationStatus(string verificationString)
    {
        if (verificationString.Contains("\"result\":0"))
        {
            Debug.LogError("Password incorrect");
            Toaster.ShowAToast("Incorrect Password\nRetry Again");
            PasswordToContinueInputField.text = "";

        }
        else if (verificationString.Contains("\"result\":1"))
        {

            Debug.LogError("Password Matched ");
            LocalSettings.isPasswordRequired = false;
            MainParentGoldProtectionPanel.SetActive(false);
            GoldAccessPanel.SetActive(false);
            LocalSettings.IsPasswordChecked = true;
            if (levelLoaderMethod != null)
            {
                levelLoaderMethod?.Invoke(levelString);
            }
        }
        else
        {
            Debug.LogError("Password incorrect");
            Toaster.ShowAToast("Incorrect Password\nRetry Again");
            PasswordToContinueInputField.text = "";
        }

    }
    IEnumerator VerifyUserPasswordAPI(float delay, string password, Action<string> OnGetString)
    {
        yield return new WaitForSeconds(delay);
        LoginWithAllAuth.Instance.ProgressTxt.text = "Verifying user Password ...";
        NetworkSettings.Instance.loadingPanel.SetActive(true);
        string url = APIStrings.VerifyPasswordGoldProtectionAPIURL + LocalSettings.GetPlayerID() + "&password=" + password;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Result: " + webRequest.result);
                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    Toaster.ShowAToast("Unable to verify\nTry again");
                }
            }
            else
            {
                string jsonString = webRequest.downloadHandler.text;
                Debug.LogError("Joson string: " + jsonString);

                OnGetString?.Invoke(jsonString);
            }
        }
        NetworkSettings.Instance.loadingPanel.SetActive(false);
    }

    #endregion


    #region Remove user password

    /// <summary>
    /// removing password api
    /// </summary>
    IEnumerator RemovePasswordAPI(string password)
    {
        LoginWithAllAuth.Instance.ProgressTxt.text = "Removing Password  ...";
        NetworkSettings.Instance.loadingPanel.SetActive(true);
        // https://teenpatti.hizztechnologies.com/api/removePassword?playerId=14255092&protection_status=no&password=a123456

        string url = APIStrings.RemovePasswordAPIURL + LocalSettings.GetPlayerID();//+ "&protection_status=no" + "&password=" + password;
        formData = new List<KeyValuePair<string, string>>();

        AddPOSTField(Password, password);
        AddPOSTField(ProtectionStatus, "no");

        WWWForm form = CreateWWWForm();
        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Result: " + webRequest.result);

                if (webRequest.result.ToString().Contains("Protocol") || webRequest.result.ToString().Contains("protocol"))
                {
                    Toaster.ShowAToast("Unable to set Password\nTry again");
                }
            }
            //else
            //{
            //    Debug.LogError("Password Set successfully, ");
            //    Toaster.ShowAToast("Password Set Successful");

            //}
            string RemovingPasswordJson = webRequest.downloadHandler.text;

            OnRemovePasswordComplate(RemovingPasswordJson);
        }
        NetworkSettings.Instance.loadingPanel.SetActive(false);
    }

    #endregion


}
#region  Goldp protection detail class json to c#

public class GoldProtectionPlayerDetail
{
    public int id { get; set; }
    public string playerID { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public object deviceId { get; set; }
    public string facebook { get; set; }
    public string google { get; set; }
    public string phone { get; set; }
    public string token_id { get; set; }
    public string auth_type { get; set; }
    public string status { get; set; }
    public string protection_enable { get; set; }
}
//public class VerifyingUserPasswordClas
//{
//    public int status { get; set; }
//    public string message { get; set; }
//    public int result { get; set; }
//}


#endregion
