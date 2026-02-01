using com.mani.muzamil.amjad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class XPLevelupdate : ES3Cloud
{
    // Start is called before the first frame update
    //const string TotalXPToAdd = "xp";
    //const string PlayerID = "playerID";
    #region Creating Instance;
    private static XPLevelupdate _instance;
    public static XPLevelupdate Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<XPLevelupdate>();
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
    void Start()
    {
        if (LocalSettings.IsMenuScene())
            UpdateXP();
        //StartCoroutine(SendPendingXPToServerAPI(65));
    }

    protected XPLevelupdate(string url, string apiKey) : base(url, apiKey)
    {

    }
    int pendingXP = 0;
    public void UpdateXP()
    {
        pendingXP = LocalSettings.GetPendingXP();

        // Debug.LogError("Check Player Get PendIng XP...." + pendingXP);
        if (pendingXP > 0)
        {
            StartCoroutine(SendPendingXPToServerAPI(pendingXP));
        }
    }

    public IEnumerator SendPendingXPToServerAPI(int xpToAdd)
    {
        yield return new WaitForSeconds(1f);
        if (LocalSettings.IsMenuScene())
            NetworkSettings.Instance.loadingPanel.SetActive(true);
        string url = APIStrings.AddXPToServerAPIURL + LocalSettings.GetPlayerID() + "&xp=" + xpToAdd;
        formData = new List<KeyValuePair<string, string>>();

        //AddPOSTField(PlayerID, LocalSettings.GetPlayerID().ToString());
        //AddPOSTField(TotalXPToAdd, xpToAdd.ToString());

        WWWForm form = CreateWWWForm();
        using (var webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 15;
            yield return SendWebRequest(webRequest);
            HandleError(webRequest, true);
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("XP Result: " + webRequest.result + "     Error code: " + webRequest.responseCode);
                //Debug.LogError("XP Result: " + webRequest.result + "     Error code: " + webRequest.);
                UpdateXP();

            }
            else
            {
                Debug.LogError("Pending xp updated successfully ");

                LocalSettings.SetPendingXP(-pendingXP);
                RestAPI.Instance.GetAndSetPlayerDetail();
                // RestAPI.Instance.FetchData(LocalSettings.GetTokenID(), Menu_Manager.Instance.SetUserNameAndOtherThings);
            }

        }
        if (LocalSettings.IsMenuScene())
            NetworkSettings.Instance.loadingPanel.SetActive(false);

    }
}
