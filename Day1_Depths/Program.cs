var input = new InputProvider<int>("Input.txt", int.TryParse).ToList();

var prevValue = input[0];
int numInc = 0;
for (int i = 1; i < input.Count; i++)
{
    if (input[i] > prevValue)
    {
        numInc++;
    }
    prevValue = input[i];
}

Console.WriteLine($"Part 1: {numInc}");

numInc = 0;
prevValue = int.MaxValue;

for (int i = 2; i < input.Count; i++)
{
    int slidingWindowValue = (int)input.GetSlidingWindowSum(i, 3);
    if (slidingWindowValue > prevValue)
    {
        numInc++;
    }
    prevValue = slidingWindowValue;
}

Console.WriteLine($"Part 2: {numInc}");