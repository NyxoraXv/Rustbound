using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Loading : MonoBehaviour
{
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.GetComponent<CanvasGroup>().DOFade(1f, 1f).From(0f);
    }

}
