using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RailNode : MonoBehaviour
{
	public RailNode next;
	public RailNode prev;

	Vector3 MoveDir()
	{
		return transform.forward;
	}

	bool IsEndRail()
	{
		return next == null;
	}

	bool IsStartRail()
	{
		return prev == null;
	}
}
