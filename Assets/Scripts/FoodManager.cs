using UnityEngine;
using TMPro;
using System.Collections;

public class FoodManager : MonoBehaviour
{
    [Header("Cài đặt Target")]
    [Tooltip("Gõ tên vật phẩm cần đếm vào đây (Ví dụ: Cheesecake)")]
    public string targetItemName; 

    [Header("UI References")]
    [Tooltip("Kéo Text số lượng vào đây")]
    public TextMeshProUGUI amountText;
    [Tooltip("Kéo icon Tick vào đây")]
    public GameObject tickIcon;

    public int currentAmount { get; private set; } = 0;

    // Đăng ký nhận sự kiện khi Hole nuốt vật phẩm
    private void OnEnable()
    {
        HoleManager.OnItemEatenEvent += HandleItemEaten;
    }

    private void OnDisable()
    {
        HoleManager.OnItemEatenEvent -= HandleItemEaten;
    }

    private void Start()
    {
        // Ẩn tick ban đầu, đảm bảo text hiện
        if (tickIcon != null) tickIcon.SetActive(false);
        if (amountText != null) amountText.gameObject.SetActive(true);

        CountItemsOnMap();
    }

    /// <summary>
    /// Đếm toàn bộ vật phẩm có trên map lúc vào game
    /// </summary>
    private void CountItemsOnMap()
    {
        currentAmount = 0;
        
        // TỐI ƯU HÓA: Quét qua danh sách tĩnh siêu nhẹ thay vì dùng FindObjectsOfType
        foreach (var item in EatableItem.AllItems)
        {
            if (item.itemName == targetItemName)
            {
                currentAmount++;
            }
        }

        // Nếu lúc bắt đầu game không có vật phẩm nào loại này trên map, tự ẩn luôn UI
        if (currentAmount == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// Được gọi tự động mỗi khi hố nuốt bất kỳ vật phẩm nào
    /// </summary>
    private void HandleItemEaten(string eatenItemName)
    {
        // Nếu đúng là vật phẩm mình đang đếm và số lượng vẫn > 0
        if (eatenItemName == targetItemName && currentAmount > 0)
        {
            currentAmount--;
            UpdateUI();

            // Khi gom đủ
            if (currentAmount == 0)
            {
                StartCoroutine(ShowTickAndHide());
                CheckWinCondition();
            }
        }
    }

    private void CheckWinCondition()
    {
        bool allDone = true;
        FoodManager[] allManagers = FindObjectsOfType<FoodManager>();
        
        foreach (var fm in allManagers)
        {
            // Nếu có bất kỳ target nào chưa gom đủ số lượng
            if (fm.currentAmount > 0)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            if (GamePlayManager.Instance != null)
            {
                GamePlayManager.Instance.GameWin();
            }
            else
            {
                // Fallback giống code cũ
                int current = PlayerPrefs.GetInt("CurrentLevel", 1);
                current++;
                int maxLevel = PlayerPrefs.GetInt("TotalLevels", 3);
                if (current > maxLevel) current = 1;
                PlayerPrefs.SetInt("CurrentLevel", current);
                PlayerPrefs.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
            }
        }
    }

    private void UpdateUI()
    {
        if (amountText != null)
        {
            amountText.text = currentAmount.ToString();
        }
    }

    private IEnumerator ShowTickAndHide()
    {
        // 1. Ẩn Text, hiện Tick xanh
        if (amountText != null) amountText.gameObject.SetActive(false);
        if (tickIcon != null) tickIcon.SetActive(true);

        // 2. Đợi 2 giây
        yield return new WaitForSeconds(2f);

        // 3. Ẩn toàn bộ cục UI Prefab này (khiến Target_bg tự động co lại)
        gameObject.SetActive(false);
    }
}
