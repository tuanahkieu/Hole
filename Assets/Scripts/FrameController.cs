using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FrameController : MonoBehaviour
{
    [Header("Preview UI (Phía trên)")]
    public Image previewFrameImage;
    
    [Header("Home Menu UI")]
    public Image homeMenuFrameImage; // Khung ngoài màn hình chính

    [Header("Frame List UI (Phía dưới)")]
    public Transform gridContent; 
    public GameObject frameItemPrefab; 
    
    [Header("Data")]
    public FrameData[] availableFrames; 

    private string currentFrameID;
    private List<FrameUI> allFrameItems = new List<FrameUI>();

    void Start()
    {
        // Lấy ID khung đã lưu
        string defaultFrame = availableFrames.Length > 0 ? availableFrames[0].frameID : "default";
        currentFrameID = PlayerPrefs.GetString("EquippedFrame", defaultFrame);

        GenerateFrameList();
        
        // Hiển thị khung to ban đầu
        FrameData equippedData = GetFrameByID(currentFrameID);
        UpdatePreviewUI(equippedData);
    }

    private void GenerateFrameList()
    {
        foreach (var frameData in availableFrames)
        {
            GameObject newObj = Instantiate(frameItemPrefab, gridContent);
            FrameUI frameUI = newObj.GetComponent<FrameUI>();
            
            bool isSelected = (frameData.frameID == currentFrameID);
            frameUI.Setup(frameData, this, isSelected);
            
            allFrameItems.Add(frameUI);
        }
    }

    public void OnFrameClicked(FrameData frameData, FrameUI clickedItem)
    {
        UpdatePreviewUI(frameData);

        foreach (var item in allFrameItems)
        {
            item.SetSelected(false);
        }

        clickedItem.SetSelected(true);

        // Lưu lại khung vừa đổi
        currentFrameID = frameData.frameID;
        PlayerPrefs.SetString("EquippedFrame", currentFrameID);
        PlayerPrefs.Save();
    }

    private void UpdatePreviewUI(FrameData data)
    {
        if (previewFrameImage != null && data != null)
        {
            previewFrameImage.sprite = data.frameIcon;
        }
    }

    private FrameData GetFrameByID(string id)
    {
        foreach(var data in availableFrames)
        {
            if (data.frameID == id) return data;
        }
        return availableFrames.Length > 0 ? availableFrames[0] : null;
    }

    public void SaveFrame()
    {
        // Lấy data của khung đang được chọn hiện tại
        FrameData data = GetFrameByID(currentFrameID);
        
        if (homeMenuFrameImage != null && data != null)
        {
            homeMenuFrameImage.sprite = data.frameIcon;
        }
        PlayerPrefs.SetString("EquippedFrame", currentFrameID);
        PlayerPrefs.Save();

        // Ép toàn bộ các ảnh có gắn AutoProfile phải cập nhật ngay lập tức
        AutoProfile[] allAutoProfiles = FindObjectsOfType<AutoProfile>();
        foreach (var profile in allAutoProfiles)
        {
            profile.UpdateImage();
        }
    }
}
