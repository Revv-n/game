using System;
using System.Collections.Generic;
using StripClub.Model.Cards;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Card;

public class PromoteInfoLoader : ILoader<PromotePatterns>
{
	private IFactory<IEnumerable<PromotePatternMapper>, PromotePatterns> patternsFactory;

	private readonly ILoader<IEnumerable<PromotePatternMapper>> mapperLoader;

	public PromoteInfoLoader(IFactory<IEnumerable<PromotePatternMapper>, PromotePatterns> patternsFactory, ILoader<IEnumerable<PromotePatternMapper>> mapperLoader)
	{
		this.patternsFactory = patternsFactory;
		this.mapperLoader = mapperLoader;
	}

	public IObservable<PromotePatterns> Load()
	{
		return mapperLoader.Load().Select(patternsFactory.Create);
	}
}
