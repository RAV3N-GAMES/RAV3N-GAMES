using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColor : MonoBehaviour
{
    [Tooltip("0번에는 오브젝트 이미지를 넣으세요.")]
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

    public void SetSortingOrder(string layer, int idx)
    {
        for(int i = 0; i < sprite.Count; i++)
        {
            sprite[i].sortingLayerName = layer;
            sprite[i].sortingOrder = idx;
        }

        sprite[0].sortingOrder = idx + 1;
    }

    public void OnTransparency(bool isTransparency)
    {
        if (isTransparency)
        {
            SetSpriteColor(new Color(1, 1, 1, 0.3f));
        }
        else
            SetSpriteColor(new Color(1, 1, 1, 1));
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
        float alpha = 1;
        if (RoomManager.isTransparency)
        {
            alpha = 0.3f;
            SetSpriteColor(new Color(1, 1, 1, 0.3f));
        }
        else
            SetSpriteColor(new Color(1, 1, 1, 1));

        sprite[0].color = new Color(1, 1, 1, alpha);
    }

}
