using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public class TrafficARVisualizer : ModuleARVisualizer {
    [SerializeField] GameObject _segmentObject = null;
    [SerializeField] [Range(50, 300)] int _carLimit = 120;

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
            int carLimit = Math.Max(2, _carLimit / loader.arSegments.Count);
            segmentObject.Init(segment.coordinateList, map, segment.jamFactor, segment.speed, carLimit);
            objects.Add(segmentGameObj);
        });
    }
}
