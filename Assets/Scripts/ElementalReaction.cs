using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ElementalReaction : MonoBehaviour
{
    MovePieces mp;
    TileManager game;
    System.Random random;

    void Start()
    {
        mp = GetComponent<MovePieces>();
        game = GetComponent<TileManager>();
        random = new System.Random(game.getRandomSeed().GetHashCode());
    }

    public void elementalReaction(Point p = null)
    {
        Point el = p;
        if (el == null)  el = mp.flipPieceValue;
        if (el.x == 2 && el.y == 1) WaterFire();
        if (el.x == 3 && el.y == 1) WaterGrass();
        if (el.x == 4 && el.y == 1) WaterLight();
        if (el.x == 5 && el.y == 1) WaterIce();
        if (el.x == 6 && el.y == 1) WaterGround();
        if (el.x == 7 && el.y == 1) WaterWind();

        if (el.x == 3 && el.y == 2) FireGrass();
        if (el.x == 4 && el.y == 2) FireLight();
    }

    void WaterFire()   //�ٲ� ���� �������� 3x3���� ����
    {
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                game.addDeadPiece(new Point(mp.endP.x + x, mp.endP.y + y));
    }

    void WaterGrass()  //����
    {
        for (int x = -2; x <= 2; x++)
            for (int y = -2; y <= 2; y++)
                if (game.GetValueAtPoint(new Point(mp.endP.x + x, mp.endP.y + y)) == 3) game.setStateAtPoint(new Point(mp.endP.x + x, mp.endP.y + y), 3);
    }

    void WaterLight()   //�ٲ� ���� �������� 7x7���� �� ���Ҹ� ����
    {
        for (int x = -3; x <= 3; x++)
            for (int y = -3; y <= 3; y++)
                if (game.GetValueAtPoint(new Point(mp.endP.x + x, mp.endP.y + y)) == 1) game.addDeadPiece(new Point(mp.endP.x + x, mp.endP.y + y));
    }

    void WaterIce()     //�ٲ� ������ �������� 7x7���� �� ���Ҹ� ��
    {
        for (int x = -3; x <= 3; x++)
            for (int y = -3; y <= 3; y++)
                if (game.GetValueAtPoint(new Point(mp.endP.x + x, mp.endP.y + y)) == 1) game.setStateAtPoint(new Point(mp.endP.x + x, mp.endP.y + y), 2);
    }

    void WaterGround()
    {
        //�ϼ��ø���
    }

    void WaterWind()    //7x7���� ���� 7���� ���Ҹ� �����ҷ� �ٲ�
    {
        List<Point> check = new List<Point>();
        for (int i = 0; i < 7; i++)
        {
            Point index = new Point(random.Next(mp.endP.x - 3, mp.endP.x + 3), random.Next(mp.endP.y - 3, mp.endP.y + 3));

            for (int j = 0; j < i; j++)
                if (check[j] == index)
                {
                    i--;
                    continue;
                }

            check.Add(index);
            game.SetValue(index, 1);
        }
    }

    void FireGrass()   //�ٲ� ���� �������� 7x7���� Ǯ ���Ҹ� ����
    {
        for (int x = -3; x <= 3; x++)
            for (int y = -3; y <= 3; y++)
                if (game.GetValueAtPoint(new Point(mp.endP.x + x, mp.endP.y + y)) == 3) game.addDeadPiece(new Point(mp.endP.x + x, mp.endP.y + y));
    }

    void FireLight()  //�ٲ� ���� �������� x�� ������� ���� 7��ŭ ���Ҹ� ����
    {
        for (int x = -3; x <= 3; x++)
            for (int y = -3; y <= 3; y++)
                if (Mathf.Abs(x) == Mathf.Abs(y))   game.addDeadPiece(new Point(mp.endP.x + x, mp.endP.y + y));
    }
}


//Point endP = mp.endP;
//Point startP = mp.startP;
//int x = 0;
//int y = 0;

//if (startP.x > endP.x)
//    x = -1;
//else if (startP.x < endP.x)
//    x = 1;
//else if (startP.y > endP.y)
//    y = -1;
//else if (startP.y < endP.y)
//    y = 1;

//if (x == 0)
//    for (int lx = -1; lx <= 1; lx++)
//        game.setStateAtPoint(new Point(mp.endP.x + lx, mp.endP.y + y), 2);
//if (y == 0)
//    for (int ly = -1; ly <= 1; ly++)
//        game.setStateAtPoint(new Point(mp.endP.x + x, mp.endP.y + ly), 2);