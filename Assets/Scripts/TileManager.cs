using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public ArrayLayout boardLayout;
    public Sprite[] pieces;
    private int width = 9;
    private int height = 14;
    Node[,] board;

    System.Random random;

    void Start()
    {

    }

    void Set()
    {
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());

        InitializeBoard();
    }

    void InitializeBoard()
    {
        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1 : fillPiece(), new Point(x, y));
            }
        }
    }

    void VerifyBoard()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Point p = new Point(x, y);
            }
        }
    }

    List<Point> isConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = GetValueAtPoint(p);
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach (Point dir in directions) //바꾸는 방향에 도형이 2개 이상 있는지 확인
        {
            List<Point> line = new List<Point>();

            int same = 0;
            for (int i = 0; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if (GetValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1)               //바꾸는 방향에 같은 모양이 1개 이상 있으면 일치
                AddPoints(ref connected, line);     //이 지점을 연결 목록에 추가

        }

        for (int i = 0; i < 2; i++) //Checking if we are in the middle of two of the same shapes
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };
            foreach (Point next in check) //Check both sides of the piece, if they are the same value, add them to the list
            {
                if (GetValueAtPoint(next) == val)
                {
                    line.Add(p);
                    same++;
                }
            }

            if (same > 1)
                AddPoints(ref connected, line);
        }

        for(int i = 0; i < 4; i++) //Check for a 2x2
        {
            List<Point> square = new List<Point>(i);

            int same = 0;
            int next = i + 1;
            if (next >= 4)
                next -= 4;

            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[next]), Point.add(p, Point.add(directions[i], directions[next])) };
            foreach (Point pnt in check) //Check both sides of the piece, if they are the same value, add them to the list
            {
                if (GetValueAtPoint(pnt) == val)
                {
                    square.Add(p);
                    same++;
                }
            }

            if (same > 2)
                AddPoints(ref connected, square);
        }

        if(main) //Checks for other 
        {
            for (int i = 0; i < connected.Count; i++)
                AddPoints(ref connected, isConnected(connected[i], false));
        }
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {

    }

    int fillPiece()
    {
        int val = 1;
        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        return val;
    }

    int GetValueAtPoint(Point P)
    {
        return board[P.x, P.y].value;
    }

    void Update()
    {

    }

    string getRandomSeed()
    {
        string seed = "";
        string aChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for (int i = 0; i < 20; i++)
            seed += aChars[Random.Range(0, aChars.Length)];
        return seed;
    }
}

[System.Serializable]
public class Node
{
    public int value;
    public Point index;

    public Node(int v, Point i)
    {
        value = v;
        index = i;
    }
}
