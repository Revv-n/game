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
		gameStarter.IsGameActive.Where((bool x) => x).Take(1).ContinueWith(GetCondition())
			.Subscribe(component.gameObject.SetActive)
			.AddTo(this);
	}

	protected abstract IObservable<bool> GetCondition();
}
