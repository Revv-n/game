using System;
using GreenT.Net.User;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Registration;

public class SilentRegistration : MonoBehaviour
{
	[SerializeField]
	private Button playButton;

	private GetDataProcessor getDataProcessor;

	private User user;

	private IDisposable disposable;

	[Inject]
	private void Init(User user, GetDataProcessor getDataProcessor)
	{
		this.user = user;
		this.getDataProcessor = getDataProcessor;
	}

	private void Start()
	{
		disposable = (from _ in playButton.OnClickAsObservable().First()
			where user.Equals(User.Unknown)
			select _).Subscribe(delegate
		{
			RegistrationRequest();
		}, delegate(Exception ex)
		{
			ex.LogException();
		});
	}

	private void OnDestroy()
	{
		disposable?.Dispose();
	}

	private void RegistrationRequest()
	{
		getDataProcessor.Request(user.PlayerID);
	}
}
