using com.mani.muzamil.amjad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class XPLevelupdate : MonoBehaviour
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

    int pendingXP = 0;
    public void UpdateXP()
    {
        pendingXP = LocalSettings.GetPendingXP();

        // Debug.LogError("Check Player Get PendIng XP...." + pendingXP);
        if (pendingXP > 0)
        {
            //StartCoroutine(SendPendingXPToServerAPI(pendingXP));
        }
    }

   
}
