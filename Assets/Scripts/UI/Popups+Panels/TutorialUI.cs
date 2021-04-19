using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] Button _closeBtn = null;

    void Start()
    {
        _closeBtn.onClick.AddListener(() => {
            Destroy(gameObject);
        });
    }
}
