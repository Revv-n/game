using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Registration.UI;

[RequireComponent(typeof(Image))]
public class CheckIcon : MonoBehaviour
{
	[SerializeField]
	private AbstractChecker checker;

	[SerializeField]
	private ValidationStateSpritesDictionary spritesDictionary;

	private Image _image;

	private IDisposable checkerStream;

	private void Awake()
	{
		_image = GetComponent<Image>();
	}

	private void OnEnable()
	{
		if (checker != null)
		{
			TrackChecker(checker);
		}
	}

	private void TrackChecker(AbstractChecker checker)
	{
		SetState(checker.State);
		checkerStream?.Dispose();
		checkerStream = ObservableExtensions.Subscribe<AbstractChecker.ValidationState>(Observable.Select<AbstractChecker, AbstractChecker.ValidationState>(checker.OnUpdate, (Func<AbstractChecker, AbstractChecker.ValidationState>)((AbstractChecker _checker) => _checker.State)), (Action<AbstractChecker.ValidationState>)SetState);
	}

	public void Set(AbstractChecker checker)
	{
		this.checker = checker;
		TrackChecker(checker);
	}

	public void SetState(AbstractChecker.ValidationState state)
	{
		spritesDictionary.TryGetValue(state, out var value);
		_image.canvasRenderer.cull = value == null;
		_image.sprite = value;
	}

	private void OnDisable()
	{
		checkerStream?.Dispose();
	}

	private void OnDestroy()
	{
		checkerStream?.Dispose();
	}
}
