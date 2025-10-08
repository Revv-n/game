using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Data;

public abstract class ConfigStructureInitializer : AbstractInitializerState, IStructureInitializer, IInitializerState
{
	protected RequestType requestType;

	protected string configVersion;

	public ConfigParser.Folder ConfigStructure { get; }

	public ConfigStructureInitializer(ConfigParser.Folder configStructure, RequestType requestType, string configVersion = null, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		ConfigStructure = configStructure;
		this.requestType = requestType;
		this.configVersion = configVersion;
	}

	public abstract IObservable<bool> Initialize();

	protected T GetContent<T>()
	{
		return ConfigStructure.GetContent<T>(requestType, configVersion);
	}

	protected T[] GetContentArray<T>()
	{
		return ConfigStructure.GetContentArray<T>(requestType, configVersion);
	}
}
public class ConfigStructureInitializer<TMapper, TEntity> : ConfigStructureInitializer
{
	protected readonly IManager<TEntity> manager;

	protected readonly IFactory<TMapper, TEntity> factory;

	public ConfigStructureInitializer(IManager<TEntity> manager, IFactory<TMapper, TEntity> factory, RequestType requestType, ConfigParser.Folder configStructure, IEnumerable<IStructureInitializer> others = null)
		: base(configStructure, requestType, null, others)
	{
		this.manager = manager;
		this.factory = factory;
	}

	public override IObservable<bool> Initialize()
	{
		try
		{
			TEntity[] entities = ((IEnumerable<TMapper>)GetContentArray<TMapper>()).Select((Func<TMapper, TEntity>)factory.Create).ToArray();
			manager.AddRange(entities);
			return Observable.Do<bool>(Observable.Return(true).Debug(typeof(TEntity)?.ToString() + " has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
			{
				isInitialized.Value = _init;
			});
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}
public class ConfigStructureInitializer<TMapper, TEntity, TEntityBase> : ConfigStructureInitializer where TEntity : TEntityBase
{
	private readonly IManager<TEntityBase> manager;

	private readonly IFactory<TMapper, TEntity> factory;

	public ConfigStructureInitializer(IManager<TEntityBase> manager, IFactory<TMapper, TEntity> factory, RequestType requestType, ConfigParser.Folder configStructure, IEnumerable<IStructureInitializer> others = null)
		: base(configStructure, requestType, null, others)
	{
		this.manager = manager;
		this.factory = factory;
	}

	public override IObservable<bool> Initialize()
	{
		try
		{
			TEntityBase[] entities = ((IEnumerable<TMapper>)GetContentArray<TMapper>()).Select((Func<TMapper, TEntity>)factory.Create).OfType<TEntityBase>().ToArray();
			manager.AddRange(entities);
			return Observable.Do<bool>(Observable.Return(true).Debug(typeof(TEntity)?.ToString() + " has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
			{
				isInitialized.Value = _init;
			});
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}
