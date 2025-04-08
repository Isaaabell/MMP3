using System;

[Serializable]
public class HighScoreElement
{
    public int place;
    public string name;
    public int score;


    public HighScoreElement(int place, string name, int score)
    {
        this.place = place;
        this.name = name;
        this.score = score;

    }
}
