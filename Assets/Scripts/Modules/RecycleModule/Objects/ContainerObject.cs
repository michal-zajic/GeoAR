using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContainerObject : MonoBehaviour {
    [SerializeField] GameObject _objectToHide = null;
    [SerializeField] MeshRenderer _renderer = null;
    [SerializeField] PieChart _pieChart = null;

    public bool disabled { get; private set; }
    public bool outOfBounds { get; private set;}

    public void SetActive(bool active) {        
        disabled = !active;
        UpdateActive();
    }

    public void Init(Container container) {
        Color accessColor = Container.GetColorFromAccessibility(container.accessibility);
        _renderer.material.color = accessColor;        
        _pieChart.Init(
            container.trashTypes.Select(type => {
                return Container.GetColorFromTrashType(type);
            }).Distinct().ToList(),
            accessColor
        );
    }

    private void UpdateActive() {
        if (disabled) {
            _renderer.enabled = false;
            _objectToHide.SetActive(false);
        } else {
            _renderer.enabled = !outOfBounds;
            _objectToHide.SetActive(!outOfBounds);
        }
    }
        
    public void OnTriggerEnter(Collider other) {
        if(other.tag != "MaskPlane") {
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
