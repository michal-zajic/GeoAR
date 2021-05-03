////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: TabBar.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//main class controlling app tab bar
public class TabBar : MonoBehaviour
{
    [SerializeField] List<TabBarButton> _tabBarButtons = null;
    [SerializeField] int _initialTab = 1;

    // Start is called before the first frame update
    void Start()
    {
        _tabBarButtons.ForEach((TabBarButton button) => {
            button.button.onClick.AddListener(() => {
                ClickedTab(_tabBarButtons.IndexOf(button));
            });
        });

        ClickedTab(_initialTab);
    }

    //when clicked, activate clicked tab and deactivate others
    private void ClickedTab(int idx) {
        for (int i = 0; i < _tabBarButtons.Count; i++) {
            _tabBarButtons[i].SetActive(i == idx);
        }
    }
}
