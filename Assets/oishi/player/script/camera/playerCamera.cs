using UnityEngine;
using System;
using UniRx;

public class playerCamera : MonoBehaviour
{
    [Tooltip("注目するオブジェクト")]
    [SerializeField]
    Transform target;

    [Header("Playerとの距離")]
    public float distanceFromPlayer = 3f;

    float desiredRotateX = 0;
    float desiredRotateY = 0;
    float currentRotateX = 0;
    float currentRotateY = 0;

    [Tooltip("Y軸周りのカメラ感度")]
    public float mouseSpeedX = 1f;
    [Tooltip("X軸周りのカメラ感度")]
    public float mouseSpeedY = 1f;

    [Header("ヌルヌル度")]
    public float angleSmoothing = 10f;
    [Header("ターゲット追尾のヌルヌル度")]
    public float moveSmooth = 50f;

    Vector3 pivotPosition;

    //[Tooltip("ターゲットとの距離")]
    //public float distance = 1.0f;

    [Tooltip("オフセット")]
    public Vector3 offset;

    [Tooltip("カメラの上下回転の限界")]
    float cameraLimitUp = 30f;

    [Tooltip("カメラの上下回転の限界")]
    float cameraLimitDown = -30f;

    CursorLockMode mode = CursorLockMode.None;

    Texture m_cursor;
    Vector3 m_position;

    //[Header("カメラの感度")]
    //public float mouseSpeed = 3f;
    private void Reset()
    {
        target = GameObject.FindWithTag("Player").transform;
    }
    public void Start()
    {
        pivotPosition = target.position;

        LockCursor();

        //m_cursor = GetComponent<GUITexture>().texture;

        //transform.forward = target.forward;
        //transform.position = transform.forward * distanceFromPlayer + offset;
    }

    void LateUpdate()
    {
        m_position = target.transform.position + offset;

        pivotPosition = Vector3.Lerp(
            pivotPosition, target.position, moveSmooth * Time.deltaTime);

        desiredRotateY -= Input.GetAxis("Horizontal2") * mouseSpeedX;
        desiredRotateX -= Input.GetAxis("Vertical2") * mouseSpeedY;

        desiredRotateX = Mathf.Clamp(desiredRotateX, cameraLimitDown, cameraLimitUp);

        currentRotateX = Mathf.Lerp(
            currentRotateX, desiredRotateX, angleSmoothing * Time.deltaTime);
        currentRotateY = Mathf.Lerp(
            currentRotateY, desiredRotateY, angleSmoothing * Time.deltaTime);

        transform.position = pivotPosition
            + Quaternion.Euler(currentRotateX, currentRotateY, 0) * Vector3.forward
            * distanceFromPlayer + offset;

        transform.LookAt(m_position);

        ChangeCursorState();

        //カメラの後方にRayを飛ばし近くのオブジェクトに当たったらその位置に移動
        //RaycastHit hit;
        //if(Physics.Raycast(target.transform.position,-transform.forward,
        //    out hit,distanceFromPlayer))
        //{
        //    transform.position = hit.point;
        //}

        //FixedAngle();

        //Ray ray = new Ray()
        //{
        //    origin = target.position + offset,
        //    direction = -transform.forward
        //};

        //RaycastHit hitInfo;
        //bool isHit = Physics.Raycast(ray, out hitInfo, distance, playersLayerMask.IgnorePlayerAndRopes);

        //if (isHit)
        //{
        //    transform.position = hitInfo.point;
        //}
        //else
        //{
        //    Vector3 position = target.position;       //初期化
        //    position -= transform.forward * distance; //ターゲットの後ろに下がって見やすいように
        //    position += offset;                       //オフセット値

        //    //座標の変更
        //    transform.position = position;
        //}
    }

    void ChangeCursorState()
    {
        //EscapeだけだとEscapeを押したときに表示しっぱなしになる
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnLockCursor();
        }

        Cursor.lockState = mode;
    }

    void LockCursor()
    {
        mode = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnLockCursor()
    {
        mode = CursorLockMode.None;
        Cursor.visible = true;
    }

    //void OnGUI()
    //{
    //    GUI.DrawTexture(new Rect(Screen.width / 2 - 128, Screen.height / 2 - 128,
    //        m_cursor.width, m_cursor.width), m_cursor);
    //}

    //void FixedAngle()
    //{
    //    Vector3 angle = transform.eulerAngles;

    //    if (Input.GetButtonDown("ResetCamera"))
    //    {
    //        angle.x = 0;
    //    }
    //    else if (angle.x >= 180)
    //    {
    //        //angleは取得時に0～360の値になるため
    //        angle.x -= 360;
    //    }
    //    //上限値・下限値を設定してカメラが変な挙動をしないように
    //    angle.x = Mathf.Clamp(angle.x, cameraLimitDown, cameraLimitUp);
    //    angle.z = 0;
    //    transform.eulerAngles = angle;
    //}

    public Vector2 GetCameraRotate()
    {
        return new Vector2(currentRotateX, currentRotateY);
    }
}