using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    public static MovePieces instance;
    TileManager game;

    NodePiece moving;
    Point newIndex;
    Vector2 mouseStart;

    public Point flipPieceValue;
    public Point startP;
    public Point endP;
    public bool match = false;
    //public Point FlipPieceValue { get { return flipPieceValue; } set { value = flipPieceValue; } }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        game = GetComponent<TileManager>();
    }

    void Update()
    {
        if (moving != null)
        {
            Vector2 dir = ((Vector2)Input.mousePosition - mouseStart);
            Vector2 nDir = dir.normalized;
            Vector2 aDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            newIndex = Point.clone(moving.index);
            Point add = Point.zero;
            if (dir.magnitude > 32) // if our mouse is 32 pixels away from the starting point of the mouse
            {
                //make add either (1, 0) | (-1, 0) | (0, 1) | (0, -1) depending on the direction of the mouse point
                if (aDir.x > aDir.y)
                    add = (new Point((nDir.x > 0) ? 1 : -1, 0));
                else if (aDir.y > aDir.x)
                    add = (new Point(0, (nDir.y > 0) ? -1 : 1));
            }
            newIndex.add(add);

            Vector2 pos = game.getPositionFromPoint(moving.index);
            if (!newIndex.Equals(moving.index))
                pos += Point.mult(new Point(add.x, -add.y), 16).ToVector();
            moving.MovePositionTo(pos);
        }
    }

    public void MovePiece(NodePiece piece)
    {
        if (moving != null || match) return;
        moving = piece;
        mouseStart = Input.mousePosition;
        startP = moving.index;
    }

    public void DropPiece()
    {
        if (moving == null || match) return;
        if (!newIndex.Equals(moving.index) && game.GetStateAtPoint(newIndex) == 1)
        {
            SetFlipPieceValue(moving.index, newIndex);
            game.flipPieces(moving.index, newIndex, true);
        }
        else
            game.ResetPiece(moving);
        endP = moving.index;


        moving = null;
        match = true;
        game.isTurn = false;
        game.Chain = 0;
    }

    public void SetFlipPieceValue(Point mv, Point iv)
    {
        if(game.GetValueAtPoint(mv) < game.GetValueAtPoint(iv))
        {
            flipPieceValue.x = game.GetValueAtPoint(iv);
            flipPieceValue.y = game.GetValueAtPoint(mv);
        }
        else
        {
            flipPieceValue.x = game.GetValueAtPoint(mv);
            flipPieceValue.y = game.GetValueAtPoint(iv);
        }
    }
}
