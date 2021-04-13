using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] Button _okButton = null;

    public void Init(Action okAction = null) {
        //StartCoroutine(FlashButton());        
        _okButton.onClick.AddListener(() => {
            if (okAction != null) {
                okAction();
            }
            StopAllCoroutines();
            Destroy(gameObject);
        });
    }

    IEnumerator FlashButton() {
        Image image = _okButton.image;
        Color currentColor = image.color;
        Color targetColor = new Color(0.15f, 1, 0.15f);
        float speed = 0.06f;
        while (true) {
            while (Mathf.Abs(image.color.r - targetColor.r) > speed + 0.01f) {
                image.color = Color.Lerp(image.color, targetColor, speed);                    
                yield return null;
            }
            var tmp = currentColor;
            currentColor = targetColor;
            targetColor = tmp;
            yield return null;
        }
    }
}
