using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    public float drainRate = 20f;
    
    public float recoveryRate = 15f;
    
    public float maxStamina = 100f;
    
    public float currentStamina = 100f;

    public float recoveryDelay = 2f;

    private float recoveryTimer;

    void Update()
    {
        if (recoveryTimer > 0)
        {
            recoveryTimer -= Time.deltaTime;
        }
        else
        {
            currentStamina += recoveryRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
    }

    public void StartRecoveryDealy()
    {
        recoveryTimer = recoveryDelay;
    }

    public void StopUsingStamina()
    {
        recoveryTimer = recoveryDelay;
    }

    public bool CanSprint()
    {
        return currentStamina > 0;
    }
}