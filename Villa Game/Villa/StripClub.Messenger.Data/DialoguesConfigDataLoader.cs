using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace StripClub.Messenger.Data;

public class DialoguesConfigDataLoader : ILoader<IEnumerable<int>, IEnumerable<Dialogue>>
{
	private readonly ILoader<IEnumerable<DialogueConfigMapper>> loader;

	private readonly IFactory<DialogueConfigMapper, Dialogue> dialogueFactory;

	public DialoguesConfigDataLoader(ILoader<IEnumerable<DialogueConfigMapper>> loader, IFactory<DialogueConfigMapper, Dialogue> dialogueFactory)
	{
		this.loader = loader;
		this.dialogueFactory = dialogueFactory;
	}

	public IObservable<IEnumerable<Dialogue>> Load(IEnumerable<int> dialogueIDCollection)
	{
		return (from _dialogueMapperSet in loader.Load()
			select _dialogueMapperSet.Where((DialogueConfigMapper _dialogueMapper) => dialogueIDCollection.Any((int _id) => _id.Equals(_dialogueMapper.ID))).ToArray()).SelectMany((DialogueConfigMapper[] x) => x).Select(dialogueFactory.Create).ToArray();
	}
}
