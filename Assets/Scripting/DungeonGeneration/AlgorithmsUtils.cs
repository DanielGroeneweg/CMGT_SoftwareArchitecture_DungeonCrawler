using UnityEngine;

public class AlgorithmsUtils
{
    
    public static bool Intersects(RectInt a, RectInt b)
    {
        return a.xMin < b.xMax &&
               a.xMax > b.xMin &&
               a.yMin < b.yMax &&
               a.yMax > b.yMin;
    }
    
    public static RectInt Intersect(RectInt a, RectInt b)
    {
        int x = Mathf.Max(a.xMin, b.xMin);
        int y = Mathf.Max(a.yMin, b.yMin);
        int width = Mathf.Min(a.xMax, b.xMax) - x;
        int height = Mathf.Min(a.yMax, b.yMax) - y;

        if (width <= 0 || height <= 0)
        {
            return new RectInt();
        }
        else
        {
            return new RectInt(x, y, width, height);
        }
    }
    
    public static void FillRectIntangle(float[,] array, RectInt area, float value)
    {
        for (int i = (int)area.y; i < area.y + area.height; i++)
        {
            for (int j = (int)area.x; j < area.x + area.width; j++)
            {
                array[i, j] = value;
            }
        }
    }
    
    public static void FillRectIntangleOutline(float[,] array, RectInt area, float value) 
    { 
        
        float endX = area.x + area.width - 1;
        float endY = area.y + area.height - 1;

        // Draw top and bottom borders
        for (int x = (int)area.x; x <= endX; x++)
        {
            array[(int)area.y, x] = value;
            array[(int)endY, x] = value;
        }

        // Draw left and right borders
        for (int y = (int)area.y + 1; y < endY; y++)
        {
            array[y, (int)area.x] = value;
            array[y, (int)endX] = value;
        }
    }

    public static void DebugRectInt(RectInt rectInt, Color color, float duration = 0f, bool depthTest = false, float height = 0.01f)
    {
        DebugExtension.DebugBounds(new Bounds(new Vector3(rectInt.center.x, rectInt.center.y, 0), new Vector3(rectInt.width, rectInt.height, height)), color, duration, depthTest);
    }
}
