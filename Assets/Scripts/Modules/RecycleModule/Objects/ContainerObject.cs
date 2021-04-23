using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Recycle object used for AR visualization, contains 3D bin object and floating pie chart
public class ContainerObject : HideableObject {
    [SerializeField] PieChart _pieChart = null;

    public void Init(Container container) {
        Color accessColor = Container.GetColorFromAccessibility(container.accessibility);
        renderer.material.color = accessColor;        
        _pieChart.Init(
            //there may be multiple containers with same trash type, so we need to distinct them as there is no need for duplicates
            container.trashTypes.Select(type => {
                return Container.GetColorFromTrashType(type);
            }).Distinct().ToList(),
            accessColor
        );
    }

}
