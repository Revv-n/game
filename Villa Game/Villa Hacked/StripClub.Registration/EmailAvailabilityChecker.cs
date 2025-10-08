using System;
using GreenT;
using GreenT.Net;
using GreenT.Registration;
using StripClub.Extensions;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.Registration;

public class EmailAvailabilityChecker : EmailFormatChecker
{
	private IDisposable checkStream;

	private IEmailCheckRequest emailCheckRequest;

	[Inject]
	public void Init(IEmailCheckRequest emailCheckRequest)
	{
		this.emailCheckRequest = emailCheckRequest;
	}

	protected override void Check(string input)
	{
		input = input.StripUnicodeCharactersFromString();
		base.Check(input);
		if (base.State == ValidationState.IsValid)
		{
			SetState(ValidationState.Undefined);
			checkStream?.Dispose();
			checkStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response>(Observable.CatchIgnore<Response, Exception>(emailCheckRequest.Check(input), (Action<Exception>)delegate(Exception ex)
			{
				ex.LogException();
				SetState(ValidationState.NotValid, 3);
			}), (Action<Response>)ProccessResponse), (Component)this);
		}
	}

	private void ProccessResponse(Response response)
	{
		if (response.Status.Equals(0))
		{
			SetState(ValidationState.IsValid);
		}
		else
		{
			SetState(ValidationState.NotValid, 2);
		}
		onUpdate.OnNext((AbstractChecker)this);
	}
}
