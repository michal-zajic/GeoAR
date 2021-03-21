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
        }
        //collision.gameObject.GetComponent<Renderer>().enabled = false;
        collision.gameObject.SetActive(false);
    }
    private void OnTriggerExit(Collider collision) {
        if (collision.tag == "MaskPlane") {
            return;
        }
        collision.gameObject.SetActive(true);
        //collision.gameObject.GetComponent<Renderer>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
