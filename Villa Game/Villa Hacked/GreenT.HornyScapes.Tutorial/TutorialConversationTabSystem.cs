using System;
using System.Collections.Generic;
using StripClub.Messenger;
using StripClub.Messenger.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialConversationTabSystem : TutorialComponentSystem<TutorialEntityComponent, TutorialEntityStepSO>
{
	[SerializeField]
	private ConversationTab conversationTab;

	public List<TutorialEntityStepSO> Steps = new List<TutorialEntityStepSO>();

	[SerializeField]
	private Toggle toggle;

	protected TutorialToggle<TutorialEntityComponent, TutorialEntityStepSO> tutorToggle;

	protected virtual void OnValidate()
	{
		if (conversationTab == null)
		{
			conversationTab = UnityEngine.Object.FindObjectOfType<ConversationTab>();
		}
	}

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		ObservableExtensions.Subscribe<Conversation>(Observable.Where<Conversation>((IObservable<Conversation>)conversationTab.OnSet, (Func<Conversation, bool>)((Conversation _value) => _value != null)), (Action<Conversation>)delegate
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
		if (TryGetComponent(conversationTab.Source.ID, out var tutorialEntityComponent))
		{
			tutorToggle.SubscribeOnActivate(tutorialEntityComponent);
		}
	}

	private bool TryGetComponent(int id, out TutorialEntityComponent tutorialEntityComponent)
	{
		tutorialEntityComponent = components.Find((TutorialEntityComponent _component) => _component.UniqID == id);
		return tutorialEntityComponent != null;
	}
}
