using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class MyGameManager : MonoBehaviour
{
    public static MyGameManager instance;


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        float angularStart = PhotonNetwork.CurrentRoom.PlayerCount * PhotonNetwork.LocalPlayer.GetPlayerNumber() ;
        //float x = 2 * angularStart ;
        //float z = 2 * angularStart;
        Vector3 position = Vector3.zero;
        switch (PhotonNetwork.LocalPlayer.GetPlayerNumber())
        {
            case 1:
                position = new Vector3(3, 3f, 0);
                break;
            case 2:
                 position = new Vector3(3,0 , 0);
                break;
        }

        
       // Quaternion rotation = Quaternion.Euler(0.0f, angularStart, 0.0f);
        print("finding player");
        if(MyPracticePlayer.MyLocalPlayerInstance== null)
        {
            Debug.Log("Instantiating player");
          //  Vector2 position = new Vector2(2,0);
            PhotonNetwork.Instantiate("PlayerTemp", position, Quaternion.identity,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
