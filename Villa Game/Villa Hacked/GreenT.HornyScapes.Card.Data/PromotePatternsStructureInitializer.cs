using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using StripClub.Model.Cards;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Card.Data;

public class PromotePatternsStructureInitializer : StructureInitializer<IEnumerable<PromotePatternMapper>>
{
	private readonly IFactory<IEnumerable<PromotePatternMapper>, PromotePatterns> patternsFactory;

	private PromotePatterns promotePatterns;

	public PromotePatternsStructureInitializer(IFactory<IEnumerable<PromotePatternMapper>, PromotePatterns> patternsFactory, PromotePatterns promotePatterns, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.patternsFactory = patternsFactory;
		this.promotePatterns = promotePatterns;
	}

	public override IObservable<bool> Initialize(IEnumerable<PromotePatternMapper> mapper)
	{
		foreach (KeyValuePair<int, Pattern> item in patternsFactory.Create(mapper))
		{
			promotePatterns.Add(item.Key, item.Value);
		}
		return Observable.Do<bool>(Observable.Return(true).Debug("Load Promote patterns", LogType.Data), (Action<bool>)isInitialized.SetValueAndForceNotify);
	}
}
