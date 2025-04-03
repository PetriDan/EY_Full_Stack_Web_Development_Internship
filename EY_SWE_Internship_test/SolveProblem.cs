namespace EY_SWE_Internship_test;

public class SolveProblem
{
    private string _fileName;
    private List<Rectangle> _rects = new List<Rectangle>();
    private Rectangle _canvas;

    public SolveProblem(string fileName, Rectangle canvas)
    {
        _canvas = canvas;
        _fileName = fileName;
        LoadFromFile();
    }

    private Rectangle ParseLineToRectangle(string line)
    {
        string[] parts = line.Split(' ');
        string name = parts[0];
        int x = int.Parse(parts[1]);
        int y = int.Parse(parts[2]);
        int width = int.Parse(parts[3]);
        int height = int.Parse(parts[4]);

        return new Rectangle(name, x, y, width, height);
    }

    private void LoadFromFile()
    {
        try
        {
            using (StreamReader reader = new StreamReader(_fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    _rects.Add(ParseLineToRectangle(line));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
    }

    public void Solve1()
    {
        List<Rectangle> keptRects = _rects
            .Where(rect =>
            {
                Rectangle overlappingRectanges = Util.FindOverlap(_canvas, rect);
                if(overlappingRectanges != null)
                    return Util.Equals(Util.FindOverlap(_canvas, rect), rect);
                return false;
            })
            .ToList();

        Console.WriteLine("Rectangles fully contained within the canvas:");
        foreach (Rectangle keptRect in keptRects)
        {
            Console.WriteLine(keptRect);
        }
        
        _rects = keptRects;
    }

    public void Solve2()
    {
        Console.WriteLine("Rectangles with no overlap:");
        for (int i = 0; i < _rects.Count - 1; i++)
        {
            int ok = 1; 
            for (int j = 0; j < _rects.Count; j++)
            {
                if(_rects[i].Name == _rects[j].Name)
                    continue;
                
                if (Util.FindOverlap(_rects[i], _rects[j]) != null)
                {
                    ok = 0;
                    break;
                }
            }
            if (ok == 1)
            {
                Console.WriteLine(_rects[i]);
            }
        }
    }

    public void Solve3()
    {
        Console.WriteLine("Rectangles that are fully contained in another rectangle:");
        for (int i = 0; i < _rects.Count; i++)
        {
            for (int j = 0; j < _rects.Count(); j++)
            {
                if (_rects[i].Name == _rects[j].Name)
                    continue;

                Rectangle overlap = Util.FindOverlap(_rects[i], _rects[j]);
                if (overlap != null && Util.Equals(overlap, _rects[j]))
                    Console.WriteLine(_rects[j]);
            }
        }
    }

    /*
     * My approach for this problem will be the following:
     * I will create a set of tuples to represent the occupied cells in the canvas.
     * The nr of unOccupied cells will be the total number of cells in the canvas minus the size of the set.
     *
     * Answers:
     *  - The number of overlapping rectangles does not directly affect this approach, but the size of the rectangles does, since the time complexity is dependant on the size of the rectangles.
     *  - This approach works well in a sparse canvas , since the number of occupied cells is small compared to the total number of cells in the canvas, but the number of rectangle in that sparse place can still influence the time to run the algorithm.
     *  - The time complexity is n*(w*h), where n is the number of rectangles, w is the width of the rectangle and h is the height of the rectangle. The space complexity is O(n), since we are storing the occupied cells in a set.
     *
     * Limitations:
     *  - Huge rectangles can cause the algorithm to take a long time to run, since we are iterating over all the cells in the rectangle.
     *  - This issue is amplified if the rectangles are overlapping, since we will be adding the same cells to the set multiple times.
     */
    public void solve4Naive()
    {
        HashSet<ValueTuple<int, int>> spMat = new HashSet<(int, int)>();
        for (int i = 0; i < _rects.Count; i++)
        {
            for (int j = _rects[i].X; j < _rects[i].X + _rects[i].Width; j++)
            {
                for (int k = _rects[i].Y; k < _rects[i].Y + _rects[i].Height; k++)
                {
                    spMat.Add(new ValueTuple<int, int>(j, k));
                }
            }
        }
        int nrOfClearCells = _canvas.Width * _canvas.Height - spMat.Count;
        Console.WriteLine($"The number of clear cells in the canvas is: {nrOfClearCells}");
    }

    /*
     * My approach for the problem:
     *  - For each interval of one on the x axis: [0-1], [1-2], ...., etc. i will create a list of the occupied intervals on the y-axis.
     *    When we see new rectangles, we try to merge the intervals, and if not possible, we add it to the sortedList, which is sorted by the start on the y-axis
     *  - After we have a dictionary of the following meaning:
     *          intervalRanges[i] -> meaning from i to i+1 on the x-axis = [[1,3], [5,7], [11,12]], it means that those intervals are occupied on the y-axis at the i interval range
     *
     *  - We than calculate the nrOfUnoccupiedCells( Area of clear cells ) by subtracting from the total nr of cells ( canvas area ), the nr of occupied cells, which we obtain from the dictionary
     *  - This approach is more efficient since it handles overlaps in a batch way
     */
    public void solve4()
    {
        Dictionary<int, SortedList<int, int>> intervalRanges = new Dictionary<int, SortedList<int, int>>();
        int minIndex = -1;
        int maxIndex = -1;
        for (int i = 0; i < _rects.Count; i++)
        {
            if(_rects[i].X < minIndex || minIndex == -1)
                minIndex = _rects[i].X;
            if(_rects[i].X + _rects[i].Width > maxIndex)
                maxIndex = _rects[i].X + _rects[i].Width;
            for (int j = _rects[i].X; j < _rects[i].X + _rects[i].Width; j++)
            {
                if (intervalRanges.ContainsKey(j))
                {
                    bool hasMerged = false;
                    foreach (var kvp in intervalRanges[j])
                    {
                        if (kvp.Key > _rects[i].Y + _rects[i].Height - 1 || kvp.Value < _rects[i].Y)
                            continue;
                            
                        if (kvp.Key < _rects[i].Y && kvp.Value > _rects[i].Y + _rects[i].Height - 1)
                        {
                            hasMerged = true;
                            break;
                        }
                        else if (_rects[i].Y <= kvp.Key && _rects[i].Y + _rects[i].Height - 1>= kvp.Key)
                        {
                            intervalRanges[j].Remove(kvp.Key);
                            intervalRanges[j].Add(_rects[i].Y, _rects[i].Y + _rects[i].Height - 1);
                            hasMerged = true;
                            break;
                        }
                        else if(kvp.Key <= _rects[i].Y && kvp.Value < _rects[i].Y + _rects[i].Height - 1)
                        {
                            intervalRanges[j][kvp.Key] = _rects[i].Y + _rects[i].Height - 1;
                            hasMerged = true;
                            break;
                        }
                        else if (kvp.Key > _rects[i].Y && kvp.Value > _rects[i].Y + _rects[i].Height - 1)
                        {
                            var auxKVP = kvp;
                            intervalRanges[j].Remove(kvp.Key);
                            intervalRanges[j].Add(_rects[i].Y, auxKVP.Value);
                            hasMerged = true;
                            break;
                        }
                    }
                    if(!hasMerged)
                        intervalRanges[j].Add(_rects[i].Y, _rects[i].Y + _rects[i].Height - 1);
                }
                else
                {
                    intervalRanges.Add(j, new SortedList<int, int>{{_rects[i].Y, _rects[i].Y+ _rects[i].Height - 1}});
                }
            }
        }

        int nrOfOccupiedCells = 0;
        for(int i = minIndex; i <= maxIndex; i++)
        {
            if (intervalRanges.ContainsKey(i))
            {
                foreach (var kvp in intervalRanges[i])
                {
                    nrOfOccupiedCells += (kvp.Value - kvp.Key + 1);
                }
            }
        }
        
        int nrOfClearCells = _canvas.Width * _canvas.Height - nrOfOccupiedCells;
        Console.WriteLine($"The number of clear cells in the canvas is: {nrOfClearCells}");
    }
}