using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

#endif
public class TileEditor : MonoBehaviour
{
    public int index;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(TileEditor))]
public class TileUnityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TileEditor theTarget = (TileEditor)target;
        theTarget.index = EditorGUILayout.IntField("Index", theTarget.index);

        int index = theTarget.index;

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GameManager.Instance.tiles.Length == 0)
            return;
        for (int y = 0; y < GameManager.Instance.gridY[GameManager.Instance.Map]; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < GameManager.Instance.gridX[GameManager.Instance.Map]; x++)
            {
               // EditorGUILayout.LabelField("Tile " + (x + (y * GameManager.Instance.gridY)).ToString(), GUILayout.Height(20f), GUILayout.Width(50f));
                if (x < (GameManager.Instance.gridX[GameManager.Instance.Map] - 1))
                    GameManager.Instance.tiles[index].tiles[(x + (y * GameManager.Instance.gridY[GameManager.Instance.Map]))].rightStat = EditorGUILayout.Toggle("", GameManager.Instance.tiles[index].tiles[(x + (y * GameManager.Instance.gridY[GameManager.Instance.Map]))].rightStat, GUILayout.Height(20f), GUILayout.Width(10f));
                GameManager.Instance.tiles[index].tiles[(x + (y * GameManager.Instance.gridY[GameManager.Instance.Map]))].UpdateWalls();

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < GameManager.Instance.gridX[GameManager.Instance.Map]; x++)
            {
                if (y < (GameManager.Instance.gridY[GameManager.Instance.Map] - 1))
                {
                    GameManager.Instance.tiles[index].tiles[(x + (y * GameManager.Instance.gridY[GameManager.Instance.Map]))].bottomStat = EditorGUILayout.Toggle("", GameManager.Instance.tiles[index].tiles[(x + (y * GameManager.Instance.gridY[GameManager.Instance.Map]))].bottomStat, GUILayout.Height(20f), GUILayout.Width(10f));
                }
                GameManager.Instance.tiles[index].tiles[(x + (y * GameManager.Instance.gridY[GameManager.Instance.Map]))].UpdateWalls();
            }

            EditorGUILayout.EndHorizontal();
        }
        int c = GameManager.Instance.tiles[0].tiles.Length;
        for (int x = 0; x < c; x++)
        {
            GameManager.Instance.tiles[1].tiles[x].rightStat = GameManager.Instance.tiles[0].tiles[x].rightStat;
            GameManager.Instance.tiles[1].tiles[x].bottomStat = GameManager.Instance.tiles[0].tiles[x].bottomStat;
            GameManager.Instance.tiles[1].tiles[x].UpdateWalls();
        }
    }
}

#endif