using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Bank.BankTabs;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialBankTabSystem : TutorialComponentSystem<TutorialEntityComponent, TutorialEntityStepSO>
{
	[SerializeField]
	private BankTabView view;

	[SerializeField]
	private Toggle toggle;

	protected TutorialToggle<TutorialEntityComponent, TutorialEntityStepSO> tutorToggle;

	private IDisposable disposable;

	public List<TutorialEntityStepSO> Steps = new List<TutorialEntityStepSO>();

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		disposable = ObservableExtensions.Subscribe<BankTab>(Observable.Where<BankTab>((IObservable<BankTab>)view.OnSet, (Func<BankTab, bool>)((BankTab _value) => _value != null)), (Action<BankTab>)delegate
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

	protected override void InitSubSystem()
	{
		if (!subsystemInited)
		{
			base.InitSubSystem();
			tutorToggle = new TutorialToggle<TutorialEntityComponent, TutorialEntityStepSO>();
			tutorToggle.Init(highlighter, toggle);
		}
	}

	protected override void DestroySubSystem()
	{
		if (subsystemInited)
		{
			base.DestroySubSystem();
			tutorToggle.Dispose();
			tutorToggle = null;
		}
	}

	protected override void SubscribeInteract()
	{
		tutorToggle.ClearActiveStream();
		if (TryGetComponent(view.Source.ID, out var tutorialEntityComponent))
		{
			tutorToggle.SubscribeOnActivate(tutorialEntityComponent);
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
