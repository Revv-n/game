using System;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.Navigation;

public abstract class AbstractNavigationMonoBehaviour : MonoBehaviour, INavigation, IDrag, IZoom, IPointerPress
{
	private Subject<Vector2> onDrag;

	private Subject<float> onZoom;

	private Subject<Vector2> onPointerDown;

	private Subject<Vector2> onPointerUp;

	private Subject<Vector2> onPointerClick;

	public bool IsDragging { get; protected set; }

	public Vector2 DragShift { get; protected set; } = Vector2.zero;


	public bool IsZooming { get; protected set; }

	public float ZoomDelta { get; protected set; }

	public IObservable<Vector2> OnDrag()
	{
		return Observable.AsObservable<Vector2>((IObservable<Vector2>)onDrag);
	}

	protected void Drag(Vector2 dragVector)
	{
		onDrag?.OnNext(dragVector);
	}

	public IObservable<float> OnZoom()
	{
		return Observable.AsObservable<float>((IObservable<float>)onZoom);
	}

	protected void Zoom(float zoom)
	{
		onZoom?.OnNext(zoom);
	}

	public IObservable<Vector2> OnPointerDown()
	{
		return Observable.AsObservable<Vector2>((IObservable<Vector2>)onPointerDown);
	}

	protected virtual void PointerDown(Vector2 pointerPosition)
	{
		onPointerDown?.OnNext(pointerPosition);
	}

	public IObservable<Vector2> OnPointerUp()
	{
		return Observable.AsObservable<Vector2>((IObservable<Vector2>)onPointerUp);
	}

	protected virtual void PointerUp(Vector2 pointerPosition)
	{
		onPointerUp?.OnNext(pointerPosition);
	}

	public IObservable<Vector2> OnPointerClick()
	{
		return Observable.AsObservable<Vector2>((IObservable<Vector2>)onPointerClick);
	}

	protected virtual void PointerClick(Vector2 pointerPosition)
	{
		onPointerClick?.OnNext(pointerPosition);
	}

	private void Awake()
	{
		onDrag = new Subject<Vector2>();
		onZoom = new Subject<float>();
		onPointerDown = new Subject<Vector2>();
		onPointerUp = new Subject<Vector2>();
		onPointerClick = new Subject<Vector2>();
	}

	protected void OnDestroy()
	{
		onDrag.OnCompleted();
		onDrag.Dispose();
		onZoom.OnCompleted();
		onZoom.Dispose();
		onPointerDown.OnCompleted();
		onPointerDown.Dispose();
		onPointerUp.OnCompleted();
		onPointerUp.Dispose();
		onPointerClick.OnCompleted();
		onPointerClick.Dispose();
	}
}
