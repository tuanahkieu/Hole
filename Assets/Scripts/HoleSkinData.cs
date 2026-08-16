using UnityEngine;

[CreateAssetMenu(fileName = "New Hole Skin", menuName = "Shop/Hole Skin Data")]
public class HoleSkinData : ScriptableObject
{
    public string skinID;          // ID duy nhất (vd: "default", "capybara")
    public string skinName;        // Tên hiển thị (vd: "Mặc định", "Capybara")
    public Sprite uiIcon;          // Hình ảnh hiển thị trong Shop
    public Material inGameMaterial;// Material/Màu sắc để thay đổi hố trong game 
                                   // (Hoặc có thể là GameObject Prefab nếu hố có hình dáng 3D khác nhau)
    public bool isUnlockedByDefault;
}
