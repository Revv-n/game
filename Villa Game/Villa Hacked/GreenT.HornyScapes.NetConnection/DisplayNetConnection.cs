using System;
using System.Collections.Generic;
using GreenT.UI;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.NetConnection;

public class DisplayNetConnection : MonoBehaviour, IInitializable
{
	private const string InProgressKey = "ui.netconnection.description.in_progress";

	private const string WaitProgressKey = "ui.netconnection.description.waiting";

	public Window Window;

	public Button Button;

	public LocalizedTextMeshPro Description;

	private NetConnectionSystem netConnectionSystem;

	private readonly CompositeDisposable streams = new CompositeDisposable();

	[Inject]
	private void Constructor(NetConnectionSystem netConnectionSystem)
	{
		this.netConnectionSystem = netConnectionSystem;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(netConnectionSystem.OnSuccess, (Action<string>)delegate
		{
			Hide();
		}), (ICollection<IDisposable>)streams);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Exception>(netConnectionSystem.OnError, (Action<Exception>)delegate
		{
			Show();
		}), (ICollection<IDisposable>)streams);
	}

	private void Awake()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>((IObservable<bool>)netConnectionSystem.IsPinging, (Action<bool>)SetState), (ICollection<IDisposable>)streams);
		Button.onClick.AddListener(CheckConnection);
	}

	private void SetState(bool isPing)
	{
		Button.interactable = !isPing;
		Description.Init(isPing ? "ui.netconnection.description.in_progress" : "ui.netconnection.description.waiting");
	}

	private void CheckConnection()
	{
		netConnectionSystem.ManualCheckConnection();
	}

	private void Show()
	{
		if (!Window.IsOpened)
		{
			Window.Open();
		}
	}

	private void Hide()
	{
		if (Window.IsOpened)
		{
			Window.Close();
		}
	}

	private void OnDestroy()
	{
		streams.Dispose();
		Button.onClick.RemoveAllListeners();
	}
}
