using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class VFieldCell
{
    public Vector3 pos;
    public Vector3 dir;

    public VFieldCell(Vector3 _pos)
    {
        pos = _pos;
        dir = _pos;
    }
}

public class VectorField : MonoBehaviour
{
    VFieldCell[,] vFCell;
    bool start = true;

    private void Awake()
    {
        vFCell = new VFieldCell[Lib.sVDim, Lib.sVDim];

        for (int x = 0; x < Lib.sVDim; x++)
        {
            for (int y = 0; y < Lib.sVDim; y++)
            {
                vFCell[x, y] = new VFieldCell
                    (new Vector3(x - Lib.sVDim / 2, y - Lib.sVDim / 2, 0));
                //floats[x, y].pos = new Vector3(x, y, 0);
            }
        }
        start = false;
        DrawField();
    }

    public Vector3 GetDirection(Vector3 _pos)
    {
        Vector3Int pos = Vector3Int.FloorToInt(_pos);
        pos.x += Lib.sVDim / 2;
        pos.y += Lib.sVDim / 2;
        if (!IsInField2D(pos.x, pos.y, Lib.sVDim))
            return Vector3.zero;
        return vFCell[pos.x, pos.y].dir;
    }

    public void DrawField()
    {
        int vD = Lib.sVDim / 2;
        for (int x = 0; x < Lib.sVDim; x++)
        {
            for (int y = 0; y < Lib.sVDim; y++)
            {
                vFCell[x, y].dir = CalcVector3(x - vD, y - vD).normalized;

                switch (Lib.sVFieldType)
                {
                    case 0:
                        vFCell[x, y].dir = CalcVector(x - vD, y - vD).normalized;
                        break;
                    case 1:
                        vFCell[x, y].dir = CalcVector1(x - vD, y - vD).normalized;
                        break;
                    case 2:
                        vFCell[x, y].dir = CalcVector2(x - vD, y - vD).normalized;
                        break;
                    case 3:
                        vFCell[x, y].dir = CalcVector3(x - vD, y - vD).normalized;
                        break;
                }
            }
        }
    }

    void v2Dir(int x, int y)
    {

    }

    Vector3 CalcVector(int x, int y)
    {
        return new Vector3(Mathf.Sqrt(x * x), Mathf.Sqrt(y * y), 0);
    }
    Vector3 CalcVector1(int x, int y)
    {
        return new Vector3(-y, x, 0);
    }
    Vector3 CalcVector2(int x, int y)
    {
        return new Vector3(Mathf.Sin(x), Mathf.Sin(y), 0);
    }

    Vector3 CalcVector3(int x, int y)
    {
        return new Vector3(x/2f,y/2f,0);
    }
    public Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
    public Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static bool IsInField2D(int _x, int _y, int _dimension)
    {
        if (_x < 0 || _x >= _dimension ||
            _y < 0 || _y >= _dimension)
            return false;

        return true;
    }
    void OnDrawGizmos()
    {
        if (start || !Lib.sDrawVField) return;
        Gizmos.color = Color.blue;
        for (int x = 0; x < Lib.sVDim; x++)
        {
            for (int y = 0; y < Lib.sVDim; y++)
            {
                Gizmos.DrawLine(vFCell[x, y].pos, vFCell[x, y].pos+vFCell[x, y].dir);
            }
        }
        //Gizmos.DrawLine(new Vector3(1, 0, 0), new Vector3(1, 1, 0));
    }
}
