﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private CardSortTest test;
    // Use this for initialization
    void Start()
    {
        var test1 = new CardSortTest(11);
        var test2 = new CardSortTest(new int[] { 26, 1, 17, 29, 0, 15, 42, 3, 13, 2, 16});

        test1.ExecuteAndLog();

        test2.ExecuteAndLog();
    }
}