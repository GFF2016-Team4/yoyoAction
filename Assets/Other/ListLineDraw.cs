using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ListLineDraw : MonoBehaviour
{
    [SerializeField, Tooltip("登録されているTransformを表示\n デバッグ用")]
    List<Transform> drawList;

    [SerializeField]
    uint maxLine = 1000;

    //使いまわし
    Vector3[] linePositions;

    LineRenderer lineRenderer;

    [SerializeField]
    private bool isDraw;

    public int count
    {
        get { return drawList.Count; }
    }

    void Awake()
    {
        lineRenderer  = GetComponent<LineRenderer>();
        linePositions = new Vector3[maxLine+1];
    }

    void Start()
    {
        FixVertexCount();
    }
    
    /// <summary>
    /// <para>描画リストにセットします </para>
    /// <para>注意:この関数は登録されているリストに上書きされます</para>
    /// </summary>
    /// <param name="setList">登録するオブジェクトリスト</param>
    public void SetDrawList(List<Transform> setList)
    {
        drawList = setList;
        FixVertexCount();
    }
    
    /// <summary>
    /// 描画リストに追加します
    /// </summary>
    /// <param name="addTransform">追加するオブジェクト</param>
    public void AddDrawList(Transform addTransform)
    {
        drawList.Add(addTransform);
        Debug.Assert(linePositions.Length <= drawList.Count);
        FixVertexCount();
    }

    /// <summary>
    /// 描画リストに追加します
    /// </summary>
    /// <param name="AddList">追加するオブジェクトリスト</param>
    public void AddDrawList(List<Transform> AddList)
    {
        drawList.AddRange(AddList);
        Debug.Assert(linePositions.Length <= drawList.Count);
        FixVertexCount();
    }

    public void Insert(int index, Transform trans)
    {
        drawList.Insert(index, trans);
        Debug.Assert(linePositions.Length <= drawList.Count);
        FixVertexCount();
    }

    /// <summary>
    /// 描画リストから削除をします
    /// </summary>
    /// <param name="removeTransform">削除するオブジェクト</param>
    public void RemoveDrawList(Transform removeTransform)
    {
        drawList.Remove(removeTransform);
        FixVertexCount();
    }

    /// <summary>
    /// 描画リストから削除をします
    /// </summary>
    /// <param name="removeList">削除をするオブジェクトリスト</param>
    public void RemoveDrawList(List<Transform> removeList)
    {
        for(int i = 0; i < drawList.Count; i++)
        {
            for(int j = 0; j < removeList.Count; j++)
            {
                if(drawList[i] != removeList[j]) continue;
                drawList  .Remove(removeList[j]);
                removeList.Remove(removeList[j]);
                break;
            }
        }
        FixVertexCount();
    }

    /// <summary>
    /// 描画を開始します
    /// </summary>
    public void DrawStart()
    {
        isDraw = true;
        lineRenderer.enabled = true;
        FixVertexCount();
    }

    /// <summary>
    /// 描画を終了します(DrawStartで再開)
    /// </summary>
    public void DrawEnd()
    {
        //停止
        lineRenderer.positionCount = 0;

        isDraw = false;
        lineRenderer.enabled = false;
    }

    private void FixVertexCount()
    {
        lineRenderer.positionCount = drawList.Count;
        Draw();
    }

    void Update()
    {
        Draw();
    }
    
    void Draw()
    {
        if(!isDraw) return;
        
        //アクティブだったオブジェクトの最後尾
        Transform lastActiveChild = transform;

        //linePositionsのカウント
        int count = 0;

        for(int i = 0; i < drawList.Count; i++)
        {
            Transform child = drawList[i];

            if(!child.gameObject.activeSelf) continue;
            //追加
            linePositions[count] = child.position;
            lastActiveChild      = child;
            count++;
        }

        for(;count < drawList.Count; count++)
        {
            //終点をいれて変な線が描画されないように
            linePositions[count] = lastActiveChild.position;
        }

        lineRenderer.SetPositions(linePositions);
    }
}