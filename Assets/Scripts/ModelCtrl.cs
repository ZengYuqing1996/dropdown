using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModelCtrl : MonoBehaviour,IPointerDownHandler,IDragHandler
{
    public Button AddModel_bbt, DeleteModel_bbt, ChangeModel_bbt, Confirm_bbt;
    public Text TimeText;
    public InputField InputName;
    public GameObject pre;//按钮预制体
    public RectTransform Cotent;
    public CameraFollow cameraFollow;

    public Dropdown BaseType;
   

    //public Dropdown BaseModifyType, ModelModifyType;

    //保存所有的生成的按钮，主要是为了方便其变色
    public Dictionary<ModelInfoData, Image> ModelDict = new Dictionary<ModelInfoData, Image>();

    List<SQLModelInfo> data_ModelInfos = new List<SQLModelInfo>();//从数据库中读取的列表生成在左面板上的dropdown上
    private ModelInfoData currentModelInfoData;
    private SQLModelInfo currentModelInfo;
    private Text tipText;

    //public event Action<ModelInfo> AddModelEvent;
    public event Action<SQLModelInfo> DeleteModelEvent;
    public event Action<SQLModelInfo> ChangeModelEvent;
    public event Action<SQLModelInfo> ConfirmEvent;

  

   
    private string dbName;
    private string SaveFolderName;//添加的模型同意保存的路径



    //UI拖动
    public RectTransform rectTransform;
    private float xMax, yMax;
    private Vector2 offsetPos;
   
    void Start()
    {
       

        InitModelInfo(DataNameManager.ModelDB);

        HideOrActiveModel(false);

        switch (BaseType.value)
        {

            case 0:
                dbName = DataNameManager.ModelDB;
                SaveFolderName = "Model";
                break;
            case 1:
                dbName = DataNameManager.PeopleDB;
                SaveFolderName = "People";
                break;
        }





        BaseType.onValueChanged.AddListener(OnBaseTypeDropDownClick);
        SaveFolderName = "Model";

    

        DeleteModelEvent += OnDeleteModelEvent;
        ChangeModelEvent += OnChangeModelEvent;
        ConfirmEvent += OnConfirmEvent;



        AddModel_bbt.onClick.AddListener(OnAddModelClick);
        DeleteModel_bbt.onClick.AddListener(OnDeleteModelClick);
        ChangeModel_bbt.onClick.AddListener(OnChangeModelClick);
        Confirm_bbt.onClick.AddListener(OnConfirmClick);
    }

    //保证在关闭时其他模型不显示
    private void HideOrActiveModel(bool v) => currentModelInfoData?.Model?.SetActive(v);


    //假设有两种类型，选择其中一个类型会加载对应的数据库，生成相应的按钮在content上
    private void OnBaseTypeDropDownClick(int arg0)
    {
        
        switch (arg0)
        {

            case 0:
                dbName = DataNameManager.ModelDB;
                SaveFolderName = "Model";
                break;
            case 1:
                dbName = DataNameManager.PeopleDB;
                SaveFolderName = "People";
                break;
        }

        ClearModelDict();
        InitModelInfo(dbName);
    }

    private void InitModelInfo(string dbName)
    {
        List<SQLModelInfo> modelinfos = SQLiteDataManager.Instance.SelectTable(dbName);
        ClearModelDict();
        foreach (var modelInfo in modelinfos)
        {
            CreateModelButton(modelInfo);
        }
    }

    private void ClearModelDict()
    {
        TimeText.text = "未知";
        // BaseModifyType.value = 0;
        //ModelModifyType.value = 0;
        InputName.text = null;
        if (ModelDict.Count > 0)
        {
            foreach (var item in ModelDict)
            {
                Destroy(item.Key.gameObject);
            }
            ModelDict.Clear();
            currentModelInfoData = null;
        }
    }
    /// <summary>
    /// modifySourcePath原本路径
    /// path更改后的路径
    /// </summary>
    /// <param name="modifySourcePath"></param>
    /// <param name="path"></param>
    private void FileCopyOverwritw(string modifySourcePath, string path)
    {

        //path = Environment.CurrentDirectory + "/Assets/Resource" + path;
        string destFileFolderPath = Path.GetDirectoryName(path);

        if (!Directory.Exists(destFileFolderPath))
        {
            Directory.CreateDirectory(destFileFolderPath);
        }
        File.Copy(modifySourcePath, path, true);


    }


    //************************************************************************************************************
    //绑定的事件
    private void OnConfirmEvent(SQLModelInfo obj)
    {

        if (currentModelInfoData == null) return;


        //currentModelInfoData.modelInfo.Basetype = BaseModifyType.value;
        //currentModelInfoData.modelInfo.Modeltype = ModelModifyType.value;
        currentModelInfoData.modelInfo.Name = InputName.text;

        currentModelInfo.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        TimeText.text = currentModelInfoData.modelInfo.Time;

        currentModelInfoData.transform.GetChild(0).GetComponent<Text>().text = currentModelInfoData.modelInfo.Name;

        //更换模型的路径
        if (currentModelInfoData.IsModifyModel)
        {
            FileCopyOverwritw(currentModelInfoData.ModifySourcePath, currentModelInfoData.modelInfo.Path);
        }

        SQLiteDataManager.Instance.UpdateTable(currentModelInfoData.modelInfo, dbName);

       

        tipText.text = "修改完成";
        tipText.gameObject.SetActive(true);
        Invoke("FadeTipText", 2);
    }



    //更换模型绑定的事件
    private void OnChangeModelEvent(SQLModelInfo obj)
    {
        currentModelInfo = currentModelInfoData.modelInfo;
        if (currentModelInfo == null) return;

        var openFileName = GetOpenFileName();
        if (openFileName != null)
        {
            string ModelPath = openFileName.file;

           
            //路径可能会修改
            currentModelInfoData.ModifySourcePath = ModelPath;

            currentModelInfoData.modelInfo.Path = ModelPath;
            SQLiteDataManager.Instance.UpdateTable(currentModelInfoData.modelInfo, dbName);

            currentModelInfoData.IsModifyModel = true;
            Destroy(currentModelInfoData.Model);//删除绑定之前的模型

            //重新加载模型,默认在 Resource下更换模型的
            currentModelInfoData.Model = Loader(SaveFolderName ,GetFileNameWithoutxtension(openFileName.fileTitle));



        }
    }
    //绑定的事件
    private void OnDeleteModelEvent(SQLModelInfo obj)
    {
        currentModelInfo = currentModelInfoData.modelInfo;
        if (currentModelInfoData == null) return;

        SQLiteDataManager.Instance.deleteTable(currentModelInfoData.modelInfo.ID, dbName);
        if (Directory.Exists(currentModelInfoData.modelInfo.Path))
        {
            Directory.Delete(currentModelInfoData.modelInfo.Path, true);
        }
    }

    //************************************************************************************************************


    //************************************************************************************************************
    //按钮事件,修改什么信息
    private void OnConfirmClick()
    {
        if (currentModelInfoData != null)
        {
            ConfirmEvent?.Invoke(currentModelInfoData.modelInfo);
        }
        else
        {
            tipText.text = "请选择要修改的模型";
            tipText.gameObject.SetActive(true);
            Invoke("FadeTipText", 2);
        }
    }

    //按钮事件。更换模型，也是打开目录选择模型
    private void OnChangeModelClick()
    {
        if (currentModelInfoData != null)
        {
            ChangeModelEvent?.Invoke(currentModelInfoData.modelInfo);
        }
        else
        {
            tipText.text = "请选择要更换的模型";
            tipText.gameObject.SetActive(true);
            Invoke("FadeTipText", 2);
        }
    }


    private void OnDeleteModelClick()
    {
        if (currentModelInfoData != null)
        {

            TimeText.text = "未知";
            // BaseModifyType.value = 0;
            //ModelModifyType.value = 0;
            InputName.text = null;
            DeleteModelEvent?.Invoke(currentModelInfoData.modelInfo);

            if (ModelDict.ContainsKey(currentModelInfoData))
            {
                Destroy(currentModelInfoData.gameObject);
                ModelDict.Remove(currentModelInfoData);
            }

        }
        else
        {
            tipText.text = "请选择要删除的模型";
            tipText.gameObject.SetActive(true);
            Invoke("FadeTipText", 2);
        }

    }



    /// <summary>
    /// 点击增加模型按钮
    /// </summary>
    private void OnAddModelClick()
    {


        var openFileName = GetOpenFileName();
        if (openFileName != null)
        {
            string ModelPath = openFileName.file;
            string ModelName = openFileName.fileTitle;

            string a = "prefab";
            char[] s = new char[1] { '.' };
            string GBName = SplitPath(s, ModelName, a);


            //SaveFolderName为子文件夹
           
            SQLModelInfo modelInfo = new SQLModelInfo();
            modelInfo.Basetype = BaseType.value;
            //modelInfo.Modeltype = ModelType.value;
            modelInfo.Name = GBName;

            modelInfo.Path = ModelPath;

            modelInfo.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            long ID = SQLiteDataManager.Instance.InsertTable(modelInfo, dbName);

            if (ID != -1)
            {
                modelInfo.ID = (int)ID;
                CreateModelButton(modelInfo);
                //FileCopyOverwritw(ModelPath, SavePath);//把原来的模型复制到统一的文件夹下,统一路径保存模型
            }
            else
            {
                tipText.text = "增加模型失败";
                tipText.gameObject.SetActive(true);
                Invoke("FadeTipText", 2);
            }
        }
    }


    /// <summary>
    /// 生成一个模型按钮
    /// </summary>
    /// <param name="modelInfo"></param>
    private void CreateModelButton(SQLModelInfo modelInfo)
    {

        GameObject btn = Instantiate(pre);
        btn.transform.SetParent(Cotent);
        btn.SetActive(true);
        btn.GetComponentInChildren<Text>().text = modelInfo.Name;

        ModelInfoData modelInfoData = btn.AddComponent<ModelInfoData>();
        modelInfoData.modelInfo = modelInfo;
        modelInfoData.FirstName = modelInfoData.modelInfo.Name;
        btn.GetComponent<Button>().onClick.AddListener(() => OnModelBtnClick(modelInfoData));
        ModelDict.Add(modelInfoData, btn.GetComponent<Image>());//保存它的image是为了能够选中该按钮的时候变颜色
    }

    //选择模型按钮
    private void OnModelBtnClick(ModelInfoData modelInfoData)
    {
        currentModelInfoData = modelInfoData;
        foreach (var item in ModelDict)
        {
            item.Value.color = item.Key.ChangeImageColor(item.Key == currentModelInfoData);
            item.Key.Active(item.Key == currentModelInfoData);
        }

        TimeText.text = modelInfoData.modelInfo.Time;
        //ModelModifyType.value = modelInfoData.modelInfo.Modeltype;
        // BaseModifyType.value = modelInfoData.modelInfo.Basetype;
        InputName.text = modelInfoData.modelInfo.Name;

        //第一次没有把模型保存下来
        if (modelInfoData.Model == null)
        {

            modelInfoData.Model = Loader(SaveFolderName ,currentModelInfoData.FirstName);
        }
        //保存了模型后，后面直接显示为true
        else
        {
            modelInfoData.Active(true);
        }
        cameraFollow.SetTarget(modelInfoData.Model.transform);
    }

    //************************************************************************************************************


    private void OnEnable()
    {
        HideOrActiveModel(false);
    }

    void FadeTipText()
    {
        tipText.gameObject.SetActive(false);
    }
    string SplitPath(char[] split, string Origin, string End)
    {
        string[] datas = Origin.Split(split, Origin.Length - End.Length);
        return datas[0];
    }


    private GameObject Loader(string path,string name)
    {
        //path代表路径，
        GameObject model = Resources.Load<GameObject>("/"+path+"/"+name);
        GameObject go = (GameObject)Instantiate(model);
        go.transform.position = new Vector3(9999,9999,9999);
        cameraFollow.SetTarget(go.transform);
        return go;
    }

    

    /// <summary>
    /// 把文件名的后缀去掉
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string GetFileNameWithoutxtension(string path)
    {
        if (File.Exists(path))
        {
            return Path.GetFileNameWithoutExtension(path);

        }
        else
        {
            return null;
        }

    }



    OpenFileName GetOpenFileName()
    {
        OpenFileName m_openfileName = new OpenFileName();
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
            return m_openfileName;

        }
        else
        {
            return null;
        }
    }


    //*******************************************实现UI拖动，不超过屏幕******************************************

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        xMax = (Screen.width - rectTransform.rect.width) / 2;
        yMax = (Screen.height - rectTransform.rect.height) / 2;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        offsetPos = eventData.position - rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            rectTransform.anchoredPosition = DragRangeLimit(eventData.position - offsetPos);
        }
    }

    private Vector3 DragRangeLimit(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, -xMax, xMax);
        pos.y= Mathf.Clamp(pos.y, -yMax, yMax);
        return pos;
    }
}
