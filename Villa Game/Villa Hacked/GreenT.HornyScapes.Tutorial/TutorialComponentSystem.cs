using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public abstract class TutorialComponentSystem<TComponent, TStep> : TutorialEntitySystem<TComponent, TStep> where TComponent : BaseTutorialComponent<TStep> where TStep : TutorialStepSO
{
	public GameObject Target;

	[SerializeField]
	private Image highlightImage;

	[SerializeField]
	private Button button;

	protected bool subsystemInited;

	protected TutorialButton<TComponent, TStep> tutorButton;

	protected TutorialHighlighter highlighter;

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		InitSubSystem();
		SubscribeOnComplete();
		return true;
	}

	protected override void InitSubSystem()
	{
		if (!subsystemInited)
		{
			highlighter = Target.AddComponent<TutorialHighlighter>();
			highlighter.Init(lightningSystem, highlightImage);
			if ((bool)button)
			{
				tutorButton = new TutorialButton<TComponent, TStep>();
				tutorButton.Init(highlighter, button);
			}
			subsystemInited = true;
		}
	}

	protected abstract void SubscribeInteract();

	protected override void DestroySubSystem()
	{
		if (subsystemInited)
		{
			highlighter.Destroy();
			if ((bool)button)
			{
				tutorButton.Dispose();
				tutorButton = null;
			}
		}
	}
}
