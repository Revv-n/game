using System;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

public class UIClickSuppressor : IDisposable
{
	private readonly ReactiveProperty<bool> _isSuppressed = new ReactiveProperty<bool>(false);

	public IReadOnlyReactiveProperty<bool> IsSuppressed => (IReadOnlyReactiveProperty<bool>)(object)_isSuppressed;

	public int Delay { get; private set; }

	public UIClickSuppressor()
	{
		Delay = 0;
	}

	public void SuppressClick()
	{
		_isSuppressed.Value = true;
		ObservableExtensions.Subscribe<long>(Observable.Take<long>(Observable.DelayFrame<long>(Observable.Where<long>(Observable.EveryLateUpdate(), (Func<long, bool>)((long _) => Input.GetMouseButtonUp(0))), Delay, (FrameCountType)0), 1), (Action<long>)delegate
		{
			UnSuppress();
		});
	}

	private void UnSuppress()
	{
		_isSuppressed.Value = false;
	}

	public void Dispose()
	{
		_isSuppressed.Dispose();
	}
}
