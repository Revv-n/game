using System.Linq;
using StripClub.Stories;
using UnityEngine;

namespace GreenT.HornyScapes.Stories.UI;

public class PhraseView : MonoBehaviour
{
	[SerializeField]
	private PhraseContainer _leftPhraseContainer;

	[SerializeField]
	private PhraseContainer _rightPhraseContainer;

	[SerializeField]
	private float _waitTimeBetweenLaters = 0.1f;

	[SerializeField]
	private SpeakerView[] _leftSpeakers;

	[SerializeField]
	private SpeakerView[] _rightSpeakers;

	private PhraseContainer _lastPhraseContainer;

	public void ApplyPhrase(Phrase phrase)
	{
		if (phrase.CharacterID == 0 && _lastPhraseContainer != null)
		{
			_lastPhraseContainer.ApplyPhrase(phrase, _waitTimeBetweenLaters);
			return;
		}
		bool flag = _leftSpeakers.Any((SpeakerView s) => s.CharacterID == phrase.CharacterID);
		if (flag)
		{
			_leftPhraseContainer.ApplyPhrase(phrase, _waitTimeBetweenLaters);
		}
		else
		{
			_rightPhraseContainer.ApplyPhrase(phrase, _waitTimeBetweenLaters);
		}
		_lastPhraseContainer = (flag ? _leftPhraseContainer : _rightPhraseContainer);
	}

	public void SkipPhraseTyping(Phrase phrase)
	{
		_lastPhraseContainer.SkipTyping(phrase);
	}
}
