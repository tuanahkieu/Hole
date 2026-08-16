using UnityEngine;
using UnityEngine.UI;

public class TaskbarManager : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button tabButton;      // Nút bấm (Ví dụ: Nút Home, Shop)
        public GameObject onVisual;   // Object "On" (Hiệu ứng khi được chọn)
        public GameObject offVisual;  // Object "Off" (Hiệu ứng khi không được chọn)
        public GameObject targetPage; // Trang giao diện tương ứng (HomePage, ShopPage...)
    }

    [Header("Cấu hình các Tab")]
    public Tab[] tabs;

    private void Start()
    {
        // Tự động gắn sự kiện click cho từng nút
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i; // Cần thiết cho delegate trong vòng lặp
            if (tabs[i].tabButton != null)
            {
                tabs[i].tabButton.onClick.AddListener(() => OnTabSelected(index));
            }
        }

        // Bật tab đầu tiên mặc định khi vừa vào game
        if (tabs.Length > 0)
        {
            OnTabSelected(0);
        }
    }

    public void OnTabSelected(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            bool isSelected = (i == index);
            
            // Bật/tắt hình ảnh của nút
            if (tabs[i].onVisual != null) tabs[i].onVisual.SetActive(isSelected);
            if (tabs[i].offVisual != null) tabs[i].offVisual.SetActive(!isSelected);
            
            // Bật/tắt các trang giao diện tương ứng (Page)
            if (tabs[i].targetPage != null) tabs[i].targetPage.SetActive(isSelected);
        }
    }
}
