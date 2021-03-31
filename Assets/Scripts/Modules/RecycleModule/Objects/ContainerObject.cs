using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContainerObject : MonoBehaviour
{
    [SerializeField] GameObject _objectToHide = null;
    [SerializeField] MeshRenderer _renderer = null;
    [SerializeField] PieChart _pieChart = null;

    public void SetActive(bool active) {
        _renderer.enabled = active;
        _objectToHide.SetActive(active);
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
}
