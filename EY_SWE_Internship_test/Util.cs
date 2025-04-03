namespace EY_SWE_Internship_test;

public class Util
{
    public static Rectangle FindOverlap(Rectangle rect1, Rectangle rect2)
    {
        int overlapX = Math.Max(rect1.X, rect2.X);
        int overlapY = Math.Max(rect1.Y, rect2.Y);
        
        int overlapWidth = Math.Min(rect1.X + rect1.Width, rect2.X + rect2.Width) - overlapX;
        int overlapHeight = Math.Min(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - overlapY;
        
        return overlapHeight > 0 && overlapWidth > 0
            ? new Rectangle($"{rect1.Name}-{rect2.Name}", overlapX,overlapY, overlapWidth, overlapHeight)
            : null;
    }

    public static bool Equals(Rectangle rect1, Rectangle rect2)
    {
        return rect1.X == rect2.X &&
               rect1.Y == rect2.Y &&
               rect1.Width == rect2.Width &&
               rect1.Height == rect2.Height;
    }
}