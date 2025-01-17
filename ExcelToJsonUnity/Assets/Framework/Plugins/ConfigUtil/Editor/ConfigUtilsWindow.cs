using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ConfigPacking.Editor
{

    /****************************************************
        文件：ConfigUtils.cs
        作者：Danny
        日期：#CreateTime#
        功能：Nothing
    *****************************************************/

    public class ConfigUtilsWindow : EditorWindow
    {
        string _excelPath;
        string _loPath;
        string _jsonPath;

        static Type[] DockedWindowTypes =
        {
            typeof(ConfigUtilsWindow),
        };

        static Vector2 _windowSize = new Vector2(600, 200);

        [MenuItem("GameUtils/Config Builder", false, 102)]
        public static void OpenWindow()
        {
            ConfigUtilsWindow window = GetWindow<ConfigUtilsWindow>("配置构建工具", true, DockedWindowTypes);
            window.minSize = window.maxSize = _windowSize;
        }

        private void Awake()
        {
            _excelPath = PlayerPrefs.GetString(nameof(_excelPath), "D:\\svn\\metarBox\\Config-MetarBox-Trunk");
            _loPath = PlayerPrefs.GetString(nameof(_loPath), Application.dataPath + "\\GameScript\\Lo");
            _jsonPath = PlayerPrefs.GetString(nameof(_jsonPath), Application.dataPath + "\\GameRes\\Config\\");
        }

        void OnEnable()
        {
            ConfigPackingManager.messageCall += Log;
        }

        void OnDisable()
        {
            ConfigPackingManager.messageCall -= Log;
        }

        private void OnGUI()
        {
            DrawInput("配置表路径:", ref _excelPath, 30);
            DrawInput("Lo输出路径:", ref _loPath, 60);
            DrawInput("json输出路径:", ref _jsonPath, 90);

            var btnRect = new Rect(50, 140, 100, 30);

            if (GUI.Button(btnRect, "保存"))
            {
                PlayerPrefs.SetString(nameof(_excelPath), _excelPath);
                PlayerPrefs.SetString(nameof(_loPath), _loPath);
                PlayerPrefs.SetString(nameof(_jsonPath), _jsonPath);

                Log("ConfigUtils 保存成功");
            }

            if (GUI.Button(new Rect(btnRect.x + (btnRect.x + btnRect.width) * 1, 140, btnRect.width, btnRect.height), "导出Lo"))
            {
                if (string.IsNullOrEmpty(_excelPath))
                {
                    Debug.LogError("表格路径不能为空!");
                }
                else if (string.IsNullOrEmpty(_loPath))
                {
                    Debug.LogError("Json输出路径不能为空!");
                }
                else
                {
                    ConfigPackingManager.ExprotLO(_excelPath, _loPath);
                }
            }

            if (GUI.Button(new Rect(btnRect.x + (btnRect.x + btnRect.width) * 2, 140, btnRect.width, btnRect.height), "导出Json"))
            {
                if (string.IsNullOrEmpty(_excelPath))
                {
                    Debug.LogError("表格路径不能为空!");
                }
                else if (string.IsNullOrEmpty(_jsonPath))
                {
                    Debug.LogError("Json输出路径不能为空!");
                    return;
                }

                ConfigPackingManager.ExprotLua(_excelPath, _jsonPath);
            }
        }

        void DrawInput(string desc, ref string inputText, float y)
        {
            Rect labelRect = new Rect(10, y, 100, 20);
            GUI.Label(labelRect, desc);

            Rect fieldRect = new Rect(labelRect.x + labelRect.width, y, 480, 20);
            inputText = GUI.TextField(fieldRect, inputText);
        }

        void Log(string text)
        {
            Debug.Log(text);
        }
    }
}