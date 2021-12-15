﻿namespace SantasToolbox;

public interface INode : IWorldObject
{
    int Cost { get; }
}

public static class AStarPathfinder
{
    public static List<T>? FindPath<T>(T start, T goal, Func<T, int> hFunc, Func<T, IEnumerable<T>> GetNeighbours)
        where T : class, INode
    {
        var openSet = new List<T> { start };
        var cameFrom = new Dictionary<T, T>();
        var gScore = new Dictionary<T, int>
        {
            [start] = 0
        };

        var fScore = new Dictionary<INode, int>
        {
            [start] = hFunc(start)
        };

        while (openSet.Count > 0)
        {
            var current = openSet.Where(w => fScore.ContainsKey(w)).OrderBy(w => fScore[w]).First();

            if (current == goal)
            {
                //reconstruct path!
                var path = new List<T>() { current };

                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Insert(0, current);
                }

                return path;
            }

            openSet.Remove(current);

            foreach (var neighbour in GetNeighbours(current))
            {
                var tentativeScore = gScore[current] + neighbour.Cost;

                var g = gScore.ContainsKey(neighbour) ? gScore[neighbour] : UInt16.MaxValue;
                if (tentativeScore < g)
                {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeScore;
                    fScore[neighbour] = tentativeScore + hFunc(neighbour);

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }
}