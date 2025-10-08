using System;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;

namespace StripClub.Registration;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ErrorMessage : MonoBehaviour, IView<string>, IView, IView<AbstractChecker>
{
	[SerializeField]
	protected AbstractChecker _checker;

	protected TextMeshProUGUI _text;

	private IDisposable stream;

	public int SiblingIndex
	{
		get
		{
			return base.transform.GetSiblingIndex();
		}
		set
		{
			base.transform.SetSiblingIndex(value);
		}
	}

	private void Awake()
	{
		_text = GetComponent<TextMeshProUGUI>();
		_text.text = string.Empty;
		if (_checker != null)
		{
			Set(_checker);
		}
	}

	public void Set(AbstractChecker checker)
	{
		_checker = checker;
		_text.text = checker.ErrorMessage(checker.ErrorCode);
		stream?.Dispose();
		stream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AbstractChecker>(_checker.OnUpdate, (Action<AbstractChecker>)UpdateText), (Component)this);
	}

	public void Set(string errorMessage)
	{
		Display(isOn: true);
		_text.text = errorMessage;
	}

	public virtual void UpdateText(AbstractChecker checker)
	{
		Display(checker.State == AbstractChecker.ValidationState.NotValid);
		_text.text = checker.ErrorMessage(checker.ErrorCode);
	}

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
	}

	public bool IsActive()
	{
		return base.gameObject.activeInHierarchy;
	}
}
