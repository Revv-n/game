using System;
using System.Collections.Generic;

namespace GreenT.HornyScapes.Data;

public abstract class StructureInitializer : AbstractInitializerState, IStructureInitializer, IInitializerState, IDisposable
{
	public StructureInitializer(IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
	}

	public abstract IObservable<bool> Initialize();
}
public abstract class StructureInitializer<T> : AbstractInitializerState, IStructureInitializer<T>, IInitializerState, IDisposable
{
	public StructureInitializer(IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
	}

	public abstract IObservable<bool> Initialize(T param);
}
public abstract class StructureInitializer<T, T2> : AbstractInitializerState, IStructureInitializer<T, T2>, IInitializerState, IDisposable
{
	public StructureInitializer(IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
	}

	public abstract IObservable<bool> Initialize(T param, T2 param2);
}
public abstract class StructureInitializer<T, T2, T3> : AbstractInitializerState, IStructureInitializer<T, T2, T3>, IInitializerState, IDisposable
{
	public StructureInitializer(IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
	}

	public abstract IObservable<bool> Initialize(T param, T2 param2, T3 param3);
}
