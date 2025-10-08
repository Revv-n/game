using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace StripClub.UI;

public abstract class AbstractPool<T2> : MonoBehaviour where T2 : MonoBehaviour
{
	private IFactory<T2> factory;

	private readonly Stack<T2> instances = new Stack<T2>();

	[Inject]
	public void Init(IFactory<T2> factory)
	{
		this.factory = factory;
	}

	public T2 GetInstance()
	{
		if (instances.Count <= 0)
		{
			return CreateInstance();
		}
		return instances.Pop();
	}

	public List<T2> CreateList(int lenght)
	{
		List<T2> list = new List<T2>();
		for (int i = 0; i < lenght; i++)
		{
			list.Add(GetInstance());
		}
		return list;
	}

	public T2 GetInstance(List<T2> collection)
	{
		T2 instance = GetInstance();
		collection.Add(instance);
		return instance;
	}

	public void Return(T2 obj)
	{
		if ((bool)obj)
		{
			obj.gameObject.SetActive(value: false);
			if (!instances.Contains(obj))
			{
				instances.Push(obj);
			}
		}
	}

	protected T2 CreateInstance()
	{
		T2 val = factory.Create();
		val.transform.SetParent(base.transform);
		return val;
	}
}
