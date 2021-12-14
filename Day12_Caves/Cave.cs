class Cave
{
    public string Name { get; init; }

    public bool IsBigCave { get; init; }

    private readonly List<Cave> neighbours = new();
    private readonly Cached<IReadOnlyCollection<Cave>> cachedReadOnlyNeighbours;
    public IReadOnlyCollection<Cave> Neighbours => cachedReadOnlyNeighbours.Value;

    public Cave(string name)
    {
        this.Name = name;
        this.IsBigCave = name[0] < 'a';
        this.cachedReadOnlyNeighbours = new Cached<IReadOnlyCollection<Cave>>(() => this.neighbours.AsReadOnly());
    }

    public void AddNeighbour(Cave cave)
    {
        if (this.neighbours.Contains(cave)) throw new Exception();
        if (cave == this) throw new Exception();

        this.neighbours.Add(cave);
    }
}
