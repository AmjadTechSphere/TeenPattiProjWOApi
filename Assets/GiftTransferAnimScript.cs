using com.mani.muzamil.amjad;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftTransferAnimScript : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public GameObject targetPos;
    public Ease easeType;
    public Vector3 TargetScale = Vector3.one;
    public float TimeToCompleteAnim = 1f;
    bool Animstop = false;
    GameManager gameManager;
    [ShowOnly] public int ReceiverID;
    [ShowOnly] public int SenderID;
    [ShowOnly] public AudioClip giftSound;
    public DOTweenAnimation tweenAnimation;
    void PlayAnimation(Vector3 targetPosition)
    {

        if (targetPosition != null)
            transform.DOMove(targetPosition, TimeToCompleteAnim, false).OnComplete(() => checkAnim()).SetEase(easeType);
        //  transform.DOScale(Vector3.one , TimeToCompleteAnim);
    }

    private void OnEnable()
    {
        //orgPos = transform.position;
        Animstop = false;
        gameManager = GameManager.Instance;
        tweenAnimation = GetComponent<DOTweenAnimation>();
        //transform.position = UIManager.Instance.GetMyPlayerInfo().playerGiftTransfer.giftInsPos.position;
        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (gameManager.playersList[i].photonView.ViewID == SenderID)
            {
                transform.position = gameManager.playersList[i].playerGiftTransfer.giftInsPos.position;
                // transform.localScale = Vector3.one * 0.3f;
            }
        }



        PlayAnimation(targetPos.transform.position);
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.one * 0.3f;
    }

    void checkAnim()
    {
        if (!Animstop)
        {
            SoundManager.Instance.PlayAudioClip(giftSound, false);
            tweenAnimation.DOPlay();
            StartCoroutine(waitForNextTarget());
            Animstop = true;
        }
        else
        {
            Animstop = false;
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                if (gameManager.playersList[i].photonView.ViewID == ReceiverID)
                {
                    LocalSettings.SetPosAndRect(gameManager.playersList[i].playerGiftTransfer.giftObject, gameManager.playersList[i].playerGiftTransfer.gift2ndPosOfPlayer.GetComponent<RectTransform>(), gameManager.playersList[i].transform);
                    // transform.localScale = Vector3.one * 0.3f;
                }
            }
            //LocalSettings.SetPosAndRect(GiftTranferManager.Instance.gift, GiftTranferManager.Instance.othePlayerGiftPos2nd.GetComponent<RectTransform>(), GiftTranferManager.Instance.othePlayerGiftPos1st.transform);
        }

    }

    IEnumerator waitForNextTarget()
    {
        float delay = 2f;
        if(giftSound)
        {
            delay = giftSound.length;
            if (delay < 2f)
                delay = 2f;
        }

        yield return new WaitForSeconds(delay);
        tweenAnimation.DORewind();
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (gameManager.playersList[i].photonView.ViewID == ReceiverID)
                targetPos = gameManager.playersList[i].playerGiftTransfer.gift2ndPosOfPlayer;
        }
        PlayAnimation(targetPos.transform.position);
    }

}
