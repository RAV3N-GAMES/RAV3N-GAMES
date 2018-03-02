using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColor : MonoBehaviour
{
    public List<SpriteRenderer> sprite;
    //색 알파값은 에디터에서 100으로 설정해 주세요
    [Tooltip("초록색")]
    public Color SuccessColor;//초록색
    [Tooltip("빨간색")]
    public Color FailColor;//빨간색

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
