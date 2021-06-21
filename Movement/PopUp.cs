using UnityEngine;

public struct PopUp
{
    public string text; 
    public Color32 textColor; 
    public float timer; 
    public float fadeTimer;

    public PopUp(string text, Color32 textColor, float timer, float fadeTimer)
    {
        this.text = text;
        this.textColor = textColor;
        this.timer = timer;
        this.fadeTimer = fadeTimer;
    }
}