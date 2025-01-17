using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using ConfigPacking;

public class MyCanvas : MonoBehaviour
{
    [SerializeField, Tooltip("项目下拉框")]
    Dropdown projectDropdown;

    [SerializeField, Tooltip("名字")]
    InputField nameInput;

    [SerializeField, Tooltip("表格路径")]
    InputField pathInput;

    [SerializeField, Tooltip("json输出路径")]
    InputField jsonInput;

    [SerializeField, Tooltip("lo输出路径")]
    InputField loInput;

    [SerializeField, Tooltip("Log文本")]
    InputField logInput;

    [SerializeField, Tooltip("Log文本")]
    Text logText;

    List<ProjectInfoVO> _infos;
    int _lastIndex = -1;

    static MyCanvas _ins;

    public static MyCanvas Ins
    {
        get { return _ins; }
    }

    private void Awake()
    {
        _ins = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //var dict = ExcelManager.Ins.LoadExcel("E:/GItLib/Dice/Project/Dice/Config/dev");
        //var jsonStr = ExcelManager.Ins.ToJson(dict);
        //Debug.Log(jsonStr);
        //ExcelManager.Ins.ExportFile(jsonStr, "E:/test/GameConfig.json");

        Screen.fullScreen = false;

        InitProjectInfo();
        this.OnValueChanged(lastIndex);
    }

    private void OnEnable()
    {
        ConfigPackingManager.messageCall += AddLog;
    }

    private void OnDisable()
    {
        ConfigPackingManager.messageCall -= AddLog;
    }

    public void SaveProjectInfo()
    {
        var info = new ProjectInfoVO();

        info.name = nameInput.text;
        info.devPath = pathInput.text;
        info.jsonPath = jsonInput.text;
        info.loPath = loInput.text;

        if (string.IsNullOrEmpty(info.name))
        {
            AddLog("项目名称不能为空!");
            return;
        }

        var isAdd = true;
        var infos = this.projectInfos;
        for (int i = 0; i < infos.Count; i++)
        {
            if (info.name == infos[i].name)
            {
                infos[i] = info;
                isAdd = false;
                break;
            }
        }

        if (isAdd)
        {
            infos.Add(info);
        }

        this.projectInfos = infos;
        this.lastIndex = infos.Count - 1;

        this.InitProjectInfo();

        AddLog("保存项目成功!");
    }

    public void DelProjectInfo()
    {
        var infos = this.projectInfos;
        var index = this.lastIndex;

        if (infos.Count == 0 || index >= infos.Count)
        {
            AddLog("项目信息为空,删除失败!");
            return;
        }

        var info = infos[index];

        infos.RemoveAt(index);
        index += 1;
        if (index >= infos.Count)
        {
            index = infos.Count - 1;
        }

        this.projectInfos = infos;
        this.lastIndex = index;

        this.InitProjectInfo();
        AddLog("项目信息\"" + info.name + "\" 已删除");
    }


    string FixedPath(string path)
    {
        string fix = path;
        if (fix.IndexOf("\\") != -1)
        {
            if (fix[fix.Length - 1] != '\\')
            {
                fix += "\\";
            }
        }
        else
        {
            if (fix[fix.Length - 1] != '/')
            {
                fix += "/";
            }
        }
        return fix;
    }

    void CreateFolder(string folder)
    {
        if (folder.IndexOf("\\") != -1)
        {
            FileManager.CreateDirPath(folder.Substring(0, folder.LastIndexOf("\\") + 1));
        }
        else
        {
            var test1 = folder.LastIndexOf("/");
            string test = folder.Substring(0, 10);
            FileManager.CreateDirPath(folder.Substring(0, folder.LastIndexOf("/") + 1));
        }
    }

    public void ExprotJson()
    {
        if (string.IsNullOrEmpty(pathInput.text))
        {
            AddLog("表格路径不能为空!");
            return;
        }
        if (string.IsNullOrEmpty(jsonInput.text))
        {
            AddLog("Json输出路径不能为空!");
            return;
        }


        string output = FixedPath(jsonInput.text);
        ConfigPackingManager.ExprotLua(pathInput.text, output);
    }

    public void ExprotServerJson()
    {
        if (string.IsNullOrEmpty(pathInput.text))
        {
            AddLog("表格路径不能为空!");
            return;
        }
        if (string.IsNullOrEmpty(jsonInput.text))
        {
            AddLog("Json输出路径不能为空!");
            return;
        }
        string output = FixedPath(jsonInput.text);
        ConfigPackingManager.ExprotLua(pathInput.text, output, true);
    }

    public void ExprotLo()
    {
        if (string.IsNullOrEmpty(pathInput.text))
        {
            AddLog("表格路径不能为空!");
            return;
        }
        if (string.IsNullOrEmpty(loInput.text))
        {
            AddLog("Json输出路径不能为空!");
            return;
        }

        ConfigPackingManager.ExprotLO(pathInput.text, loInput.text);
    }

    void Log(string str)
    {
        logInput.text = "";
        logText.text = "";
        AddLog(str);
    }

    public void AddLog(string str)
    {
        var formatStr = "[$time$] $msg$\n";
        var timestr = DateTime.Now.ToLocalTime().ToString();
        var logStr = formatStr.Replace("$time$", timestr).Replace("$msg$", str);
        logText.text = logStr + logText.text;

        int maxLength = 5000;
        if (logText.text.Length > maxLength)
            logText.text = logText.text.Remove(maxLength, logText.text.Length - maxLength);
        logInput.text = logText.text;
    }

    void InitProjectInfo()
    {
        var infos = this.projectInfos;

        var options = new List<string>();
        for (int i = 0; i < infos.Count; i++)
        {
            options.Add(infos[i].name);
        }

        projectDropdown.ClearOptions();
        projectDropdown.AddOptions(options);
        projectDropdown.onValueChanged.AddListener(this.OnValueChanged);

        projectDropdown.value = lastIndex;
        projectDropdown.RefreshShownValue();
    }
    private void OnValueChanged(int index)
    {
        lastIndex = index;
        var infos = this.projectInfos;
        if (infos.Count == 0)
        {
            return;
        }

        var info = infos[index];

        nameInput.text = info.name;
        pathInput.text = info.devPath;
        jsonInput.text = info.jsonPath;
        loInput.text = info.loPath;
    }

    public List<ProjectInfoVO> projectInfos
    {
        get
        {
            if (_infos == null)
            {
                var str = PlayerPrefs.GetString("PROJECT_INFOS", "[]");
                _infos = JsonMapper.ToObject<List<ProjectInfoVO>>(str);
            }

            return _infos;
        }
        set
        {
            _infos = value;
            var str = JsonMapper.ToJson(_infos);
            PlayerPrefs.SetString("PROJECT_INFOS", str);
        }
    }

    public int lastIndex
    {
        get
        {
            if (_lastIndex == -1)
            {
                _lastIndex = PlayerPrefs.GetInt("PROJECT_LAST_INDEX", 0);
            }

            return _lastIndex;
        }
        set
        {
            _lastIndex = value;
            PlayerPrefs.SetInt("PROJECT_LAST_INDEX", _lastIndex);
        }
    }
}
