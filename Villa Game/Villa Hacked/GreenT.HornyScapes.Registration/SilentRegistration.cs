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
		disposable = ObservableExtensions.Subscribe<Unit>(Observable.Where<Unit>(Observable.First<Unit>(UnityUIComponentExtensions.OnClickAsObservable(playButton)), (Func<Unit, bool>)((Unit _) => user.Equals(User.Unknown))), (Action<Unit>)delegate
		{
			RegistrationRequest();
		}, (Action<Exception>)delegate(Exception ex)
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
