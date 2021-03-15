using UnityEngine;
using UnityARInterface;

public class PlaceMapOnARPlane : MonoBehaviour
{

	[SerializeField]
	private Transform _mapTransform;

	[SerializeField] ARFocusSquare _focusSquare = null;

	LockMode _mode = LockMode.unlocked;


	void Start()
	{
		//ARPlaneHandler.returnARPlane += PlaceMap;
		//ARPlaneHandler.resetARPlane += ResetPlane;
	}

	public void LockStateChangeTo(LockMode mode) {
		_mode = mode; 
    }

    private void Update() {
        if(_mode == LockMode.unlocked) {
			//_mapTransform.position = _focusSquare.currentTransform.position;
			_mapTransform.position = _focusSquare.lastPosition;
		}
    }
}
