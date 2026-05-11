using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hiển thị danh sách trái tim trong UI, "lắng nghe" HealthSystem của Player
/// </summary>
public class HeartDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthSystem playerHealth;     // Kéo Player có HealthSystem vào
    [SerializeField] private Image heartPrefab;              // Prefab là Image (Sprite trái tim)
    [SerializeField] private Transform heartsContainer;      // Panel/Horizontal Layout Group chứa tim

    private Image[] hearts;

    private void Start()
    {
        // Tạo sẵn các trái tim dựa trên máu tối đa
        CreateHearts();
        UpdateHearts(playerHealth.CurrentHP);
    }

    private void OnEnable()
    {
        // Đăng ký sự kiện khi máu thay đổi
        if (playerHealth != null)
            playerHealth.OnHPChanged.AddListener(UpdateHearts);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHPChanged.RemoveListener(UpdateHearts);
    }

    /// <summary>
    /// Sinh số tim bằng maxHP
    /// </summary>
    private void CreateHearts()
    {
        hearts = new Image[playerHealth.MaxHP];

        for (int i = 0; i < playerHealth.MaxHP; i++)
        {
            Image newHeart = Instantiate(heartPrefab, heartsContainer);
            hearts[i] = newHeart;
        }
    }

    /// <summary>
    /// Bật/tắt tim dựa trên HP hiện tại
    /// </summary>
    private void UpdateHearts(int currentHP)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // Tim thứ i "đầy" nếu i < currentHP, ngược lại là "rỗng"
            hearts[i].enabled = i < currentHP;
            // Hoặc bạn có thể đổi màu: hearts[i].color = i < currentHP ? Color.red : Color.gray;
        }
    }

    private void UpdateHearts(int currentHP, int maxHP)
    {
        // Khi OnHPChanged gọi với 2 tham số, ta chỉ cần currentHP
        UpdateHearts(currentHP);
    }
}