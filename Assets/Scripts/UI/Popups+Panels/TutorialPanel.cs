using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] Button _notAgainButton = null;
    [SerializeField] Button _okButton = null;

    public void Init(Action notAgainAction = null, Action okAction = null) {
        _notAgainButton.onClick.AddListener(() => {
            if (notAgainAction != null) {
                notAgainAction();
            }
            Destroy(gameObject);
        });
        _okButton.onClick.AddListener(() => {
            if (okAction != null) {
                okAction();
            }
            Destroy(gameObject);
        });
    }
}
