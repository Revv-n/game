using System;
using StripClub.Utility;
using UnityEngine;

namespace StripClub.Model.Data;

[CreateAssetMenu(menuName = "StripClub/Shop/Time/Chained/Node", order = 1)]
public class NodePeriodTime : ChainedPeriodTime, ISerializationCallbackReceiver
{
	[Tooltip("Total period duration")]
	[SerializeField]
	protected string offsetString;

	protected TimeSpan offset;

	[ReadOnly]
	[SerializeField]
	protected HeadPeriodTime head;

	[ReadOnly]
	[SerializeField]
	protected ChainedPeriodTime previous;

	public TimeSpan Offset => offset;

	public ChainedPeriodTime Previous => previous;

	public override DateTime StartTime => Previous.ExpirationTime - Offset;

	public override DateTime ExpirationTime
	{
		get
		{
			DateTime dateTime = StartTime + periodDuration - offset;
			if (!(dateTime < head.ChainExpirationTime))
			{
				return head.ChainExpirationTime;
			}
			return dateTime;
		}
	}

	public override bool IsInclude(DateTime currentTime)
	{
		if (StartTime < currentTime)
		{
			return currentTime < ExpirationTime;
		}
		return false;
	}

	public override void OnValidate()
	{
		Event current = Event.current;
		if (current != null)
		{
			Debug.Log("[ScriptableObject]" + base.name + " " + current.commandName);
		}
		if (current != null && (current.commandName.Equals("Duplicate") || current.commandName.Equals("Paste")))
		{
			head = null;
			previous = null;
			next = null;
		}
		if (base.Next != null && base.Next is NodePeriodTime)
		{
			NodePeriodTime nodePeriodTime = base.Next as NodePeriodTime;
			if (nodePeriodTime.Previous != this)
			{
				nodePeriodTime.SetPrevious(this);
			}
		}
		if (Previous != null && Previous.Next != null && Previous.Next != this)
		{
			Previous.SetNext(this);
		}
		base.OnValidate();
		SaleTime.ValidateTimeSpan(ref offsetString, ref offset);
		SetHead();
		head?.CalcTotalPeriodDuration();
	}

	protected virtual void SetHead()
	{
		if (Previous is NodePeriodTime)
		{
			NodePeriodTime nodePeriodTime = Previous as NodePeriodTime;
			if (head != nodePeriodTime.head)
			{
				head = nodePeriodTime.head;
			}
		}
		else if (Previous is HeadPeriodTime)
		{
			head = Previous as HeadPeriodTime;
		}
		(base.Next as NodePeriodTime)?.SetHead();
	}

	public override void SetNext(ChainedPeriodTime next)
	{
		base.SetNext(next);
		SetHead();
	}

	public virtual void SetPrevious(ChainedPeriodTime previous)
	{
		this.previous = previous;
		SetHead();
	}

	public override void OnBeforeSerialize()
	{
		base.OnBeforeSerialize();
		offsetString = offset.ToString();
	}

	public override void OnAfterDeserialize()
	{
		base.OnAfterDeserialize();
		TimeSpan.TryParse(offsetString, out offset);
	}
}
