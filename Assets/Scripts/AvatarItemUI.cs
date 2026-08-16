using UnityEngine;
using UnityEngine.UI;

public class AvatarItemUI : MonoBehaviour
{
    public Image avatarIcon;
    public GameObject selectedBorder;
    
    private AvatarData myData;
    private AvatarProfileController controller;

    public void Setup(AvatarData data, AvatarProfileController ctrl, bool isSelected)
    {
        myData = data;
        controller = ctrl;

        if (avatarIcon != null) avatarIcon.sprite = data.avatarIcon;
        SetSelected(isSelected);
        
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (controller != null)
        {
            controller.OnAvatarClicked(myData, this);
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedBorder != null) selectedBorder.SetActive(isSelected);
    }
}
