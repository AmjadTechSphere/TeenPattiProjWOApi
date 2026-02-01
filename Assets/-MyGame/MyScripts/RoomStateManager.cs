using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace com.mani.muzamil.amjad
{
    public class RoomStateManager : MonoBehaviourPunCallBacksWithNSSCallBacks
    {
        public static RoomStateManager Instance;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;

        }

        public RoomState.STATE CurrentRoomState;


        public void UpdateLocalRoomState(RoomState.STATE state)
        {
            CurrentRoomState = state;
        }

        public void UpdateCurrentRoomState(RoomState.STATE state)
        {
            //Room State and Room Property should be update by Master Client and hence will be updated on all Clients.
            //Now every client will receive callback on network and update accordingly.
            //UpdateLocalRoomState(state);
            if (PhotonNetwork.IsMasterClient)
            {
                UpdateRoomStateProperty(state);
                photonView.RPC("UpdateThisStateOnNetwork", RpcTarget.All, state, "");
            }
        }

        public void UpdateCurrentState(RoomState.STATE state, string infoText)
        {
            //Room State and Room Property should be update by Master Client and hence will be updated on all Clients.
            //Now every client will receive callback on network and update accordingly.
            if (PhotonNetwork.IsMasterClient)
            {
                UpdateRoomStateProperty(state);
                photonView.RPC("UpdateThisStateOnNetwork", RpcTarget.All, state, infoText);
            }
        }


        public void UpdateCurrentStateOnShowBtn(RoomState.STATE state)
        {
            //Room State and Room Property should be update by Master Client and hence will be updated on all Clients.
            //Now every client will receive callback on network and update accordingly.

            UpdateRoomStateProperty(state);
            photonView.RPC("UpdateThisStateOnNetwork", RpcTarget.All, state, "");

        }

        public void UpdateCurrentStateOnShowBtn(RoomState.STATE state, string infoText)
        {
            //Room State and Room Property should be update by Master Client and hence will be updated on all Clients.
            //Now every client will receive callback on network and update accordingly.

            UpdateRoomStateProperty(state);
            photonView.RPC("UpdateThisStateOnNetwork", RpcTarget.All, state, infoText);

        }



        public RoomState.STATE GetCurrentRoomState()
        {
            return CurrentRoomState;
        }

        [PunRPC]
        public void UpdateThisStateOnNetwork(RoomState.STATE state, string info)
        {
            CurrentRoomState = state;
            Debug.Log("Current State is Set to " + state);
            OnUpdateCurrentState(state, info);
        }

        void UpdateRoomStateProperty(RoomState.STATE state)
        {
            if (PhotonNetwork.IsConnectedAndReady)
                LocalSettings.GetCurrentRoom.SetRoomStateProperty(LocalSettings.roomState, state);
        }


        void OnUpdateCurrentState(RoomState.STATE state, string infoText)
        {
            switch (state)
            {
                case RoomState.STATE.WaitingForPlayers:
                    TriggerStateWaitingForPlayers();
                    break;

                case RoomState.STATE.GameIsStarting:
                    TriggerStateGameIsStarting();
                    break;

                case RoomState.STATE.CardDistributing:
                    TriggerStateCardDistributing();
                    break;

                case RoomState.STATE.GameIsPlaying:
                    TriggerStateGameIsPlaying();
                    break;

                case RoomState.STATE.GameSideShow:
                    TriggerStateSideShow();
                    break;

                case RoomState.STATE.WaitingForResults:
                    TriggerStateWaitingForResults(infoText);
                    break;

                case RoomState.STATE.ShowingResults:
                    TriggerStateShowingResults();
                    break;


                case RoomState.STATE.ABFirstTurn:
                    TriggerStateabFirstTurn();
                    break;

                case RoomState.STATE.ABSecondTurn:
                    TriggerStateabSecondTurn();
                    break;
            }
        }



        public bool IsStarted()
        {
            //Debug.LogError("My Room State RPC is: " + GetCurrentState().ToString());
            //Debug.LogError("My Room State Property is: " + room.GetRoomStateProperty(LocalSettings.roomState));
            if (!MatchHandler.IsPoker())
                return GetCurrentRoomState() != RoomState.STATE.GameIsStarting && GetCurrentRoomState() != RoomState.STATE.WaitingForPlayers;
            else
                return GetCurrentRoomState() != RoomState.STATE.GameIsStarting && GetCurrentRoomState() != RoomState.STATE.WaitingForPlayers && GetCurrentRoomState() != RoomState.STATE.CardDistributing;
        }




        public bool GetIsNotInStartedState()
        {
            return GetCurrentRoomState() == RoomState.STATE.GameIsStarting ||
                GetCurrentRoomState() == RoomState.STATE.WaitingForPlayers ||
                GetCurrentRoomState() == RoomState.STATE.CardDistributing;
        }


        #region OverrideFunctions


        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(LocalSettings.roomState))
            {
                Debug.Log("Changed in Room State To " + propertiesThatChanged.Values);
            }
            //Debug.Log("Room Property Updated" + propertiesThatChanged);
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
        }

        #endregion        

    }
}