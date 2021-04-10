using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    [SerializeField] GameObject _objectToHide = null;
    [SerializeField] MeshRenderer _renderer = null;

    protected new MeshRenderer renderer => _renderer;

    public bool disabled { get; private set; }
    public bool outOfBounds { get; private set; }

    public void SetActive(bool active) {
        disabled = !active;
        UpdateActive();
    }

    private void UpdateActive() {
        if (disabled) {
            _renderer.enabled = false;
            if(_objectToHide != null)
                _objectToHide.SetActive(false);
        } else {
            _renderer.enabled = !outOfBounds;
            if(_objectToHide != null)
                _objectToHide.SetActive(!outOfBounds);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag != "MaskPlane") {
            return;
        }
        outOfBounds = true;
        UpdateActive();
    }
    public void OnTriggerExit(Collider other) {
        if (other.tag != "MaskPlane") {
            return;
        }
        outOfBounds = false;
        UpdateActive();
    }
}
