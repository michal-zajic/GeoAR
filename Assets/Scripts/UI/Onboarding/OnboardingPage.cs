using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//single page in onboarding tutorial
//contains events for specific buttons
public class OnboardingPage : MonoBehaviour
{
    [SerializeField] Button _nextButton = null;
    [SerializeField] Button _notAgainButton = null;
    [SerializeField] Button _okButton = null;

    public delegate void OnNext();
    public event OnNext onNext;

    public delegate void OnFinish();
    public event OnFinish onFinish;

    public delegate void OnNotAgain();
    public event OnNotAgain onNotAgain;

    private void Start() {
        if(_nextButton != null)
        _nextButton.onClick.AddListener(() => { onNext(); });
        if(_notAgainButton != null)
        _notAgainButton.onClick.AddListener(() => { onNotAgain(); });
        if(_okButton != null)
        _okButton.onClick.AddListener(() => {
            onFinish();
        });
    }
}
