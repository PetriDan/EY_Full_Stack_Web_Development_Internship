namespace EY_SWE_Internship_test;

public class Rectangle
{
    public string Name{get;set;}
    public int X{get;set;}
    public int Y{get;set;}
    public int Width{get;set;}
    public int Height{get;set;}
    
    public Rectangle(string name, int x, int y, int width, int height)
    {
        Name = name;
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
    
    public override string ToString()
    {
        return $"Name: {Name}, X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
    }
    
}