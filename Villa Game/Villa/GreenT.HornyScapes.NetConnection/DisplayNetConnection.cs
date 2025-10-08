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
		netConnectionSystem.OnSuccess.Subscribe(delegate
		{
			Hide();
		}).AddTo(streams);
		netConnectionSystem.OnError.Subscribe(delegate
		{
			Show();
		}).AddTo(streams);
	}

	private void Awake()
	{
		netConnectionSystem.IsPinging.Subscribe(SetState).AddTo(streams);
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
