using System;
using System.Collections.Generic;
using GreenT.HornyScapes.StarShop.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialStarShopSystem : TutorialComponentSystem<TutorialEntityComponent, TutorialEntityStepSO>
{
	[SerializeField]
	private StarShopView taskView;

	public List<TutorialEntityStepSO> Steps = new List<TutorialEntityStepSO>();

	private IDisposable disposable;

	protected virtual void OnValidate()
	{
		if (taskView == null)
		{
			taskView = UnityEngine.Object.FindObjectOfType<StarShopView>();
		}
	}

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		disposable = taskView.OnSet.Subscribe(delegate
		{
			SubscribeInteract();
		});
		return true;
	}

	protected override void InitComponents()
	{
		foreach (TutorialEntityStepSO step in Steps)
		{
			TutorialEntityComponent tutorialEntityComponent = new TutorialEntityComponent(step, groupManager);
			if (tutorialEntityComponent.IsInited.Value)
			{
				components.Add(tutorialEntityComponent);
			}
		}
	}

	protected override void SubscribeInteract()
	{
		tutorButton.ClearActiveStream();
		if (TryGetComponent(taskView.Source.ID, out var tutorialEntityComponent))
		{
			tutorButton.SubscribeOnActivate(tutorialEntityComponent);
		}
	}

	private bool TryGetComponent(int id, out TutorialEntityComponent tutorialEntityComponent)
	{
		tutorialEntityComponent = components.Find((TutorialEntityComponent _component) => _component.UniqID == id);
		return tutorialEntityComponent != null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		disposable?.Dispose();
	}
}
