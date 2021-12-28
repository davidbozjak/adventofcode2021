using System.Drawing;
using System.Text.RegularExpressions;
using System.Diagnostics;

var instructions = new InputProvider<Instruction?>("Input.txt", GetInstruction)
    .Where(w => w != null)
    .Cast<Instruction>()
    .ToList();

Console.WriteLine(instructions.Count);

int minX = instructions.Select(w => w.MinX).Min();
int maxX = instructions.Select(w => w.MaxX).Max();
int minY = instructions.Select(w => w.MinY).Min();
int maxY = instructions.Select(w => w.MaxY).Max();
int minZ = instructions.Select(w => w.MinZ).Min();
int maxZ = instructions.Select(w => w.MaxZ).Max();

long countOn = 0;
long lastPercentage = int.MinValue;

for (long x = minX; x <= maxX; x++)
{
    var relavantInstructions = instructions
        .Where(w => x >= w.MinX && x <= w.MaxX);

    if (!relavantInstructions.Any()) continue;

    var percentage = (x - minX) / (maxX - minX);
    if (percentage != lastPercentage)
    {
        Console.WriteLine($"{DateTime.Now} : ~{percentage}% done");
        lastPercentage = percentage;
    }

    for (long y = relavantInstructions.Min(w => w.MinY); y <= relavantInstructions.Max(w => w.MaxY); y++)
    {
        relavantInstructions = relavantInstructions
            .Where(w => y >= w.MinY && y <= w.MaxY);

        if (!relavantInstructions.Any()) continue;

        for (long z = relavantInstructions.Min(w => w.MinZ); z <= relavantInstructions.Max(w => w.MaxZ); z++)
        {
            relavantInstructions = relavantInstructions
                .Where(w => z >= w.MinZ && z <= w.MaxZ);

            if (!relavantInstructions.Any()) continue;

            bool isOn = false;

            foreach (var instruction in relavantInstructions)
            {
                isOn = instruction.On;
            }

            if (isOn) 
                countOn++;
        }
    }
}

Console.WriteLine($"Part 2: Count on: {countOn}");

//var boundingRegion = new BitRegion(minX, maxX, minY, maxY, minZ, maxZ) { Value = false };

////Console.WriteLine(long.MaxValue);

//foreach (var instruction in instructions)
//{
//    var region = new BitRegion(
//        instruction.MinX, instruction.MaxX,
//        instruction.MinY, instruction.MaxY,
//        instruction.MinZ, instruction.MaxZ)
//    {
//        Value = instruction.On
//    };

//    boundingRegion.Update(region);
//}


//Console.WriteLine($"Part 2: Count on: {boundingRegion.CountOn}");


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

[DebuggerDisplay("X range: {MaxX - MinX} Y: {MaxY - MinY} Z: {MaxZ - MinZ}")]
class BitRegion
{
    public int MinX { get; }
    public int MaxX { get; }
    public int MinY { get; }
    public int MaxY { get; }
    public int MinZ { get; }
    public int MaxZ { get; }

    public bool Value { get; set; }

    public long Size => (long)Math.Max(1, this.MaxX - this.MinX) * (long)Math.Max(1, this.MaxY - this.MinY) * (long)Math.Max(1, this.MaxZ - this.MinZ);

    //public long CountOn { get; private set; }

    private readonly Cached<long> cachedCountOn;
    public long CountOn => this.subregions.Count == 0 ?
        this.Value ? this.Size : 0 :
        this.subregions.Sum(w => w.CountOn);

    private readonly List<BitRegion> subregions = new();

    public BitRegion(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    {
        this.MinX = minX;
        this.MaxX = maxX;
        this.MinY = minY;
        this.MaxY = maxY;
        this.MinZ = minZ;
        this.MaxZ = maxZ;

        this.cachedCountOn = new Cached<long>(CalculateCountOn);

        //this.CountOn = initialValue ?
        //    this.Size
        //    : 0;
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

    public void Update(BitRegion region)
    {
        var intersect = GetIntersectingRegion(this, region);

        if (intersect != null)
        {
            bool fullyWithinOneRegion = false;

            foreach (var subregion in this.subregions)
            {
                var interesectBetweenSubRegions = GetIntersectingRegion(subregion, region);

                if (interesectBetweenSubRegions != null)
                {


                    Console.WriteLine("Intersecting subregions!");
                }
                else
                {

                }

                subregion.Update(region);
            }

            if (!fullyWithinOneRegion)
            {
                this.subregions.Add(intersect);
            }

            this.cachedCountOn.Reset();

            //if (this.Value)
            //{
            //    this.CountOn = this.Size - this.subregions.Where(w => w.Value == false).Sum(w => w.Size);
            //}
            //else
            //{
            //    this.CountOn = this.subregions.Sum(w => w.CountOn);
            //}
        }
    }

    private static (int startOfIntersect, int endOfIntersect)? GetIntersectOnLine(int min1, int max1, int min2, int max2)
    {
        var maxMin = Math.Max(min1, min2);
        var minMax = Math.Min(max1, max2);

        if (minMax < maxMin) return null;
        return (maxMin, minMax);
    }

    private long CalculateCountOn()
    {
        if (this.Value)
        {
            return this.Size - this.subregions.Where(w => w.Value == false).Sum(w => w.CountOn);
        }
        else
        {
            return this.subregions.Sum(w => w.CountOn);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BitRegion other)
        {
            return false;
        }

        if (other.MinX != this.MinX) return false;
        if (other.MaxX != this.MaxX) return false;
        if (other.MinY != this.MinY) return false;
        if (other.MaxY != this.MaxY) return false;
        if (other.MinZ != this.MinZ) return false;
        if (other.MaxZ != this.MaxZ) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return
            this.MinX.GetHashCode() *
            this.MaxX.GetHashCode() *
            this.MinY.GetHashCode() *
            this.MaxY.GetHashCode() *
            this.MinZ.GetHashCode() *
            this.MaxZ.GetHashCode();
    }
}