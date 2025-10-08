using System;

namespace GreenT.Settings.Data;

public interface IGetConfigUrlParameters
{
	IObservable<ConfigurationInfo> Get();
}
