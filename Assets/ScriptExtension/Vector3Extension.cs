using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Vector3Extension
{
	public static bool IsZero(this Vector3 self)
	{
		return self.x == 0.0f &&
			   self.y == 0.0f &&
			   self.z == 0.0f;
	}
}
