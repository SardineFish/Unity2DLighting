using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public static class MathUtility
{
    public static Vector3 ToVector3(this Vector4 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }
    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }
    public static Vector3 ToVector3(this Vector2 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }
    public static Vector3 ToVector3XZ(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
    public static Vector3 ClipY(this Vector3 v)
    {
        return Vector3.Scale(v, new Vector3(1, 0, 1));
    }
    public static Vector3 Set(this Vector3 v, float x=float.NaN, float y=float.NaN, float z = float.NaN)
    {
        x = float.IsNaN(x) ? v.x : x;
        y = float.IsNaN(y) ? v.y : y;
        z = float.IsNaN(z) ? v.z : z;
        return new Vector3(x, y, z);
    }

    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector2 ToVector2XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    public static Vector2 Abs(this Vector2 v)
    {
        return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
    }
    public static Color Set(this Color color, float r = float.NaN, float g=float.NaN, float b=float.NaN, float a = float.NaN)
    {
        color.r = float.IsNaN(r) ? color.r : r;
        color.g = float.IsNaN(g) ? color.g : g;
        color.b = float.IsNaN(b) ? color.b : b;
        color.a = float.IsNaN(a) ? color.a : a;
        return color;
    }

    public static int SignInt(float x)
    {
        if (x > 0)
            return 1;
        else if (x < 0)
            return -1;
        return 0;
    }

    public static float MapAngle(float ang)
    {
        if (ang > 180)
            ang -= 360;
        else if (ang < -180)
            ang += 360;
        return ang;
    }

    public static Quaternion QuaternionBetweenVector(Vector3 u, Vector3 v)
    {
        u = u.normalized;
        v = v.normalized;
        var cosOfAng = Vector3.Dot(u, v);
        var halfCos = Mathf.Sqrt(0.5f * (1.0f + cosOfAng));
        var halfSin = Mathf.Sqrt(0.5f * (1.0f - cosOfAng));
        var axis = Vector3.Cross(u, v);
        var quaternion = new Quaternion(halfSin * axis.x, halfSin * axis.y, halfSin * axis.z, halfCos);
        return quaternion;
    }

    public static float ToAng(float y,float x)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    public static float ToAng(Vector2 v)
    {
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    public static Vector2 Rotate(Vector2 v, float rad)
    {
        var rx = Mathf.Cos(rad);
        var ry = Mathf.Sin(rad);
        // (vx + vy i) * (rx + ry i)
        return new Vector2(rx * v.x - ry * v.y, rx * v.y + ry * v.x);
    }
}