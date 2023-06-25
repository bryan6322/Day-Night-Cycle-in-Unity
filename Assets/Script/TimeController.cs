using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeController : MonoBehaviour
{
    [Header("Time Infos")]
    [SerializeField]
    private float timeMultiplier = 2000f; // Multiplier for time progression

    [SerializeField]
    private float startHour = 12f; // Starting hour of the game

    [SerializeField]
    private TextMeshProUGUI timeText; // Text component to display the current time

    private DateTime currentTime; // Current time in the game

    [Header("Light Infos")]
    [SerializeField]
    private Light sunLight; // Sunlight component

    [SerializeField]
    private float maxSunLightIntensity = 1f; // Maximum intensity of the sunlight

    [SerializeField]
    private Light moonLight; // Moonlight component

    [SerializeField]
    private float maxMoonLightIntensity = 0.5f; // Maximum intensity of the moonlight

    [SerializeField]
    private float sunriseHour = 7f; // Hour of sunrise

    [SerializeField]
    private float sunsetHour = 20.5f; // Hour of sunset

    [SerializeField]
    private Color dayAmbientLight = new Color(0.5607843f, 0.6039216f, 0.682353f); // Ambient light color during the day

    [SerializeField]
    private Color nightAmbientLight = new Color(0.1568628f, 0.3137255f, 0.3921569f); // Ambient light color during the night

    [Space]
    [SerializeField]
    private AnimationCurve lightChangeCurve; // Curve for controlling light intensity changes

    private TimeSpan sunrinseTime; // Time of sunrise
    private TimeSpan sunsetTime; // Time of sunset

    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour); // Set the initial current time

        sunrinseTime = TimeSpan.FromHours(sunriseHour); // Set the sunrise time
        sunsetTime = TimeSpan.FromHours(sunsetHour); // Set the sunset time
    }

    void Update()
    {
        UpdateTimeOfDay(); // Update the current time
        RotateSun(); // Rotate the sun based on the time
        UpdateTimeSettings(); // Update time-related settings
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier); // Update the current time based on the time multiplier

        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm"); // Display the current time in the assigned text component
        }
    }

    private void RotateSun()
    {
        float sunLightRotation;

        if (currentTime.TimeOfDay > sunrinseTime && currentTime.TimeOfDay < sunsetTime)
        {
            // Calculate the percentage of time passed between sunrise and sunset
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunrinseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunrinseTime, currentTime.TimeOfDay);
            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage); // Rotate the sun from 0 to 180 degrees based on the percentage
        }
        else
        {
            // Calculate the percentage of time passed between sunset and sunrise
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunrinseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);
            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage); // Rotate the sun from 180 to 360 degrees based on the percentage
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right); // Apply the sun's rotation
    }

    private void UpdateTimeSettings()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down); // Calculate the dot product between the sun's forward direction and downward direction

        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct)); // Set the sunlight intensity based on the dot product and the light change curve

        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct)); // Set the moonlight intensity based on the dot product and the light change curve

        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct)); // Set the ambient light color based on the dot product and the light change curve
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24); // If the difference is negative, add 24 hours to account for the time crossing into the next day
        }

        return difference;
    }
}
