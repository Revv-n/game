using DG.Tweening;

namespace GreenT.HornyScapes.Animations;

public class ChainAnimationGroup : AnimationGroup
{
	public override Sequence Play()
	{
		sequence = base.Play();
		for (int i = 0; i < animations.Count; i++)
		{
			sequence = sequence.Append(animations[i].Play());
		}
		sequence = sequence.SetLoops(loops, loopType);
		return sequence;
	}
}
