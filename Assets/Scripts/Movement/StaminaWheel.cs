using TPC;
using TPC.Movement;
using TPC.StateMachine;
using UnityEngine;
using UnityEngine.UI;

public class StaminaWheel : MonoBehaviour
{
    [SerializeField] private Stamina stamina;
    [SerializeField] private Health health;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider usageSlider;
    [SerializeField] private Image healthBar;
    [SerializeField] private float modifierX = 50f;
    [SerializeField] private float modifierY = -50f;

    private void Awake()
    {
        if (stamina == null)
            stamina = GameObject.FindObjectOfType<Stamina>();
        if (health == null)
            health = GameObject.FindObjectOfType<PlayerStateMachine>().GetComponent<Health>();
    }

    private void Update()
    {
        if (stamina.CurrentStamina == stamina.MaxStamina)
        {
            staminaSlider.gameObject.SetActive(false);
            usageSlider.gameObject.SetActive(false);
        }
        else
        {
            staminaSlider.gameObject.SetActive(true);
            usageSlider.gameObject.SetActive(true);
        }

        Vector3 playerPosition = stamina.transform.position;
        Vector3 offset = new Vector3(modifierX, modifierY, 0f); // Adjust as needed
        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerPosition);

        screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);

        usageSlider.transform.position = screenPos + offset;
        staminaSlider.transform.position = screenPos + offset;

        usageSlider.value = stamina.CurrentStamina / stamina.MaxStamina * 1.05f;
        staminaSlider.value = stamina.CurrentStamina / stamina.MaxStamina;

        healthBar.fillAmount = health.GetHealthPercentage();
    }
}