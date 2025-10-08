using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tutorial;

public abstract class TutorialEntitySystem<TComponent, KStep> : MonoBehaviour where TComponent : BaseTutorialComponent<KStep> where KStep : TutorialStepSO
{
	protected CompositeDisposable destroyStream = new CompositeDisposable();

	protected List<TComponent> components = new List<TComponent>();

	protected bool isDestroy;

	public bool ActiveSystem;

	protected TutorialGroupManager groupManager;

	protected TutorialLightningSystem lightningSystem;

	protected TutorialSystem tutorialSystem;

	[Inject]
	private void InnerInit(TutorialGroupManager groupManager, TutorialSystem tutorialSystem, TutorialLightningSystem lightningSystem)
	{
		this.tutorialSystem = tutorialSystem;
		this.groupManager = groupManager;
		this.lightningSystem = lightningSystem;
	}

	private void Awake()
	{
		if (IsOff())
		{
			Destroy();
		}
		TryInitSystem();
		bool IsOff()
		{
			return !ActiveSystem;
		}
	}

	public virtual bool TryInitSystem()
	{
		if (isDestroy || !ActiveSystem)
		{
			return false;
		}
		InitComponents();
		if (TryDestroy())
		{
			return false;
		}
		return true;
	}

	protected abstract void InitComponents();

	protected abstract void InitSubSystem();

	protected abstract void DestroySubSystem();

	protected virtual bool TryDestroy()
	{
		bool num = CheckComplete();
		if (num)
		{
			Destroy();
		}
		return num;
	}

	protected virtual bool CheckComplete()
	{
		bool flag = true;
		if (components == null)
		{
			return true;
		}
		if (components.Count > 0)
		{
			foreach (TComponent component in components)
			{
				if (!component.StepModel.IsComplete.Value)
				{
					TutorialGroupSteps group = groupManager.GetGroup(component.GroupID);
					flag &= group.IsCompleted.Value;
				}
			}
		}
		return flag;
	}

	protected virtual void SubscribeOnComplete()
	{
		if (components == null)
		{
			return;
		}
		foreach (TComponent component in components)
		{
			component.StepModel.IsComplete.Where((bool _value) => _value).Subscribe(delegate
			{
				TryDestroy();
			}).AddTo(destroyStream);
		}
	}

	protected virtual void Destroy()
	{
		isDestroy = true;
		DestroySubSystem();
		DestroyComponents();
		Object.Destroy(this);
		void DestroyComponents()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].Dispose();
				components[i] = null;
			}
			components = null;
		}
	}

	protected virtual void OnDestroy()
	{
		if (!isDestroy)
		{
			Destroy();
		}
		destroyStream.Dispose();
	}
}
