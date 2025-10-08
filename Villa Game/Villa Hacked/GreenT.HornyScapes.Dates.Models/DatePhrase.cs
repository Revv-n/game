using GreenT.HornyScapes.Stories;

namespace GreenT.HornyScapes.Dates.Models;

public class DatePhrase : Phrase
{
	public int ID { get; }

	public string[] BackgroundIds { get; }

	public DateBackgroundData[] BackgroundDatas { get; private set; }

	public bool IsFade { get; }

	public bool IsChangingAfterEnd { get; }

	public string[] BackgroundSounds { get; }

	public string[] SoundEffects { get; }

	public DatePhrase(int id, int stepId, bool isFade, int[,] charactersVisible, int characterId, string[] backgroundIds, string[] backgroundSounds, string[] soundEffects, bool isChangingAfterEnd, string text, string name)
		: base(stepId, charactersVisible, characterId, text, name)
	{
		ID = id;
		BackgroundIds = backgroundIds;
		IsFade = isFade;
		IsChangingAfterEnd = isChangingAfterEnd;
		SoundEffects = soundEffects;
		BackgroundSounds = backgroundSounds;
	}

	public void SetBackgroundDatas(DateBackgroundData[] backgroundDatas)
	{
		BackgroundDatas = backgroundDatas;
	}
}
