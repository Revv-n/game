using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.HornyScapes.Tutorial;
using UniRx;
using UnityEngine;

public class TutorialTaskMergeSystem : TutorialComponentSystem<TutorialEntityComponent, TutorialEntityStepSO>
{
	[SerializeField]
	private TaskView view;

	public List<TutorialEntityStepSO> Steps = new List<TutorialEntityStepSO>();

	private IDisposable disposable;

	protected virtual void OnValidate()
	{
		if (view == null)
		{
			view = UnityEngine.Object.FindObjectOfType<TaskView>();
		}
	}

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		disposable = ObservableExtensions.Subscribe<Task>(view.OnSet, (Action<Task>)delegate
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
		if (TryGetComponent(view.Source.ID, out var tutorialEntityComponent))
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
