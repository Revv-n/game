using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.UI;

public abstract class SetActiveComponentByLocked : MonoBehaviour
{
	public Component component;

	protected GameStarter gameStarter;

	[Inject]
	private void Init(GameStarter gameStarter)
	{
		this.gameStarter = gameStarter;
	}

	protected virtual void Awake()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.ContinueWith<bool, bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), 1), GetCondition()), (Action<bool>)component.gameObject.SetActive), (Component)this);
	}

	protected abstract IObservable<bool> GetCondition();
}
