// See https://aka.ms/new-console-template for more information

namespace EY_SWE_Internship_test 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            (int canvadWidth, int canvadHeight) = ReadCanvasSize();
            Rectangle canvas = new Rectangle("canvas", 0, 0, canvadWidth, canvadHeight);
            SolveProblem sp = new SolveProblem("Resources/input.txt", canvas);
            sp.Solve1();
            sp.Solve2();
            sp.Solve3();
            sp.solve4();
            
        }

        private static (int, int) ReadCanvasSize()
        {
            int number1;
            int number2;

            Console.Write("Enter the canvas width: ");
            while (!int.TryParse(Console.ReadLine(), out number1))
            {
                Console.WriteLine("Invalid input. Please enter a whole number.");
                Console.Write("Enter the canvas width: ");
            }

            Console.Write("Enter the canvas height : ");
            while (!int.TryParse(Console.ReadLine(), out number2))
            {
                Console.WriteLine("Invalid input. Please enter a whole number.");
                Console.Write("Enter the canvas height: ");
            }
            
            return (number1, number2);
        }
    }
}