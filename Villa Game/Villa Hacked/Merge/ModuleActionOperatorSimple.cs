using System;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace Merge;

public abstract class ModuleActionOperatorSimple : ModuleActionOperator
{
	[SerializeField]
	protected ModuleActionBlock block;

	[Inject]
	protected ICurrencyProcessor CurrencyProcessor;

	public override event Action<ModuleActionOperator> OnAction;

	protected virtual void Start()
	{
		block.Button.AddClickCallback(AtButtonClick);
		Init();
	}

	protected virtual void AtButtonClick()
	{
		OnAction?.Invoke(this);
	}

	protected virtual void SetState(int price)
	{
		block.StateView.SetValueColor((!CurrencyProcessor.IsEnough(CurrencyType.Hard, price)) ? 1 : 0);
	}

	protected virtual void Init()
	{
	}
}
