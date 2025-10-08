using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class CollideTracker : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private Subject<Unit> onClick = new Subject<Unit>();

	private Unit unit;

	public IObservable<Unit> OnClick => onClick.AsObservable();

	public void OnPointerClick(PointerEventData eventData)
	{
		onClick.OnNext(unit);
	}
}
