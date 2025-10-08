using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GreenT.Types;

public class RepeatProtectionInfiniteEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
	private IList<T> dataSet;

	private Queue<T> queue = new Queue<T>();

	private Random rand = new Random();

	public int NonRepeatingCount { get; }

	public T Current { get; private set; }

	object IEnumerator.Current => Current;

	public RepeatProtectionInfiniteEnumerator(IList<T> list, int nonrepeatingCount)
	{
		rand = new Random((int)DateTime.Now.Ticks);
		dataSet = list;
		NonRepeatingCount = nonrepeatingCount;
		PresetQueue();
	}

	public bool MoveNext()
	{
		if (!queue.Any())
		{
			return false;
		}
		Current = queue.Dequeue();
		dataSet.Add(Current);
		EnqueRandom();
		return true;
	}

	public void Reset()
	{
		while (queue.Any())
		{
			T item = queue.Dequeue();
			dataSet.Add(item);
		}
		PresetQueue();
	}

	private void PresetQueue()
	{
		for (int i = 0; i != NonRepeatingCount; i++)
		{
			if (!dataSet.Any())
			{
				break;
			}
			EnqueRandom();
		}
	}

	private void EnqueRandom()
	{
		int index = rand.Next(dataSet.Count);
		T item = dataSet[index];
		queue.Enqueue(item);
		dataSet.RemoveAt(index);
	}

	public void Dispose()
	{
	}
}
