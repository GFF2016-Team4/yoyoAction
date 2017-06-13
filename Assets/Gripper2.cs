using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gripper2 : MonoBehaviour
{
	public enum State
	{
		None,
		Shoot,          //ロープが射出中状態,
		NoSimulate,     //ロープが動かない状態,
		TarzanMove,     //ロープが動く状態(ターザン移動),
		RailMove,       //ロープが動く状態(レール移動),
		CircleMove,     //ロープが動く状態(円運動),
		TakeUp          //ロープが巻き取り状態,
	}

	[SerializeField]
	private float shootSpeed = 1;

	[SerializeField]
	private float takeupSpeed = 1;

	[SerializeField]
	private float railMoveSpeed = 1.0f;

	private RopeSimulate simulate;
	private GameObject   ropeInst;
	private Transform    ropeOrigin;
	private State state = State.None;

	public delegate void Callback(Collider collider);
	public Callback callback;

	static GameObject RopePrefab;
	static GameObject GripperPrefab;

	//発射時に使う変数
	private LineRenderer lineRenderer;
	private Vector3      shootDir;
	private float        shootDistance;
	private Vector3?     hitPos;

	//ロープ全般
	private Vector3?     syncPosition;

	//レール移動時に使う変数
	private RailNode     nowMoveRailNode;

	// ----- ----- ----- -----
	//定数
	// ----- ----- ----- -----
	private int   TAIL_INDEX = 0;
	private int ORIGIN_INDEX = 1;

	/// <summary> ロープ末尾の座標を取得します </summary>
	public Vector3 FetchTailPosition()
	{
		if (state == State.Shoot ||
			state == State.TakeUp)
		{
			return lineRenderer.GetPosition(TAIL_INDEX);
		}

		return simulate.tailPosition;
	}

	/// <summary> ロープ末尾の座標を格納します </summary>
	public void ApplyTailPosition(Vector3 position)
	{
		syncPosition = position;
	}

	void Update()
    {
		switch (state)
		{
			case State.Shoot:
				Shoot();
				break;

			case State.NoSimulate:
				simulate.SimulationStop();
				break;

			case State.TarzanMove:
				simulate.SimulationStart();
				break;

			case State.RailMove:
				RailMove();
				break;

			case State.CircleMove:
				simulate.SimulationStart();
				break;

			case State.TakeUp:
				TakeUp();
				break;
		}
	}

	private void LateUpdate()
	{
		if (!syncPosition.HasValue) return;

		if (state == State.Shoot ||
			state == State.TakeUp)
		{
			lineRenderer.SetPosition(TAIL_INDEX,   syncPosition.Value);
			lineRenderer.SetPosition(ORIGIN_INDEX, transform.position);

			return;
		}

		simulate.tailPosition = syncPosition.Value;
		syncPosition = null;
	}

	public void ChangeState(State state)
	{
		this.state = state;
	}

	private static void LoadResources()
	{
		if (GripperPrefab == null)
		{
			GripperPrefab = Resources.Load<GameObject>("Gripper");
		}
		if (RopePrefab    == null)
		{
			RopePrefab    = Resources.Load<GameObject>("Rope");
		}
	}

	private void CreateRope(Vector3 origin, Vector3 tail)
	{
		if (ropeInst != null)
		{
			Debug.LogError("既にロープは生成されています");
			return;
		}

		ropeInst = Instantiate(RopePrefab);
		simulate = ropeInst.GetComponent<RopeSimulate>();

		simulate.InitPosition(origin, tail);

		//最初は動かないようにしておく
		simulate.SimulationStop();
	}

	#region 射出関係

	//オブジェクトの生成
	public static Gripper2 Shoot(Vector3 origin, Vector3 direction, float distance)
	{
		Debug.Assert(!direction.IsZero(), "引数 directionにゼロベクトルを指定することはできません");

		//Awakeだと読み込んでいないまま使われるケースがあるため
		LoadResources();

		GameObject inst = Instantiate(GripperPrefab, origin, Quaternion.identity);
		var gripper = inst.GetComponent<Gripper2>();
		gripper.ShootInitialize(direction, distance);
		return gripper;
	}

	//射出の初期化
	public void ShootInitialize(Vector3 direction, float distace)
	{
		shootDir      = direction.normalized;
		shootDistance = distace;
		state         = State.Shoot;

		lineRenderer = GetComponent<LineRenderer>();

		lineRenderer.positionCount = 2; //originとtailの２つ

		//初期値をいれておかないと変になる可能性
		lineRenderer.SetPosition(  TAIL_INDEX, transform.position);
		lineRenderer.SetPosition(ORIGIN_INDEX, transform.position);
		transform.forward = shootDir;

		GetComponent<Rigidbody>().AddForce(transform.forward * shootSpeed, ForceMode.Impulse);
	}

	//射出中
	private void Shoot()
	{
		//if (hitPos.HasValue &&
		//	Vector3.Distance(hitPos.Value, transform.position) < shootSpeed+0.5f)
		//{
		//	transform.position = hitPos.Value;
		//}

		//transform.position += shootDir * shootSpeed * Time.deltaTime;

		//transform.position += ;

		Vector3 originPos = transform.position;
		Vector3 tailPos = lineRenderer.GetPosition(TAIL_INDEX);

		transform.rotation = Quaternion.LookRotation(shootDir);

		if (Vector3.Distance(originPos, tailPos) > shootDistance)
		{
			state = State.TakeUp;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(state);

		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().Sleep();

		if (state == State.Shoot)
		{
			Vector3 origin = lineRenderer.GetPosition(ORIGIN_INDEX);
			Vector3 tail   = lineRenderer.GetPosition(  TAIL_INDEX);
			callback(other);

			if (state != State.TakeUp)
			{
				//ここで初めてロープを生成する

				CreateRope(origin, tail);
				lineRenderer.enabled = false;
			}
		}
	}

	#endregion //射出関係

	#region 巻き取り関係

	public void GripperEnd()
	{
		if (simulate == null) return;
		simulate.SimulationStop();
		Destroy(simulate);
		state = State.TakeUp;
	}


	private void TakeUp()
	{
		Vector3 tailPos   = lineRenderer.GetPosition(TAIL_INDEX);
		Vector3 originPos = Vector3.MoveTowards(transform.position, tailPos, takeupSpeed*Time.deltaTime);
		lineRenderer.SetPosition(ORIGIN_INDEX, originPos);
		transform.position = originPos;

		if (Vector3.Distance(tailPos, originPos) < 0.05f)
		{
			Destroy(gameObject);
		}
	}

	#endregion //巻き取り関係

	#region レール移動関係

	public void SetRailNode(RailNode node)
	{
		nowMoveRailNode = node;
	}

	private void RailMove()
	{
		Debug.Assert(nowMoveRailNode != null, "不正な値です");

		simulate.SimulationStart();
		simulate.ReCalcDistance(5);

		Vector3 originPos = transform.position;
		Vector3 move = Vector3.MoveTowards(originPos, nowMoveRailNode.endPos, railMoveSpeed*Time.deltaTime);

		simulate.originPosition = move;
		transform.position      = move;

		if (Vector3.Distance(move, nowMoveRailNode.endPos) < 0.05f)
		{
			if (nowMoveRailNode.isEndRail)
			{
				//Destroy(ropeInst);
				//lineRenderer.enabled = true;
				//state = State.TakeUp;
			}
			else
			{
				nowMoveRailNode = nowMoveRailNode.next;
			}
		}
		//simulate.originPosition
	}

	#endregion //レール移動関係
}
