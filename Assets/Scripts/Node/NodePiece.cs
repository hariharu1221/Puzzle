using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour , IPointerDownHandler , IPointerUpHandler
{
    [Header("값 설정")]
    public int value;
    public int state;
    public Point index;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;


    SpriteRenderer sprite;
    bool updating;
    Image img;
    Image onimg;

    GameObject statevalue;

    public void Initialize(int v, Point p, Sprite piece, int s, GameObject stateval)
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        state = s;
        value = v;
        statevalue = stateval;

        SetIndex(p);

        img.sprite = piece;
    }

    public void SetState(int s)
    {
        state = s;
        statevalue.GetComponent<StateValue>().SetIndex(index, state);
    }
    
    public void SetValue(int v, Sprite piece)
    {
        value = v;
        img.sprite = piece;
    }

    public GameObject getStateValue()
    {
        return statevalue;
    }

    public void SetIndex(Point p)
    {
        index = p;
        SetState(state);
        ResetPosition();
        UpdateName();
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
        transform.name = "Node [" + index.x + "," + index.y + "]";
    }

    public bool UpdatePiece()
    {
        if(Vector3.Distance(rect.anchoredPosition,pos) > 1)
        {
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
        return true;
        // return false if it is not moving
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (updating) return;
        if (state == 1) MovePieces.instance.MovePiece(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (state == 1) MovePieces.instance.DropPiece();
    }

    public Texture2D MergeTextures(Texture2D img, Texture2D overlay)
    {

        Color[] cols1 = img.GetPixels();
        Color[] cols2 = overlay.GetPixels();
        for (var i = 0; i < cols1.Length; ++i)
        {
            float rOut = (cols2[i].r * cols2[i].a) + (cols1[i].r * (1 - cols2[i].a));
            float gOut = (cols2[i].g * cols2[i].a) + (cols1[i].g * (1 - cols2[i].a));
            float bOut = (cols2[i].b * cols2[i].a) + (cols1[i].b * (1 - cols2[i].a));
            float aOut = cols2[i].a + (cols1[i].a * (1 - cols2[i].a));

            cols1[i] = new Color(rOut, gOut, bOut, aOut);
        }
        overlay.SetPixels(cols1);
        overlay.Apply();

        return overlay;
    }
}
