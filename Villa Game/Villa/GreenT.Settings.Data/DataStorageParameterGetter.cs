using System;
using GreenT.Data;
using UniRx;

namespace GreenT.Settings.Data;

public class DataStorageParameterGetter
{
	private const string @default = "default";

	private readonly IDataStorage dataStorage;

	private readonly string key;

	private string parameter;

	public DataStorageParameterGetter(IDataStorage dataStorage, string storageKey)
	{
		this.dataStorage = dataStorage;
		key = storageKey;
	}

	public IObservable<string> Get()
	{
		string value = ((!dataStorage.HasKey(key)) ? "default" : dataStorage.GetString(key));
		return Observable.Return(value);
	}
}
