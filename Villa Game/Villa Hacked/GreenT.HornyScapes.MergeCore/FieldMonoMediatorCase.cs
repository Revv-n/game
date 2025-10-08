using System;
using GreenT.Types;

namespace GreenT.HornyScapes.MergeCore;

[Serializable]
public struct FieldMonoMediatorCase
{
	public FieldMonoMediator MainFieldMonoMediator;

	public FieldMonoMediator EventFieldMonoMediator;

	public FieldMonoMediator Get(ContentType contentType)
	{
		if (contentType != 0)
		{
			return EventFieldMonoMediator;
		}
		return MainFieldMonoMediator;
	}
}
