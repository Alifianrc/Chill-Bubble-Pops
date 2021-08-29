using UnityEngine;

// Contain user data

[System.Serializable]
public class SaveData
{
    private int highScore;

    public SaveData()
    {
        highScore = 0;
    }
    public SaveData(int value)
    {
        highScore = value;
    }

    public int GetHighScore()
    {
        return highScore;
    }
    public void SetHighScore(int value)
    {
        highScore = value;
    }

}
