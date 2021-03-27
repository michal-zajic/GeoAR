using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

public class Container {
    public enum TrashType {
        tintedGlass = 1,
        electric,
        metals,
        cartons,
        paper,
        plastics,
        clearGlass,
        textiles
    }
    public enum Accessibility {
        everyone = 1, owners = 2, unknown = 3
    }

    public Vector2d coordinates;
    public List<TrashType> trashTypes;
    public Accessibility accessibility;

}
