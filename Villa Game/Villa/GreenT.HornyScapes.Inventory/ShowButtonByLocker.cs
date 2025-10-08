using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Inventory;

public abstract class ShowButtonByLocker : MonoBehaviour
{
	public Button showButton;

	protected GameStarter gameStarter;

	[Inject]
	private void Init(GameStarter gameStarter)
	{
		this.gameStarter = gameStarter;
	}

	protected virtual void Awake()
	{
		gameStarter.IsGameActive.Where((bool x) => x).Take(1).ContinueWith(GetCondition())
			.Subscribe(showButton.SetActive)
			.AddTo(this);
	}

	protected abstract IObservable<bool> GetCondition();
}
