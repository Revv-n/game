using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.UI;

public abstract class ShowCanvasByLocker : MonoBehaviour
{
	public Canvas showCanvas;

	protected GameStarter gameStarter;

	[Inject]
	private void Init(GameStarter gameStarter)
	{
		this.gameStarter = gameStarter;
	}

	protected virtual void Awake()
	{
		gameStarter.IsGameActive.Where((bool x) => x).Take(1).ContinueWith(GetCondition())
			.Subscribe(showCanvas.gameObject.SetActive)
			.AddTo(this);
	}

	protected abstract IObservable<bool> GetCondition();
}
