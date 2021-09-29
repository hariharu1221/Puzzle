using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateValue : MonoBehaviour
{
    [Header("값")]
    public int state;
    public Point index;

    [Header("스프라이트")]
    public Sprite[] statesSprite;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;

    Image img;

    public void Initialize(int s, Point p)
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        SetIndex(p, s);
    }

    public void SetIndex(Point p, int s)
    {
        index = p;
        state = s;
        ResetPosition();
        UpdateName();
        SetImg();
    }

    public void SetImg()
    {
        img.sprite = statesSprite[state - 1];
        if (state == 1) this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }

    public void ResetPosition()
    {
        pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
    }

    public void MovePosition(Vector2 move)
    {
        rect.anchoredPosition += move * Time.deltaTime * 16f;
    }

    public void MovePositionTo(Vector2 move)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
    }

    void UpdateName()
    {
        transform.name = "State [" + index.x + "," + index.y + "]";
    }
}
