using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

//Data structure for recycle containers
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

    public static Color GetColorFromTrashType(TrashType type) {
        switch (type) {
            case TrashType.paper:
                return new Color32(0, 35, 245, 255);
            case TrashType.plastics:
                return new Color32(254, 255, 84, 255);
            case TrashType.tintedGlass:
                return new Color32(66, 149, 41, 255);
            case TrashType.metals:
                return new Color32(86, 123, 134, 255);
            case TrashType.clearGlass:
                return new Color32(227, 254, 223, 255);
            case TrashType.cartons:
                return new Color32(225, 127, 47, 255);
            case TrashType.electric:
                return new Color32(138, 26, 16, 255);
            case TrashType.textiles:
                return new Color32(232, 99, 93, 255);
            default:
                return new Color32(0, 0, 0, 1);
        }
    }

    public static Color GetColorFromAccessibility(Accessibility accessibility) {
        return accessibility == Accessibility.everyone ? Color.green : accessibility == Accessibility.owners ? Color.red : Color.gray;
    }

}
