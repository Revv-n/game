using System.Collections.Generic;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Data;

public class StructureInitializerFactory<TInitializer> : IFactory<ConfigParser.Folder, TInitializer>, IFactory where TInitializer : IStructureInitializer
{
	private readonly DiContainer diContainer;

	private readonly RequestType requestType;

	public StructureInitializerFactory(DiContainer diContainer, RequestType requestType)
	{
		this.diContainer = diContainer;
		this.requestType = requestType;
	}

	public TInitializer Create(ConfigParser.Folder configStructure)
	{
		object[] array = new object[2] { configStructure, requestType };
		return diContainer.Instantiate<TInitializer>((IEnumerable<object>)array);
	}
}
