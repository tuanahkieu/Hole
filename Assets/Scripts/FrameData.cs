using UnityEngine;

[CreateAssetMenu(fileName = "New Frame", menuName = "Profile/Frame Data")]
public class FrameData : ScriptableObject
{
    public string frameID;       // ID duy nhất, Vd: "frame_circle", "frame_square"
    public Sprite frameIcon;     // Hình ảnh hiển thị frame
    public bool isUnlockedByDefault = true;
}
