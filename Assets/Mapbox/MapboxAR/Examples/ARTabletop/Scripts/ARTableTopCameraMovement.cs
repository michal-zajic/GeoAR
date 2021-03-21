namespace Mapbox.Examples
{
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Utilities;
	using Mapbox.Utils;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using System;

	public class ARTableTopCameraMovement : MonoBehaviour
	{
		[SerializeField] Camera _camera = null;
		private Vector3 _origin;
		private Vector3 _mousePosition;
		private Vector3 _mousePositionPrevious;
		private bool _shouldDrag;

		private bool clickedOk = false;

		private void LateUpdate()
		{
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
				foreach (var touch in Input.touches) {
					if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
						return;
					}
				}
				clickedOk = true;
            }
            if (Input.GetMouseButtonUp(0) && Input.touchCount < 2) {
				clickedOk = false;
            }
			if (Input.touchSupported && Input.touchCount > 0)
			{
				HandleTouch();
			}
			else
			{
				HandleMouseAndKeyBoard();
			}
		}

		void HandleMouseAndKeyBoard()
		{
			// zoom
			float scrollDelta = 0.0f;
			scrollDelta = Input.GetAxis("Mouse ScrollWheel");
			ScaleMapUsingTouchOrMouse(scrollDelta);

			//pan mouse
			RotateMapUsingTouchOrMouse();
		}

		void HandleTouch()
		{
			float zoomFactor = 0.0f;
			//pinch to zoom.
			switch (Input.touchCount)
			{
				case 1:
					{
						RotateMapUsingTouchOrMouse();
					}
					break;
				case 2:
					{
						// Store both touches.
						Touch touchZero = Input.GetTouch(0);
						Touch touchOne = Input.GetTouch(1);

						// Find the position in the previous frame of each touch.
						Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
						Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

						// Find the magnitude of the vector (the distance) between the touches in each frame.
						float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
						float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

						// Find the difference in the distances between each frame.
						zoomFactor = 0.004f * (touchDeltaMag - prevTouchDeltaMag);
					}
					ScaleMapUsingTouchOrMouse(zoomFactor);
					break;
				default:
					break;
			}
		}

		void ScaleMapUsingTouchOrMouse(float zoomFactor)
		{
			gameObject.transform.localScale += new Vector3(zoomFactor, zoomFactor, zoomFactor);			
		}


		void RotateMapUsingTouchOrMouse()
		{
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && clickedOk)
			{				
				var mapViewportPos = _camera.WorldToViewportPoint(transform.position);
				if(!(mapViewportPos.x > 0 && mapViewportPos.x < 1 && mapViewportPos.y > 0 && mapViewportPos.y < 1)) {
					return;
                }
								
				var mousePosScreen = Input.mousePosition;
				_mousePosition = mousePosScreen;

				if (_shouldDrag == false)
				{
					_shouldDrag = true;
					_origin = mousePosScreen;
				}
			}
			else
			{
				_shouldDrag = false;
			}

			if (_shouldDrag == true)
			{				
				var mapScreenPos = _camera.WorldToScreenPoint(transform.position);
				var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
				if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f)
				{
					_mousePositionPrevious = _mousePosition;

					var offsetDelta = _origin - _mousePosition;
					var offset = offsetDelta.x;

					if (Mathf.Abs(offset) > 0.0f)
					{
						if(mapScreenPos.y < _origin.y) {
							offset *= -1;
                        } 
						transform.Rotate(Vector3.up, offset * 0.3f);
					}
					_origin = _mousePosition;
				}
				else
				{
					if (EventSystem.current.IsPointerOverGameObject())
					{
						return;
					}
					_mousePositionPrevious = _mousePosition;
					_origin = _mousePosition;
				}
			}
		}
	}
}
