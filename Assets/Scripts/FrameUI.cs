using UnityEngine;
using UnityEngine.UI;

public class FrameUI : MonoBehaviour
{

    public Image frameIc;
    public GameObject selectedBorder;
    private FrameData myData;
    private FrameController controller;

    public void Setup(FrameData data, FrameController ctrl, bool isSelected)
    {
        myData = data;
        controller = ctrl;

        if(frameIc != null) frameIc.sprite = data.frameIcon;
        SetSelected(isSelected);

        Button btn = GetComponent<Button>();
        if(btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if(controller != null)
        {
            controller.OnFrameClicked(myData, this);
        }
    }
    
    public void SetSelected(bool isSelected)
    {
        if(selectedBorder != null) selectedBorder.SetActive(isSelected);
    }
}