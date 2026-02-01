
using com.mani.muzamil.amjad;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionsManager : MonoBehaviourPunCallbacks
{
    public static PositionsManager Instance;

    public int playerPosition;

    [ShowOnly]
    public bool[] isBooked = new bool[5];


    [ShowOnly]
    public bool[] localIsBooked;

    [ShowOnly]
    public Transform[] localSeats;

    PhotonView thisphoton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        thisphoton = GetComponent<PhotonView>();

        // Initialize the local copy of the isBooked array
        // if (!MatchHandler.isWingoLottary())
        // {
        localIsBooked = new bool[isBooked.Length];
        Array.Copy(isBooked, localIsBooked, isBooked.Length);
        //}
        //else
        //{
        // isBooked = new bool[LocalSettings.GetMaxPlayers()];
        // localIsBooked = new bool[isBooked.Length];
        // Array.Copy(isBooked, localIsBooked, isBooked.Length);
        //}


    }


    public void AssignMyLocalPositionWithAllOtherClients()
    {
        GameManager gameManagerInstance = GameManager.Instance;
        int myNetworkSeat = gameManagerInstance.myLocalSeat;


        // Iterate over all seat indices and adjust them
        for (int i = 0; i < gameManagerInstance.playersList.Count; i++)
        {
            // if (CheckPositioAvailability())
            // {
            // Calculate the difference between myNetwork seat and desired seat indices
            int indexDiff = gameManagerInstance.playersList[i].myNetworkSeat - myNetworkSeat;
            int adjustedIndex = (indexDiff + 5) % 5;
            //gameManagerInstance.position_availability[adjustedIndex].networkSeat = adjustedIndex;
            Debug.Log("Altered Seats are " + adjustedIndex);
            gameManagerInstance.position_availability[adjustedIndex].is_reserved = gameManagerInstance.playersList[i].gameObject;
            if (!MatchHandler.isWingoLottary())
            {
                if (MatchHandler.isDragonTiger())
                    LocalSettings.SetPosAndRect(gameManagerInstance.playersList[i].gameObject, gameManagerInstance.position_availability[adjustedIndex].Pos, DragonTigerManager.Instance.DTPlayerSittingPositions[0].transform.parent);
                else
                    LocalSettings.SetPosAndRect(gameManagerInstance.playersList[i].gameObject, gameManagerInstance.position_availability[adjustedIndex].Pos, gameManagerInstance.PlayerTable);
            }
            else
            {
                LocalSettings.SetPosAndRect(gameManagerInstance.playersList[i].gameObject, gameManagerInstance.position_availability[adjustedIndex].Pos, WingoManager.Instance.WingoPositions[0].transform.parent);
                // StartCoroutine(WhenSettingFull());
            }
            // }
            //  else
            //{
            //    if (MatchHandler.isWingoLottary())
            //    {
            //        if (i >= GameManager.Instance.position_availability.Length)
            //            LocalSettings.SetPosAndRect(gameManagerInstance.playersList[i].gameObject, gameManagerInstance.positionAvailabilityFull, gameManagerInstance.positionAvailabilityFull.parent);
            //    }
            //}


        }
        if (MatchHandler.isWingoLottary() || MatchHandler.isDragonTiger())
            gameManagerInstance.WhenSettingFull();
        //Debug.LogError("Altering the seats");        
    }


    public void ReArrangePlayerSeatsAccordingToNetworkPositions()
    {
        GameManager gameManagerInstance = GameManager.Instance;
        int myNetworkSeat = gameManagerInstance.myLocalSeat;

        // Iterate over all seat indices and adjust them
        for (int i = 0; i < gameManagerInstance.position_availability.Length; i++)
        {
            // Calculate the difference between myNetwork seat and desired seat indices
            int indexDiff = myNetworkSeat;
            int adjustedIndex = (indexDiff + 5) % 5;
            //Debug.Log("Altered Seats are " + adjustedIndex);
            gameManagerInstance.position_availability[i].sitHere.GetComponent<SitHere>().positionToSit = adjustedIndex;
            gameManagerInstance.position_availability[i].networkSeat = adjustedIndex;
            myNetworkSeat++;
        }
        gameManagerInstance.position_availability[0].is_reserved = null;

    }


    void ClearTheReserveSeatsFirst()
    {
        GameManager gameManagerInstance = GameManager.Instance;
        for (int i = 0; i < gameManagerInstance.playersList.Count; i++)
        {
            gameManagerInstance.position_availability[i].is_reserved = null;
        }
    }




    public void AssignPositionOfthisPlayer(PlayerInfo info)
    {
        Debug.Log("I am calling---------------");
        int position = ReturnAvailableSeatAndSet();
        //info.AssignNetworkSeat(position);

        // Update the local copy of the isBooked array
        localIsBooked[position] = true;

        LocalSettings.GetCurrentRoom.SetRoomSeatingRecord("room_seating", isBooked);

        SendRPC(info.photonView, "AssignNetworkSeatToAllInstanceOfThisPlayer", RpcTarget.AllBuffered, position);
        info.player.SetCustomData(LocalSettings.networkPosition, position);
        // Update custom property for room
        //ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
        //roomProps[info.photonView.ViewID.ToString()] = position;
        //PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }

    public void ReleasePosition(int position)
    {
        // Update the local copy of the isBooked array
        localIsBooked[position] = false;

        Debug.Log("Master Left but still here coming");

        // Call an RPC to update the isBooked array on all clients
        SendRPC(thisphoton, "UpdateIsBookedArray", RpcTarget.AllBuffered, localIsBooked);
    }

    [PunRPC]
    public void UpdateIsBookedArray(bool[] newIsBooked)
    {
        // Update the isBooked array with the new values
        isBooked = newIsBooked;
    }

    private int ReturnAvailableSeatAndSet()
    {
        for (int i = 0; i < isBooked.Length; i++)
        {
            if (!isBooked[i])
            {
                SendRPC(thisphoton, "BookThisSeat", RpcTarget.AllBuffered, i);
                return i;
            }
        }
        return 0;
    }

    public void SitHere(int pos)
    {
        Debug.Log("Seat here " + pos);
        LocalSettings.GetCurrentRoom.SetRoomSeatingRecord("room_seating", isBooked);
        UIManager.Instance.GetMyPlayerInfo().AssignNetworkSeat(pos);
        //SendRPC(.photonView, "AssignNetworkSeatToAllInstanceOfThisPlayer", RpcTarget.AllBuffered, pos);
        UIManager.Instance.GetMyPlayerInfo().player.SetCustomData(LocalSettings.networkPosition, pos);

        UIManager.Instance.GetMyPlayerInfo().ActivatePlayerAgainOnNetwork();

        thisphoton.RPC("BookThisSeat", RpcTarget.All, pos);
    }




    [PunRPC]
    public void BookThisSeat(int index)
    {
        //   Debug.LogError("Out of Range Index is " + index);
        isBooked[index] = true;
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Masterrrrrrrrrrrrrrrrrrr Left");
        //foreach (var rpc in bufferedRPCs)
        //{
        //    if (rpc.TargetView != null)
        //    {
        //        rpc.TargetView.RPC(rpc.RpcMethodName, newMasterClient, rpc.RpcData);
        //    }
        //}
        //bufferedRPCs.Clear();
        RefreshListOnMasterClientSwitched();


    }
    private List<RpcContainer> bufferedRPCs = new List<RpcContainer>();
    void RefreshListOnMasterClientSwitched()
    {
        for (int i = 0; i < isBooked.Length; i++)
        {
            isBooked[i] = false;
            localIsBooked[i] = false;
        }

        for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
        {
            GameManager.Instance.playersList[i].ReAssignNetworkSeat();
            localIsBooked[GameManager.Instance.playersList[i].myNetworkSeat] = true;
            isBooked[GameManager.Instance.playersList[i].myNetworkSeat] = true;
        }


    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        AssignMyLocalPositionWithAllOtherClients();
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered the Room named " + newPlayer.NickName);
        if (!thisphoton.IsMine)
        {
            AssignMyLocalPositionWithAllOtherClients();
            Invoke("RefreshAfterSomeTime", 2f);
        }
    }

    void RefreshAfterSomeTime()
    {
        AssignMyLocalPositionWithAllOtherClients();
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Player name ---- " + targetPlayer.NickName + " Updated its " + changedProps.Values);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //Debug.Log("This Room Seating Updated as " + propertiesThatChanged.ToStringFull());
    }






    public void SendRPC(PhotonView targetView, string rpcMethodName, RpcTarget target, params object[] rpcData)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            targetView.RPC(rpcMethodName, target, rpcData);
            //Buffered All RPC's for the future players to play when the master client switched
            //photonView.RPC("BufferedRPCtoAllClients", RpcTarget.All, targetView, rpcMethodName, rpcData);
            //BufferedRPCtoAllClients(targetView, rpcMethodName, rpcData);
        }
        else
        {
            //bufferedRPCs.Add(new RpcContainer(targetView, rpcMethodName, rpcData));
        }
    }




}

[Serializable]
public class RpcContainer
{
    public PhotonView TargetView;
    public string RpcMethodName;
    public object[] RpcData;

    public RpcContainer(PhotonView targetView, string rpcMethodName, object[] rpcData)
    {
        TargetView = targetView;
        RpcMethodName = rpcMethodName;
        RpcData = rpcData;
    }
}

