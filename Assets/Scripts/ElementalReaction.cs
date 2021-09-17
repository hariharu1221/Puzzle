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

    void WaterFire()   //�ٲ� ���� �������� 3x3���� ����
    {
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                game.addDeadPiece(new Point(mp.endP.x + x, mp.endP.y + y));
    }

    void WaterGrass()
    {
        
    }

    void WaterLight()    //�ٲ� ���� �������� 7x7���� �� ���Ҹ� ����
    {
        for (int x = -3; x <= 3; x++)
            for (int y = -3; y <= 3; y++)
                if(game.GetValueAtPoint(new Point(mp.endP.x + x, mp.endP.y + y)) == 1)  game.addDeadPiece(new Point(mp.endP.x + x, mp.endP.y + y));
    }

    void WaterIce()     //�ٲ� ������ �������� �� 3��- ���� ���� state���� 2�� �ٲ�---
    {
        Point endP = mp.endP;
        Point startP = mp.startP;
        int x = 0;
        int y = 0;

        if (startP.x > endP.x)
            x = -1;
        else if (startP.x < endP.x)
            x = 1;
        else if (startP.y > endP.y)
            y = -1;
        else if (startP.y < endP.y)
            y = 1;

        if (x == 0)
            for (int lx = -1; lx <= 1; lx++)
                game.setStateAtPoint(new Point(mp.endP.x + lx, mp.endP.y + y), 2);
        if (y == 0)
            for (int ly = -1; ly <= 1; ly++)
                game.setStateAtPoint(new Point(mp.endP.x + x, mp.endP.y + ly), 2);

    }
}
