using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Characters;

public class CharacterLoader : ILoader<IEnumerable<CharacterInfo>>
{
	private readonly ILoader<IEnumerable<CharacterInfoMapper>> mapperLoader;

	private readonly IFactory<CharacterInfoMapper, CharacterInfo> characterFactory;

	public CharacterLoader(ILoader<IEnumerable<CharacterInfoMapper>> mapperLoader, IFactory<CharacterInfoMapper, CharacterInfo> characterFactory)
	{
		this.mapperLoader = mapperLoader;
		this.characterFactory = characterFactory;
	}

	public IObservable<IEnumerable<CharacterInfo>> Load()
	{
		return (from x in mapperLoader.Load().Do(delegate(IEnumerable<CharacterInfoMapper> _mappers)
			{
				Console.SendLogCollection("Loaded characters mappers: ", _mappers.Select((CharacterInfoMapper _map) => _map.id));
			})
			select x.Select(characterFactory.Create).ToArray()).Catch(delegate(Exception innerEx)
		{
			throw innerEx.SendException("On character loading");
		}).Debug("Load characters overview", LogType.Data);
	}
}
