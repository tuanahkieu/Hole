using UnityEngine;
using TMPro;

public class PlayerNameController : MonoBehaviour
{
    [Header("Giao diện")]
    public TMP_InputField nameInput;
    public GameObject namePopup; // Để tắt bảng này đi sau khi bấm Tiếp tục

    public void SaveName()
    {
        // Kiểm tra xem ô nhập liệu có chữ nào không
        if (nameInput != null && !string.IsNullOrEmpty(nameInput.text))
        {
            // Lưu tên vào máy
            PlayerPrefs.SetString("PlayerName", nameInput.text);
            PlayerPrefs.Save();

            AutoPlayerName[] allNameDisplays = FindObjectsOfType<AutoPlayerName>();
            foreach (var nameUI in allNameDisplays)
            {
                nameUI.UpdateName();
            }

            // Đóng bảng NamePopup
            if (namePopup != null)
            {
                namePopup.SetActive(false);
            }
        }
    }
}
