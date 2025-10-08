using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.Utilities;
using UnityEngine;

namespace GreenT.HornyScapes.UI.Meta;

public class CameraAnimation : GreenT.HornyScapes.Animations.Animation
{
	public Bounds MovementBounds;

	public Camera Camera;

	public Vector3 Position;

	[Header("Move to Object")]
	public float MoveDuration = 1f;

	public Ease ToObjectEase;

	[Header("ScaleIn to Object")]
	public float ScaleInDuration;

	public float ZoomOrthoSize = 6.6f;

	public float MinZoomOrthoSize = 6f;

	public float MaxZoomOrthoSize = 20f;

	public Ease ScaleInToObjectEase;

	[Header("Wait on Object")]
	public float UnZoomWaitTime = 1f;

	[Header("ScaleOut to Object")]
	public float ScaleOutDuration;

	public float UnZoomOrthoSize = 14f;

	public Ease ScaleOutToObjectEase;

	private Vector3 initPosition;

	private float initOrthoSize;

	public override Sequence Play()
	{
		float endValue = Mathf.Clamp(ZoomOrthoSize, MinZoomOrthoSize, MaxZoomOrthoSize);
		sequence = base.Play().Append(Camera.transform.DOMove(Position, MoveDuration).SetEase(ToObjectEase)).Join(Camera.DOOrthoSize(endValue, ScaleInDuration).SetEase(ScaleInToObjectEase))
			.AppendInterval(UnZoomWaitTime)
			.Append(Camera.DOOrthoSize(UnZoomOrthoSize, ScaleOutDuration).OnUpdate(delegate
			{
				CameraTools.FitCameraPosition(Camera, MovementBounds);
			}))
			.SetEase(ScaleOutToObjectEase);
		return sequence;
	}

	public override void Init()
	{
		base.Init();
		initPosition = Camera.transform.position;
		initOrthoSize = Camera.orthographicSize;
	}

	public override void ResetToAnimStart()
	{
		Stop();
		Camera.transform.position = initPosition;
		Camera.orthographicSize = initOrthoSize;
	}
}
