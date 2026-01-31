using System;

public static class GameEvents
{
    // Un singur eveniment care transmite un nume (string)
    public static event Action<string> OnPlaySound;
    public static event System.Action<string> OnStopSound;


    public static void TriggerSound(string soundName)
    {
        OnPlaySound?.Invoke(soundName);
    }


    public static void TriggerStopSound(string soundName)
    {
        OnStopSound?.Invoke(soundName);
    }
}