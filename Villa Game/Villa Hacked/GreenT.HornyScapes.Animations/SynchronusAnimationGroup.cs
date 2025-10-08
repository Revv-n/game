using DG.Tweening;

namespace GreenT.HornyScapes.Animations;

public class SynchronusAnimationGroup : AnimationGroup
{
	private Sequence additive;

	public override Sequence Play()
	{
		sequence = base.Play();
		additive = DOTween.Sequence();
		for (int i = 0; i < animations.Count; i++)
		{
			additive = additive.Join(animations[i].Play());
		}
		sequence = sequence.Append(additive).SetLoops(loops, loopType);
		return sequence;
	}

	public override void Stop()
	{
		base.Stop();
		if (additive.IsActive())
		{
			additive.Kill();
			additive = null;
		}
	}
}
