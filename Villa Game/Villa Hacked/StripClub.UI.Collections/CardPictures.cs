using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Collections;

public class CardPictures : MonoBehaviour
{
	[SerializeField]
	private Image cardImage;

	private ReactiveProperty<int> selected = new ReactiveProperty<int>();

	public IReadOnlyReactiveProperty<int> Selected => (IReadOnlyReactiveProperty<int>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<int>((IObservable<int>)selected);

	public Sprite[] Sprites { get; private set; }

	public void Init(IEnumerable<Sprite> sprites, int currentNumber = 0)
	{
		Sprites = sprites.ToArray();
		Select(currentNumber);
	}

	public void Select(int number)
	{
		cardImage.sprite = Sprites[number];
		selected.Value = number;
	}
}
