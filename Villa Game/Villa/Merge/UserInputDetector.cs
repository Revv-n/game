using System;
using UniRx;
using UnityEngine;

namespace Merge;

public abstract class UserInputDetector : MonoBehaviour
{
	private Subject<UserInputDetector> onDrag = new Subject<UserInputDetector>();

	private Subject<UserInputDetector> onDragBegin = new Subject<UserInputDetector>();

	private Subject<UserInputDetector> onDragEnd = new Subject<UserInputDetector>();

	private Subject<UserInputDetector> onPointerDown = new Subject<UserInputDetector>();

	private Subject<UserInputDetector> onPointerUp = new Subject<UserInputDetector>();

	private Subject<UserInputDetector> onClick = new Subject<UserInputDetector>();

	public bool IsPointerDown { get; protected set; }

	public bool IsDragging { get; protected set; }

	public Vector2 PointerScreenPosition { get; protected set; } = Vector2.zero;


	public Vector2 PointerShift { get; protected set; } = Vector2.zero;


	public Vector2 PointerDownScreenPosition { get; protected set; } = Vector2.zero;


	public Vector2 PreviousScreenPosition { get; protected set; } = Vector2.zero;


	public virtual IObservable<UserInputDetector> OnDragging()
	{
		return onDrag.AsObservable();
	}

	public virtual IObservable<UserInputDetector> OnDragBegin()
	{
		return onDragBegin.AsObservable();
	}

	public virtual IObservable<UserInputDetector> OnDragEnd()
	{
		return onDragEnd.AsObservable();
	}

	public virtual IObservable<UserInputDetector> OnPointerDown()
	{
		return onPointerDown.AsObservable();
	}

	public virtual IObservable<UserInputDetector> OnPointerUp()
	{
		return onPointerUp.AsObservable();
	}

	public virtual IObservable<UserInputDetector> OnClick()
	{
		return onClick.AsObservable();
	}

	protected void Drag()
	{
		onDrag?.OnNext(this);
	}

	protected void DragBegin()
	{
		onDragBegin?.OnNext(this);
	}

	protected void DragEnd()
	{
		onDragEnd?.OnNext(this);
	}

	protected void PointerDown()
	{
		onPointerDown?.OnNext(this);
	}

	protected void PointerUp()
	{
		onPointerUp?.OnNext(this);
	}

	protected void Click()
	{
		onClick?.OnNext(this);
	}

	protected virtual void OnDestroy()
	{
		DisposbeSubject(onDrag);
		DisposbeSubject(onDragBegin);
		DisposbeSubject(onDragEnd);
		DisposbeSubject(onPointerDown);
		DisposbeSubject(onPointerUp);
		DisposbeSubject(onClick);
	}

	protected static void DisposbeSubject<T>(Subject<T> subject)
	{
		subject.OnCompleted();
		subject.Dispose();
	}
}
