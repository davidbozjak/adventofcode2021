class Cave
{
    public string Name { get; init; }

    public bool IsBigCave { get; init; }

    private readonly List<Cave> neighbours = new();
    public IReadOnlyCollection<Cave> Neighbours => this.neighbours.AsReadOnly();

    public Cave(string name)
    {
        this.Name = name;
        this.IsBigCave = name[0] < 'a';
    }

    public void AddNeighbour(Cave cave)
    {
        if (this.neighbours.Contains(cave)) throw new Exception();
        if (cave == this) throw new Exception();

        this.neighbours.Add(cave);
    }
}
