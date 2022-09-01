using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;

public class MaZeMapJosnShowWinow : EditorWindow
{
    private string MaZeMapJsonFilePath = "";
    string MaZeMapJsonFileName = "";
    OpenFileName m_openfileName = null;
    GameObject m_boxprefab;
    private GameObject mapparent;

    [MenuItem("MyTools/观察物体")]
    public static void ShowWindow()
    {
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(MaZeMapJosnShowWinow));
        editorWindow.titleContent = new GUIContent("观察物体");
    }


    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        MaZeMapJsonFileName = EditorGUILayout.TextField("prefab文件路径", Path.GetFileName(MaZeMapJsonFileName), GUILayout.Width(270), GUILayout.Height(20));
        if (GUILayout.Button("选择prefab文件", GUILayout.Width(130), GUILayout.Height(30)))
        {
            m_openfileName = new OpenFileName();
            m_openfileName.structSize = Marshal.SizeOf(m_openfileName);
            m_openfileName.filter = "prefab文件(*.prefab)\0*.prefab";
            m_openfileName.file = new string(new char[256]);
            m_openfileName.maxFile = m_openfileName.file.Length;
            m_openfileName.fileTitle = new string(new char[64]);
            m_openfileName.maxFileTitle = m_openfileName.fileTitle.Length;
            m_openfileName.initialDir = (Application.dataPath + "/Configs/AlienMapConfig").Replace("/", "\\");//默认打开路径
            m_openfileName.title = "选择prefab文件";
            m_openfileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

            if (LocalDialog.GetSaveFileName(m_openfileName))
            {
                Debug.Log(m_openfileName.file);
                MaZeMapJsonFileName = m_openfileName.fileTitle;
                MaZeMapJsonFilePath = m_openfileName.file;
            }
        }
        GUILayout.EndHorizontal();
        //============================
        if (GUILayout.Button("显示地形", GUILayout.Width(100), GUILayout.Height(50)))
        {
            if (!string.IsNullOrEmpty(MaZeMapJsonFilePath) && MaZeMapJsonFilePath.EndsWith(".prefab"))
            {
                if (m_boxprefab == null)
                    m_boxprefab = AssetDatabase.LoadAssetAtPath<GameObject>(MaZeMapJsonFilePath);     
                
                    GameObject box = GameObject.Instantiate(m_boxprefab);
                    //box.transform.SetParent(mapparent.transform);
                    
                
            }
            else
                Debug.LogError("请检查文件路径:" + MaZeMapJsonFilePath);
        }


    }
}