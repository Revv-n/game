using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class TutorialLightningSystem : LightningSystem
{
	protected override Tween StartLightning(Material imageMaterial, float from, float to)
	{
		SetLightning(imageMaterial, from);
		return CreateTween().SetLoops(-1);
		Tween ChangeToValue(float _to, float _from, float _duration)
		{
			return DOTween.To(() => _from, delegate(float x)
			{
				_from = x;
				SetLightning(imageMaterial, x);
			}, _to, _duration);
		}
		Tween CreateTween()
		{
			return DOTween.Sequence().Append(ChangeToValue(to, to / 2f, duration)).Append(ChangeToValue(to / 2f, to, duration / 2f));
		}
	}
}
