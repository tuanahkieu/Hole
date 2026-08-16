using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; // Thêm thư viện TMPro để làm việc với Text Mesh Pro
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject infoPopup;
    [SerializeField] private GameObject settingPopup; 
    [SerializeField] private GameObject playPopup; 
    [SerializeField] private GameObject playBtn; 
    [SerializeField] private GameObject playBtnWithCoin; 

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject EditInfor; 
    [SerializeField] private GameObject namePopup; 

    
    [System.Serializable]
    public struct ProfileTab
    {
        public GameObject tabButtonOn;  // Nửa On của nút bấm
        public GameObject tabButtonOff; // Nửa Off của nút bấm
        public GameObject contentPanel; // Cái bảng nội dung (ScrollAvatar, ScrollFrame...)
    }
    
    [Header("Profile Tabs")]
    [SerializeField] private ProfileTab[] profileTabs; 
    
    [Header("Item Costs")]
    [SerializeField] private int energyFocusCost = 700;
    [SerializeField] private int timeFocusCost = 700;
    [SerializeField] private TextMeshProUGUI playBtnWithCoinText;
    
    [Header("Level Manager")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private int totalScenes = 3; 

    [Header("Heart Manager")]
    [SerializeField] private int defaultHeart = 5; // Số mạng mặc định có thể chỉnh trong Inspector

    [Header("Gold Manager")]
    [SerializeField] private TextMeshProUGUI totalGoldText;
    [SerializeField] private TextMeshProUGUI heart;
    [SerializeField] private TextMeshProUGUI timeWaitText;
    [SerializeField] private float timeWait = 300f;

    [Header("Audio Manager")]
    [SerializeField] private AudioManager audioManager;




    private int currentLevel = 1;
    private float currentTime;
    
    // Caching animators for performance
    private Animator infoAnimator;
    private Animator settingAnimator;

    private void Start()
    {
        audioManager.playHomeMusic();
        Application.targetFrameRate = Screen.currentResolution.refreshRate;

        currentTime = timeWait;

        // Lưu lại cấu hình tổng số màn chơi để các Scene khác biết
        PlayerPrefs.SetInt("TotalLevels", totalScenes);
        PlayerPrefs.Save();

        // Lấy cấp độ hiện tại từ PlayerPrefs, mặc định là 1 nếu chưa chơi lần nào
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        if (currentLevel > totalScenes)
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            PlayerPrefs.Save();
        }
        
        if (levelText != null)
        {
            levelText.text = "Cấp độ\n" + currentLevel.ToString();
        }

        if (totalGoldText != null)
        {
            int totalGold = PlayerPrefs.GetInt("TotalGold", 0);
            totalGoldText.text = totalGold.ToString();
        }

        if (heart != null)
        {
            int currentHeart = PlayerPrefs.GetInt("Heart", defaultHeart);
            heart.text = currentHeart.ToString();
        }

        // Tự động load Avatar và Frame ngoài màn hình chính khi vừa vào game
        if (EditInfor != null)
        {
            AvatarProfileController avatarCtrl = EditInfor.GetComponent<AvatarProfileController>();
            if (avatarCtrl != null && avatarCtrl.availableAvatars != null)
            {
                string savedAvatar = PlayerPrefs.GetString("EquippedAvatar", "");
                foreach (var data in avatarCtrl.availableAvatars)
                {
                    if (data.avatarID == savedAvatar)
                    {
                        if (avatarCtrl.homeMenuAvatarImage != null) avatarCtrl.homeMenuAvatarImage.sprite = data.avatarIcon;
                        break;
                    }
                }
            }

            FrameController frameCtrl = EditInfor.GetComponent<FrameController>();
            if (frameCtrl != null && frameCtrl.availableFrames != null)
            {
                string savedFrame = PlayerPrefs.GetString("EquippedFrame", "");
                foreach (var data in frameCtrl.availableFrames)
                {
                    if (data.frameID == savedFrame)
                    {
                        if (frameCtrl.homeMenuFrameImage != null) frameCtrl.homeMenuFrameImage.sprite = data.frameIcon;
                        break;
                    }
                }
            }
        }
    }

    public void LoadGameScene()
    {

        int sceneIndexToLoad = ((currentLevel - 1) % totalScenes) + 1;

        string sceneName = "GameScene " + sceneIndexToLoad;
        int heartRemain = PlayerPrefs.GetInt("Heart", defaultHeart);
        if(heartRemain > 0){
            SceneManager.LoadScene(sceneName);
        }
        else{
            Debug.LogWarning("Đã hết lượt chơi. Hãy chờ hồi sinh hoặc xem quảng cáo.");
        }
    }





    public void OpenPopupInfo()
    {
        if (infoPopup != null)
        {
            infoPopup.SetActive(true); 
        }
        else
        {
            Debug.LogWarning("Chưa gán object Info vào biến infoPopup trong GameManager!");
        }
        audioManager.playBtnClickSound();
    }
    public void OpenPopupSetting(){
        if(settingPopup != null){
            settingPopup.SetActive(true);
        }
        audioManager.playBtnClickSound();

    }
    public void OpenPopupPlay(){
        if(playPopup != null){
            playPopup.SetActive(true);
            
            // Xóa lựa chọn cũ mỗi khi mở bảng lên, bắt buộc người chơi phải chọn lại từ đầu
            PlayerPrefs.SetString("SelectedFocus", "");
            PlayerPrefs.Save();

            // Tắt toàn bộ viền xanh
            if (energyBorder != null) energyBorder.SetActive(false);
            if (timeBorder != null) timeBorder.SetActive(false);

            // Bật nút Chơi bình thường, tắt nút Chơi bằng xu
            if (playBtn != null) playBtn.SetActive(true);
            if (playBtnWithCoin != null) playBtnWithCoin.SetActive(false);
        }
        audioManager.playBtnClickSound();
    }
    public void ClosePopupPlay(){
        if(playPopup != null){
            playPopup.SetActive(false);
        }
        audioManager.playBtnClickSound();
    }

    public void ClosePopupInfo()
    {
        if (infoPopup != null)
        {
            if (infoAnimator == null) infoAnimator = infoPopup.GetComponentInChildren<Animator>();

            if (infoAnimator != null) infoAnimator.SetBool("isClosePopupInfor", true);
            else if (animator != null) animator.SetBool("isClosePopupInfor", true);
            
            StartCoroutine(WaitAndDeactivatePopup(infoPopup, infoAnimator != null ? infoAnimator : animator, "isClosePopupInfo"));
        }
        audioManager.playBtnClickSound();
    }

    public void ClosePopupSetting()
    {
        if(settingPopup != null){
            if (settingAnimator == null) settingAnimator = settingPopup.GetComponentInChildren<Animator>();

            if (settingAnimator != null) settingAnimator.SetBool("isClosePopupSetting", true);
            else if (animator != null) animator.SetBool("isClosePopupSetting", true);
            
            StartCoroutine(WaitAndDeactivatePopup(settingPopup, settingAnimator != null ? settingAnimator : animator, "isClosePopupSetting"));
        }
        audioManager.playBtnClickSound();
    }

    private System.Collections.IEnumerator WaitAndDeactivatePopup(GameObject popup, Animator anim, string boolName)
    {
        yield return new WaitForSeconds(0.3f);
        
        if (popup != null) popup.SetActive(false);
        if (anim != null) anim.SetBool(boolName, false);
    }


    [Header("Play Focus Selection")]
    [SerializeField] private GameObject energyBorder;
    [SerializeField] private GameObject timeBorder;

    public void SelectEnergyFocus()
    {
        if (PlayerPrefs.GetString("SelectedFocus", "") == "Energy")
        {
            if (energyBorder != null) energyBorder.SetActive(false);
            PlayerPrefs.SetString("SelectedFocus", "");

            if (playBtn != null) playBtn.SetActive(true);
            if (playBtnWithCoin != null) playBtnWithCoin.SetActive(false);
        }
        else
        {
            if (energyBorder != null) energyBorder.SetActive(true);
            
            if (timeBorder != null) timeBorder.SetActive(false);
            PlayerPrefs.SetString("SelectedFocus", "Energy");

            if (playBtn != null) playBtn.SetActive(false);
            if (playBtnWithCoin != null) playBtnWithCoin.SetActive(true);

            if (playBtnWithCoinText != null) playBtnWithCoinText.text = energyFocusCost.ToString();
        }
        PlayerPrefs.Save();
    }

    public void SelectTimeFocus()
    {
        if (PlayerPrefs.GetString("SelectedFocus", "") == "Time")
        {
            if (timeBorder != null) timeBorder.SetActive(false);
            PlayerPrefs.SetString("SelectedFocus", "");

            if (playBtn != null) playBtn.SetActive(true);
            if (playBtnWithCoin != null) playBtnWithCoin.SetActive(false);
        }
        else
        {
            if (energyBorder != null) energyBorder.SetActive(false);
            if (timeBorder != null) timeBorder.SetActive(true);
            PlayerPrefs.SetString("SelectedFocus", "Time");

            if (playBtn != null) playBtn.SetActive(false);
            if (playBtnWithCoin != null) playBtnWithCoin.SetActive(true);

            if (playBtnWithCoinText != null) playBtnWithCoinText.text = timeFocusCost.ToString();
        }
        PlayerPrefs.Save();
    }

    public void PlayWithCoin()
    {
        int totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        string selectedFocus = PlayerPrefs.GetString("SelectedFocus", "");
        int cost = 0;

        if (selectedFocus == "Energy")
        {
            cost = energyFocusCost;
        }
        else if (selectedFocus == "Time")
        {
            cost = timeFocusCost;
        }

        if (totalGold >= cost)
        {
            // Trừ tiền
            totalGold -= cost;
            PlayerPrefs.SetInt("TotalGold", totalGold);
            PlayerPrefs.Save();

            // Cập nhật UI tiền
            if (totalGoldText != null) totalGoldText.text = totalGold.ToString();

            // Vào game
            LoadGameScene();
        }
        else
        {
            ShowNotEnoughCoinText();
        }
    }

    private void ShowNotEnoughCoinText()
    {
        Transform parentTransform = playPopup != null ? playPopup.transform : (FindObjectOfType<Canvas>()?.transform);
        if (parentTransform == null) return;

        // Tạo một Text UI linh động để đảm bảo nó luôn đè lên trên cùng của mọi Popup
        GameObject uiTextObj = new GameObject("NotEnoughCoinText");
        uiTextObj.transform.SetParent(parentTransform, false);
        
        // Gắn TextMeshPro UI vào
        TextMeshProUGUI tmpUI = uiTextObj.AddComponent<TextMeshProUGUI>();
        
        // Sao chép Font từ text có sẵn để tránh lỗi font chữ không hỗ trợ tiếng Việt
        if (playBtnWithCoinText != null)
        {
            tmpUI.font = playBtnWithCoinText.font;
            tmpUI.fontSharedMaterial = playBtnWithCoinText.fontSharedMaterial;
        }

        tmpUI.text = "Không đủ xu";
        tmpUI.color = Color.black;
        tmpUI.fontSize = 80; 
        tmpUI.alignment = TextAlignmentOptions.Center;
        
        // Không cho phép xuống dòng, ép chữ luôn nằm trên 1 hàng ngang
        tmpUI.enableWordWrapping = false;
        tmpUI.overflowMode = TextOverflowModes.Overflow;
        
        // Đặt vị trí ban đầu nằm ở giữa Popup
        RectTransform rect = uiTextObj.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero; 
        
        // Chỉnh kích thước của khung UI rộng ra để chữ không bị bóp
        rect.sizeDelta = new Vector2(800, 200);
        
        // Dùng Coroutine thay vì DOTween để đảm bảo 100% không bị lỗi Tween
        StartCoroutine(AnimateNotEnoughCoin(uiTextObj, tmpUI, rect));
    }

    private System.Collections.IEnumerator AnimateNotEnoughCoin(GameObject obj, TextMeshProUGUI txt, RectTransform rect)
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, 150); // Bay lên 150 pixel
        Color startColor = txt.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            
            if (rect != null) rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            if (txt != null) txt.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            
            yield return null;
        }
        
        if (obj != null) Destroy(obj);
    }

    // Hàm cập nhật thời gian hồi mạng chơi
    public void UpdateTimerDisplay(float time){
        if(timeWaitText != null){
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);

            timeWaitText.text = string.Format("{0:00}:{1:00}", minutes, seconds);   
        }
    }
    private void Update(){
        int h = PlayerPrefs.GetInt("Heart", defaultHeart);

        if (h >= 5)
        {
            if (timeWaitText != null) timeWaitText.text = "Đầy";
            
            // Xóa mốc thời gian nếu tim đã đầy để tránh lỗi khi dùng tim lại
            if (PlayerPrefs.HasKey("NextHeartTime")) {
                PlayerPrefs.DeleteKey("NextHeartTime");
                PlayerPrefs.Save();
            }
            return; // Dừng lại, không cho đếm lùi nữa
        }

        // Lấy mốc thời gian nhận tim tiếp theo (theo thời gian thực)
        string nextTimeStr = PlayerPrefs.GetString("NextHeartTime", "");
        if (string.IsNullOrEmpty(nextTimeStr))
        {
            // Vừa mới mất 1 tim (trước đó đang đầy), thiết lập mốc thời gian mới
            System.DateTime next = System.DateTime.Now.AddSeconds(timeWait);
            PlayerPrefs.SetString("NextHeartTime", next.ToBinary().ToString());
            PlayerPrefs.Save();
        }
        else
        {
            long temp = 0;
            if (long.TryParse(nextTimeStr, out temp))
            {
                System.DateTime nextTime = System.DateTime.FromBinary(temp);
                System.TimeSpan diff = nextTime - System.DateTime.Now;

                if (diff.TotalSeconds <= 0)
                {
                    // Đã qua thời gian nhận tim
                    double secondsPassed = -diff.TotalSeconds; 
                    
                    // Tính xem trong thời gian offline được hồi bao nhiêu tim
                    int heartsToAdd = 1 + Mathf.FloorToInt((float)(secondsPassed / timeWait));
                    h += heartsToAdd;
                    
                    if (h >= 5) 
                    {
                        h = 5;
                        PlayerPrefs.SetInt("Heart", h);
                        PlayerPrefs.DeleteKey("NextHeartTime"); // Đầy thì không cần đếm nữa
                    }
                    else
                    {
                        PlayerPrefs.SetInt("Heart", h);
                        // Cập nhật lại mốc thời gian hồi tim kế tiếp
                        System.DateTime next = nextTime.AddSeconds(heartsToAdd * timeWait);
                        PlayerPrefs.SetString("NextHeartTime", next.ToBinary().ToString());
                    }
                    
                    if (heart != null) heart.text = h.ToString();
                    PlayerPrefs.Save();
                }
                else
                {
                    // Vẫn đang đếm lùi, cập nhật UI
                    UpdateTimerDisplay((float)diff.TotalSeconds);
                }
            }
        }
    }
    public void OpenEditInfor(){
        if(EditInfor != null){
            EditInfor.SetActive(true);
            SelectProfileTab(0); // Mặc định nhảy về tab Avatar (Element 0) khi mở bảng
        }
        audioManager.playBtnClickSound();
    }
    public void CloseEditInfor(){
        if(EditInfor != null){
            EditInfor.SetActive(false);
        }
        audioManager.playBtnClickSound();
    }

    public void SelectProfileTab(int tabIndex)
    {
        for (int i = 0; i < profileTabs.Length; i++)
        {
            bool isSelected = (i == tabIndex);
            
            // Bật/tắt hiệu ứng nút
            if (profileTabs[i].tabButtonOn != null) profileTabs[i].tabButtonOn.SetActive(isSelected);
            if (profileTabs[i].tabButtonOff != null) profileTabs[i].tabButtonOff.SetActive(!isSelected);
            
            // Bật/tắt bảng nội dung
            if (profileTabs[i].contentPanel != null) profileTabs[i].contentPanel.SetActive(isSelected);
        }
    }

    public void OpenNamePopup(){
        if(namePopup != null){
            namePopup.SetActive(true);
        }
        audioManager.playBtnClickSound();
    }
    public void CloseNamePopup(){
        if(namePopup != null){
            namePopup.SetActive(false);
        }
        audioManager.playBtnClickSound();
    }

}