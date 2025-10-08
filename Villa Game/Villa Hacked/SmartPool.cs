using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmartPool<T> where T : Component
{
	private class SmartPullNode
	{
		public T value;

		public bool in_pool;

		public SmartPullNode(T value, bool in_pool)
		{
			this.value = value;
			this.in_pool = in_pool;
		}
	}

	private T prefab;

	private Transform default_parent;

	private List<SmartPullNode> list;

	public int ActiveCount { get; private set; }

	public int TotalCount => list.Count;

	public Transform CurrentParent => default_parent;

	public T CurrentPrefab => prefab;

	public bool UseDefaultTransformValidation { get; set; }

	public List<T> ActiveElements => (from x in list
		where !x.in_pool
		select x.value).ToList();

	public event Action<T> OnItemCreated;

	public event Action<T> OnItemPop;

	public SmartPool<T> ActivateDefaultTransformValidation()
	{
		UseDefaultTransformValidation = true;
		return this;
	}

	public SmartPool(T prefab, Transform parent)
	{
		this.prefab = prefab;
		default_parent = parent;
		list = new List<SmartPullNode>();
	}

	public SmartPool(T prefab)
	{
		this.prefab = prefab;
		list = new List<SmartPullNode>();
	}

	public T Pop()
	{
		SmartPullNode smartPullNode = list.FirstOrDefault((SmartPullNode x) => x.in_pool);
		smartPullNode = smartPullNode ?? ExtendPull();
		smartPullNode.in_pool = false;
		ActiveCount--;
		if (smartPullNode.value is IPoolBeforePopListener)
		{
			(smartPullNode.value as IPoolBeforePopListener).BeforePopFromPool();
		}
		SetItemActive(smartPullNode.value, active: true);
		if (smartPullNode.value is IPoolPopListener)
		{
			(smartPullNode.value as IPoolPopListener).OnPopFromPool();
			this.OnItemPop?.Invoke(smartPullNode.value);
		}
		return smartPullNode.value;
	}

	public T Pop(Transform parent)
	{
		T val = Pop();
		val.transform.SetParent(parent);
		val.transform.localScale = Vector3.one;
		return val;
	}

	public bool Pop(out T result)
	{
		SmartPullNode smartPullNode = list.FirstOrDefault((SmartPullNode x) => x.in_pool);
		bool result2 = smartPullNode == null;
		smartPullNode = smartPullNode ?? ExtendPull();
		smartPullNode.in_pool = false;
		ActiveCount--;
		SetItemActive(smartPullNode.value, active: true);
		if (smartPullNode.value is IPoolPopListener)
		{
			(smartPullNode.value as IPoolPopListener).OnPopFromPool();
			this.OnItemPop?.Invoke(smartPullNode.value);
		}
		result = smartPullNode.value;
		return result2;
	}

	public List<T> Pop(int count, Action<T> initValidation = null)
	{
		List<T> list = new List<T>();
		for (int i = 0; i < count; i++)
		{
			if (Pop(out var result))
			{
				initValidation?.Invoke(result);
			}
			list.Add(result);
		}
		return list;
	}

	public void ReturnItemInPull(T item)
	{
		SmartPullNode smartPullNode = list.FirstOrDefault((SmartPullNode x) => x.value == item && !x.in_pool);
		if (smartPullNode != null)
		{
			if (item is IPoolReturnListener)
			{
				((IPoolReturnListener)item).BeforeReturnInPool();
			}
			smartPullNode.in_pool = true;
			SetItemActive(item, active: false);
			if (default_parent != null)
			{
				item.transform.SetParent(default_parent);
			}
			ActiveCount++;
		}
	}

	public void ReturnAllItems()
	{
		foreach (SmartPullNode item in list.Where((SmartPullNode x) => !x.in_pool))
		{
			ReturnItemInPull(item.value);
		}
	}

	private List<SmartPullNode> ExtendPull(int size)
	{
		List<SmartPullNode> list = new List<SmartPullNode>();
		for (int i = 0; i < size; i++)
		{
			list.Add(ExtendPull());
		}
		return list;
	}

	private SmartPullNode ExtendPull()
	{
		T item = ((prefab is IPoolCreationable poolCreationable) ? ((T)poolCreationable.Create()) : CreateDefault());
		SetItemActive(item, active: false);
		if (item is IPoolReturner poolReturner)
		{
			poolReturner.ReturnInPool = delegate
			{
				ReturnItemInPull(item);
			};
		}
		SmartPullNode smartPullNode = new SmartPullNode(item, in_pool: true);
		list.Add(smartPullNode);
		ActiveCount++;
		this.OnItemCreated?.Invoke(item);
		return smartPullNode;
	}

	private T CreateDefault()
	{
		T val = UnityEngine.Object.Instantiate(prefab);
		val.transform.SetParent(default_parent);
		if (UseDefaultTransformValidation)
		{
			val.transform.localPosition = Vector3.zero;
			val.transform.localScale = Vector3.one;
		}
		return val;
	}

	public void ClearPool()
	{
		if (list.Count != 0 && list[0] is MonoBehaviour)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				UnityEngine.Object.Destroy((list[i] as MonoBehaviour).gameObject);
			}
			list.Clear();
		}
	}

	private void SetItemActive(T item, bool active)
	{
		if (item is IPoolActivatable poolActivatable)
		{
			poolActivatable.SetActiveForPool(active);
		}
		else
		{
			item.gameObject.SetActive(active);
		}
	}
}
