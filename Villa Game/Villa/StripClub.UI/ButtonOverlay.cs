using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI;

public class ButtonOverlay : Selectable
{
	[SerializeField]
	private Selectable sourceButton;

	private Sprite defaultOverlay;

	private CompositeDisposable subscribes = new CompositeDisposable();

	protected override void OnEnable()
	{
		sourceButton.OnSelectAsObservable().Subscribe(OnSelect).AddTo(subscribes);
		sourceButton.OnDeselectAsObservable().Subscribe(OnDeselect).AddTo(subscribes);
		sourceButton.OnPointerDownAsObservable().Subscribe(OnPointerDown).AddTo(subscribes);
		sourceButton.OnPointerUpAsObservable().Subscribe(OnPointerUp).AddTo(subscribes);
		sourceButton.OnPointerEnterAsObservable().Subscribe(OnPointerEnter).AddTo(subscribes);
		sourceButton.OnPointerExitAsObservable().Subscribe(OnPointerExit).AddTo(subscribes);
		sourceButton.ObserveEveryValueChanged((Selectable btn) => btn.interactable).Subscribe(delegate(bool _interactable)
		{
			base.interactable = _interactable;
		}).AddTo(subscribes);
	}

	protected override void OnDisable()
	{
		subscribes?.Clear();
	}

	protected override void OnDestroy()
	{
		subscribes?.Dispose();
	}
}
