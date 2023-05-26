using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarSprite;

    public void UpdateHealthBar(float maxHealth, float currentHealth) //updates the fill level on the UI meter.
    {
        _healthBarSprite.fillAmount = currentHealth / maxHealth;
    }
}
