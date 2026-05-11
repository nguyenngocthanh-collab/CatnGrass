using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Đây là nơi chứa 3 cục máu của mèo
    public int health = 3;

    // Hàm này sẽ được cây gai gọi để trừ máu
    public void TakeDamage(int amount)
    {
        health -= amount; // Trừ đi số lượng máu (thường là 1)
        Debug.Log("Mèo bị thương! Máu còn lại: " + health);

        if (health <= 0)
        {
            Debug.Log("Hết máu! Game Over.");
            // Sau này bạn có thể thêm lệnh để hiện màn hình "Thua cuộc" ở đây
        }
    }
}