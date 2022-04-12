using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingClyinder : MonoBehaviour
{
    private bool _filled;
    private float _filledValue;
 
    public void IncementClyiderVolume(float value) 
    {
        _filledValue += value;
        if (_filledValue > 1)
        {
            float leftValue = _filledValue - 1;
            int clyinderCount = PlayerController.current.clyinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (clyinderCount - 1) - 0.25f, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f, transform.localScale.y, transform.localScale.z);
            PlayerController.current.CreateClyinder(leftValue);
            //Silindirin boyutunu tam olarak 1 yap
            //1den fazla olarak kalan deðeri de yeni silindir oluþtur.
        }else if (_filledValue <0)
        {
            PlayerController.current.DestroyClyinder(this); 
            //Karakterimize silindiri yok etmesini söyleyeceðiz.
        }else
        {
            int clyinderCount = PlayerController.current.clyinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x ,-0.5f*(clyinderCount-1)- 0.25f* _filledValue , transform.localPosition.z);
            transform.localScale = new Vector3(0.5f * _filledValue, transform.localScale.y, 0.5f * _filledValue);
            //Silindirin boyutunu güncelleyeceðiz.
        }
    }
}
