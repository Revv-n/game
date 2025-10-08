using System;
using System.Linq;
using GreenT.Model.Collections;
using UniRx;

namespace GreenT.HornyScapes.Meta.Decorations;

public class DecorationManager : SimpleManager<Decoration>
{
	public IObservable<Decoration> OnUpdate { get; }

	public DecorationManager()
	{
		OnUpdate = collection.ToObservable().Merge(onNew).SelectMany((Decoration decoration) => decoration.OnUpdate);
	}

	public Decoration GetItem(int id)
	{
		try
		{
			return collection.First((Decoration _item) => _item.ID == id);
		}
		catch (InvalidOperationException innerException)
		{
			throw innerException.SendException($"{GetType().Name}: There is no {typeof(Decoration)} with id: {id} \n");
		}
	}

	public void Initialize()
	{
		foreach (Decoration item in Collection)
		{
			item.Initialize();
		}
	}
}
