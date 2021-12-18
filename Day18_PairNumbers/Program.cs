﻿using System.Diagnostics;

var numbers = new InputProvider<PairNumber?>("Input.txt", GetPairNumber)
    .Where(w => w != null)
    .Cast<PairNumber>().ToList();

//foreach (var number in numbers)
//{
//    Console.WriteLine(number);
//    number.Reduce();
//    Console.WriteLine(number);
//}

var addedNumber = numbers[0];
foreach (var number in numbers.Skip(1))
{
    addedNumber = addedNumber + number;
    addedNumber.Reduce();
    Console.WriteLine(addedNumber);
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
            int level = 1;
            for (endOfLeft = 2; level > 0; endOfLeft++)
            {
                if (str[endOfLeft] == '[') level++;
                else if (str[endOfLeft] == ']') level--;
            }

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

    public PairNumber(PairNumber left, PairNumber right)
    {
        this.Value = null;
        this.Parent = null;
        this.Left = left;
        this.Right = right;

        left.Parent = this;
        right.Parent = this;
    }

    public void Reduce()
    {
        bool hasChanged;
        do
        {
            Console.WriteLine(this);
            hasChanged = this.Reduce(0);
        }while (hasChanged);
    }

    private bool Reduce(int level)
    {
        if (level == 4)
        {
            if (this.Value == null) // only pair numbers can be exploded
            {
                this.Explode();
                return true;
            }
        }
        else
        {
            if (this.Left != null && this.Left.Reduce(level + 1))
                return true;

            if (this.Right != null && this.Right.Reduce(level + 1))
                return true;
        }

        if (this.Value != null && this.Value >= 10)
        {
            this.Split();
            return true;
        }

        return false;
    }

    private void Explode()
    {
        if (this.Parent == null) throw new Exception();
        if (this.Left == null || this.Left.Value == null) throw new Exception();
        if (this.Right == null || this.Right.Value == null) throw new Exception();

        var firstLeftNumber = FirstNormalNumberOnLeft(this, true);

        if (firstLeftNumber != null)
        {
            if (firstLeftNumber.Value == null) throw new Exception();
            firstLeftNumber.Value += this.Left.Value;
        }

        var firstRightNumber = FirstNormalNumberOnRight(this, true);

        if (firstRightNumber != null)
        {
            if (firstRightNumber.Value == null) throw new Exception();
            firstRightNumber.Value += this.Right.Value;
        }

        this.Left = null;
        this.Right = null;
        this.Value = 0;

        static PairNumber? FirstNormalNumberOnLeft(PairNumber node, bool upPhase)
        {
            if (upPhase)
            {
                if (node.Parent == null) return null;

                if (node.Parent.Left == node)
                    return FirstNormalNumberOnLeft(node.Parent, true);

                if (node.Parent.Left.Value != null) return node.Parent.Left;
                else return FirstNormalNumberOnLeft(node.Parent.Left, false);
            }
            else
            {
                if (node.Right.Value != null) return node.Right;
                else return FirstNormalNumberOnLeft(node.Right, false);
            }
        }

        static PairNumber? FirstNormalNumberOnRight(PairNumber node, bool upPhase)
        {
            if (upPhase)
            {
                if (node.Parent == null) return null;

                if (node.Parent.Right == node)
                    return FirstNormalNumberOnRight(node.Parent, true);

                if (node.Parent.Right.Value != null) return node.Parent.Right;
                else return FirstNormalNumberOnRight(node.Parent.Right, false);
            }
            else
            {
                if (node.Left.Value != null) return node.Left;
                else return FirstNormalNumberOnRight(node.Left, false);
            }
        }
    }

    private void Split()
    {
        if (this.Value == null) throw new Exception();
        if (this.Left != null || this.Right != null) throw new Exception();

        this.Left = new PairNumber((int)(this.Value / 2.0), this);
        this.Right = new PairNumber((int)(this.Value - this.Left.Value), this);
        this.Value = null;
    }

    public void GetMagnitude()
    {
        throw new NotImplementedException();
    }

    public static PairNumber operator +(PairNumber number1, PairNumber number2)
    {
        return new PairNumber(number1, number2);
    }

    public override string ToString()
    {
        if (this.Value != null) return this.Value.Value.ToString();
        else return $"[{this.Left},{this.Right}]";
    }
}