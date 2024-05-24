using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetailRotate : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[SerializeField]
	private float _rotationSpeed = 2;
	private bool _isPresset;
	private Camera _mainCamera;

	void Start()
	{
		_mainCamera = Camera.main;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (_isPresset && eventData.button == PointerEventData.InputButton.Left)
		{
			float horizontalInput = eventData.delta.x;
			float verticalInput = eventData.delta.y;

			Vector3 rotation = new Vector3(verticalInput, -horizontalInput, 0);
			Rotate(rotation);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			_isPresset = true;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			_isPresset = false;
		}
	}

	private void Rotate(Vector3 rotation)
	{
		Vector3 relativeRight = _mainCamera.transform.TransformDirection(Vector3.right);
		Vector3 relativeUp = _mainCamera.transform.TransformDirection(Vector3.up);

		transform.Rotate(relativeUp, rotation.y * _rotationSpeed * Time.deltaTime, Space.World);
		transform.Rotate(relativeRight, -rotation.x * _rotationSpeed * Time.deltaTime, Space.World);
	}
}