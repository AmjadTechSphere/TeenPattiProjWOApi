using DG.Tweening;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

namespace com.mani.muzamil.amjad
{
    public class BetAmountToTargetAnim : MonoBehaviour
    {
        [HideInInspector]
        public GameObject targetPos;
        public Ease easeType;
        public Vector3 TargetScale = Vector3.one;
        public float TimeToCompleteAnim = 1f;

        void PlayAnimation(Vector3 targetPosition)
        {
            if (targetPosition != null)
                transform.DOMove(targetPosition, TimeToCompleteAnim, false).OnComplete(() => checkAnim()).SetEase(easeType);
            transform.DOScale(Vector3.one, TimeToCompleteAnim);
        }

        private void OnEnable()
        {
            //orgPos = transform.position;
            transform.position = transform.parent.position;
            PlayAnimation(targetPos.transform.position);
        }
        
        private void OnDisable()
        {
            transform.localScale = Vector3.one;
        }

        void checkAnim()
        {
            gameObject.SetActive(false);
            
        }



    }
}
