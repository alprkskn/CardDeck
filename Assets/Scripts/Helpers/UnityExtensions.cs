using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
{
    public static void SetAlpha(this Color c, float alpha)
    {
        c.a = alpha;
    }
}