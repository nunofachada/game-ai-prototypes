using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDefinedPathGenerator : AbstractPathGenerator
{
	[SerializeField]
	private Vector2[] pathPoints;

    public override IList<Vector2> GeneratePath() {
		return pathPoints;
	}
}
