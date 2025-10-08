using TMPro;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class RealTimeCheatInfo : MonoBehaviour
{
	private LocalClock _clock;

	[SerializeField]
	private TMP_Text _text;

	[Inject]
	public void Construct(IClock clock)
	{
		if (clock is LocalClock clock2)
		{
			_clock = clock2;
		}
	}

	private void Update()
	{
		if (_clock != null)
		{
			_text.text = $"Current      {_clock.GetTime()}\n" + $"Skip  ----> {_clock.skipTime} || GlobalSkip {_clock.GlobalOffset}\n" + $"Real Global {_clock.GetTime() - _clock.skipTime}\n" + $"Real Local  {_clock.GetTime() - _clock.skipTime - _clock.GlobalOffset}";
		}
	}
}
