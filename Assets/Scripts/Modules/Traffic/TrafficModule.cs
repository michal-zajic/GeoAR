using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficModule : Module {
    [SerializeField] string moduleName = null;
    [TextArea(3, 10)] [SerializeField] string description = null;
    [SerializeField] Sprite icon = null;
    [SerializeField] GameObject tutorialObject = null;

    public override string GetDescription() {
        return description;
    }

    public override Sprite GetIcon() {
        return icon;
    }

    public override string GetName() {
        return moduleName;
    }

    public override GameObject GetTutorialObject() {
        return tutorialObject;
    }

    public override float GetMinZoom() {
        return 0;
    }
}
