using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using StripClub.Messenger;
using StripClub.Messenger.Data;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Messenger.Data;

public class DialogueLockerInitializer : StructureInitializerViaArray2<DialogueConfigMapper, DialogueLocker>
{
	private readonly MapperStructureInitializer<DialogueConfigMapper> dialogueMapperStructureInitializer;

	private IDisposable stream;

	public DialogueLockerInitializer(ICollectionSetter<DialogueLocker> lockerSetter, IFactory<DialogueConfigMapper, DialogueLocker> factory, MapperStructureInitializer<DialogueConfigMapper> dialogueMapperStructureInitializer, IEnumerable<IStructureInitializer> others = null)
		: base(lockerSetter, factory, others)
	{
		this.dialogueMapperStructureInitializer = dialogueMapperStructureInitializer;
	}

	public override IObservable<bool> Initialize(IEnumerable<DialogueConfigMapper> mappers)
	{
		stream?.Dispose();
		stream = ObservableExtensions.Subscribe<bool>(dialogueMapperStructureInitializer.Initialize(mappers));
		return base.Initialize(mappers);
	}
}
