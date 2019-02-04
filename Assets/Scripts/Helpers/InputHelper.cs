using UnityEngine;

public class InputHelper
{
    public static Vector3 GetCursorPosition()
    {
        var cursorPosition = Input.mousePosition;

        if(Input.touchCount > 0)
        {
            cursorPosition = Input.GetTouch(0).position;
        }

        return cursorPosition;
    }
}
