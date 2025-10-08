using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class ConcreteObjectiveBaseFactory : IFactory<TaskMapper, int, ContentType, IObjective>, IFactory
{
	protected readonly TaskObjectiveIcons _objectiveIcons;

	protected readonly GameSettings _gameSettings;

	protected readonly ICurrencyProcessor _currencyProcessor;

	protected readonly ISaver _saver;

	protected readonly Regex _objectiveRegex;

	protected ConcreteObjectiveBaseFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, string regexRules)
	{
		_objectiveIcons = objectiveIcons;
		_gameSettings = gameSettings;
		_currencyProcessor = currencyProcessor;
		_saver = saver;
		_objectiveRegex = new Regex(regexRules);
	}

	public abstract IObjective Create(TaskMapper mapper, int index, ContentType contentType);

	protected SavableObjectiveData CreateGainData(int id, int reqValue)
	{
		return new SavableObjectiveData(id, reqValue);
	}
}
