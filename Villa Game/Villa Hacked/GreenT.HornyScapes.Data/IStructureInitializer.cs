using System;

namespace GreenT.HornyScapes.Data;

public interface IStructureInitializer : IInitializerState
{
	IObservable<bool> Initialize();
}
public interface IStructureInitializer<in T> : IInitializerState
{
	IObservable<bool> Initialize(T param);
}
public interface IStructureInitializer<in T1, in T2> : IInitializerState
{
	IObservable<bool> Initialize(T1 param1, T2 param2);
}
public interface IStructureInitializer<in T1, in T2, in T3> : IInitializerState
{
	IObservable<bool> Initialize(T1 param1, T2 param2, T3 param3);
}
