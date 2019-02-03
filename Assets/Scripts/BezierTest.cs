using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTest : MonoBehaviour
{

    public Transform[] ControlPoints;
    public int Samples;

    private BezierSpline _bezier;

    // Use this for initialization
    void Start()
    {
        _bezier = new BezierSpline(ControlPoints);
    }

    // Update is called once per frame
    void Update()
    {
        var step = 1f / Samples;

        Vector3 tangent, normal;

        for (int i = 1; i <= Samples; i++)
        {
            var p1 = _bezier.GetPoint(step * (i - 1), out tangent, out normal);
            var p2 = _bezier.GetPoint(step * i, out tangent, out normal);

            Debug.DrawLine(p1, p2, Color.green);
            Debug.DrawLine(p2, p2 + normal, Color.red);
        }

        var cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var t = _bezier.FindTOnPoint(cursorPosition);

        var p = _bezier.GetPoint(t, out tangent, out normal);

        Debug.DrawLine(p, p + normal, Color.yellow);
    }
}
