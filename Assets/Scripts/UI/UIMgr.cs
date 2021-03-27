using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    [SerializeField] GameObject _modulePanel = null;

    public void SetModulePanel(bool active) {
        _modulePanel.SetActive(active);
    }
}
