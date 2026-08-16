using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject settingPopup; 
    [SerializeField] private GameObject winGamePopup; 
    [SerializeField] private GameObject exitPopup; 
    [SerializeField] private GameObject SizeUpPopup; 
    [SerializeField] private GameObject MagnetPopup; 
    [SerializeField] private TextMeshProUGUI goldText; 
    [SerializeField] private TextMeshProUGUI totalGoldText; 
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject freeze;
    [SerializeField] private GameObject CompassPopup;
    [SerializeField] private GameObject FreezePopup;
    
    [Header("Costs")]
    [SerializeField] private int sizeUpCost = 200;
    [SerializeField] private int magnetCost = 200;
    [SerializeField] private int compassCost = 200;
    [SerializeField] private int freezeCost = 200;

    [Header("Cost UI Texts")]
    [SerializeField] private TextMeshProUGUI sizeUpCostText;
    [SerializeField] private TextMeshProUGUI magnetCostText;
    [SerializeField] private TextMeshProUGUI compassCostText;
    [SerializeField] private TextMeshProUGUI freezeCostText;
    
    [SerializeField] private AudioManager audioManager;

    private int currentMatchGold = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    private void Start()
    {
        UpdateInGameTotalGold();
    }

    public void UpdateInGameTotalGold()
    {
        if (totalGoldText != null)
        {
            totalGoldText.text = PlayerPrefs.GetInt("TotalGold", 0).ToString();
        }
    }
    //+coin
    public void AddGold(int amount)
    {
        currentMatchGold += amount;
    }
    //Win
    public void GameWin()
    {
        if (winGamePopup != null)
        {
            winGamePopup.SetActive(true);
            audioManager.playWinMusic();
            if (coin != null) coin.SetActive(true);
            
            if (goldText != null)
            {
                goldText.text = "x0";

                DOVirtual.Int(0, currentMatchGold, 0.5f, (v) => {
                    if (goldText != null) goldText.text = "x" + v.ToString();
                }).SetUpdate(true).SetId(this);
            }

            int oldTotalGold = PlayerPrefs.GetInt("TotalGold", 0);
            int newTotalGold = oldTotalGold + currentMatchGold;
            PlayerPrefs.SetInt("TotalGold", newTotalGold);

            if (totalGoldText != null)
            {
                totalGoldText.text = oldTotalGold.ToString();
                DOVirtual.Int(oldTotalGold, newTotalGold, 0.5f, (v) => {
                    if (totalGoldText != null) totalGoldText.text = v.ToString();
                }).SetUpdate(true).SetId(this);
            }

            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            currentLevel++;
            int maxLevel = PlayerPrefs.GetInt("TotalLevels", 3);
            if (currentLevel > maxLevel) currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);

            PlayerPrefs.Save();

            Time.timeScale = 0f; 
        }
        else
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            currentLevel++;
            int maxLevel = PlayerPrefs.GetInt("TotalLevels", 3);
            if (currentLevel > maxLevel) currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            PlayerPrefs.Save();
            SceneManager.LoadScene("HomeScene");
        }
    }

    public void OpenPopupSetting(){
        if(settingPopup != null){
            settingPopup.SetActive(true);
            Time.timeScale = 0f; // Tạm dừng thời gian trong game
            audioManager.playBtnClickSound();
        }
    }

    public void ClosePopupSetting(){
        if(settingPopup != null){
            settingPopup.SetActive(false);
            audioManager.playBtnClickSound();
            Time.timeScale = 1f; 
        }
    }

    public void ExitGame(){
        Time.timeScale = 1f; 
        int oldHeart = PlayerPrefs.GetInt("Heart", 5);
        int newHeart = oldHeart - 1;
        PlayerPrefs.SetInt("Heart", newHeart);
        PlayerPrefs.Save();
        audioManager.playBtnClickSound();
        SceneManager.LoadScene("HomeScene");

    }
    public void OpenExitPopup(){
        ClosePopupSetting();
        audioManager.playBtnClickSound();
        if(exitPopup != null){
            exitPopup.SetActive(true);
            Time.timeScale = 0f; 
        }        
    }
    public void CloseExitPopup(){
        if(exitPopup != null){
            exitPopup.SetActive(false);
            Time.timeScale = 1f; 
            audioManager.playBtnClickSound();
        }        
    }
    public void LoadHomeScene(){
        audioManager.playBtnClickSound();
        Time.timeScale = 1f; // Khôi phục thời gian bình thường trước khi đổi Scene
        SceneManager.LoadScene("HomeScene");
    }

    public void LoadNextLevel(){
        Time.timeScale = 1f; 
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        SceneManager.LoadScene("GameScene " + currentLevel);
    }

    
    public void CloseSizeUpPopup(){
        audioManager.playBtnClickSound();
        if (coin != null) coin.SetActive(false);
        if(SizeUpPopup != null){
            SizeUpPopup.SetActive(false);
            Time.timeScale = 1f; 
        }        
    }

    public void OpenSizeUpPopup(){
        audioManager.playBtnClickSound();
        if (coin != null) coin.SetActive(true);
        if(SizeUpPopup != null){
            if (sizeUpCostText != null) sizeUpCostText.text = sizeUpCost.ToString();
            SizeUpPopup.SetActive(true);
            Time.timeScale = 0f; 
        }        
    }

    public void BuySizeUpWithCoin()
    {
        audioManager.playBtnClickSound();
        int totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        if (totalGold >= sizeUpCost)
        {
            totalGold -= sizeUpCost;
            PlayerPrefs.SetInt("TotalGold", totalGold);
            PlayerPrefs.Save();
            
            UpdateInGameTotalGold();

            HoleManager holeManager = FindObjectOfType<HoleManager>();
            if (holeManager != null)
            {
                holeManager.SizeUp();
            }

            CloseSizeUpPopup();
        }
        else
        {
            ShowNotEnoughCoinText();
        }
    }


    public void CloseMagnetPopup(){
        audioManager.playBtnClickSound();
        if (coin != null) coin.SetActive(false);
        if(MagnetPopup != null){
            MagnetPopup.SetActive(false);
            Time.timeScale = 1f; 
        }        
    }

    public void OpenMagnetPopup(){
        audioManager.playBtnClickSound();
        if (coin != null) coin.SetActive(true);
        if(MagnetPopup != null){
            if (magnetCostText != null) magnetCostText.text = magnetCost.ToString();
            MagnetPopup.SetActive(true);
            Time.timeScale = 0f; 
        }        
    }

    public void BuyMagnetWithCoin()
    {
        audioManager.playBtnClickSound();
        int totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        if (totalGold >= magnetCost)
        {
            totalGold -= magnetCost;
            PlayerPrefs.SetInt("TotalGold", totalGold);
            PlayerPrefs.Save();
            
            UpdateInGameTotalGold();

            HoleManager holeManager = FindObjectOfType<HoleManager>();
            if (holeManager != null)
            {
                holeManager.Magnet();
            }

            CloseMagnetPopup();
        }
        else
        {
            ShowNotEnoughCoinText();
        }
    }

    // Compass
    public void Compass()
    {
        // Thêm tham số 'true' để Unity tìm cả những Object đang bị ẩn (tàng hình)
        TargetPointer[] pointers = Resources.FindObjectsOfTypeAll<TargetPointer>();
        if (pointers.Length > 0)
        {
            TargetPointer pointer = pointers[0];
            // Bắt buộc phải bật GameObject lên thì Script mới chạy được bộ đếm thời gian (Coroutine)
            pointer.gameObject.SetActive(true); 
            pointer.ActivateCompass(10f);
            audioManager.playCompassSound();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy TargetPointer trong Scene!");
        }
    }

    public void CloseCompassPopup(){
        audioManager.playBtnClickSound();
        if (coin != null) coin.SetActive(false);
        if(CompassPopup != null){
            CompassPopup.SetActive(false);
            Time.timeScale = 1f; 
        }        
    }

    public void OpenCompassPopup(){
        audioManager.playBtnClickSound();
        if (coin != null) coin.SetActive(true);
        if(CompassPopup != null){
            if (compassCostText != null) compassCostText.text = compassCost.ToString();
            CompassPopup.SetActive(true);
            Time.timeScale = 0f; 
        }        
    }

    public void BuyCompassWithCoin()
    {
        audioManager.playBtnClickSound();
        int totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        if (totalGold >= compassCost)
        {
            totalGold -= compassCost;
            PlayerPrefs.SetInt("TotalGold", totalGold);
            PlayerPrefs.Save();
            
            UpdateInGameTotalGold();

            // Gọi thẳng hàm Compass() đã được viết sẵn trong GamePlayManager
            Compass();

            CloseCompassPopup();
        }
        else
        {
            ShowNotEnoughCoinText();
        }
    }

    // Freeze
    public void Freeze()
    {
        audioManager.playFreezeSound();
        StartCoroutine(FreezeRoutine());
    }

    private System.Collections.IEnumerator FreezeRoutine()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.isTimeFreezed = true;
        }
        
        if (freeze != null) freeze.SetActive(true);
        
        yield return new WaitForSeconds(10f);
        
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.isTimeFreezed = false;
        }
        
        if (freeze != null) freeze.SetActive(false);
    }

    public void CloseFreezePopup(){
        if (coin != null) coin.SetActive(false);
        if(FreezePopup != null){
            audioManager.playBtnClickSound();
            FreezePopup.SetActive(false);
            Time.timeScale = 1f; 
        }        
    }

    public void OpenFreezePopup(){
        audioManager.playBtnClickSound();
        if (coin != null) coin.SetActive(true);
        if(FreezePopup != null){
            if (freezeCostText != null) freezeCostText.text = freezeCost.ToString();
            FreezePopup.SetActive(true);
            Time.timeScale = 0f; 
        }        
    }

    public void BuyFreezeWithCoin()
    {
        audioManager.playBtnClickSound();
        int totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        if (totalGold >= freezeCost)
        {
            totalGold -= freezeCost;
            PlayerPrefs.SetInt("TotalGold", totalGold);
            PlayerPrefs.Save();
            
            UpdateInGameTotalGold();

            // Gọi thẳng hàm Freeze() đã được viết sẵn trong GamePlayManager
            Freeze();

            CloseFreezePopup();
        }
        else
        {
            ShowNotEnoughCoinText();
        }
    }

    private void ShowNotEnoughCoinText()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Tạo một Text UI linh động để đảm bảo nó luôn đè lên trên cùng của mọi Popup
        GameObject uiTextObj = new GameObject("NotEnoughCoinText");
        uiTextObj.transform.SetParent(canvas.transform, false);
        
        // Gắn TextMeshPro UI vào
        TextMeshProUGUI tmpUI = uiTextObj.AddComponent<TextMeshProUGUI>();
        tmpUI.text = "Không đủ xu";
        tmpUI.color = Color.white;
        tmpUI.fontSize = 80; // Kích thước chữ lớn một chút để dễ đọc
        tmpUI.alignment = TextAlignmentOptions.Center;
        
        // Không cho phép xuống dòng, ép chữ luôn nằm trên 1 hàng ngang
        tmpUI.enableWordWrapping = false;
        tmpUI.overflowMode = TextOverflowModes.Overflow;
        
        // Đặt vị trí ban đầu nằm ở giữa màn hình (hoặc ngay trên con Capybara)
        RectTransform rect = uiTextObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -200); // Nằm dưới tâm màn hình một chút
        
        // Chỉnh kích thước của khung UI rộng ra để chữ không bị bóp
        rect.sizeDelta = new Vector2(800, 200);
        
        // Hiệu ứng bay lên và mờ dần
        // Bắt buộc dùng SetUpdate(true) để hiệu ứng vẫn mượt kể cả khi Game đang bị đóng băng (Time.timeScale = 0)
        rect.DOAnchorPosY(100, 1.5f).SetUpdate(true);
        tmpUI.DOFade(0, 1.5f).SetUpdate(true).OnComplete(() =>
        {
            // Hủy object sau khi hiệu ứng kết thúc
            Destroy(uiTextObj);
        });
    }
}
