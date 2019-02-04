using UnityEngine.UI;

public static class UnityExtensions
{
    public static void SetAlpha(this Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}