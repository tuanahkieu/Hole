using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinItemUI : MonoBehaviour
{
    public Image skinIcon;
    public TextMeshProUGUI skinNameText;
    public GameObject equippedCheckmark; // Kéo thả Nút V hoặc Viền sáng (báo đã trang bị)
    public GameObject selectedBackground;   // Kéo thả nền Green
    public GameObject unselectedBackground; // Kéo thả nền Blue
    
    private HoleSkinData myData;
    private ShopController shopController;

    public void Setup(HoleSkinData data, ShopController controller, bool isEquipped)
    {
        myData = data;
        shopController = controller;

        if (skinIcon != null) skinIcon.sprite = data.uiIcon;
        if (skinNameText != null) skinNameText.text = data.skinName;
        
        SetEquippedState(isEquipped);
        
        // Gắn sự kiện click cho nút (Cần component Button trên GameObject này)
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnItemClicked);
        }
    }

    private void OnItemClicked()
    {
        if (shopController != null)
        {
            shopController.OnSkinSelected(myData, this);
        }
    }

    public void SetEquippedState(bool isEquipped)
    {
        if (equippedCheckmark != null) equippedCheckmark.SetActive(isEquipped);
        if (selectedBackground != null) selectedBackground.SetActive(isEquipped);
        if (unselectedBackground != null) unselectedBackground.SetActive(!isEquipped);
    }
}
