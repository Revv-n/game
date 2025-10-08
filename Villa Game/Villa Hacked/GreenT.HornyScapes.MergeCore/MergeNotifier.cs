using System;
using Merge;
using UniRx;

namespace GreenT.HornyScapes.MergeCore;

public class MergeNotifier
{
	private readonly Subject<GameItem> _onNotify = new Subject<GameItem>();

	public IObservable<GameItem> OnNotify => (IObservable<GameItem>)_onNotify;

	public void Notify(GameItem gameItem)
	{
		_onNotify.OnNext(gameItem);
	}
}
