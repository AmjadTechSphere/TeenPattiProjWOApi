
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class RestAPILuqman : ES3Cloud
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


        protected RestAPILuqman(string url, string apiKey) : base(url, apiKey)
        {

        }

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

        public void Receive()
        {
            StartCoroutine(ReceivingData());
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


        void RetrieveImageFromDB(string imagePath)
        {
            StartCoroutine(LoadImage(ImageURL + imagePath, sprite =>
            {
                ProfileImage.sprite = sprite;
            }));
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
            string URL = CreateNewUserURL;

            formData = new List<KeyValuePair<string, string>>();


            // Add the image byte array to the form       
            AddPOSTField(UserName, "MyTest2ss31July");
            AddPOSTField(Email, "lucmanmaniJuly@gmail.com");
            AddPOSTField(DeviceID, "luqmanDeviceID"/*MyDeviceId*/);
            AddPOSTField(Diamonds, "100");
            AddPOSTField(XP, "000");
            AddPOSTField(Chips, "4000");


            WWWForm form = CreateWWWForm();

            byte[] imageBytes = GetSpriteBytes(ProfilePic);
            form.AddBinaryData(Image, imageBytes);

            using (var webRequest = UnityWebRequest.Post(URL, form))
            {
                webRequest.timeout = 10;
                yield return SendWebRequest(webRequest);
                HandleError(webRequest, true);

                Debug.Log(webRequest.downloadHandler.text.ToString());
            }

        }


        IEnumerator ReceivingData()
        {
            string URL = GettingUserDataURL + MyDeviceId;

            using (var webRequest = UnityWebRequest.Get(URL))
            {
                //webRequest.timeout = 1;
                yield return SendWebRequest(webRequest);
                HandleError(webRequest, true);

                string jsonString = webRequest.downloadHandler.text;
                Debug.Log("Json String Is " + jsonString);

                // Deserialize the JSON string
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonString);

                // Access the deserialized data
                Debug.Log(playerData.success);
                Debug.Log(playerData.player.username);
                Debug.Log(playerData.player.image);
                Debug.Log(playerData.player.player_details[0].diamond);
                Debug.Log(playerData.total_chips);

                RetrieveImageFromDB(playerData.player.image);

            }
        }

        IEnumerator GettingTotalChips(Action<BigInteger> textAction)
        {
            string URL = GettingUserDataURL + MyDeviceId;

            using (var webRequest = UnityWebRequest.Get(URL))
            {
                //webRequest.timeout = 1;
                yield return SendWebRequest(webRequest);
                HandleError(webRequest, true);

                string jsonString = webRequest.downloadHandler.text;
                Debug.Log("Json String Is " + jsonString);

                // Deserialize the JSON string
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonString);

                Debug.Log(playerData.total_chips);
                if(textAction!= null)
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
            string URL = AddingChipsURL + MyDeviceId + "&" + Chips + "=" + chips;

            using (var webRequest = UnityWebRequest.Get(URL))
            {
                webRequest.timeout = 10;
                yield return SendWebRequest(webRequest);
                HandleError(webRequest, true);
                Debug.Log(webRequest.downloadHandler.text.ToString());
                LocalSettings.SetNetworkCashBool(true);                
            }
        }

        IEnumerator SubtractingChips(BigInteger chips)
        {
            string URL = SubtractingChipsURL + MyDeviceId + "&" + Chips + "=" + chips;

            using (var webRequest = UnityWebRequest.Get(URL))
            {
                webRequest.timeout = 10;
                yield return SendWebRequest(webRequest);
                HandleError(webRequest, true);
                if (webRequest.responseCode == 420)
                {
                    Debug.Log("Sorry, You Dont Have Enough Chips");
                }
                else
                {
                    Debug.Log("Chips Subtrated");
                    LocalSettings.SetNetworkCashBool(true);
                }
                Debug.Log(webRequest.downloadHandler.text.ToString());

            }
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