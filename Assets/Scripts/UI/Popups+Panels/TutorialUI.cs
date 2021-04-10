using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] Button _closeBtn = null;
    // Start is called before the first frame update
    void Start()
    {
        _closeBtn.onClick.AddListener(() => {
            Destroy(gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
