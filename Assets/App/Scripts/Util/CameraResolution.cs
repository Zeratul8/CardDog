using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    public enum ClearMode
    {
        None,
        Clear
    }
    [SerializeField]
    ClearMode m_mode;

    Camera m_camera;

    

    void Start()
    {
        //렌더링 영역 설정
        m_camera = GetComponent<Camera>();
        Rect viewRect = m_camera.rect;
        //Screen.width : 기기의 해상도 기준
        var scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
        var scaleWidth = 1f / scaleHeight;
        if(scaleWidth>1f)
        {
            viewRect.height = scaleHeight;
            viewRect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            viewRect.width = scaleWidth;
            viewRect.x = (1f - scaleHeight) / 2f;
        }
        m_camera.rect = viewRect;
    }

    private void OnPreCull()
    {
        if(m_mode == ClearMode.Clear)
        {
            //오픈GL로 빈 부분 까만색으로 채워주기
            GL.Clear(true, true, Color.black, 1f);
        }
    }

}
