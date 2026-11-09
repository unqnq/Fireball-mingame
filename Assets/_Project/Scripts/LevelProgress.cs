using UnityEngine;

// Current level number, saved in PlayerPrefs.
public static class LevelProgress
{
    private const string Key = "Level";

    public static int Current => PlayerPrefs.GetInt(Key, 1);

    public static void CompleteCurrent()
    {
        PlayerPrefs.SetInt(Key, Current + 1);
        PlayerPrefs.Save();
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(Key);
    }
}
