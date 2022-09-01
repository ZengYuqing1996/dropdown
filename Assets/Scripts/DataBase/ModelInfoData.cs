using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelInfoData : MonoBehaviour
{
    public SQLModelInfo modelInfo { get; set; }

    public  string FirstName { get; set; }
    public GameObject Model { get; set; }

    public string ModifySourcePath { get; set; }

    public bool IsModifyModel { get; set; }

    public Color ChangeImageColor(bool isSelect)
    {
        return isSelect ? new Color(85f / 255f, 85f / 255f, 85f / 255f, 100f / 255f) : Color.white;
    }

    public void Active(bool isActive)
    {
        if (Model != null)
        {
            Model.SetActive(isActive);
        }
    }

    private void OnDestroy()
    {
        Destroy(Model);
    }

}
