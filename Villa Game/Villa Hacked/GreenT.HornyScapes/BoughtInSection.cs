using System;
using UniRx;

namespace GreenT.HornyScapes;

[Serializable]
public class BoughtInSection
{
	public readonly int SectionId;

	public ReactiveProperty<int> Value;

	public BoughtInSection(int sectionId, int value)
	{
		SectionId = sectionId;
		Value = new ReactiveProperty<int>(value);
	}
}
