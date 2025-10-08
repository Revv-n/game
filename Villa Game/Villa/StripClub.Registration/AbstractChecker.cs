using System;
using GreenT.Localizations;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.Registration;

public abstract class AbstractChecker : MonoBehaviour
{
	public enum ValidationState
	{
		Undefined,
		IsValid,
		NotValid
	}

	[Tooltip("Allow validate same data due to repeate validation call")]
	[SerializeField]
	private bool allowRepeatedValidation;

	protected Subject<AbstractChecker> onUpdate = new Subject<AbstractChecker>();

	private LocalizationService _localizationService;

	public string Data { get; private set; }

	public int ErrorCode { get; private set; }

	protected abstract string ErrorLocalizationPrefix { get; }

	public IObservable<AbstractChecker> OnUpdate => onUpdate;

	public ValidationState State { get; private set; }

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public void Validate(string input)
	{
		if (allowRepeatedValidation || string.IsNullOrEmpty(Data) || !Data.Equals(input))
		{
			Data = input;
			RepeatValidateCurrentData();
		}
	}

	public void RepeatValidateCurrentData()
	{
		Check(Data);
		onUpdate.OnNext(this);
	}

	protected abstract void Check(string input);

	protected void SetState(ValidationState state, int errorCode = 0)
	{
		ErrorCode = errorCode;
		State = state;
	}

	public virtual string ErrorMessage(int errorCode)
	{
		if (errorCode == 0)
		{
			return string.Empty;
		}
		return _localizationService.Text(ErrorLocalizationPrefix + errorCode);
	}
}
