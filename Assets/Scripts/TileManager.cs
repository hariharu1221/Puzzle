using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class TileManager : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;

    [Header("Prefabs")]
    public GameObject nodePiece;
    public GameObject statePiece;

    [Header("Text")]
    public Text scoreText;

    public int width = 9;
    public int height = 14 ;
    int[] fills;
    Node[,] board;

    ElementalReaction er;
    MovePieces mp;

    List<NodePiece> update;
    List<FlippedPieces> flipped;
    public List<NodePiece> dead;

    System.Random random;

    float plusSe = 0;
    int score = 0;
    private int chain = 0;
    private bool isturn = false;
    private int turn = 0;

    public int Chain
    {
        get { return chain; }
        set { chain = value; }
    }

    public bool isTurn
    {
        get { return isturn; }
        set { isturn = value; }
    }

    public int Turn
    {
        get { return turn; }
        set { turn = value; }
    }

    void Start()
    {
        Set();
    }

    void Update()
    {
        game_Flip();
        game_Effect();
    }

    void game_Flip()    //플립 업데이트
    {
        List<NodePiece> finishedUpdating = new List<NodePiece>();
        for (int i = 0; i < update.Count; i++)  //업데이트 카운트가 생기면
        {
            NodePiece piece = update[i];
            if (!piece.UpdatePiece()) finishedUpdating.Add(piece);  //완료 업데이트로 넘김
        }
        for (int i = 0; i < finishedUpdating.Count; i++)   //완료 업데이트 카운트 만큼
        {
            NodePiece piece = finishedUpdating[i];
            FlippedPieces flip = getFlipped(piece);
            NodePiece flippedPiece = null;

            int x = (int)piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);

            List<Point> connected = isConnected(piece.index, true);
            bool wasFlipped = (flip != null);

            if (wasFlipped) //만약 플립했을 때 업데이트
            {
                flippedPiece = flip.getOtherPiece(piece);   //저장 했던 피스값과 교환
                AddPoints(ref connected, isConnected(flippedPiece.index, true));    //매칭되었는지 확인
            }
            if (connected.Count == 0)  //매칭이 되지 않은 경우
            {
                if (wasFlipped && flippedPiece != null) //만약 플립한 경우
                    flipPieces(piece.index, flippedPiece.index, false); //Flip back
                mp.match = false;
                Turn++;
            }
            else //매칭이 된 경우
            {
                isTurn = true;
                if (isTurn) Chain++;
                foreach (Point pnt in connected) //연결된 원소 제거
                {
                    addDeadPiece(pnt);
                }
                if (i == finishedUpdating.Count - 1 && mp.match) //원소 반응 실행
                {
                    plusSe = 0.5f;
                    if (mp.match) Invoke("Element", 0.5f);
                    mp.match = false;
                }
            }

            flipped.Remove(flip);  //업데이트 후 플립 제거
            update.Remove(piece);
            if (i == finishedUpdating.Count - 1)
                Invoke("ApplyGravityToBoard", (0.2f + plusSe)); //원소 내려옴
        }
    }

    void game_Effect()
    {
        
        scoreText.text = "Score: " + score + " Chain: " + chain;
    }

    void Element()
    {
        er.elementalReaction(); //원소 반응
    }

    void ApplyGravityToBoard() //중력 작용
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = (height-1); y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int val = GetValueAtPoint(p);
                if (val != 0) continue; //If it is not a hole, do nothing
                for(int ny = (y-1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextval = GetValueAtPoint(next);
                    if (nextval == 0)
                        continue;
                    if (nextval != -1)  //If we did not hit an end, but its not 0 then use this to fill the current hole
                    {
                        Node got = getNodeAtPoint(next);
                        NodePiece piece = got.getPiece();

                        //Set the hole
                        node.SetPiece(piece);
                        update.Add(piece);

                        //Replace the hole
                        got.SetPiece(null);
                    }
                    else //Hit an end
                    {
                        //Fill in the whole
                        int newVal = fillPiece();
                        NodePiece piece;
                        Point fallPnt = new Point(x, (-1 - fills[x]));
                        if (dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            revived.rect.anchoredPosition = getPositionFromPoint(fallPnt);
                            piece = revived;

                            dead.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            rect.anchoredPosition = getPositionFromPoint(fallPnt);
                            piece = n;

                        }
                        piece.Initialize(newVal, p, pieces[newVal - 1], 1, piece.getStateValue());

                        Node hole = getNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    break;
                }
            }
        }

        plusSe = 0;
    }

    FlippedPieces getFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].getOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }

    void Set()
    {
        fills = new int[width];
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        er = GetComponent<ElementalReaction>();
        mp = GetComponent<MovePieces>();
        scoreText.text = "Score: " + score;

        InitializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }

    void InitializeBoard()
    {
        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1 : fillPiece(), new Point(x, y), 1);
            }
        }
    }

    void VerifyBoard()
    {
        List<int> remove;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Point p = new Point(x, y);
                int val = GetValueAtPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();
                while(isConnected(p, true).Count > 0)
                {
                    val = GetValueAtPoint(p);
                    if (!remove.Contains(val))
                        remove.Add(val);
                    setValueAtPoint(p, newvalue(ref remove));
                }
            }
        }
    }

    void InstantiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Node node = getNodeAtPoint(new Point(x, y));

                int val = node.value;
                int sta = node.state;
                if (val <= 0) continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rectPiece = p.GetComponent<RectTransform>();
                rectPiece.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));

                GameObject s = Instantiate(statePiece, gameBoard);
                StateValue spiece = s.GetComponent<StateValue>();
                RectTransform srectPiece = s.GetComponent<RectTransform>();
                srectPiece.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                s.transform.parent = p.transform;
                s.transform.SetParent(p.transform);

                spiece.Initialize(sta, new Point(x, y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1], sta, s);

                node.SetPiece(piece);
            }
        }
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        //piece.flipped = null;
        update.Add(piece);
    }

    public void flipPieces(Point one, Point two, bool main)
    {
        if (GetValueAtPoint(one) < 0) return;

        Node nodeOne = getNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.getPiece();
        if (GetValueAtPoint(two) > 0)
        {
            Node nodeTwo = getNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.getPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if (main)
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else
            ResetPiece(pieceOne);
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
            for (int i = 1; i < 3; i++)
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
                    line.Add(next);
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
                    square.Add(pnt);
                    same++;
                }
            }

            if (same > 2)
                AddPoints(ref connected, square);
        }

        if(main) //Checks for other matches along the current
        {
            for (int i = 0; i < connected.Count; i++)
                AddPoints(ref connected, isConnected(connected[i], false));
        }

        /* UNNESSASARY | REMOVE THIS
        if (connected.Count > 0)
            connected.Add(p);
        */
        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach(Point p in add)
        {
            bool doadd = true;
            for(int i = 0; i < points.Count; i++)
            {
                if(points[i].Equals(p))
                {
                    doadd = false;
                    break;
                }
            }

            if (doadd) points.Add(p);
        }
    }

    int fillPiece() //value 랜덤 설정
    {
        int val = 1;
        //val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        val = random.Next(1, pieces.Length + 1);
        return val;
    }

    public int GetStateAtPoint(Point P)
    {
        if (P.x < 0 || P.x >= width || P.y < 0 || P.y >= height) return -1;
        return board[P.x, P.y].state;
    }

    public void setStateAtPoint(Point P, int s) ////set p위치의 state = 원소 상태
    {
        if (P.x < 0 || P.x >= width || P.y < 0 || P.y >= height) return;
        if (board[P.x, P.y].value < 1) return;
        board[P.x, P.y].state = s;
        Node node = getNodeAtPoint(P);
        NodePiece nodePiece = node.getPiece();
        nodePiece.SetState(s);
    }

    public int GetValueAtPoint(Point P)
    {
        if (P.x < 0 || P.x >= width || P.y < 0 || P.y >= height) return -1;
        return board[P.x, P.y].value;
    }

    void setValueAtPoint(Point p, int v)    //set p위치의 value = 원소 종류
    {
        board[p.x, p.y].value = v;
    }

    public void SetValue(Point p, int v)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return;
        if (board[p.x, p.y].value < 1) return;
        board[p.x, p.y].value = v;
        Node node = getNodeAtPoint(p);
        NodePiece piece = node.getPiece();
        piece.SetValue(v, pieces[v - 1]);
    }

    public Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    int newvalue(ref List<int> remove)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < pieces.Length; i++)
            available.Add(i + 1);
        foreach (int i in remove)
            available.Remove(i);

        if (available.Count <= 0) return 0;
        return available[random.Next(0, available.Count)];
    }

    public string getRandomSeed()  //랜덤 시드
    {
        string seed = "";
        string aChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for (int i = 0; i < 20; i++)
            seed += aChars[Random.Range(0, aChars.Length)];
        return seed;
    }

    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(32 + (64 * p.x), -32 - (64 * p.y));
    }

    public void addDeadPiece(Point p, int score = 100) //p위치의 블럭을 지우고 점수를 얻음
    {
        if (GetValueAtPoint(p) < 0) return;
        Node node = getNodeAtPoint(p);
        NodePiece nodePiece = node.getPiece();

        if (GetStateAtPoint(p) == 1)
        {
            if (nodePiece != null)
            {
                nodePiece.gameObject.SetActive(false);
                dead.Add(nodePiece);
            }
            node.SetPiece(null);
        }
        else if (GetStateAtPoint(p) == 2)
        {
            setStateAtPoint(p, 1);
        }

        this.score += score * ((Chain + 5) / 5);
    }
}

[System.Serializable]
public class Node
{
    public int value;
    public int state;
    public Point index;
    NodePiece piece;

    public Node(int v, Point i, int s)
    {
        value = v;
        index = i;
        state = s;
    }

    public void SetPiece(NodePiece p)
    {
        piece = p;
        state = (piece == null) ? 0 : piece.state;
        value = (piece == null) ? 0 : piece.value;
        if (piece == null) return;
        piece.SetIndex(index);
    }

    public NodePiece getPiece()
    {
        return piece;
    }
}


[System.Serializable]
public class FlippedPieces
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece o, NodePiece t)
    {
        one = o; two = t;
    }

    public NodePiece getOtherPiece(NodePiece p)
    {
        if (p == one)
            return two;
        else if (p == two)
            return one;
        else
            return null;
    }
}