var input = new InputProvider<int[]>("Input.txt", ParseRow).ToList();

var count = input.Count;

var lowPoints = new List<int>();
var basins = new Dictionary<(int x, int y), int>();

for (int y = 0; y < input.Count; y++)
{
    for (int x = 0; x < input[0].Length; x++)
    {
        var surrounding = new List<int>();

        if (y - 1 >= 0) surrounding.Add(input[y - 1][x]);
        if (y + 1 < input.Count) surrounding.Add(input[y + 1][x]);
        if (x - 1 >= 0) surrounding.Add(input[y][x - 1]);
        if (x + 1 < input[0].Length) surrounding.Add(input[y][x + 1]);

        var value = input[y][x];
        if (value < surrounding.Min())
        {
            lowPoints.Add(value);
            basins[(x, y)] = 0;
        }
    }
}

Console.WriteLine($"Part 1: {lowPoints.Select(w => w + 1).Sum()}");

foreach (var basin in basins.Keys)
{
    FillBasin(basin.x, basin.y, basin, new List<(int x, int y)>());
}

var product = 1;

foreach (var size in basins.Values.OrderByDescending(w => w).Take(3))
{
    product *= size;
}

Console.WriteLine($"Part 2: {product}");

void FillBasin(int x, int y, (int, int) basin, IList<(int x, int y)> path)
{
    if (path.Contains((x, y))) return;

    path.Add((x, y));

    var value = input[y][x];

    if (value == 9) return;

    basins[basin]++;

    if (y - 1 >= 0) FillBasin(x, y - 1, basin, path);
    if (y + 1 < input.Count) FillBasin(x, y + 1, basin, path);
    if (x - 1 >= 0) FillBasin(x - 1, y, basin, path);
    if (x + 1 < input[0].Length) FillBasin(x + 1, y, basin, path);
}

static bool ParseRow(string? input, out int[] row)
{
    row = new int[0];

    if (string.IsNullOrWhiteSpace(input)) return false;

    row = input.ToCharArray().Select(w => w - '0').ToArray();

    return true;
}