using System;
using UnityEngine;

namespace StripClub.Model.Data;

[Serializable]
public class BioMapper
{
	public int Age;

	public string Occupation;

	[TextArea]
	public string Description;

	public float FeetSize;

	public string BreastSize;

	public float Height;

	public string Likes;

	public string Dislikes;
}
