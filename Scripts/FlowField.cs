using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Segments;

namespace Segments 
{
    public class LineSegment
    {
        public Vector3 position;
        public Vector3 direction;
        public LineSegment(params Vector3[] _v3)
        {
            position = _v3[0];
            if (_v3.Length > 1)
                direction = _v3[1];
        }
    }
    public class FieldSegment
    {
        public bool isPopulated = false;
        public Vector3 direction;
    }

    public class TempSegment
    {
        public int x, y;
        public Vector3 direction;
        public TempSegment(int _x, int _y, Vector3 _direction)
        {
            x = _x;
            y = _y;
            direction = _direction;
        }
    }
    public struct NeighbourContainer
    {
        public bool hasNeighbours;
        public Vector3 direction;
        public NeighbourContainer(bool _hasNeighbours, Vector3 _direction)
        {
            hasNeighbours = _hasNeighbours;
            direction = _direction;
        }
    }
}
public class FlowField : MonoBehaviour
{
    bool IsDrawing = false;
    bool isEditor = true;
    bool IsLineSuccessful = false;
    //[SerializeField] bool isSmoothStep = true;

    List<LineSegment> mLineList;
    List<LineSegment> mCurrentLineList;
    List<LineSegment> connectedLineList;
    FieldSegment[,] mLineField;
    Vector3 lastLineSegment;
    List<Vector3> border;

    Player mPlayer;

    private void Awake()
    {
        border = new List<Vector3>();
        mCurrentLineList = new List<LineSegment>();
        mPlayer = GameObject.Find("Player").GetComponent<Player>();
        mLineField = new FieldSegment[Lib.sFlowFieldDim, Lib.sFlowFieldDim];
        CreateBorder();
    }
    private void Start()
    {
        isEditor = false;
        ClearField();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (!IsDrawing)
            {
                IsDrawing = true;
                mLineList = new List<LineSegment>();
            }
            bool tryLine = DrawLine();
            if (!IsLineSuccessful && tryLine)
                IsLineSuccessful = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) && IsLineSuccessful)
        {
            IsLineSuccessful = true;
            IsDrawing = false;
            AddLineDirection();
            mCurrentLineList = new List<LineSegment>(mLineList);
            lastLineSegment = new Vector3(0, 0, 999);
            ClearField();
            PopulateLineArray();
        }
    }

    public Vector3 GetDirection(Vector3 position)
    {
        int x = (int)position.x,
            y = (int)position.y;
        if(VectorField.IsInField2D(x,y,Lib.sFlowFieldDim))
        {
            FieldSegment ls = mLineField[x, y];
            if (ls.isPopulated)
                return ls.direction;
        }
        return new Vector3(0, 0, 0);
    }
    public void UpdateSegmentList()
    {
        List<LineSegment> lListTemp = new List<LineSegment>();
        for (int x = 0; x < Lib.sFlowFieldDim; x++)
        {
            for (int y = 0; y < Lib.sFlowFieldDim; y++)
            {
                if(mLineField[x, y].isPopulated)
                {
                    lListTemp.Add(new LineSegment(new Vector3(x, y, 0), 
                        mLineField[x, y].direction));
                }
            }
        }
        mCurrentLineList = lListTemp;
    }
    public void ExpandSegments()
    {
        List<TempSegment> tempLineSegments = new List<TempSegment>();
        for (int x = 0; x < Lib.sFlowFieldDim; x++)
        {
            for (int y = 0; y < Lib.sFlowFieldDim; y++)
            {
                if (mLineField[x, y].isPopulated) continue;

                NeighbourContainer nc = GetNeighbourDirection(x, y);
                if (!nc.hasNeighbours) continue;
                tempLineSegments.Add(new TempSegment(x, y, nc.direction));
            }
        }
        foreach (TempSegment ts in tempLineSegments)
        {
            mLineField[ts.x, ts.y].direction = ts.direction;
            mLineField[ts.x, ts.y].isPopulated = true;
        }
    }
    public void SmoothFlowField()
    {
        List<TempSegment> tempLineSegments = new List<TempSegment>();
        for (int x = 0; x < Lib.sFlowFieldDim; x++)
        {
            for (int y = 0; y < Lib.sFlowFieldDim; y++)
            {
                if (!mLineField[x, y].isPopulated) continue;

                NeighbourContainer nc = GetNeighbourDirection(x, y);
                if (!nc.hasNeighbours) continue;

                tempLineSegments.Add(new TempSegment(x, y,
                    (mLineField[x, y].direction + nc.direction).normalized));
            }
        }
        foreach (TempSegment ts in tempLineSegments)
        {
            mLineField[ts.x, ts.y].direction = ts.direction;
            mLineField[ts.x, ts.y].isPopulated = true;
        }
    }
    void ClearField()
    {
        for (int x = 0; x < Lib.sFlowFieldDim; x++)
        {
            for (int y = 0; y < Lib.sFlowFieldDim; y++)
            {
                mLineField[x, y] = new FieldSegment();
            }
        }
    }

    void PopulateLineArray()
    {
        foreach(LineSegment ls in mCurrentLineList)
        {
            int xPos = (int)ls.position.x,
                yPos = (int)ls.position.y;
            if (xPos < 0 || xPos >= Lib.sFlowFieldDim ||
               yPos < 0 || yPos >= Lib.sFlowFieldDim)
                continue;
            mLineField[xPos, yPos].direction = ls.direction;
            mLineField[xPos, yPos].isPopulated = true;
        }
    }

    NeighbourContainer GetNeighbourDirection(int _x, int _y)
    {
        Vector3 nDirection = Vector3.zero;
        bool hasNeighbours = false;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (!VectorField.IsInField2D(x + _x, y + _y, Lib.sFlowFieldDim))
                    continue;
                if (!mLineField[x + _x, y + _y].isPopulated)
                    continue;
                nDirection += mLineField[x + _x, y + _y].direction;
                hasNeighbours = true;
            }
        }
        if(!hasNeighbours) return new NeighbourContainer(hasNeighbours, nDirection);
        return new NeighbourContainer(hasNeighbours, nDirection.normalized);
    }

    Vector2Int GetPositionFromV3(Vector2Int _pos, Vector3 _dir)
    {
        return new Vector2Int((int)(_pos.x + _dir.x), (int)(_pos.y + _dir.y));
    }
    
    bool DrawLine()
    {
        Vector3 mousePos = Vector3Int.FloorToInt(mPlayer.GetMousPosition());
        if (mousePos.x < 0 || mousePos.y >= Lib.sFlowFieldDim ||
           mousePos.y < 0 || mousePos.y >= Lib.sFlowFieldDim) 
            return false;
        if (lastLineSegment.z < 999)
            if (mousePos != lastLineSegment)
            {
                mLineList.Add(new LineSegment(Vector3Int.FloorToInt(mousePos)));
            }
        lastLineSegment = mousePos;
        return true;
    }
    void AddLineDirection()
    {
        if (mLineList.Count == 0) return;
        Vector3 lastPosition = mLineList[0].position;
        foreach (LineSegment ls in mLineList)
        {
            ls.direction = (lastPosition - ls.position).normalized;
            lastPosition = ls.position;
        }
    }
    void CreateBorder()
    {
        border.Add(new Vector3(-1, -1, 0));
        border.Add(new Vector3(Lib.sFlowFieldDim, -1, 0));
        border.Add(new Vector3(Lib.sFlowFieldDim, Lib.sFlowFieldDim, 0));
        border.Add(new Vector3(-1, Lib.sFlowFieldDim, 0));
        border.Add(new Vector3(-1, -1, 0));
    }

    void OnDrawGizmos()
    {
        if (isEditor || !Lib.sDrawFField) return;
        bool jump = true;
        Vector3 lastVB = new Vector3(0,0,0);
        foreach(Vector3 vB in border)
        {
            if(jump)
            {
                lastVB = vB;
                jump = false;
                continue;
            }
            Gizmos.DrawLine(vB, lastVB);
            lastVB = vB;
        }

        Gizmos.color = Color.green;
        foreach (LineSegment ls in mCurrentLineList)
        {
            Gizmos.DrawLine(ls.position, ls.position + ls.direction);
        }
    }

    ////unused
    //void ConnectSegments()
    //{
    //    Vector3 sPos = Vector3.zero,
    //            sDir = Vector3.zero;
    //    bool start = true;
    //    connectedLineList = new List<LineSegment>();
    //    foreach (LineSegment ls in mCurrentLineList)
    //    {
    //        connectedLineList.Add(ls);
    //        if (start)
    //        {
    //            sPos = ls.position;
    //            sDir = ls.direction;
    //            start = false;
    //            continue;
    //        }
    //        int tries = 50;
    //        Vector2Int vTarget = new Vector2Int((int)ls.position.x, (int)ls.position.y),
    //                   vCurrent = new Vector2Int((int)sPos.x, (int)sPos.y),
    //                   vNew = GetPositionFromV3(vCurrent, sDir);

    //        while(vCurrent != vTarget || tries < 20)
    //        {
    //            tries++;
    //            int multi = 1;

    //            //is sdir 0?
    //            if (sDir.x < 0.01f && sDir.x > -0.01f ||
    //               sDir.y < 0.01f && sDir.y > -0.01f)
    //                break;

    //            while(vNew == vCurrent)
    //            {
    //                //breeak
    //                multi++;
    //                vNew = GetPositionFromV3(vCurrent, sDir * multi);
    //            }
    //            if (vNew.x < 0 || vNew.x >= mLineFieldDimension ||
    //               vNew.y < 0 || vNew.y >= mLineFieldDimension) break;

    //            Vector3 newPos = new Vector3(vNew.x, vNew.y, 0);
    //            mLineField[vNew.x, vNew.y].direction = sDir;
    //            connectedLineList.Add(new LineSegment(newPos, sDir));
    //            vCurrent = vTarget;
    //        }
    //        sPos = ls.position;
    //        sDir = ls.direction;
    //    }
    //}

}
