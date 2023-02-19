using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleAnimation : MonoBehaviour
{

    private RectTransform TitleText;
    // Start is called before the first frame update
    void Start()
    {
        TitleText = GetComponent<RectTransform>();
        TitleText.DOAnchorPos(Vector2.zero, 1.2f, false).SetEase(Ease.InOutBounce);
    }

}
