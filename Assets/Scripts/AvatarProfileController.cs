using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AvatarProfileController : MonoBehaviour
{
    [Header("Preview UI (Phía trên)")]
    public Image previewAvatarImage;
    
    [Header("Home Menu UI")]
    public Image homeMenuAvatarImage; // Kéo object "Avatar" của nút bấm ngoài màn hình chính vào đây

    [Header("Avatar List UI (Phía dưới)")]
    public Transform gridContent; 
    public GameObject avatarItemPrefab; 
    
    [Header("Data")]
    public AvatarData[] availableAvatars; 

    private string currentAvatarID;
    private List<AvatarItemUI> allAvatarItems = new List<AvatarItemUI>();

    void Start()
    {
        // Lấy ID avatar đã lưu
        string defaultAvatar = availableAvatars.Length > 0 ? availableAvatars[0].avatarID : "default";
        currentAvatarID = PlayerPrefs.GetString("EquippedAvatar", defaultAvatar);

        GenerateAvatarList();
        
        // Hiển thị avatar to ban đầu
        AvatarData equippedData = GetAvatarByID(currentAvatarID);
        UpdatePreviewUI(equippedData);
    }

    private void GenerateAvatarList()
    {
        foreach (var avatarData in availableAvatars)
        {
            GameObject newObj = Instantiate(avatarItemPrefab, gridContent);
            AvatarItemUI avatarUI = newObj.GetComponent<AvatarItemUI>();
            
            bool isSelected = (avatarData.avatarID == currentAvatarID);
            avatarUI.Setup(avatarData, this, isSelected);
            
            allAvatarItems.Add(avatarUI);
        }
    }

    public void OnAvatarClicked(AvatarData avatarData, AvatarItemUI clickedItem)
    {
        UpdatePreviewUI(avatarData);

        foreach (var item in allAvatarItems)
        {
            item.SetSelected(false);
        }

        clickedItem.SetSelected(true);

        // Lưu lại avatar vừa đổi
        currentAvatarID = avatarData.avatarID;
        PlayerPrefs.SetString("EquippedAvatar", currentAvatarID);
        PlayerPrefs.Save();
    }

    private void UpdatePreviewUI(AvatarData data)
    {
        if (previewAvatarImage != null && data != null)
        {
            previewAvatarImage.sprite = data.avatarIcon;
        }


    }

    private AvatarData GetAvatarByID(string id)
    {
        foreach(var data in availableAvatars)
        {
            if (data.avatarID == id) return data;
        }
        return availableAvatars.Length > 0 ? availableAvatars[0] : null;
    }
    public void SaveAvatar()
    {
        // Lấy data của avatar đang được chọn hiện tại
        AvatarData data = GetAvatarByID(currentAvatarID);
        
        if (homeMenuAvatarImage != null && data != null)
        {
            homeMenuAvatarImage.sprite = data.avatarIcon;
        }
        PlayerPrefs.SetString("EquippedAvatar", currentAvatarID);
        PlayerPrefs.Save();

        // Ép toàn bộ các ảnh có gắn AutoProfile (ví dụ bảng Infor đang mở phía sau) phải cập nhật ngay lập tức
        AutoProfile[] allAutoProfiles = FindObjectsOfType<AutoProfile>();
        foreach (var profile in allAutoProfiles)
        {
            profile.UpdateImage();
        }
    }
}
