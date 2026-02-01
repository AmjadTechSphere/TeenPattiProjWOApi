using com.mani.muzamil.amjad;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class Settings : MonoBehaviour
{

    public Transform OnOffBtn;
    public Transform ViberateBtn;
    public Transform SuperBaharBtn;
    public Transform Notification;
    // Start is called before the first frame update
    void Start()
    {

        Notification.gameObject.SetActive(true);
        SuperBaharBtn.parent.gameObject.SetActive(false);
        SetSoundBtn(OnOffBtn, LocalSettings.GetSoundEffect());
        if (!LocalSettings.IsMenuScene() && MatchHandler.IsAndarBahar())
        {
            SuperBaharBtn.parent.gameObject.SetActive(true);
            Notification.gameObject.SetActive(false);
            SetSoundBtn(SuperBaharBtn, LocalSettings.Get_Super_Bahar_Status());
        }

            SetViberationBtn(LocalSettings.mobilVibration);

    }


    public void SetSuperBaharValue()
    {
        LocalSettings.Set_Super_Bahar_Status(!LocalSettings.Get_Super_Bahar_Status());
        SetSoundBtn(SuperBaharBtn, LocalSettings.Get_Super_Bahar_Status());
    }


    public void SetSoundStatus()
    {
        LocalSettings.SetSoundEffect(!LocalSettings.GetSoundEffect());
        SetSoundBtn(OnOffBtn, LocalSettings.GetSoundEffect());

        // BG music handling
        Menu_Manager menu_Manager = FindObjectOfType<Menu_Manager>();
        if (menu_Manager != null)
            menu_Manager.HandleBGMusic();
    }

    public void SetViberationBtnStatus()
    {
        LocalSettings.mobilVibration = !LocalSettings.mobilVibration;
        SetViberationBtn(LocalSettings.mobilVibration);

    }

    void SetSoundBtn(Transform ButtonName, bool isTrue)
    {
        ButtonName.GetChild(0).gameObject.SetActive(isTrue);
        ButtonName.GetChild(1).gameObject.SetActive(!isTrue);
    }


    void SetViberationBtn(bool isTrue)
    {
        ViberateBtn.GetChild(0).gameObject.SetActive(isTrue);
        ViberateBtn.GetChild(1).gameObject.SetActive(!isTrue);
        LocalSettings.Vibrate();
    }


    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }


    public void ShareIDOnSocialMedia()
    {
        NativeShare ns = new NativeShare();
        string msg = "Hi, Lets be friends. \n Here is my ID: " + LocalSettings.GetPlayerID() + "\nPut this ID in add friends and send me the request";
        //string playerID = LocalSettings.GetPlayerID().ToString();
        ns.SetText(msg);

        ns.Share();

    }
}
