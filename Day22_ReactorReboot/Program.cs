using System.Diagnostics;
using System.Text.RegularExpressions;

var instructions = new InputProvider<Instruction?>("Input.txt", GetInstruction)
    .Where(w => w != null)
    .Cast<Instruction>()
    .ToList();

var regions = instructions.Select(w => new BitRegion(w.MinX, w.MaxX, w.MinY, w.MaxY, w.MinZ, w.MaxZ)).ToList();
var cappingRegion = new BitRegion(-50, 50, -50, 50, -50, 50);
var regionsCappedTo50 = regions.Select(w => BitRegion.GetIntersectingRegion(w, cappingRegion))
    .ToList();

var totalOnCapped = Enumerable.Range(0, instructions.Count)
    .Where(w => instructions[w].On)
    .Where(w => regionsCappedTo50[w] != null)
    .Sum(w => GetUniqueVolume(regionsCappedTo50[w], regionsCappedTo50.Where(region => region != null).Skip(w + 1)));

var totalOn = Enumerable.Range(0, instructions.Count)
    .Where(w => instructions[w].On)
    .Sum(w => GetUniqueVolume(regions[w], regions.Skip(w + 1)));

Console.WriteLine($"Part 1: {totalOnCapped}");
Console.WriteLine($"Part 2: {totalOn}");

// A recursive method that only gives volume for the cube that never appears in another (following) region
static long GetUniqueVolume(BitRegion region, IEnumerable<BitRegion> otherRegions)
{
    var conflicts = new List<BitRegion>();

    foreach (var otherRegion in otherRegions)
    {
        var intersection = BitRegion.GetIntersectingRegion(region, otherRegion);

        if (intersection != null)
        {
            conflicts.Add(intersection);
        }
    }

    long volume = region.Size;
    volume -= Enumerable.Range(0, conflicts.Count).Sum(w => GetUniqueVolume(conflicts[w], conflicts.Skip(w + 1)));

    return volume;
}

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input)) return false;

    Regex numRegex = new(@"-?\d+");

    bool on = input.StartsWith("on");
    var numbers = numRegex.Matches(input)
        .Select(w => int.Parse(w.Value))
        .ToArray();

    if (numbers.Length != 6) throw new Exception();

    value = new Instruction(on, numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5]);

    return true;
}

record Instruction(bool On, int MinX, int MaxX, int MinY, int MaxY, int MinZ, int MaxZ);

[DebuggerDisplay("Size: {Size} X {MinX}..{MaxX} ({MaxX - MinX + 1}) Y {MinY}..{MaxY} ({MaxY - MinY + 1}) Z {MinZ}..{MaxZ} ({MaxZ - MinZ + 1})")]
class BitRegion
{
    public int MinX { get; }
    public int MaxX { get; }
    public int MinY { get; }
    public int MaxY { get; }
    public int MinZ { get; }
    public int MaxZ { get; }

    public long Size => (long)(this.MaxX - this.MinX + 1) * (this.MaxY - this.MinY + 1) * (this.MaxZ - this.MinZ + 1);

    public BitRegion(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    {
        this.MinX = minX;
        this.MaxX = maxX;
        this.MinY = minY;
        this.MaxY = maxY;
        this.MinZ = minZ;
        this.MaxZ = maxZ;
    }

    public static BitRegion? GetIntersectingRegion(BitRegion region1, BitRegion region2)
    {
        var intersectX = GetIntersectOnLine(region1.MinX, region1.MaxX, region2.MinX, region2.MaxX);
        if (intersectX == null) return null;

        var intersectY = GetIntersectOnLine(region1.MinY, region1.MaxY, region2.MinY, region2.MaxY);
        if (intersectY == null) return null;

        var intersectZ = GetIntersectOnLine(region1.MinZ, region1.MaxZ, region2.MinZ, region2.MaxZ);
        if (intersectZ == null) return null;

        return new BitRegion(
            intersectX.Value.startOfIntersect, intersectX.Value.endOfIntersect,
            intersectY.Value.startOfIntersect, intersectY.Value.endOfIntersect,
            intersectZ.Value.startOfIntersect, intersectZ.Value.endOfIntersect);
    }

    private static (int startOfIntersect, int endOfIntersect)? GetIntersectOnLine(int min1, int max1, int min2, int max2)
    {
        var maxMin = Math.Max(min1, min2);
        var minMax = Math.Min(max1, max2);

        if (minMax < maxMin) return null;
        return (maxMin, minMax);
    }
}