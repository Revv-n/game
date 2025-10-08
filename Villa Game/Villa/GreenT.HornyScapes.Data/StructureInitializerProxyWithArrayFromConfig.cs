using System;
using System.Collections.Generic;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Data;

public class StructureInitializerProxyWithArrayFromConfig<T> : IStructureInitializer<ConfigParser.Folder, RequestType>, IInitializerState, IStructureInitializer<ConfigParser.Folder, RequestType, string>
{
	public IStructureInitializer<IEnumerable<T>> StructureInitializer { get; }

	public IReadOnlyReactiveProperty<bool> IsInitialized => StructureInitializer.IsInitialized;

	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized => StructureInitializer.IsRequiredInitialized;

	public StructureInitializerProxyWithArrayFromConfig(IStructureInitializer<IEnumerable<T>> structureInitializers)
	{
		StructureInitializer = structureInitializers;
	}

	public IObservable<bool> Initialize(ConfigParser.Folder configStructure, RequestType requestType, string version)
	{
		T[] contentArray = configStructure.GetContentArray<T>(requestType, version);
		return StructureInitializer.Initialize(contentArray);
	}

	public IObservable<bool> Initialize(ConfigParser.Folder configStructure, RequestType requestType)
	{
		return Initialize(configStructure, requestType, string.Empty);
	}
}
