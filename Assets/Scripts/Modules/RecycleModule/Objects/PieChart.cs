using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    [SerializeField] Sprite circle = null;

    public void Init(List<Color> colors, Color background, bool drawBackground = false) {
        if (drawBackground) {
            GameObject backGroundObj = new GameObject("background", typeof(RectTransform), typeof(Image));
            ResetTransform(backGroundObj);
            SetRectTransform(backGroundObj, 0);
            Image im = backGroundObj.GetComponent<Image>();
            im.color = background;
            im.sprite = circle;
        }

        for (int i = colors.Count - 1; i >= 0; i--) {
            GameObject pieceObj = new GameObject(colors[i].ToString(), typeof(RectTransform), typeof(Image));
            ResetTransform(pieceObj);
            SetRectTransform(pieceObj, drawBackground ? 0.06f : 0);
            Image im = pieceObj.GetComponent<Image>();
            im.sprite = circle;
            im.color = colors[i];
            im.type = Image.Type.Filled;
            im.fillOrigin = (int)Image.OriginVertical.Top;
            im.fillAmount = (i + 1) * (1.0f / colors.Count);
        }
    }

    void SetRectTransform(GameObject obj, float offset) {
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.offsetMin = new Vector2(offset, offset);
        rt.offsetMax = new Vector2(-offset, -offset);
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
    }

    void ResetTransform(GameObject obj) {
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
    }
}
