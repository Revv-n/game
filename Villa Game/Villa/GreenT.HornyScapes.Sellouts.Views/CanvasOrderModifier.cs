using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class CanvasOrderModifier : MonoBehaviour
{
	[SerializeField]
	private Canvas _source;

	[SerializeField]
	private Canvas _target;

	public void Check(bool isOverrideSorting)
	{
		Observable.NextFrame().ObserveOnMainThread().Subscribe(delegate
		{
			_target.overrideSorting = isOverrideSorting;
			_target.sortingOrder = _source.sortingOrder + 1;
		})
			.AddTo(this);
	}
}
