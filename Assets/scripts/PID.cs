using Unity.VisualScripting;
using UnityEngine;

public class PIDController
{
    private float P;
    private float I;
    private float D;
    private float targetValue;
    private float currentValue;
    private float integral;
    private float previousError;
    private float controlOutput;

    public PIDController(float p, float i, float d)
    {
        P = p;
        I = i;
        D = d;
        targetValue = 0f;
        currentValue = 0f;
        integral = 0f;
        previousError = 0f;
    }

    public void SetTargetValue(float target)
    {
        //make sure target is not nan, or it could contaminate the PID
        if (float.IsNaN(target))
        {
            return;
        }
        targetValue = target;
    }

    public void Update(float deltaTime)
    {
        // Calculate the error
        float error = targetValue - currentValue;

        // Calculate the proportional term
        float proportionalTerm = P * error;

        // Calculate the integral term and prevent wind-up
        integral += error * deltaTime;
        integral = Mathf.Clamp(integral, -1f, 1f);

        // Calculate the derivative term
        float derivativeTerm = D * (error - previousError) / deltaTime;

        // Calculate the control output
        controlOutput = proportionalTerm + (I * integral) + derivativeTerm;

        // Update previous error for the next frame
        previousError = error;
    }

    public void SetCurrentValue(float value)
    {
        currentValue = value;
    }

    public float GetControlOutput()
    {
        return controlOutput;
    }
}