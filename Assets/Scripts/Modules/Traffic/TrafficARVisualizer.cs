using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public class TrafficARVisualizer : ModuleARVisualizer {
    [SerializeField] GameObject _carObject = null;

    public override void Disable() {
        objects.ForEach(obj => {
            obj.GetComponent<ContainerObject>().SetActive(false);
        });
    }

    public override void Enable() {
        objects.ForEach(obj => {
            obj.GetComponent<ContainerObject>().SetActive(true);
        });
    }

    public override void Draw(ModuleDataLoader data, AbstractMap map) {
        TrafficDataLoader loader = data as TrafficDataLoader;
        //vlastni car objekt
        //da se mu trasa
        //posunuje se counter
        // jede k bodu kde je counter, pokud dost blizko, updatne se counter, pokud posledni, despawn
    }
}
