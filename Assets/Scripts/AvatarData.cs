using UnityEngine;

[CreateAssetMenu(fileName = "New Avatar", menuName = "Profile/Avatar Data")]
public class AvatarData : ScriptableObject
{
    public string avatarID;       // ID duy nhất, Vd: "ava_rabbit", "ava_bear"
    public Sprite avatarIcon;     // Hình ảnh hiển thị avatar
    public bool isUnlockedByDefault = true;
}
