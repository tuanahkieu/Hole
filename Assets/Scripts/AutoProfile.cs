using UnityEngine;
using UnityEngine.UI;

public class AutoProfile : MonoBehaviour
{
    [Header("Loại hình ảnh")]
    [Tooltip("Tích vào nếu đây là Avatar. Bỏ tích nếu đây là Frame (Khung).")]
    public bool isAvatar = true; 

    private void OnEnable()
    {
        // Hàm này tự động chạy mỗi khi Object này được bật lên (ví dụ: khi mở Popup)
        UpdateImage();
    }

    public void UpdateImage()
    {
        Image img = GetComponent<Image>();
        if (img == null) return;

        if (isAvatar)
        {
            string savedAvatar = PlayerPrefs.GetString("EquippedAvatar", "");
            
            // Tự động quét tìm tất cả AvatarData trong thư mục Resources
            AvatarData[] allData = Resources.LoadAll<AvatarData>(""); 
            foreach (var data in allData)
            {
                if (data.avatarID == savedAvatar) 
                { 
                    img.sprite = data.avatarIcon; 
                    break; 
                }
            }
        }
        else
        {
            string savedFrame = PlayerPrefs.GetString("EquippedFrame", "");
            
            // Tự động quét tìm tất cả FrameData trong thư mục Resources
            FrameData[] allData = Resources.LoadAll<FrameData>(""); 
            foreach (var data in allData)
            {
                if (data.frameID == savedFrame) 
                { 
                    img.sprite = data.frameIcon; 
                    break; 
                }
            }
        }
    }
}
