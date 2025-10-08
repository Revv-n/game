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
		return Observable.ToArray<Dialogue>(Observable.Select<DialogueConfigMapper, Dialogue>(Observable.SelectMany<DialogueConfigMapper[], DialogueConfigMapper>(Observable.Select<IEnumerable<DialogueConfigMapper>, DialogueConfigMapper[]>(loader.Load(), (Func<IEnumerable<DialogueConfigMapper>, DialogueConfigMapper[]>)((IEnumerable<DialogueConfigMapper> _dialogueMapperSet) => _dialogueMapperSet.Where((DialogueConfigMapper _dialogueMapper) => dialogueIDCollection.Any((int _id) => _id.Equals(_dialogueMapper.ID))).ToArray())), (Func<DialogueConfigMapper[], IEnumerable<DialogueConfigMapper>>)((DialogueConfigMapper[] x) => x)), (Func<DialogueConfigMapper, Dialogue>)dialogueFactory.Create));
	}
}
