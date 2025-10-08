using System;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

public class UIClickSuppressor : IDisposable
{
	private readonly ReactiveProperty<bool> _isSuppressed = new ReactiveProperty<bool>(initialValue: false);

	public IReadOnlyReactiveProperty<bool> IsSuppressed => _isSuppressed;

	public int Delay { get; private set; }

	public UIClickSuppressor()
	{
		Delay = 0;
	}

	public void SuppressClick()
	{
		_isSuppressed.Value = true;
		(from _ in Observable.EveryLateUpdate()
			where Input.GetMouseButtonUp(0)
			select _).DelayFrame(Delay).Take(1).Subscribe(delegate
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
