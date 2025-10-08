using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StripClub.UI;

public class ButtonOverlay : Selectable
{
	[SerializeField]
	private Selectable sourceButton;

	private Sprite defaultOverlay;

	private CompositeDisposable subscribes = new CompositeDisposable();

	protected override void OnEnable()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BaseEventData>(ObservableTriggerExtensions.OnSelectAsObservable((UIBehaviour)sourceButton), (Action<BaseEventData>)OnSelect), (ICollection<IDisposable>)subscribes);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BaseEventData>(ObservableTriggerExtensions.OnDeselectAsObservable((UIBehaviour)sourceButton), (Action<BaseEventData>)OnDeselect), (ICollection<IDisposable>)subscribes);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PointerEventData>(ObservableTriggerExtensions.OnPointerDownAsObservable((UIBehaviour)sourceButton), (Action<PointerEventData>)OnPointerDown), (ICollection<IDisposable>)subscribes);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PointerEventData>(ObservableTriggerExtensions.OnPointerUpAsObservable((UIBehaviour)sourceButton), (Action<PointerEventData>)OnPointerUp), (ICollection<IDisposable>)subscribes);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PointerEventData>(ObservableTriggerExtensions.OnPointerEnterAsObservable((UIBehaviour)sourceButton), (Action<PointerEventData>)OnPointerEnter), (ICollection<IDisposable>)subscribes);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PointerEventData>(ObservableTriggerExtensions.OnPointerExitAsObservable((UIBehaviour)sourceButton), (Action<PointerEventData>)OnPointerExit), (ICollection<IDisposable>)subscribes);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(ObserveExtensions.ObserveEveryValueChanged<Selectable, bool>(sourceButton, (Func<Selectable, bool>)((Selectable btn) => btn.interactable), (FrameCountType)0, false), (Action<bool>)delegate(bool _interactable)
		{
			base.interactable = _interactable;
		}), (ICollection<IDisposable>)subscribes);
	}

	protected override void OnDisable()
	{
		CompositeDisposable obj = subscribes;
		if (obj != null)
		{
			obj.Clear();
		}
	}

	protected override void OnDestroy()
	{
		CompositeDisposable obj = subscribes;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
