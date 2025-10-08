using System;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.ObserveOnMainThread<Unit>(Observable.NextFrame((FrameCountType)0)), (Action<Unit>)delegate
		{
			_target.overrideSorting = isOverrideSorting;
			_target.sortingOrder = _source.sortingOrder + 1;
		}), (Component)this);
	}
}
