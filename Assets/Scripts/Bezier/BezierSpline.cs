using UnityEngine;

/// <summary>
/// A primitive 3-point bezier sampler.
/// Used to sort and place cards.
/// </summary>
public class BezierSpline
{
    private Transform[] _controlPoints;

    public BezierSpline(Transform[] controlPoints)
    {
        _controlPoints = controlPoints;
    }

    public Vector3 GetPoint(float t, out Vector3 tangent, out Vector3 normal)
    {
        var p1 = _controlPoints[0].position;
        var p2 = _controlPoints[1].position;
        var p3 = _controlPoints[2].position;

        var d1 = p2 - p1;
        var d2 = p3 - p2;

        // Calculate mid points
        var m1 = p1 + (d1 * t);
        var m2 = p2 + (d2 * t);

        var pos = (m1 * (1 - t) + m2 * (t));

        tangent = m2 - pos;
        tangent.Normalize();

        normal = tangent;
        var temp = normal.x;
        normal.x = -normal.y;
        normal.y = temp;

        return pos;
    }

    /// <summary>
    /// A dummy method to find the closest t value for the given position.
    /// Currently this assumes the curve as wide along the x axis.
    /// This will be the case for the deck on this task.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public float FindTOnPoint(Vector3 point)
    {
        var t = (point.x - _controlPoints[0].position.x) / (_controlPoints[2].position.x - _controlPoints[0].position.x);

        return Mathf.Clamp01(t);
    }
}
