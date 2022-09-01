using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    //距离
    public float distance = 10;
    //横向角度
    public float rot = 0;
    //纵向角度
    private float roll = 10f * Mathf.PI * 2 / 360;
    //目标物体
    public Transform target;
    public GameObject cameras;
    //横向旋转速度
    public float rotSpeed = 0.2f;
    //纵向角度范围
    private float maxRoll = 70f * Mathf.PI * 2 / 360;
    private float minRoll = -10f * Mathf.PI * 2 / 360;
    //纵向旋转速度
    private float rollSpeed = 0.2f;
    //距离范围
    public float maxDistance = 22f;
    public float minDistance = 5f;
    //距离变化速度
    public float zoomSpeed = 0.2f;

    void Start()
    {
        
        //SetTarget(GameObject.Find("Tank"));
    }

    void LateUpdate()
    {
        if (target == null) return;
        //目标的坐标
        Vector3 targetPos = target.transform.position;
        //用三角函数计算相机位置
        Vector3 cameraPos;
        float d = distance * Mathf.Cos(roll);
        float height = distance * Mathf.Sin(roll);
        cameraPos.x = targetPos.x + d * Mathf.Cos(rot);
        cameraPos.z = targetPos.z + d * Mathf.Sin(rot);
        cameraPos.y = targetPos.y + height;
        
        cameras.transform.position = cameraPos;
        //对准目标
        cameras.transform.LookAt(target.transform);

        if (Input.GetMouseButton(0))
        {
            //纵向旋转
            Rotate();
            //横向旋转
            Roll();
            //调整距离
        }

        Zoom();
    }

    //设置目标
    public void SetTarget(Transform target)
    {
             
            this.target = target;       
    }

    //横向旋转
    void Rotate()
    {
        float w = Input.GetAxis("Mouse X") * rotSpeed;
        rot -= w;
    }

    //纵向旋转
    void Roll()
    {
        float w = Input.GetAxis("Mouse Y") * rollSpeed * 0.5f;

        roll -= w;
        if (roll > maxRoll)
            roll = maxRoll;
        if (roll < minRoll)
            roll = minRoll;
    }

    //调整距离
    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (distance > minDistance)
                //distance -= zoomSpeed;
            distance = Mathf.Lerp(distance, distance - zoomSpeed, Time.deltaTime * 4f);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (distance < maxDistance)
                //distance += zoomSpeed;
            distance = Mathf.Lerp(distance, distance + zoomSpeed, Time.deltaTime * 4f);
        }
    }
}