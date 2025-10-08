using System;
using System.Collections.Generic;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialSummonSystem : TutorialComponentSystem<TutorialComponent, TutorialStepSO>
{
	[SerializeField]
	private SummonLotView summonView;

	public List<TutorialStepSO> Steps = new List<TutorialStepSO>();

	private IDisposable disposable;

	protected virtual void OnValidate()
	{
		if (summonView == null)
		{
			summonView = UnityEngine.Object.FindObjectOfType<SummonLotView>();
		}
	}

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		disposable = summonView.OnSet.Where((SummonLot _lot) => _lot != null).Subscribe(delegate
		{
			SubscribeInteract();
		});
		return true;
	}

	protected override void InitComponents()
	{
		foreach (TutorialStepSO step in Steps)
		{
			TutorialComponent tutorialComponent = new TutorialComponent(step, groupManager);
			if (tutorialComponent.IsInited.Value)
			{
				components.Add(tutorialComponent);
			}
		}
	}

	protected override void SubscribeInteract()
	{
		if (components == null)
		{
			return;
		}
		foreach (TutorialComponent component in components)
		{
			tutorButton.SubscribeOnActivate(component);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		disposable?.Dispose();
	}
}
