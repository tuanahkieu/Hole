using UnityEngine;
using System.Collections.Generic;

public class EatableItem : MonoBehaviour
{
    // TỐI ƯU HÓA: Thay vì dùng FindObjectsOfType làm lag máy, ta dùng danh sách tĩnh (Static List)
    public static List<EatableItem> AllItems = new List<EatableItem>();

    [Header("Định danh vật phẩm")]
    public string itemName; // Đặt tên để đếm target (VD: "Cheesecake", "Burger")

    [Header("Thông số vật phẩm")]
    public int scoreValue = 1; // Năng lượng/điểm số
    public int goldValue = 1; // Số vàng kiếm được khi ăn vật này

    public string normalizedItemName { get; private set; }

    // Tự động báo danh khi vật phẩm xuất hiện trên map
    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(itemName))
        {
            normalizedItemName = itemName.Trim().ToLower();
        }
        else
        {
            normalizedItemName = "";
        }
        AllItems.Add(this);
    }

    // Tự động gạch tên khỏi danh sách khi bị ăn hoặc bị ẩn
    private void OnDisable()
    {
        AllItems.Remove(this);
    }
}
