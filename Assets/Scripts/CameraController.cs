using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private CinemachineFreeLook _freeLookCamera;
	[SerializeField]
	private float _animationDuration = 2;

	public CinemachineFreeLook FreeLookCamera { get => _freeLookCamera; set => _freeLookCamera = value; }

	void Update()
	{
		if (Input.GetMouseButton(1))
		{
			FreeLookCamera.m_XAxis.m_InputAxisName = "Mouse X";
			FreeLookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
		}
		else
		{
			FreeLookCamera.m_XAxis.m_InputAxisName = "";
			FreeLookCamera.m_YAxis.m_InputAxisName = "";
			FreeLookCamera.m_XAxis.m_InputAxisValue = 0;
			FreeLookCamera.m_YAxis.m_InputAxisValue = 0;
		}
	}

	public void SetTarget(Transform target)
	{
		FreeLookCamera.Follow = target;
		FreeLookCamera.LookAt = target;
	}

	public void UpdateRadius(Transform target, float coefficient = 1)
	{
		float radius = CalculateRadius(target);
		AnimateOrbits(radius, _animationDuration, coefficient);
	}

	public void AnimateOrbits(float radius, float duration, float coefficient = 1)
	{
		if (FreeLookCamera != null)
		{
			radius *= coefficient;
			DOTween.To(() => FreeLookCamera.m_Orbits[0].m_Height, x => FreeLookCamera.m_Orbits[0].m_Height = x, radius, duration).SetEase(Ease.InOutSine);
			DOTween.To(() => FreeLookCamera.m_Orbits[1].m_Radius, x => FreeLookCamera.m_Orbits[1].m_Radius = x, radius, duration).SetEase(Ease.InOutSine);
			DOTween.To(() => FreeLookCamera.m_Orbits[2].m_Height, x => FreeLookCamera.m_Orbits[2].m_Height = x, -radius, duration).SetEase(Ease.InOutSine);
		}
	}

	private float CalculateRadius(Transform target)
	{
		Bounds bounds = CalculateBounds(target);
		return bounds.size.magnitude * 1.5f;
	}

	Bounds CalculateBounds(Transform target)
	{
		Renderer targetRenderer = target.GetComponent<Renderer>();
		Bounds bounds = targetRenderer != null ? targetRenderer.bounds : new Bounds(target.position, Vector3.zero);

		foreach (Transform child in target)
		{
			AddRendererBoundsRecursive(child, ref bounds);
		}

		return bounds;
	}

	void AddRendererBoundsRecursive(Transform target, ref Bounds bounds)
	{
		Renderer renderer = target.GetComponent<Renderer>();
		if (renderer != null)
		{
			bounds.Encapsulate(renderer.bounds);
		}

		foreach (Transform child in target)
		{
			AddRendererBoundsRecursive(child, ref bounds);
		}
	}
}