using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RailNode : MonoBehaviour
{
	public RailNode next;
	public RailNode prev;

	public Vector3 startPos { get; private set; }
	public Vector3 endPos   { get; private set; }

	public Vector3 moveDir     => transform.forward;
	public bool    isStartRail => prev == null;
	public bool    isEndRail   => next == null;

	private void Awake()
	{
		startPos = transform.position - transform.forward * transform.localScale.z/2;
		endPos   = transform.position + transform.forward * transform.localScale.z/2;
	}

	private void OnDrawGizmos()
	{
		Vector3 offset = Vector3.up;
		Gizmos.DrawLine(startPos+offset, endPos+offset);
	}
}
