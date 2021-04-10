using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContainerObject : HideableObject {
    [SerializeField] PieChart _pieChart = null;

    public void Init(Container container) {
        Color accessColor = Container.GetColorFromAccessibility(container.accessibility);
        renderer.material.color = accessColor;        
        _pieChart.Init(
            container.trashTypes.Select(type => {
                return Container.GetColorFromTrashType(type);
            }).Distinct().ToList(),
            accessColor
        );
    }

}
