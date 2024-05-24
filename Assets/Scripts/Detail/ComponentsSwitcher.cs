using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class ComponentsSwitcher : MonoBehaviour
{
	[SerializeField]
	private Transform _detail;
	[SerializeField]
	private Toggle _componentSwitcherPrefab;
	[SerializeField]
	private ToggleGroup _componentsToggleGroup;
	[SerializeField]
	private CameraController _freeLookCamera;
	[SerializeField]
	private float _distanceBetweenComponents = 0.1f;
	[SerializeField]
	private float _animationDuration = 2;

	private Dictionary<Toggle, GameObject> _componentsList;
	private Toggle _stateSwitcher;

	private float _heightLayout;

	private void Start()
	{
		InitSwitcher();
	}

	private void OnValidate()
	{
		if(_stateSwitcher == null)
		{
			_stateSwitcher = this.GetComponent<Toggle>();
			if(_stateSwitcher == null)
			{
				Debug.LogError("ComponentsSwitcher Error: Component toggle not found");
			}
		}

		if(_detail != null)
		{
			TextMeshProUGUI stateSwitcherName = _stateSwitcher.GetComponentInChildren<TextMeshProUGUI>();
			stateSwitcherName.text = _detail.name;
		}
	}

	private void InitSwitcher()
	{
		if (_componentsList == null)
		{
			_componentsList = new Dictionary<Toggle, GameObject>();
		}

		for (int i = 0; i < _detail.childCount; ++i)
		{
			GameObject component = _detail.GetChild(i).gameObject;
			Toggle toggleComponent = Instantiate(_componentSwitcherPrefab, _componentsToggleGroup.transform).GetComponent<Toggle>();

			_componentsList.Add(toggleComponent, component);
			toggleComponent.GetComponentInChildren<TextMeshProUGUI>().text = component.name;
			toggleComponent.group = _componentsToggleGroup;
			toggleComponent.onValueChanged.AddListener(SelectComponent);
		}

		RectTransform rectTransform = _componentsToggleGroup.GetComponent<RectTransform>();
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		_heightLayout = rectTransform.rect.height;
		_componentsToggleGroup.transform.DOLocalMoveY(_heightLayout, 0);
	}

	public void ChangeState(bool newState)
	{
		float coefficient = 2f;
		if(newState)
		{
			float distance = _distanceBetweenComponents * _componentsList.Count;
			float step = distance / 2;

			UpdateCameraTarget(_detail.parent, _detail, coefficient);
			_componentsToggleGroup.transform.DOLocalMoveY(0, _animationDuration)
															.OnComplete(() => UpdateCameraTarget(_detail.parent, _detail));
			foreach (var component in _componentsList)
			{
				component.Value.transform.DOLocalMoveY(step, _animationDuration);
														
				step -= _distanceBetweenComponents;
			}
		}
		else
		{
			_componentsToggleGroup.transform.DOLocalMoveY(_heightLayout, _animationDuration);
			foreach (var component in _componentsList)
			{
				component.Value.transform.DOLocalMoveY(0, _animationDuration)
														.OnComplete(() => UpdateCameraTarget(_detail.parent, _detail));
				component.Key.isOn = false;
				UpdateCameraTarget(_detail.parent, _detail, 1 / coefficient);
			}
		}
	}

	private void SelectComponent(bool isOn)
	{
		if(!_componentsToggleGroup.AnyTogglesOn())
		{
			foreach(var component in _componentsList)
			{
				component.Value.SetActive(true);
			}
			UpdateCameraTarget(_detail.parent, _detail);
		}
		else
		{
			foreach (var component in _componentsList)
			{
				component.Value.SetActive(component.Key.isOn);
				if (component.Key.isOn)
				{
					UpdateCameraTarget(component.Value.transform.GetChild(0), component.Value.transform);
				}
			}
		}
	}

	private void UpdateCameraTarget(Transform targetFollow, Transform targetRadius, float coefficient = 1.5f)
	{
		_freeLookCamera.UpdateRadius(targetRadius, coefficient);
		_freeLookCamera.SetTarget(targetFollow);
	}
}