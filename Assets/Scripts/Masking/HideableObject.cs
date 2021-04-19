using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Objects with this class are hidden if they collide with mask planes
public class HideableObject : MonoBehaviour
{
    [SerializeField] GameObject _objectToHide = null;
    [SerializeField] MeshRenderer _renderer = null;

    protected new MeshRenderer renderer => _renderer;

    public bool disabled { get; private set; }
    public bool outOfBounds { get; private set; }

    //external source such as ModuleVisualizer may disable these objects even if they are visible
    public void SetActive(bool active) {
        disabled = !active;
        UpdateActive();
    }

    //determines the visibility of object based on disabled and outOfBounds properties
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

    //if this object collides with mask plane, mark it as out of bounds
    public void OnTriggerEnter(Collider other) {
        if (other.tag != "MaskPlane") {
            return;
        }
        outOfBounds = true;
        UpdateActive();
    }
    //when it leaves, unmark it
    public void OnTriggerExit(Collider other) {
        if (other.tag != "MaskPlane") {
            return;
        }
        outOfBounds = false;
        UpdateActive();
    }
}
