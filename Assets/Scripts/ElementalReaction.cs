using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ElementalReaction : MonoBehaviour
{
    MovePieces mp;
    TileManager game;

    void Start()
    {
        mp = GetComponent<MovePieces>();
        game = GetComponent<TileManager>();
    }

    public void elementalReaction()
    {
        Point el = mp.flipPieceValue;
        if (el.x == 2 && el.y == 1)   WaterFire();
        if (el.x == 3 && el.y == 1)   WaterGrass();
        if (el.x == 4 && el.y == 1)   WaterLight();
        if (el.x == 5 && el.y == 1)   WaterIce();
    }

    void WaterFire()   //바꾼 블럭을 기준으로 3x3블럭이 터짐
    {
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                game.addDeadPiece(new Point(mp.svmove.x + x, mp.svmove.y + y));
    }

    void WaterGrass()
    {
        
    }

    void WaterLight()    //바꾼 블럭을 기준으로 7x7내의 물 원소를 지움
    {
        for (int x = -3; x <= 3; x++)
            for (int y = -3; y <= 3; y++)
                if(game.GetValueAtPoint(new Point(mp.svmove.x + x, mp.svmove.y + y)) == 1)  game.addDeadPiece(new Point(mp.svmove.x + x, mp.svmove.y + y));
    }

    void WaterIce()
    {

    }
}
