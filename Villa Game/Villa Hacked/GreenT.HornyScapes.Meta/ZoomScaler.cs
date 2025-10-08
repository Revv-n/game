using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class ZoomScaler : AbstractZoomScaler
{
	[SerializeField]
	private float startCameraSize;

	[SerializeField]
	private Vector3 startScale;

	private float cameraSize;

	private bool zoomEnable;

	private Camera camera;

	private ICameraChanger _cameraChanger;

	[Inject]
	private void Construct(ICameraChanger cameraChanger)
	{
		_cameraChanger = cameraChanger;
	}

	public override void StartScaling()
	{
		zoomEnable = true;
		camera = _cameraChanger.MainCamera ?? Camera.main;
	}

	private void Update()
	{
		if (zoomEnable && !camera.orthographicSize.Equals(cameraSize))
		{
			ScaleFromCamera();
		}
	}

	private void ScaleFromCamera()
	{
		cameraSize = camera.orthographicSize;
		float num = cameraSize / startCameraSize;
		Vector3 vector = startScale * num;
		Vector3 localScale = default(Vector3);
		localScale.x = Mathf.Clamp(vector.x, 0f, float.PositiveInfinity);
		localScale.y = Mathf.Clamp(vector.y, 0f, float.PositiveInfinity);
		localScale.z = Mathf.Clamp(vector.z, 0f, float.PositiveInfinity);
		base.transform.localScale = localScale;
	}
}
