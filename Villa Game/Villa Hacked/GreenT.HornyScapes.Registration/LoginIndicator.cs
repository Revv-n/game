using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Registration;

public abstract class LoginIndicator : MonoBehaviour
{
	private User userData;

	private IDisposable updateStream;

	public bool IsLogged => userData.Type.Contains(User.State.Registered);

	[Inject]
	private void Init(User userData)
	{
		this.userData = userData;
	}

	private void Awake()
	{
		SetLogged(IsLogged);
		updateStream = ObservableExtensions.Subscribe<User>(userData.OnUpdate, (Action<User>)delegate
		{
			SetLogged(IsLogged);
		});
	}

	protected abstract void SetLogged(bool isLogged);

	private void OnDestroy()
	{
		updateStream?.Dispose();
	}
}
