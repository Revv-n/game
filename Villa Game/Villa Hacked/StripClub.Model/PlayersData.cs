using System;
using GreenT.Data;
using GreenT.HornyScapes;
using StripClub.Extensions;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace StripClub.Model;

[Serializable]
public class PlayersData : IPlayerBasics, ISavableState
{
	private readonly PlayerExperience playerExperience;

	private readonly IFactory<Currencies> balanceFactory;

	private readonly IFactory<RestorableValue<int>> energyFactory;

	private readonly IFactory<RestorableValue<int>> messagesFactory;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Currencies Balance { get; private set; }

	public ReactiveProperty<int> Level { get; private set; }

	public RestorableValue<int> Energy { get; private set; }

	public RestorableValue<int> Replies { get; private set; }

	public PlayerExperience Experience => playerExperience;

	string ISavableState.UniqueKey()
	{
		return "PlayersData";
	}

	public PlayersData(PlayerExperience playerExperience, IFactory<Currencies> balanceFactory, [Inject(Id = "Energy")] IFactory<RestorableValue<int>> energyFactory, [Inject(Id = "Messages")] IFactory<RestorableValue<int>> messagesFactory)
	{
		this.playerExperience = playerExperience;
		this.balanceFactory = balanceFactory;
		this.energyFactory = energyFactory;
		this.messagesFactory = messagesFactory;
		Level = new ReactiveProperty<int>();
	}

	public void Init()
	{
		if (Energy != null)
		{
			Energy.OnValueChanged -= OnEnergyValueChanged;
		}
		Balance = balanceFactory.Create();
		Energy = energyFactory.Create();
		Replies = messagesFactory.Create();
		OnEnergyValueChanged(Energy.Value);
		Energy.OnValueChanged += OnEnergyValueChanged;
		void OnEnergyValueChanged(int value)
		{
			Balance.Get(CurrencyType.Energy).Value = value;
		}
	}

	Memento ISavableState.SaveState()
	{
		return new PlayerBasicsMemento(this);
	}

	void ISavableState.LoadState(Memento memento)
	{
		PlayerBasicsMemento playerBasicsMemento = (PlayerBasicsMemento)memento;
		Level.Value = playerBasicsMemento.level;
	}

	public ReactiveProperty<int> GetCurrency(CurrencyType type)
	{
		return Balance.Get(type);
	}
}
