using System;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Data;

public class StructureInitializerProxyWithElementFromConfig<T> : IStructureInitializer<ConfigParser.Folder, RequestType>, IInitializerState, IStructureInitializer<ConfigParser.Folder, RequestType, string>
{
	public IStructureInitializer<T> StructureInitializer { get; }

	public IReadOnlyReactiveProperty<bool> IsInitialized => StructureInitializer.IsInitialized;

	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized => StructureInitializer.IsRequiredInitialized;

	public StructureInitializerProxyWithElementFromConfig(IStructureInitializer<T> structureInitializers)
	{
		StructureInitializer = structureInitializers;
	}

	public IObservable<bool> Initialize(ConfigParser.Folder configStructure, RequestType requestType, string version)
	{
		T content = configStructure.GetContent<T>(requestType, version);
		return StructureInitializer.Initialize(content);
	}

	public IObservable<bool> Initialize(ConfigParser.Folder configStructure, RequestType requestType)
	{
		return Initialize(configStructure, requestType, string.Empty);
	}
}
