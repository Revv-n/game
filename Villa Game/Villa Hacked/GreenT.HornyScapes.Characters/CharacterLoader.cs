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
		return Observable.Catch<CharacterInfo[], Exception>(Observable.Select<IEnumerable<CharacterInfoMapper>, CharacterInfo[]>(Observable.Do<IEnumerable<CharacterInfoMapper>>(mapperLoader.Load(), (Action<IEnumerable<CharacterInfoMapper>>)delegate(IEnumerable<CharacterInfoMapper> _mappers)
		{
			Console.SendLogCollection("Loaded characters mappers: ", _mappers.Select((CharacterInfoMapper _map) => _map.id));
		}), (Func<IEnumerable<CharacterInfoMapper>, CharacterInfo[]>)((IEnumerable<CharacterInfoMapper> x) => x.Select((Func<CharacterInfoMapper, CharacterInfo>)characterFactory.Create).ToArray())), (Func<Exception, IObservable<CharacterInfo[]>>)delegate(Exception innerEx)
		{
			throw innerEx.SendException("On character loading");
		}).Debug("Load characters overview", LogType.Data);
	}
}
