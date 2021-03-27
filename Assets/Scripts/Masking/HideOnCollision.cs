using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider collision) {
        if(collision.tag == "MaskPlane" || collision.gameObject.layer == LayerMask.NameToLayer("ARGameObject")) {
            return;
        } else if (collision.tag == "ModuleObject") {
            collision.GetComponent<MeshRenderer>().enabled = false;
        } else
            collision.gameObject.SetActive(false);
    }
    private void OnTriggerExit(Collider collision) {
        if (collision.tag == "MaskPlane") {
            return;
        }
        else if(collision.tag == "ModuleObject") {
            collision.GetComponent<MeshRenderer>().enabled = true;
        } else
            collision.gameObject.SetActive(true);
        //collision.gameObject.GetComponent<Renderer>().enabled = true;
    }
}
