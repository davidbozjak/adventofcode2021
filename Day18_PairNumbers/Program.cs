using System.Diagnostics;

var numbers = new InputProvider<PairNumber?>("Input.txt", GetPairNumber)
    .Where(w => w != null)
    .Cast<PairNumber>().ToList();

foreach (var number in numbers)
{
    Console.WriteLine(number);
    number.Reduce(0);
    Console.WriteLine(number);
}


static bool GetPairNumber(string? input, out PairNumber? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input)) return false;

    value = new PairNumber(input, null);

    return true;
}

[DebuggerDisplay("{this.ToString()}")]
class PairNumber
{
    public PairNumber? Left { get; private set; }

    public PairNumber? Right { get; private set; }

    public PairNumber? Parent { get; private set; }

    public int? Value { get; private set; }

    public PairNumber(string str, PairNumber? parent)
    {
        this.Parent = parent;

        if (str[0] != '[') throw new Exception();
        if (str[^1] != ']') throw new Exception();

        int endOfLeft;

        if (str[1] != '[')
        {
            endOfLeft = str.IndexOf(',');
            this.Left = new PairNumber(int.Parse(str[1..endOfLeft]), this);
        }
        else
        {
            endOfLeft = str[..^1].LastIndexOf(']') + 1;
            this.Left = new PairNumber(str[1..endOfLeft], this);
        }

        endOfLeft++;

        if (str[endOfLeft] != '[')
        {
            this.Right = new PairNumber(int.Parse(str[endOfLeft..str.IndexOf(']', endOfLeft)]), this);
        }
        else
        {
            this.Right = new PairNumber(str[endOfLeft..^1], this);
        }
    }

    public PairNumber(int value, PairNumber parent)
    {
        this.Value = value;
        this.Parent = parent;
    }

    public void Reduce(int level)
    {
        if (this.Value != null) return;

        if (level == 4)
        {
            this.Explode();
        }
        else
        {
            this.Left?.Reduce(level + 1);
            this.Right?.Reduce(level + 1);
        }

        //if (this.Left.Value != null && this.Left.Value > 10)
        //    this.Left.Split();

        //if (this.Right.Value != null && this.Right.Value > 10)
        //    this.Right.Split();
    }

    private void Explode()
    {
        if (this.Parent == null) throw new Exception();
        if (this.Left == null || this.Left.Value == null) throw new Exception();
        if (this.Right == null || this.Right.Value == null) throw new Exception();

        PairNumber? parentLeft = this.Parent?.Left;

        if (parentLeft == this)
        {
            while (parentLeft == parentLeft?.Parent?.Left) parentLeft = parentLeft?.Parent;
            parentLeft = parentLeft?.Parent;
        }

        if (parentLeft != null)
        {
            PairNumber right = parentLeft;
            while (right.Right != null) right = right.Right;

            if (right.Value == null) throw new Exception();
            right.Value += this.Left.Value;
        }

        PairNumber? parentRight = this.Parent?.Right;

        if (parentRight == this)
        {
            while (parentRight == parentRight?.Parent?.Right) parentRight = parentRight?.Parent;
            parentRight = parentRight?.Parent?.Right;
        }

        if (parentRight != null)
        {
            PairNumber left = parentRight;
            while (left.Left != null) left = left.Left;

            if (left.Value == null) throw new Exception();
            left.Value += this.Right.Value;
        }

        this.Left = null;
        this.Right = null;
        this.Value = 0;
    }

    private void Split()
    {
        throw new NotImplementedException();
    }

    public void GetMagnitude()
    {
        throw new NotImplementedException();
    }

    public PairNumber Addition(PairNumber number)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        if (this.Value != null) return this.Value.Value.ToString();
        else return $"[{this.Left},{this.Right}]";
    }
}