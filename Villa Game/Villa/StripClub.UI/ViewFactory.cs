using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class ViewFactory<TSource, TResult> : IFactory<TSource, TResult>, IFactory where TResult : Object, IView<TSource>
{
	protected readonly DiContainer diContainer;

	protected readonly Transform objectContainer;

	private readonly TResult prefab;

	public ViewFactory(DiContainer diContainer, Transform objectContainer, TResult prefab)
	{
		this.diContainer = diContainer;
		this.objectContainer = objectContainer;
		this.prefab = prefab;
	}

	public virtual TResult Create(TSource option)
	{
		TResult val = diContainer.InstantiatePrefabForComponent<TResult>(prefab, objectContainer);
		val.Set(option);
		return val;
	}
}
