using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RailNode : MonoBehaviour
{
	public RailNode next;
	public RailNode prev;

	private Vector3 startPos;
	private Vector3 endPos;

	private void Awake()
	{
		Vector3 pos = new Vector3()
		{
			x = 0.0f,
			y = 0.0f,
			z = transform.localScale.z/2
		};
		startPos = transform.position - pos;
		endPos   = transform.position + pos;
	}

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

	Vector3 StartRailPos()
	{
		return new Vector3();

		//return transform.position + new Vector3
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		Vector3 offset = new Vector3(0, 1, 0);
		Gizmos.DrawLine(startPos+offset, endPos+offset);
	}
}
