using System;
using Merge;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Settings.UI;

public class PlayerIDSetter : MonoBehaviour, IDisposable
{
	[SerializeField]
	private TextMeshProUGUI userIDField;

	private User user;

	private CompositeDisposable compositeDisposabe = new CompositeDisposable();

	private const string userIDText = "User ID:";

	private const string errorText = "unknown";

	[Inject]
	public void Construct(User user)
	{
		this.user = user;
	}

	private void Start()
	{
		SetActualID(user);
		user.OnUpdate.Subscribe(SetActualID).AddTo(compositeDisposabe);
	}

	private void SetActualID(User user)
	{
		string playerID = this.user.PlayerID;
		userIDField.text = ((!playerID.IsNullOrEmpty()) ? ("User ID: " + this.user.PlayerID) : "User ID: unknown");
	}

	public void Dispose()
	{
		compositeDisposabe?.Dispose();
	}
}
