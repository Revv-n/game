using System;

namespace StripClub.Model.Data;

public interface ILoader<T>
{
	IObservable<T> Load();
}
public interface ILoader<in KParam, out TResult>
{
	IObservable<TResult> Load(KParam param);
}
