using System;

[Serializable]
public class WeightNode<T> : IHasWeight
{
	public T value;

	public float weight;

	public float Weight => weight;

	public WeightNode(T value, float weight)
	{
		this.value = value;
		this.weight = weight;
	}
}
