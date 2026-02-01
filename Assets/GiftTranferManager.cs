using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class GiftTranferManager : MonoBehaviour
    {

        public GiftCollection giftPopularCollection;
        public GiftCollection giftAllCollection;
        public GameObject giftPopularBtn;
        public GameObject giftAllBtn;
        public GameObject PopularGiftPanel;
        public GameObject AllGiftPanel;
        UIManager uIManager;

        public RectTransform giftInspos;
        public GameObject markSign;

        private static GiftTranferManager _Instance;
        public static GiftTranferManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = GameObject.FindObjectOfType<GiftTranferManager>();

                return _Instance;
            }
        }

        int ProfileIndex;
        Transform giftPopularBtnsParent;
        Transform giftAllBtnsParent;
        // Start is called before the first frame update
        private void Awake()
        {

            _Instance = this;
        }

        void Start()
        {
            uIManager = UIManager.Instance;
            giftPopularBtnsParent = giftPopularBtn.transform.parent;
            giftAllBtnsParent = giftAllBtn.transform.parent;
            CreategiftBtn(giftPopularCollection, giftPopularBtn);
            CreategiftBtn(giftAllCollection, giftAllBtn);
            ActiveMyMode(PopularGiftPanel.name);
            SetAllInUseLabelFalse(giftPopularBtnsParent, popularIndex);
            SetAllInUseLabelFalse(giftAllBtnsParent, AllIndex);
            //CreateAvatorBtns();
        }


        public void ClickOnUseBtn()
        {
            if (UpdateFrames.Instance.avatarpanel.activeSelf)
            {
                LocalSettings.SetprofilePic(ProfileIndex);

            }
            else
            {
                LocalSettings.SetprofileFrame(UpdateFrames.Instance.profileFrameIndex);
                UpdateFrames.Instance.GetProfileImage();
            }



        }



        /// <summary>
        /// ///////////////////////////////////////////////////////////////
        /// </summary>


        public void CreategiftBtn(GiftCollection gift, GameObject InsBtn)
        {
            for (int i = 0; i < gift.giftPopularArray.Length; i++)
            {
                GameObject giftBtn = Instantiate(InsBtn);
                LocalSettings.SetPosAndRect(giftBtn, InsBtn.GetComponentInChildren<RectTransform>(), InsBtn.transform.parent);
                giftBtn.GetComponent<Image>().sprite = gift.giftPopularArray[i].Sprites;
                giftBtn.GetComponentInChildren<TMP_Text>().text = gift.giftPopularArray[i].giftCost.ToString();
                giftBtn.SetActive(true);

                // Adding button listener
                Button BtnAvat = giftBtn.GetComponentInChildren<Button>();
                BtnAvat.onClick.AddListener(() => OnAvatorSelectBtnClick());
            }
        }

        void SetAllInUseLabelFalse(Transform parnetObj, int index)
        {
            for (int i = 0; i < parnetObj.childCount; i++)
            {
                parnetObj.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            parnetObj.GetChild(index).GetChild(0).gameObject.SetActive(true);
        }
        [ShowOnly]
        public int popularIndex = 1;
        [ShowOnly]
        public int AllIndex = 1;
        void OnAvatorSelectBtnClick()
        {
            int indexOfChild = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
            //   selectAvatar.sprite = AvatorSpritesSquare.Sprites[indexOfChild - 1];
            popularIndex = PopularGiftPanel.activeInHierarchy ? indexOfChild : popularIndex;
            Debug.LogError("check here brother......." + popularIndex);
            AllIndex = AllGiftPanel.activeInHierarchy ? indexOfChild : AllIndex;
            SetAllInUseLabelFalse(PopularGiftPanel.activeInHierarchy ? giftPopularBtnsParent : giftAllBtnsParent, indexOfChild);

            // ProfileIndex = indexOfChild - 1;

        }

        public void ActiveMyMode(string name)
        {
            PopularGiftPanel.SetActive(name.Equals(PopularGiftPanel.name));
            AllGiftPanel.SetActive(name.Equals(AllGiftPanel.name));
        }


        public void TranferGiftToPlayers()
        {
            

            int indexSprite = 0;
            if (PopularGiftPanel.activeInHierarchy)
            {
                indexSprite = popularIndex - 1;
                uIManager.GetMyPlayerInfo().playerGiftTransfer.CheckGiftQuality(true);
            }
            else
            {
                indexSprite = AllIndex - 1;
                uIManager.GetMyPlayerInfo().playerGiftTransfer.CheckGiftQuality(false);
            }
            if (uIManager.GetMyPlayerInfo().playerGiftTransfer.sendToAll)
                uIManager.GetMyPlayerInfo().playerGiftTransfer.forAllPlayerSendGift(indexSprite);
            else
                uIManager.GetMyPlayerInfo().playerGiftTransfer.TranferGiftToPlayers(indexSprite);

            uIManager.GiftTranferpanel.SetActive(false);
            // if(giftObject != null)
            //     Destroy(giftObject);
            //  giftObject = Instantiate(uIManager.GetMyPlayerInfo().giftGameObject.gameObject );
            // LocalSettings.SetPosAndRect(giftObject, uIManager.GetMyPlayerInfo().giftInsPos,uIManager.GetMyPlayerInfo().gameObject.transform);
            // giftObject.GetComponent<RectTransform>().localScale *= 100;
            // giftObject.transform.GetComponent<GiftTransferAnimScript>().targetPos = othePlayerGiftPos1st;
            // giftObject.transform.GetComponent<SpriteRenderer>().sprite = (PopularGiftPanel.activeInHierarchy ? giftPopularCollection.giftPopularArray[popularIndex -1].Sprites : giftAllCollection.giftPopularArray[AllIndex -1].Sprites);
            // Debug.LogError("check here brother......." + popularIndex);
            // uIManager.GiftTranferpanel.SetActive(false);
            //giftObject.gameObject.SetActive(true);

        }

        public void SendTOAllBtn()
        {
            uIManager.GetMyPlayerInfo().playerGiftTransfer.sendToAll = uIManager.GetMyPlayerInfo().playerGiftTransfer.sendToAll ? false : true;
            markSign.SetActive(uIManager.GetMyPlayerInfo().playerGiftTransfer.sendToAll);
        }

    }
}
