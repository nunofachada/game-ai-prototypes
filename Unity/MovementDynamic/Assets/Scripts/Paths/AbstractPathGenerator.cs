using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPathGenerator : MonoBehaviour {

	public abstract IList<Vector2> GeneratePath();
}
