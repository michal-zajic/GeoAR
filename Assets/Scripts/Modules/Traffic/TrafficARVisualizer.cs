using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public class TrafficARVisualizer : ModuleARVisualizer {
    [SerializeField] GameObject _segmentObject = null;
    [SerializeField] [Range(50, 300)] int _carLimit = 100;

    public override void Disable() {
        objects.ForEach(obj => {
            obj.GetComponent<SegmentObject>().SetActive(false);
        });
    }

    public override void Enable() {
        objects.ForEach(obj => {
            obj.GetComponent<SegmentObject>().SetActive(true);
        });
    }

    public override void Draw(ModuleDataLoader data, AbstractMap map) {
        DestroyObjects();
        TrafficDataLoader loader = data as TrafficDataLoader;
        loader.arSegments.ForEach(segment => {
            GameObject segmentGameObj = Instantiate(_segmentObject, transform);            
            SegmentObject segmentObject = segmentGameObj.GetComponent<SegmentObject>();
            segmentObject.Init(segment.coordinateList, map, segment.jamFactor, segment.speed, _carLimit);
            objects.Add(segmentGameObj);
        });
    }
}
