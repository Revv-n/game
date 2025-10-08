using System;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Providers;

[Serializable]
public class DatesSoundtracksInfo
{
	[SerializeField]
	private DatesSoundtracks[] _soundtracks;

	[SerializeField]
	private float _fadeIn;

	[SerializeField]
	private float _fadeOut;

	public DatesSoundtracks[] Soundtracks => _soundtracks;

	public float FadeIn => _fadeIn;

	public float FadeOut => _fadeOut;
}
