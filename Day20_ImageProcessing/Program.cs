using System.Drawing;

var enhancedStringProvider = new InputProvider<string?>("EnhanceString.txt", GetString).ToList();
var enhancedLookup = enhancedStringProvider[0].ToCharArray();

var input = new InputProvider<char[]>("Input.txt", ParseRow).ToList();
var printer = new WorldPrinter();

for (int y = 0; y < input.Count; y++)
{
    for (int x = 0; x < input[0].Length; x++)
    {
        var instance = Pixel.Factory.GetOrCreateInstance((x, y));
        instance.IsLightPixel = input[y][x] == '#';
    }
}

int minX = Pixel.Factory.AllCreatedInstances.Select(w => w.X).Min() - 2;
int maxX = Pixel.Factory.AllCreatedInstances.Select(w => w.X).Max() + 2;
int minY = Pixel.Factory.AllCreatedInstances.Select(w => w.Y).Min() - 2;
int maxY = Pixel.Factory.AllCreatedInstances.Select(w => w.Y).Max() + 2;

InitializeBorder();
var valuesToProcess = Pixel.Factory.AllCreatedInstances.ToList();

for (int step = 0; step < 50; step++)
{
    foreach (var pixel in valuesToProcess)
    {
        pixel.CalculateEnhance(enhancedLookup);
    }

    valuesToProcess.ForEach(w => w.ApplyEnhance());

    minX--;
    minY--;
    maxX++;
    maxY++;

    var excludingBorder = valuesToProcess.Where(w => w.X > minX + 1 && w.X < maxX - 1 && w.Y > minY + 1 && w.Y < maxY - 1);

    Console.WriteLine($"Step {step + 1}: All: {valuesToProcess.Count} Exluding: {excludingBorder.Count()} Number of light pixels: {excludingBorder.Where(w => w.IsLightPixel).Count()}");
}

void InitializeBorder()
{
    int minX = Pixel.Factory.AllCreatedInstances.Select(w => w.X).Min();
    int maxX = Pixel.Factory.AllCreatedInstances.Select(w => w.X).Max();
    int minY = Pixel.Factory.AllCreatedInstances.Select(w => w.Y).Min();
    int maxY = Pixel.Factory.AllCreatedInstances.Select(w => w.Y).Max();

    for (int x = minX - 300; x <= maxX + 300; x++)
    {
        for (int y = minY - 300; y <= maxY + 300; y++)
        {
            var instance = Pixel.Factory.GetOrCreateInstance((x, y));
        }
    }
}

static bool ParseRow(string? input, out char[] row)
{
    row = Array.Empty<char>();

    if (string.IsNullOrWhiteSpace(input)) return false;

    row = input.ToCharArray();

    return true;
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

class Pixel : IWorldObject
{
    public static UniqueFactory<(int x, int y), Pixel> Factory = new(w => new Pixel(w.x, w.y));
    public Point Position {get; init;}

    public char CharRepresentation => this.IsLightPixel ? '#' : '.';

    public int X => this.Position.X;
    
    public int Y => this.Position.Y;

    public int Z => 0;

    public bool IsLightPixel { get; set; }

    private bool nextValue;

    public Pixel(int x, int y)
    {
        this.Position = new Point(x, y);
    }

    public void CalculateEnhance(char[] enhanceLookup)
    {
        var chars = new[]
            {
                Factory.GetOrCreateInstance((X - 1, Y - 1)),
                Factory.GetOrCreateInstance((X, Y - 1)),
                Factory.GetOrCreateInstance((X + 1, Y - 1)),
                Factory.GetOrCreateInstance((X - 1, Y)),
                Factory.GetOrCreateInstance((X, Y)),
                Factory.GetOrCreateInstance((X + 1, Y)),
                Factory.GetOrCreateInstance((X - 1, Y + 1)),
                Factory.GetOrCreateInstance((X, Y + 1)),
                Factory.GetOrCreateInstance((X + 1, Y + 1))
            }
            .Select(w => w.IsLightPixel ? '1' : '0')
            .ToArray();

        var strValue = new string(chars);
        var lookupIndex = Convert.ToInt16(strValue, 2);

        this.nextValue = enhanceLookup[lookupIndex] == '#';
    }

    public void ApplyEnhance()
    {
        this.IsLightPixel = this.nextValue;
        this.nextValue = false;
    }
}