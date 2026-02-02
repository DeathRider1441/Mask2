using System;
using UnityEngine;

public static class GameEvents
{
    // Un singur eveniment care transmite un nume (string)
    public static event Action<string> OnPlaySound;
    public static event System.Action<string> OnStopSound;
    public static event System.Action<Vector3, float> OnNoiseMade;

    // Evenimente noi pentru starea jucătorului
    public static event Action OnPlayerDeath;
    public static event Action<int> OnPlayerHit; // Trimitem viața rămasă ca parametru
    public static System.Action OnLevelWin;
    public static void TriggerLevelWin() => OnLevelWin?.Invoke();

    // ... restul evenimentelor tale (OnPlaySound, etc.) ...

    public static void TriggerPlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }

    public static void TriggerPlayerHit(int currentHealth)
    {
        OnPlayerHit?.Invoke(currentHealth);
    }



    public static void TriggerSound(string soundName)
    {
        OnPlaySound?.Invoke(soundName);
    }


    public static void TriggerStopSound(string soundName)
    {
        OnStopSound?.Invoke(soundName);
    }


    public static void TriggerNoise(Vector3 position, float range)
    {
        OnNoiseMade?.Invoke(position, range);
    }
}