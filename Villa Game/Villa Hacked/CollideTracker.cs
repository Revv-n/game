using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class CollideTracker : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private Subject<Unit> onClick = new Subject<Unit>();

	private Unit unit;

	public IObservable<Unit> OnClick => Observable.AsObservable<Unit>((IObservable<Unit>)onClick);

	public void OnPointerClick(PointerEventData eventData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		onClick.OnNext(unit);
	}
}
