using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModulePanel : MonoBehaviour
{
    [SerializeField] GameObject _moduleEntryPrefab = null;
    [SerializeField] Transform _contentRect = null;
    [SerializeField] Button _backButton = null;
    [SerializeField] Button _miniPanelButton = null;
    [SerializeField] Image _miniPanelModuleImage = null;
    [SerializeField] Sprite _cancelIcon = null;

    List<ModulePanelEntry> _entries = new List<ModulePanelEntry>();

    RectTransform _rt;
    float speed = 100;
    // Start is called before the first frame update
    void Start()
    {
        _rt = GetComponent<RectTransform>();
        Finder.moduleMgr.modules.ForEach((module) => {
            GameObject obj = Instantiate(_moduleEntryPrefab, _contentRect);
            ModulePanelEntry entry = obj.GetComponent<ModulePanelEntry>();
            entry.Init(module.GetName(), module.GetDescription(), module.GetIcon(), ModuleClicked);            
            _entries.Add(entry);
        });
        _backButton.onClick.AddListener(() => {
            Dismiss();
        });
        _miniPanelButton.onClick.AddListener(() => {
            Summon();
        });
        if (Finder.moduleMgr.activeModule == null) {
            _miniPanelModuleImage.sprite = _cancelIcon;
        } else {
            ModuleClicked(Finder.moduleMgr.activeModule.name);
        }
    }

    void ModuleClicked(string name) {
        _entries.ForEach(entry => {
            if (entry.name == name) {
                if (entry.active) {
                    entry.SetActive(false);
                    Finder.moduleMgr.SetActiveModule(null);
                    _miniPanelModuleImage.sprite = _cancelIcon;
                } else {
                    entry.SetActive(true);
                    Finder.moduleMgr.SetActiveModule(entry.name);
                    _miniPanelModuleImage.sprite = Finder.moduleMgr.activeModule.GetIcon();
                }
            } else {
                entry.SetActive(false);                
            }
        });
    }

    IEnumerator MovePanel(bool show) {
        float destX = show ? 0 : _rt.rect.width;

        while((show && _rt.anchoredPosition.x >= destX + speed) || (!show && _rt.anchoredPosition.x <= destX - speed)) {
            Vector2 pos = _rt.anchoredPosition;
            _rt.anchoredPosition = new Vector2(pos.x + Mathf.Sign((destX - pos.x)) * speed, pos.y);
            yield return null;
        }
        _rt.anchoredPosition = new Vector2(destX, _rt.anchoredPosition.y);
    }

    public void Summon() {
        StopAllCoroutines();
        StartCoroutine(MovePanel(true));
    }

    public void Dismiss() {
        StartCoroutine(MovePanel(false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
