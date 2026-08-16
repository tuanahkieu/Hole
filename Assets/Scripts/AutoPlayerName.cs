using UnityEngine;
using TMPro;

public class AutoPlayerName : MonoBehaviour
{
    private void OnEnable()
    {
        UpdateName();
    }

    public void UpdateName()
    {
        TextMeshProUGUI txt = GetComponent<TextMeshProUGUI>();
        if (txt != null)
        {
            // Lấy tên đã lưu. Nếu chưa có tên (người chơi mới), tự tạo một tên ngẫu nhiên kiểu Player1234
            string defaultName = "Player" + Random.Range(1000, 9999);
            string savedName = PlayerPrefs.GetString("PlayerName", defaultName);
            txt.text = savedName;
        }
    }
}
