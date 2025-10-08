using System;
using System.Collections.Generic;
using DG.Tweening;
using GreenT.Utilities;
using ModestTree;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta.Navigation;

public class NavigationController : MonoBehaviour
{
	[Range(0.01f, 0.3f)]
	[SerializeField]
	private float scrollPower = 0.05f;

	[SerializeField]
	private float scrollTime = 0.15f;

	[SerializeField]
	private Ease scrollEase = Ease.OutSine;

	[SerializeField]
	protected Vector2 MaxOrthographicSize = new Vector2(2f, 10f);

	[SerializeField]
	private Vector3 defaultPosition;

	[SerializeField]
	private int defaultSize = 14;

	private Sequence sequence;

	private INavigation navigation;

	private CompositeDisposable disposables;

	[field: SerializeField]
	public Camera Camera { get; private set; }

	[field: SerializeField]
	public Bounds MovementBounds { get; protected set; }

	public bool Active { get; private set; }

	public bool Drag { get; private set; }

	[Inject]
	public void Init(INavigation navigation)
	{
		this.navigation = navigation;
	}

	private void Awake()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		if (!Camera)
		{
			SetCamera();
		}
		Assert.IsNotNull((object)Camera);
		disposables = new CompositeDisposable();
	}

	private void OnDestroy()
	{
		disposables.Dispose();
	}

	public virtual void Start()
	{
		Camera.transform.position = defaultPosition;
		Camera.orthographicSize = defaultSize;
	}

	protected virtual void SetCamera()
	{
		Camera = Camera.main;
	}

	public void SetMovementBounds(Bounds bounds)
	{
		MovementBounds = bounds;
		CameraTools.FitCameraPosition(Camera, bounds);
	}

	public virtual void SnapTo(Bounds bounds)
	{
		Camera.orthographicSize = CalculateOrthoSize(bounds);
		Camera.transform.position = CalculateCameraPosition(bounds);
		CameraTools.FitCameraPosition(Camera, MovementBounds);
	}

	private void ZoomCamera(float scrollDelta)
	{
		if (scrollDelta != 0f)
		{
			float num = Mathf.Sign(scrollDelta);
			float value = Camera.orthographicSize * (1f - num * scrollPower);
			float max = Mathf.Min(MovementBounds.size.y / 2f, MovementBounds.size.x / Camera.aspect / 2f, MaxOrthographicSize.y);
			float endValue = Mathf.Clamp(value, MaxOrthographicSize.x, max);
			if (sequence.IsActive())
			{
				sequence.Kill();
			}
			sequence = DOTween.Sequence().Append(Camera.DOOrthoSize(endValue, scrollTime).SetEase(scrollEase));
		}
	}

	private void MoveCamera(Vector2 shift)
	{
		float num = Camera.orthographicSize * 2f / Screen.safeArea.height;
		shift *= num;
		Vector4 cameraWorldBorders = CameraTools.GetCameraWorldBorders(Camera);
		if ((shift.x < 0f && cameraWorldBorders.x + shift.x < MovementBounds.min.x) || (shift.x > 0f && cameraWorldBorders.z + shift.x > MovementBounds.max.x))
		{
			shift *= Vector2.up;
		}
		if ((shift.y < 0f && cameraWorldBorders.y + shift.y < MovementBounds.min.y) || (shift.y > 0f && cameraWorldBorders.w + shift.y > MovementBounds.max.y))
		{
			shift *= Vector2.right;
		}
		Camera.transform.position += (Vector3)shift;
	}

	protected Vector3 CalculateCameraPosition(Bounds bounds)
	{
		return bounds.center + Camera.transform.position.z * Vector3.forward;
	}

	protected float CalculateOrthoSize(Bounds bounds)
	{
		Vector4 cameraWorldBorders = CameraTools.GetCameraWorldBorders(Camera);
		Vector2 vector = new Vector2(cameraWorldBorders.z - cameraWorldBorders.x, cameraWorldBorders.w - cameraWorldBorders.y);
		float num = bounds.size.x / vector.x;
		float num2 = bounds.size.y / vector.y;
		float num3 = num + num2;
		return Mathf.Clamp(Camera.orthographicSize * num3, MaxOrthographicSize.x, MaxOrthographicSize.y);
	}

	public void Activate(bool active)
	{
		if (Active == active)
		{
			return;
		}
		Active = active;
		disposables.Clear();
		if (Active)
		{
			IObservable<Vector2> observable = navigation.OnDrag();
			IObservable<float> observable2 = navigation.OnZoom();
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Vector2>(observable, (Action<Vector2>)MoveCamera), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<float>(observable2, (Action<float>)ZoomCamera), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<Vector2>(observable), new IObservable<Unit>[1] { Observable.AsUnitObservable<float>(observable2) }), (Action<Unit>)delegate
			{
				CameraTools.FitCameraPosition(Camera, MovementBounds);
			}), (ICollection<IDisposable>)disposables);
		}
	}

	public void SetDrag(bool hardState)
	{
		Drag = hardState;
	}
}
