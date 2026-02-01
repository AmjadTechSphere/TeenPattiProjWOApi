using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace com.mani.muzamil.amjad
{

    public class NetworkSettings : MonoBehaviourPunCallbacks
    {

        public static NetworkSettings Instance;
        public bool checkGameStart;
        public GameObject menuPanel, loadingPanel, LoadingPanelOld;
        public int minimumBetAmount;
        TypedLobby sqlLobby = new TypedLobby("MySqlLobby", LobbyType.SqlLobby);
        //public enum RoomFiltersBetAB
        //{
        //    Bet1000AB = 1000,
        //    Bet10000AB = 10000,
        //    Bet100000AB = 100000,
        //    Bet200000AB = 200000,
        //    Bet500000AB = 500000,
        //    Bet1000000AB = 1000000,
        //    Bet5000000AB = 5000000,
        //    Bet10000000AB = 10000000,
        //}
        //public RoomFiltersBetAB currentRoomFilterBetAB;
        //public enum RoomFiltersBetWL
        //{
        //    Bet1000WL = 1000,
        //    Bet10000WL = 10000,
        //    Bet100000WL = 100000,
        //    Bet1000000WL = 1000000,
        //    Bet10000000WL = 10000000,
        //}
        //public RoomFiltersBetWL currentRoomFilterBetWL;
        //public enum RoomFilters
        //{
        //    CLASSIC = 0,
        //    MUFFLIS = 1,
        //    HUKM = 2,
        //    ANDAR_BAHAR = 3,
        //    WINGOLOTTARY = 4,
        //    LuckyWar = 5,
        //    VrMuflis = 6,
        //    VrRoyal = 7,
        //    VrAK47 = 8,
        //    DeluxeClassic = 9,
        //    DeluxeJoker = 10,
        //    DeluxeHukam = 11,
        //    DeluxeMuflis = 12,
        //    DeluxePotBlind = 13,
        //    Poker = 14,
        //    Chatai = 15,
        //    Rummy = 16,
        //    DeluxeTable = 17,
        //    BagumPakad = 18,
        //    PlauOnHotspot = 19,
        //    PlayWithFriends = 20,
        //    None = 21
        //}
        //public RoomFilters currentRoomFilter;

        public string RoomName;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

        }

        // Start is called before the first frame update
        void Start()
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
                // MyDeviceId = deviceID;
                // Use the device ID in your code
            }
            ActiveMyPanel(loadingPanel.name);
            PhotonNetwork.AutomaticallySyncScene = true;
            if (PlayerPrefs.HasKey("name"))
                PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("name") /*+ Random.Range(100, 1000)*/;
            //PhotonNetwork.LocalPlayer.NickName = "Player " + Random.Range(100, 1000);
            if (PhotonNetwork.IsConnected)
            {
                ActiveMyPanel(menuPanel.name);
                // PhotonNetwork.GetCustomRoomList(TypedLobby.Default, sqlLobby.ToString());
            }
            else
                PhotonNetwork.ConnectUsingSettings();

            //StartCoroutine(SwtichRoom());
            StartCoroutine(SwtichRoom());
            //PhotonNetwork.PhotonServerSettings.AppSettings.Server = "127.0.0.1";
            //PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
            //PhotonNetwork.ConnectUsingSettings();
        }

        public static List<RoomInfo> roomInfos;

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (roomInfos != null)
                roomInfos.Clear();
            roomInfos = roomList;
            Debug.Log("Room Name: " + roomInfos.Count + " | Player Count: " + "/");
            foreach (RoomInfo roomInfo in roomInfos)
            {
                if (roomInfo == null)
                {
                    roomInfos.Remove(roomInfo);
                }
                Debug.Log(roomInfo.CustomProperties +
                 "   Room Name: " + roomInfo.Name + " | Player Count: " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers);
            }
        }
        IEnumerator SwtichRoom()
        {
            if (LocalSettings.isSwitchRoom)
            {
                //if (!PhotonNetwork.JoinLobby())
                //    PhotonNetwork.JoinLobby();
                LoadingPanelOld.SetActive(true);

                yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
                RefreshRoomList();
                loadingPanel.SetActive(true);
                yield return new WaitForSeconds(1f);
                // RoomName = (MatchHandler.CurrentMatch + (int)LocalSettings.MinBetAmount).ToString();
                //RoomEntranceProperty(MatchHandler.CurrentMatch, (int)LocalSettings.MinBetAmount);
                if (checkRoomId())
                    PhotonNetwork.JoinRoom(LocalSettings.GetSetRoomID);
                else
                    RoomEntranceProperty(MatchHandler.CurrentMatch, (int)LocalSettings.MinBetAmount);

            }
        }
        public void RefreshRoomList()
        {
            if (!PhotonNetwork.JoinLobby())
                PhotonNetwork.JoinLobby();
            // Get the room list with default filters
        }
        bool checkRoomId()
        {
            foreach (RoomInfo roomInfo in roomInfos)
            {
               // Debug.LogError("PlayerCOunt......" + roomInfo.PlayerCount);
                if (roomInfo.PlayerCount == 0)
                {
                   // Debug.LogError("PlayerCOunt....22.." + roomInfo.PlayerCount);
                    roomInfos.Remove(roomInfo);
                }
            }
            RoomName = MatchHandler.CurrentMatch + ((int)LocalSettings.MinBetAmount).ToString();
            Debug.LogError("Total Rooms......" + roomInfos.Count);
            for (int i = 0; i < roomInfos.Count; i++)
            {
                string roomInfoName = stripString(roomInfos[i].CustomProperties.ToStringFull());
                //Debug.LogError(" roomInfoName   " + roomInfoName);
                if (RoomName == roomInfoName/*roomInfoName.Contains(RoomName)*/)
                {
                   // Debug.LogError("Current i ......" + i + "   Check Room name  " + roomInfoName + "  localName   " + LocalSettings.GetSetRoomID);
                    if (roomInfos[i].Name == LocalSettings.GetSetRoomID)
                    {
                        if (i == roomInfos.Count - 1)
                        {
                           // Debug.LogError("check value i...1......" + i);
                            LocalSettings.GetSetRoomID = roomInfos[0].Name.ToString();
                        }

                        else
                        {
                           // Debug.LogError("check value i...2......" + i);
                            LocalSettings.GetSetRoomID = roomInfos[i + 1].Name.ToString();
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        string stripString(string str)
        {
            return str.Substring(1, str.Length - 4);
        }
        string cheeckStringBool(RoomInfo roomInfo)
        {
            string Name = roomInfo.CustomProperties.ToString();
            if (Name.Contains(RoomName))
            {
                Name = RoomName;
            }
            return Name;
        }

        private void Update()
        {
            if (checkGameStart)
            {
                CheckGameStart();
            }
        }

        void CheckGameStart()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                playgame();
            }

        }



        public void JoinRandomRoom(BigInteger EnumNumber)
        {



            //if (MatchHandler.IsTeenPatti())
            //{
            //    ActiveMyPanel(loadingPanel.name);
            //    //currentRoomFilter = (RoomFilters)EnumNumber;
            //    PhotonHashtable NORMAL = new PhotonHashtable() { { currentRoomFilter.ToString(), 1 } };
            //    PhotonNetwork.JoinRandomRoom(NORMAL, (byte)LocalSettings.GetMaxPlayers());
            //    // Debug.LogError("   hashtable " + NORMAL + "   CurrentRoomName " + currentRoomFilter + "  tableName " + MatchHandler.CurrentMatch + "  BetAmmount " + EnumNumber);
            //}
            //else if (MatchHandler.IsAndarBahar())
            //{

            //    LocalSettings.MinABBetAmount = EnumNumber;
            //    // Menu_Manager.Instance.SelectTable(3);

            //    currentRoomFilterBetAB = (RoomFiltersBetAB)EnumNumber;
            //    PhotonHashtable NORMAL = new PhotonHashtable() { { currentRoomFilterBetAB.ToString(), 1 } };
            //    PhotonNetwork.JoinRandomRoom(NORMAL, (byte)LocalSettings.GetMaxPlayers());


            //}
            //else if (MatchHandler.isWingoLottary())
            //{
            //Before
            //LocalSettings.MinWLBetAmount = EnumNumber;
            //ActiveMyPanel(loadingPanel.name);
            //currentRoomFilterBetWL = (RoomFiltersBetWL)EnumNumber;
            // RoomName = MatchHandler.CurrentMatch + EnumNumber.ToString();
            //PhotonHashtable NORMAL = new PhotonHashtable() { { RoomName.ToString(), 1 } };
            //PhotonNetwork.JoinRandomRoom(NORMAL, (byte)LocalSettings.GetMaxPlayers());

            //After

            //}

            RoomEntranceProperty(MatchHandler.CurrentMatch, EnumNumber);

        }




        public void  OnBtnClickPlayAgainBtn()
        {
            RoomEntranceProperty(MatchHandler.CurrentMatch, LocalSettings.MinBetAmount);
        }

        public void RoomEntranceProperty(MatchHandler.MATCH currentMatchHandler, BigInteger EnumNumber)
        {
            // AssignMimumBet(EnumNumber);
            LocalSettings.MinBetAmount = EnumNumber;

           // Debug.LogError("MinimumBetAmount   " + LocalSettings.Rs(LocalSettings.MinBetAmount));
           // return;
            ActiveMyPanel(loadingPanel.name);
            RoomName = MatchHandler.CurrentMatch + EnumNumber.ToString();
            Debug.Log("Room Name: " + RoomName);
            PhotonHashtable NORMAL = new PhotonHashtable() { { RoomName.ToString(), 1 } };
            byte roomEntranceNumber = (byte)LocalSettings.GetMaxPlayers();
            PhotonNetwork.JoinRandomRoom(NORMAL, roomEntranceNumber);
            // PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, sqlLobby);
            ///

        }



        #region Goto other room when switch room

        public void getRoomList()
        {
            //RoomInfo[] rooms = PhotonNetwork.GetRoomList();
            TypedLobby loby = TypedLobby.Default;
            Debug.LogError("loby name: " + loby.Name);
        }
        void JoinExistingRoom()
        {

        }

        void joinNewRoom()
        {
            byte expectedMaxPlayers;
            RoomOptions roomOptions = new RoomOptions();
            expectedMaxPlayers = (byte)LocalSettings.GetMaxPlayers();
            roomOptions.MaxPlayers = expectedMaxPlayers;

            PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default);

        }

        #endregion
        void AssignMimumBet(int miniBetAmount)
        {
            switch (MatchHandler.CurrentMatch)
            {
                case MatchHandler.MATCH.Classic:

                    break;
                case MatchHandler.MATCH.Mufflis:
                    break;
                case MatchHandler.MATCH.HUukm:
                    break;
                case MatchHandler.MATCH.Andar_Bahar:
                    LocalSettings.MinBetAmount = miniBetAmount;
                    break;
                case MatchHandler.MATCH.Wingo_Lottery:
                    LocalSettings.MinBetAmount = miniBetAmount;
                    break;
                case MatchHandler.MATCH.Lucky_War:
                    break;
                case MatchHandler.MATCH.VrMuflis:
                    break;
                case MatchHandler.MATCH.Dragon_Tiger:
                    break;
                case MatchHandler.MATCH.VrAK47:
                    break;
                case MatchHandler.MATCH.DeluxeClassic:
                    break;
                case MatchHandler.MATCH.DeluxeJoker:
                    break;
                case MatchHandler.MATCH.DeluxeHukam:
                    break;
                case MatchHandler.MATCH.DeluxeMuflis:
                    break;
                case MatchHandler.MATCH.DeluxePotBlind:
                    break;
                case MatchHandler.MATCH.Poker:
                    break;
                case MatchHandler.MATCH.Chatai:
                    break;
                case MatchHandler.MATCH.Rummy:
                    break;
                case MatchHandler.MATCH.DeluxeTable:
                    break;
                case MatchHandler.MATCH.BagumPakad:
                    break;
                case MatchHandler.MATCH.PlauOnHotspot:
                    break;
                case MatchHandler.MATCH.PlayWithFriends:
                    break;
                case MatchHandler.MATCH.None:
                    break;
                default:
                    break;
            }

        }






        //void SetMinmumAmount(int EnumNumber, RoomFilters RoomFiltersBetAB)
        //{
        //    ActiveMyPanel(loadingPanel.name);
        //    currentRoomFilterBetAB = (RoomFiltersBetAB)EnumNumber;
        //    PhotonHashtable NORMAL = new PhotonHashtable() { { currentRoomFilterBetAB.ToString(), 1 } };
        //    PhotonNetwork.JoinRandomRoom(NORMAL, (byte)LocalSettings.GetMaxPlayers());
        //}


        public void playgame()
        {
            checkGameStart = false;
            PhotonNetwork.LoadLevel("Gameplay");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected To Master");

            if (!LocalSettings.isSwitchRoom)
            {
                if (!PhotonNetwork.JoinLobby())
                    PhotonNetwork.JoinLobby();
                LoadingPanelOld.SetActive(false);
            }
            ActiveMyPanel(menuPanel.name);
            AssignPicToPlayerProperties(LocalSettings.GetprofilePic());
            
            AssignFrameToPlayerProperties(LocalSettings.GetprofileFrame());
            PhotonNetwork.LocalPlayer.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            //StartCoroutine(SwtichRoom());
        }
        // ye method int value k lye tha jab hub profile picture Game main se set kr rahe thy
        public void AssignPicToPlayerProperties(int val)
        {
            PhotonNetwork.LocalPlayer.SetCustomData(LocalSettings.ProfilePic, val);
        }
        // ye method int value k lye tha jab hub profile picture Server  main se Set kr rahe thy
        public void AssignPicToPlayerPropertiesStringForm(string val)
        {
          //  Debug.LogError("Check Name of image...." + val);
            PhotonNetwork.LocalPlayer.SetCustomString(LocalSettings.ProfilePicNameKey, val);
        }

        public void AssignFrameToPlayerProperties(int val)
        {
            PhotonNetwork.LocalPlayer.SetCustomData(LocalSettings.ProfileFrame, val);
        }

        void OnPhotonRandomJoinFailed()
        {
            byte expectedMaxPlayers;
            RoomOptions roomOptions = new RoomOptions();
            //roomOptions.CleanupCacheOnLeave = false;
            //roomOptions.PlayerTtl = -1;
            //roomOptions.EmptyRoomTtl = 30000;
            // PhotonNetwork.KeepAliveInBackground = 50f;

            expectedMaxPlayers = (byte)LocalSettings.GetMaxPlayers();

            roomOptions.MaxPlayers = expectedMaxPlayers;
            //Debug.LogError("TotalPlayer" + roomOptions.MaxPlayers);
            roomOptions.CustomRoomProperties = new PhotonHashtable() { { RoomName.ToString(), 1 } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomName.ToString() };
            //if (MatchHandler.IsTeenPatti())
            //{
            //    roomOptions.CustomRoomProperties = new PhotonHashtable() { { currentRoomFilter.ToString(), 1 } };
            //    roomOptions.CustomRoomPropertiesForLobby = new string[] { currentRoomFilter.ToString() };
            //}
            //else if (MatchHandler.IsAndarBahar())
            //{
            //    roomOptions.CustomRoomProperties = new PhotonHashtable() { { currentRoomFilterBetAB.ToString(), 1 } };
            //    roomOptions.CustomRoomPropertiesForLobby = new string[] { currentRoomFilterBetAB.ToString() };
            //}
            //else if (MatchHandler.isWingoLottary())
            //{
            //    roomOptions.CustomRoomProperties = new PhotonHashtable() { { RoomName.ToString(), 1 } };
            //    roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomName.ToString() };
            //}

            PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
            //Debug.LogError("Check Lobby Name....." + sqlLobby.Name);
            //Debug.LogError("Check Lobby Name....." + sqlLobby);

            //  PhotonNetwork.GetCustomRoomList(TypedLobby.Default, TypedLobby.Default, true);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            OnPhotonRandomJoinFailed();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Player Joined ");
            if (PhotonNetwork.IsMasterClient)
            {
                print("you are master client, waiting for full amount of players to start the game");
                checkGameStart = true;
            }
        }
        void ActiveMyPanel(string myPanelName)
        {
            menuPanel.SetActive(myPanelName.Equals(menuPanel.name));
            loadingPanel.SetActive(myPanelName.Equals(loadingPanel.name));
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
        {
            if (changedProps.ContainsKey(LocalSettings.ProfilePic))
            {
                Debug.Log("Pic Updated");
            }
            Debug.Log("Call Wasted");
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        }


    }
}