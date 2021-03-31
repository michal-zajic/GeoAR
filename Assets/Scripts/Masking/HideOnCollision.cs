using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnCollision : MonoBehaviour
{
    List<GameObject> hiddenObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider collision) {
        if(collision.tag == "MaskPlane" || collision.gameObject.layer == LayerMask.NameToLayer("ARGameObject")) {
            return;
        } else if (collision.tag != "ModuleObject")
            collision.gameObject.SetActive(false);
    }
    private void OnTriggerExit(Collider collision) {
        if (collision.tag == "MaskPlane") {
            return;
        } else if (collision.tag != "ModuleObject")
            collision.gameObject.SetActive(true);
    }
}
