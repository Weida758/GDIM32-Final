using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void Start()
    {
        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.OnGoldChanged += UpdateGold;
            UpdateGold(PlayerWallet.Instance.Gold);
        }
    }

    private void OnDestroy()
    {
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.OnGoldChanged -= UpdateGold;
    }

    private void UpdateGold(int total)
    {
        goldText.text = total.ToString();
    }
}