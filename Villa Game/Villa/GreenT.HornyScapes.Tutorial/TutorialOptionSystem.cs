using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Messenger.UI;
using GreenT.Types;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialOptionSystem : TutorialComponentSystem<TutorialCompositeEntityComponent, TutorialCompositeEntityStepSO>
{
	[SerializeField]
	private ResponseOptionView optionView;

	public List<TutorialCompositeEntityStepSO> Steps = new List<TutorialCompositeEntityStepSO>();

	private IDisposable disposable;

	protected virtual void OnValidate()
	{
		if (optionView == null)
		{
			optionView = UnityEngine.Object.FindObjectOfType<ResponseOptionView>();
		}
	}

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		disposable = optionView.OnSet.Subscribe(delegate
		{
			SubscribeInteract();
		});
		return true;
	}

	protected override void InitComponents()
	{
		foreach (TutorialCompositeEntityStepSO step in Steps)
		{
			TutorialCompositeEntityComponent tutorialCompositeEntityComponent = new TutorialCompositeEntityComponent(step, groupManager);
			if (tutorialCompositeEntityComponent.IsInited.Value)
			{
				components.Add(tutorialCompositeEntityComponent);
			}
		}
	}

	protected override void SubscribeInteract()
	{
		if (TryGetComponent(optionView.Source.DialogueSerialID, out var tutorialEntityComponent))
		{
			tutorButton.SubscribeOnActivate(tutorialEntityComponent);
		}
	}

	private bool TryGetComponent(CompositeIdentificator compositeIDs, out TutorialCompositeEntityComponent tutorialEntityComponent)
	{
		tutorialEntityComponent = components.Find((TutorialCompositeEntityComponent _component) => _component.CompositeIDs == compositeIDs);
		return tutorialEntityComponent != null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		disposable?.Dispose();
	}
}
