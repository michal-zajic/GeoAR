using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertPanel : MonoBehaviour
{
    [SerializeField] Text _descriptionCenterText = null;
    [SerializeField] Text _descriptionSatelliteText = null;
    [SerializeField] Button _okButton = null;
    [SerializeField] Button _notAgainButton = null;

    public void Init(bool satellite, Action okAction = null, Action notAgainAction = null) {
        _descriptionCenterText.gameObject.SetActive(!satellite);
        _descriptionSatelliteText.gameObject.SetActive(satellite);
        _okButton.onClick.AddListener(() => {
            if (okAction != null)
                okAction();
            Destroy(gameObject);
        });
        _notAgainButton.onClick.AddListener(() => {
            if(notAgainAction != null)
                notAgainAction();
            Destroy(gameObject);
        });
    }
}
