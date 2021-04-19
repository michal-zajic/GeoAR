using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandler : MonoBehaviour
{
	//checks whether the user is touching user interface - used during raycasting, so for example the map wont move when user pans over some UI
    public static bool IsTouchingUI() {
		if (EventSystem.current.IsPointerOverGameObject())
			return true;
		foreach (var touch in Input.touches) {
			if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
				return true;
			}
		}
		return false;
	}
}
