using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopController : MonoBehaviour
{
    [Header("Preview UI (Phía trên)")]
    public Image previewImage;
    public TextMeshProUGUI previewNameText;
    
    [Header("Skin List UI (Phía dưới)")]
    public Transform gridContent; // Nơi chứa các ô chọn Skin (Content của ScrollView)
    public GameObject skinItemPrefab; // Kéo thả Prefab ô chọn Skin vào đây
    
    [Header("Data")]
    public HoleSkinData[] allSkins; // Kéo thả tất cả các file dữ liệu Skin vào đây

    private string currentEquippedSkinID;
    private List<SkinItemUI> allSkinItems = new List<SkinItemUI>();

    void Start()
    {
        // 1. Lấy ID skin đang trang bị
        string defaultSkin = "capybara";
        currentEquippedSkinID = PlayerPrefs.GetString("EquippedHole", defaultSkin);

        // 2. Sinh ra các nút trong danh sách
        GenerateSkinList();
        
        // 3. Hiển thị thông tin skin đang dùng lên phần Preview
        HoleSkinData equippedData = GetSkinByID(currentEquippedSkinID);
        UpdatePreviewUI(equippedData);
    }

    private void GenerateSkinList()
    {
        // Tạo item mới dựa trên Data
        foreach (var skinData in allSkins)
        {
            GameObject newObj = Instantiate(skinItemPrefab, gridContent);
            SkinItemUI skinUI = newObj.GetComponent<SkinItemUI>();
            
            bool isEquipped = (skinData.skinID == currentEquippedSkinID);
            skinUI.Setup(skinData, this, isEquipped);
            
            allSkinItems.Add(skinUI);
        }
    }

    // Hàm này được gọi bởi SkinItemUI khi người chơi click vào
    public void OnSkinSelected(HoleSkinData skinData, SkinItemUI clickedItem)
    {
        // 1. Cập nhật hình to phía trên
        UpdatePreviewUI(skinData);

        // 2. Tắt dấu check ở tất cả các ô khác
        foreach (var item in allSkinItems)
        {
            item.SetEquippedState(false);
        }

        // 3. Bật dấu check cho ô vừa bấm
        clickedItem.SetEquippedState(true);

        // 4. Lưu lại cấu hình trang bị mới để dùng trong GameScene
        currentEquippedSkinID = skinData.skinID;
        PlayerPrefs.SetString("EquippedHole", currentEquippedSkinID);
        PlayerPrefs.Save();
    }

    private void UpdatePreviewUI(HoleSkinData skin)
    {
        if (skin == null) return;
        if (previewImage != null) previewImage.sprite = skin.uiIcon;
        if (previewNameText != null) previewNameText.text = skin.skinName;
    }

    private HoleSkinData GetSkinByID(string id)
    {
        foreach(var skin in allSkins)
        {
            if (skin.skinID == id) return skin;
        }
        return allSkins.Length > 0 ? allSkins[0] : null;
    }
}
