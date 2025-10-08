using System;
using UnityEngine;

namespace StripClub.Registration.UI;

[Serializable]
public class ValidationStateSpritesDictionary : SerializableDictionary<AbstractChecker.ValidationState, Sprite>
{
}
