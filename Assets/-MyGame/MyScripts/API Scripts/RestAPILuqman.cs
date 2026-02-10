
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class RestAPILuqman : MonoBehaviour
    {
        public const string ID = "id";
        public const string UserName = "username";
        public const string Email = "email";
        public const string DeviceID = "deviceId";
        public const string Facebook = "facebook";
        public const string Apple = "apple";
        public const string Google = "google";
        public const string Image = "image";
        public const string PhoneNumber = "phone_number";
        public const string PlayerDetails = "player_details";

        public const string PlayerID = "player_id";
        public const string Diamonds = "diamond";
        public const string Chips = "chips";
        public const string XP = "xp";

        public const string TotalChips = "total_chips";


        [Header("Profile Pic Related Data")]
        public Sprite ProfilePic;
        public Image ProfileImage;



        [Header("All BreakPoints Used")]
        public string GettingUserDataURL = "https://teenpati.ukregaliastore.co.uk/api/players/details?deviceId=";
        public string CreateNewUserURL = "https://teenpati.ukregaliastore.co.uk/api/players/create?deviceId=";
        public string ImageURL = "https://teenpati.ukregaliastore.co.uk/players/images/";
        public string AddingChipsURL = "https://teenpati.ukregaliastore.co.uk/api/add/player/chips?deviceId=";
        public string SubtractingChipsURL = "https://teenpati.ukregaliastore.co.uk/api/delete/player/chips?deviceId=";


        [Header("Device Id & Email")]
        public string MyDeviceId;


        [Header("Add Or Remove Chips")]
        public int ChipsToAdd;
        public int ChipsToMinus;


        #region Creating Instance;
        private static RestAPILuqman _instance;
        public static RestAPILuqman Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RestAPILuqman>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (_instance != this)
                    Destroy(gameObject);
            }
        }
        #endregion


   
        private void OnEnable()
        {
            GetDeviceUniqueID();
        }


        void GetDeviceUniqueID()
        {
            string deviceID = SystemInfo.deviceUniqueIdentifier;
            if (string.IsNullOrEmpty(deviceID))
            {
                Debug.LogError("Device ID is not available on this platform.");
                // Handle the case where the device ID is not available
            }
            else
            {
                Debug.Log("Device ID: " + deviceID);
                MyDeviceId = deviceID;
                // Use the device ID in your code
            }
        }

        public void Send()
        {
            StartCoroutine(SendingData());
        }

        

        public void AddChips(BigInteger chips)
        {
            StartCoroutine(AddingChips(chips));
        }

        public void SubtractChips(BigInteger chips)
        {
            StartCoroutine(SubtractingChips(chips));
        }

        public void GetChips(Action<BigInteger> textAction)
        {
            // networkkkkkkkkkkkkkkkkkkk
            // Should uncomment to get network cash
            return;
            StartCoroutine(GettingTotalChips(textAction));

        }


      



        public static byte[] GetSpriteBytes(Sprite sprite)
        {
            Texture2D texture = sprite.texture;
            byte[] bytes = null;

            if (texture != null)
            {
                bytes = texture.EncodeToPNG();
            }

            return bytes;
        }


        #region API Calling

        IEnumerator SendingData()
        {
            
                yield return new WaitForSeconds(0);

            

        }


       

        IEnumerator GettingTotalChips(Action<BigInteger> textAction)
        {
            string URL = GettingUserDataURL + MyDeviceId;

            using (var webRequest = UnityWebRequest.Get(URL))
            {
                //webRequest.timeout = 1;
                yield return new WaitForSeconds(0);

                string jsonString = webRequest.downloadHandler.text;
                Debug.Log("Json String Is " + jsonString);

                // Deserialize the JSON string
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonString);

                Debug.Log(playerData.total_chips);
                if (textAction != null)
                    textAction.Invoke(playerData.total_chips);

            }
        }




        IEnumerator LoadImage(string imageUrl, Action<Sprite> onSuccess)
        {
            UnityWebRequest www = UnityWebRequest.Get(imageUrl);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(www.downloadHandler.data);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), UnityEngine.Vector2.zero);

            onSuccess(sprite);
        }

        IEnumerator AddingChips(BigInteger chips)
        {
            yield return new WaitForSeconds(0);
            LocalSettings.SetNetworkCashBool(true);
            
        }

        IEnumerator SubtractingChips(BigInteger chips)
        {
            yield return new WaitForSeconds(0);
            LocalSettings.SetNetworkCashBool(true);

        }
        #endregion


        void HandleErrorCode(UnityWebRequest webRequest, bool errorIfDataIsDownloaded)
        {

        }

    }

    // Define a PlayerData class to match the structure of the JSON data
    [System.Serializable]
    public class PlayerData
    {
        public string success;
        public int total_chips;
        public MyPlayer player;
    }

    [System.Serializable]
    public class MyPlayer
    {
        public int id;
        public string username;
        public string email;
        public string deviceId;
        public string facebook;
        public string apple;
        public string google;
        public string image;
        public string created_at;
        public string updated_at;
        public string phone_number;
        public PlayerDetail[] player_details;
    }

    [System.Serializable]
    public class PlayerDetail
    {
        public int id;
        public string player_id;
        public string diamond;
        public string chips;
        public string xp;
        public string created_at;
        public string updated_at;
    }
}