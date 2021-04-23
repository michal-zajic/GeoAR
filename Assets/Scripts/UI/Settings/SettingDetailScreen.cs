using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//screen containing individual setting picker options
public class SettingDetailScreen : MonoBehaviour
{
    [SerializeField] Text _headlineText = null;
    [SerializeField] Text _descriptionText = null;
    [SerializeField] Transform _itemContainer = null;
    [SerializeField] Button _navigationBackButton = null;

    [SerializeField] GameObject _detailItemPrefab = null;

    string _currentItem = "";
    List<SettingDetailItem> details;

    public void Initialize(string title, string description, List<string> items, string currentItem, UnityAction<string> onClick) {
        _headlineText.text = title;
        _currentItem = currentItem;
        _descriptionText.text = description;
        _navigationBackButton.onClick.AddListener(() => {
            Dismiss();
        });

        details = new List<SettingDetailItem>();
        items.ForEach((string item) => {
            GameObject itemDetail = Instantiate(_detailItemPrefab, _itemContainer);
            SettingDetailItem detail = itemDetail.GetComponent<SettingDetailItem>();
            details.Add(detail);
            detail.Initialize(item, item == _currentItem, () => {
                currentItem = item;
                details.ForEach((SettingDetailItem obj) => {
                    obj.UpdateChanged(false);
                });
                detail.UpdateChanged(true);
                onClick(currentItem);
            });
        });

        Summon();
    }

    IEnumerator AnimateIn(bool movingIn) {
        RectTransform rt = GetComponent<RectTransform>();
        float destX = movingIn ? 0 : GameObject.Find("UI").GetComponent<RectTransform>().rect.width;
        while (rt.anchoredPosition.x != destX) {
            Vector2 pos = rt.anchoredPosition;
            rt.anchoredPosition = new Vector2(pos.x + Mathf.Sign(destX - pos.x) * 100, pos.y);

            if(rt.anchoredPosition.x < destX && movingIn) {
                rt.anchoredPosition = new Vector2(destX, pos.y);
            } else if (rt.anchoredPosition.x > destX && !movingIn) {
                rt.anchoredPosition = new Vector2(destX, pos.y);
            }
            yield return null;
        }

        if(!movingIn) {
            Destroy(gameObject);
        }
    }

    void Summon() {
        StopAllCoroutines();
        StartCoroutine(AnimateIn(true));
    }

    void Dismiss() {
        StopAllCoroutines();
        StartCoroutine(AnimateIn(false));
    }
}
