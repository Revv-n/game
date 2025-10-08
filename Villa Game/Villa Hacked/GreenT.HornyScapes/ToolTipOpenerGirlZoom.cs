using System;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Meta.Navigation;
using GreenT.HornyScapes.ToolTips;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class ToolTipOpenerGirlZoom : GirlToolTipOpener
{
	[SerializeField]
	[Range(0f, 0.5f)]
	private float viewPercenetFromScreenSides = 0.3f;

	[SerializeField]
	private float cameraSizeForBubble = 5f;

	[SerializeField]
	private float zoomDelay = 5f;

	private ICameraChanger _cameraChanger;

	private INavigation _navigation;

	private Action<Unit> onChildChanged;

	private Action<float> onZoomCallback;

	private IDisposable zoomDisposable;

	private IDisposable observableDisposable;

	private Transform child;

	private Camera Camera => _cameraChanger.MainCamera;

	[Inject]
	private void Construct(IConstants<int> intConstants, ICameraChanger cameraChanger, INavigation navigation)
	{
		showTime = TimeSpan.FromSeconds(intConstants["phrase_hiring"]);
		_cameraChanger = cameraChanger;
		_navigation = navigation;
		onChildChanged = SetFirstChild;
		onZoomCallback = ScaleFromCamera;
	}

	private void Start()
	{
		StartZoomToolTip();
	}

	private void StartZoomToolTip()
	{
		zoomDisposable?.Dispose();
		IObservable<Unit> observable = Observable.Share<Unit>(Observable.First<Unit>(ObservableTriggerExtensions.OnTransformChildrenChangedAsObservable((Component)base.transform)));
		observableDisposable = ObservableExtensions.Subscribe<Unit>(observable, onChildChanged);
		zoomDisposable = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<float>(Observable.SkipUntil<float, Unit>(Observable.Delay<float>(_navigation.OnZoom(), TimeSpan.FromSeconds(zoomDelay)), observable), onZoomCallback), (Component)this);
	}

	private void SetFirstChild(Unit _)
	{
		child = base.transform.GetChild(0);
	}

	private void ScaleFromCamera(float _)
	{
		base.ReadyToActivate.Value = (bool)child && child.gameObject.activeSelf && Camera.orthographicSize < cameraSizeForBubble && IsVisible(child.position);
		if (base.ReadyToActivate.Value)
		{
			StartZoomToolTip();
		}
	}

	private bool IsVisible(Vector3 position)
	{
		Vector3 vector = Camera.WorldToViewportPoint(position);
		if (vector.x >= viewPercenetFromScreenSides && vector.x <= 1f - viewPercenetFromScreenSides && vector.y >= viewPercenetFromScreenSides)
		{
			return vector.y <= 1f - viewPercenetFromScreenSides;
		}
		return false;
	}

	private void OnDisable()
	{
		observableDisposable?.Dispose();
		zoomDisposable?.Dispose();
	}
}
