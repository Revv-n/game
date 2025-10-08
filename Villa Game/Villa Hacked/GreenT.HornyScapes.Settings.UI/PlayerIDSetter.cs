using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<User>(user.OnUpdate, (Action<User>)SetActualID), (ICollection<IDisposable>)compositeDisposabe);
	}

	private void SetActualID(User user)
	{
		string playerID = this.user.PlayerID;
		userIDField.text = ((!playerID.IsNullOrEmpty()) ? ("User ID: " + this.user.PlayerID) : "User ID: unknown");
	}

	public void Dispose()
	{
		CompositeDisposable obj = compositeDisposabe;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
