using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviourPunCallbacks
{

    public Player player;



    public void SettingProperty(string key, int value)
    {
        player.SetCustomData(key, value);
    }


    public int GettingProperty(string key)
    {
        return player.GetCustomData(key);
    }


}
