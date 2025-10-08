using System;
using GreenT.HornyScapes.External.StripClub._Scripts.NewEventScripts;
using GreenT.Types;
using JetBrains.Annotations;
using Merge;
using StripClub.NewEvent.Data;
using UniRx;

namespace GreenT.HornyScapes.MergeCore;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class MergeFieldProvider
{
	private readonly EventProvider _provider;

	private readonly EventStateService _stateService;

	private readonly MergeFieldRepository mergeFieldRepository;

	private MergeField coreField;

	private readonly Subject<MergeField> onNew = new Subject<MergeField>();

	public IObservable<MergeField> OnNew => (IObservable<MergeField>)onNew;

	public MergeFieldProvider(EventProvider provider, EventStateService stateService)
	{
		_provider = provider;
		_stateService = stateService;
		mergeFieldRepository = new MergeFieldRepository();
	}

	public bool TryGetData(ContentType type, out MergeField field)
	{
		if (type != ContentType.Event)
		{
			return mergeFieldRepository.TryGetMain(out field);
		}
		return TryGetEventData(out field);
	}

	private bool TryGetEventData(out MergeField field)
	{
		field = null;
		if (!_stateService.HaveActiveEvent)
		{
			return false;
		}
		field = _provider.CurrentCalendarProperty.Value.Item2.Data.MergeField;
		return field != null;
	}

	public bool TryAdd(MergeField mergeField)
	{
		bool num = mergeFieldRepository.TryAdd(mergeField);
		if (num)
		{
			onNew.OnNext(mergeField);
		}
		return num;
	}

	public void InvokeSaveAllItems()
	{
		mergeFieldRepository.SaveAllFields();
	}

	public bool Contains(MergeField field)
	{
		return mergeFieldRepository.Contains(field);
	}

	public bool ContainsOneOrMoreFields(ContentType type)
	{
		return mergeFieldRepository.ContainsOneOrMoreFields(type);
	}

	public bool TryRemove(MergeField field)
	{
		return mergeFieldRepository.TryRemove(field);
	}

	public bool TryGetFieldWithItem(GameItem findGameItem, out MergeField data)
	{
		return mergeFieldRepository.TryGetFieldWithItem(findGameItem, out data);
	}

	public void Purge()
	{
		mergeFieldRepository.Purge();
	}

	public void Preload(FieldMonoMediatorCase fieldMediators)
	{
		mergeFieldRepository.Preload(fieldMediators);
	}
}
