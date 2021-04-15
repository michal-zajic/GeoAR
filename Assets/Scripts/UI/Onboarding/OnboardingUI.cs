using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingUI : MonoBehaviour
{
    [SerializeField] List<GameObject> pages = null;

    int _currentPage = 0;

    public void Show() {
        _currentPage = 0;
        if(pages != null && pages.Count > 0) {
            ShowCurrentPage();
        }
    }

    void ShowCurrentPage() {
        if (_currentPage >= pages.Count)
            return;

        GameObject obj = Instantiate(pages[_currentPage], transform);
        OnboardingPage page = obj.GetComponent<OnboardingPage>();
        page.onNext += () => {
            Destroy(obj);
            _currentPage += 1;
            ShowCurrentPage();
        };
        page.onFinish += () => {
            Destroy(obj);
        };
        page.onNotAgain += () => {
            Settings.instance.Set(Settings.Setting.showOnboarding, false);
            Destroy(obj);
        };

    }
}
