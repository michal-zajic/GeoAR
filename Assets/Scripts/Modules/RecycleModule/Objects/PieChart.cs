using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    [SerializeField] Sprite circle = null;
    [SerializeField] Image _background = null;

    public void Init(List<Color> colors, Color background, bool drawBackground = false) {
        if (drawBackground) {        
            _background.color = background;
            _background.color -= new Color32(0, 0, 0, 50);
        }

        for (int i = colors.Count - 1; i >= 0; i--) {
            GameObject pieceObj = new GameObject(colors[i].ToString(), typeof(RectTransform), typeof(Image));
            ResetTransform(pieceObj);
            SetRectTransform(pieceObj, drawBackground ? 0.03f : 0);
            Image im = pieceObj.GetComponent<Image>();
            im.sprite = circle;
            im.color = colors[i];
            im.type = Image.Type.Filled;
            im.fillOrigin = (int)Image.OriginVertical.Top;
            im.fillAmount = 1.0f / colors.Count;
            im.color -= new Color32(0,0,0, 50);

            float rotation = (i + 1) * (360.0f / colors.Count);
            pieceObj.transform.Rotate(Vector3.forward, rotation);
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
