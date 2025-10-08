using System;
using UniRx;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public sealed class TutorialToggle<TComponent, KStep> : IDisposable where TComponent : BaseTutorialComponent<KStep> where KStep : TutorialStepSO
{
	private Toggle toggle;

	private TutorialHighlighter tutorialHighlighter;

	private BaseTutorialComponent<KStep> activeComponent;

	private CompositeDisposable onActivateStream = new CompositeDisposable();

	private IDisposable onCompleteStream;

	public void Init(TutorialHighlighter tutorialHighlighter, Toggle toggle)
	{
		this.tutorialHighlighter = tutorialHighlighter;
		this.toggle = toggle;
	}

	public void SubscribeOnActivate(TComponent component)
	{
		if (component.IsActive)
		{
			OnActivate(component);
		}
		else
		{
			onActivateStream.Add(component.OnActivate.Subscribe(OnActivate));
		}
	}

	public void ClearActiveStream()
	{
		onActivateStream.Clear();
	}

	private void OnActivate(BaseTutorialComponent<KStep> step)
	{
		toggle.onValueChanged.RemoveListener(OnClick);
		onCompleteStream?.Dispose();
		_ = activeComponent;
		activeComponent = step;
		tutorialHighlighter.Activate(step.BlockScreen, step.IsLight);
		onCompleteStream = step.StepModel.IsComplete.Where((bool _value) => _value).Subscribe(delegate
		{
			OnComplete();
		});
		toggle.onValueChanged.AddListener(OnClick);
	}

	private void OnClick(bool arg0)
	{
		if (arg0)
		{
			activeComponent.CompleteStep();
			onCompleteStream.Dispose();
		}
	}

	private void OnComplete()
	{
		toggle.onValueChanged.RemoveListener(OnClick);
		tutorialHighlighter.Deactivate();
	}

	public void Dispose()
	{
		onActivateStream.Dispose();
		onCompleteStream?.Dispose();
	}
}
