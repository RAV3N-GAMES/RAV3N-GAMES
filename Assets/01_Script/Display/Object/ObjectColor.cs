using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColor : MonoBehaviour
{
    public List<SpriteRenderer> sprite;

    public Color SuccessColor;
    public Color FailColor;

    void SetSpriteColor(Color _color)
    {
        for (int i = 0; i < sprite.Count; i++)
        {
            sprite[i].color = _color;
        }
    }

    public void OnColor(bool possible)
    {
        if (possible)
            SetSpriteColor(SuccessColor);
        else
            SetSpriteColor(FailColor);
    }

    public void OffColor()
    {
        SetSpriteColor(new Color(255, 255, 255, 255));
    }

}
