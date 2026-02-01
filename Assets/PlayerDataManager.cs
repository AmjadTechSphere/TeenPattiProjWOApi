using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.mani.muzamil.amjad
{
    public class PlayerDataManager : MonoBehaviour
    {

        public GameObject playerData;

        private static PlayerDataManager _instance;
        public static PlayerDataManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<PlayerDataManager>();
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

    }
}
