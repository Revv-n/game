namespace GreenT.HornyScapes.Animations;

public class AnimationStartersAutoBind : AnimationStartersBind
{
	private void Awake()
	{
		InitStarters();
	}

	private void OnEnable()
	{
		Bind();
	}
}
