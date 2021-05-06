////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: LegendUI.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ui for module legend
public class LegendUI : MonoBehaviour
{
    [SerializeField] Button _closeBtn = null;

    void Start()
    {
        _closeBtn.onClick.AddListener(() => {
            Destroy(gameObject);
        });
    }
}
