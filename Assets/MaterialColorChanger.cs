using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialColorChanger : MonoBehaviour
{
    [System.Serializable]
    private struct EnableKeyword
    {
        [Header("キーワードを有効にするマテリアル")]
        public int      materialIndex;

        [Header("有効にするキーワード")]
        public string[] enableKeyword;
    }

    [System.Serializable]
    private struct ColorList
    {
        [Header("適用するマテリアル番号")]
        public int    materialIndex;

        [Header("適用するパラメータ名")]
        public string propertyName;

        [Header("色の強さ(Emissionなど)")]
        public float  power;

        [Header("色")]
        public Color  color;
    }

    [SerializeField]
    private EnableKeyword[] enableKeyword;

    [SerializeField]
    private ColorList[]     colorList;

    void Awake()
    {

    }

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        foreach (EnableKeyword enableKeyword in enableKeyword)
        {
            Material mat = renderer.materials[enableKeyword.materialIndex];

            foreach (string keyword in enableKeyword.enableKeyword)
            {
                if (keyword == "") { continue; }
                mat.EnableKeyword(keyword);
            }
        }

        foreach (ColorList colorList in colorList)
        {
            Material mat = renderer.materials[colorList.materialIndex];
            mat.SetColor(colorList.propertyName, colorList.color * colorList.power);
        }
    }

    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        
    }
}
