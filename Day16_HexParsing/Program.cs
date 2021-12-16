﻿var hexString = new InputProvider<string>("Input.txt", GetString).First();

var binaryString = string.Join(string.Empty, hexString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

int sumOfAllVersions = 0;
PacketPhase phase = PacketPhase.Version;
var rootPacket = new Packet(null);
Packet? currentPacket = rootPacket;

for (int i = 0; currentPacket != null;)
{
    if (phase == PacketPhase.Version)
    {
        var version = BinaryToDec(binaryString.Substring(i, 3));
        currentPacket.Version = version;

        sumOfAllVersions += version;
        i += 3;
        phase = PacketPhase.TypeId;
        currentPacket.DecreaseRemainingCharsIfNeeded(3);

        continue;
    }

    if (phase == PacketPhase.TypeId)
    {
        var id = BinaryToDec(binaryString.Substring(i, 3));
        currentPacket.Type = id switch
        {
            0 => PacketType.Sum,
            1 => PacketType.Product,
            2 => PacketType.Minimum,
            3 => PacketType.Maximum,
            4 => PacketType.LiteralValue,
            5 => PacketType.GreaterThan,
            6 => PacketType.LessThan,
            7 => PacketType.EqualTo,
            _ => throw new Exception()
        };

        i += 3;
        currentPacket.DecreaseRemainingCharsIfNeeded(3);

        phase = PacketPhase.Data;
        continue;
    }

    if (phase == PacketPhase.Data)
    {
        if (currentPacket.Type == PacketType.LiteralValue)
        {
            currentPacket.DecreaseRemainingCharsIfNeeded(5);
            currentPacket.AddValue(binaryString.Substring(i + 1, 4));
            
            if (binaryString[i] == '0')
            {
                currentPacket = currentPacket.FinalizePacket();
                phase = PacketPhase.Version;
            }

            i += 5;
            continue;
        }
        else if(currentPacket.Type != PacketType.LiteralValue)
        {
            var lengthTypeId = BinaryToDec(binaryString.Substring(i, 1));
            
            if (lengthTypeId == 0)
            {
                var totalLengthInBits = BinaryToDec(binaryString.Substring(i + 1, 15));

                currentPacket.DecreaseRemainingCharsIfNeeded(16);
                currentPacket.remainingChars = totalLengthInBits;

                i += 16;
            }
            else if (lengthTypeId == 1)
            {
                var numberOfSubpackets = BinaryToDec(binaryString.Substring(i + 1, 11));

                for (int j = 0; j < numberOfSubpackets; j++)
                    currentPacket.subPackets.Add(new Packet(currentPacket));

                i += 12;
                currentPacket.DecreaseRemainingCharsIfNeeded(12);
            }

            currentPacket = currentPacket.FinalizePacket();
            phase = PacketPhase.Version;
            continue;
        }
    }
}

Console.WriteLine($"Part1: Sum of all versions: {sumOfAllVersions}");
Console.WriteLine($"Part2: Value of outermost packet: {rootPacket.GetValue()}");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

static int BinaryToDec(string str)
{
    return Convert.ToInt32(str, 2);
}

enum PacketPhase { Version, TypeId, Data };
enum PacketType { Sum, Product, Minimum, Maximum, LiteralValue, GreaterThan, LessThan, EqualTo, Unknown};
enum Mode { Unknown, RemainingBits, Packets};

class Packet
{
    public int startIndex { get; set; }
    public int Version { get; set; }
    public int remainingChars { get; set; }

    public List<Packet> subPackets = new();
    public Packet Parent { get; }

    private string literalValueString = null;

    public int GetValue()
    {
        switch (this.Type)
        {
            case PacketType.Sum:
                return this.subPackets.Sum(w => w.GetValue());
            case PacketType.Product:
                var product = 1;
                foreach (var packet in this.subPackets) product *= packet.GetValue();
                return product;
            case PacketType.Minimum:
                return this.subPackets.Min(w => w.GetValue());
            case PacketType.Maximum:
                return this.subPackets.Max(w => w.GetValue());
            case PacketType.LiteralValue:
                return Convert.ToInt32(this.literalValueString, 2);
            case PacketType.GreaterThan:
                return this.subPackets[0].GetValue() > this.subPackets[1].GetValue() ? 1 : 0;
            case PacketType.LessThan:
                return this.subPackets[0].GetValue() < this.subPackets[1].GetValue() ? 1 : 0;
            case PacketType.EqualTo:
                return this.subPackets[0].GetValue() == this.subPackets[1].GetValue() ? 1 : 0;
            default: throw new Exception();
        }
    }

    public PacketType Type { get; set; } = PacketType.Unknown;

    public Packet(Packet parent)
    {
        this.Parent = parent;
    }

    public void AddValue(string valueFragment)
    {
        if (literalValueString == null) literalValueString = valueFragment;
        else
        {
            literalValueString = literalValueString + valueFragment;
        }
    }

    public Packet? FinalizePacket()
    {
        if (this.remainingChars > 0)
        {
            var newPacket = new Packet(this);
            this.subPackets.Add(newPacket);
            return newPacket;
        }

        if (this.Parent == null) return null;

        var index = this.Parent.subPackets.IndexOf(this);
        if (this.Parent.subPackets.Count > index + 1) 
            return this.Parent.subPackets[index + 1];

        return Parent?.FinalizePacket();
    }

    public void DecreaseRemainingCharsIfNeeded(int charsRead)
    {
        if (this.remainingChars > 0)
        {
            this.remainingChars -= charsRead;
        }

        Parent?.DecreaseRemainingCharsIfNeeded(charsRead);
    }
}