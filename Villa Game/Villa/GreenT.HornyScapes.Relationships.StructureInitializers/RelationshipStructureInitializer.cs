using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Relationships.Factories;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using Zenject;

namespace GreenT.HornyScapes.Relationships.StructureInitializers;

public class RelationshipStructureInitializer : StructureInitializerViaArray<RelationshipMapper, Relationship>
{
	private readonly RelationshipMapperProvider _mapperProvider;

	public RelationshipStructureInitializer(RelationshipMapperProvider mapperProvider, RelationshipProvider provider, RelationshipFactory factory, IEnumerable<IStructureInitializer> others = null)
		: base((IManager<Relationship>)provider, (IFactory<RelationshipMapper, Relationship>)factory, others)
	{
		_mapperProvider = mapperProvider;
	}

	public override IObservable<bool> Initialize(IEnumerable<RelationshipMapper> mappers)
	{
		_mapperProvider.AddRange(mappers);
		return base.Initialize(mappers);
	}
}
