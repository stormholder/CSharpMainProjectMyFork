using UnityEngine;

namespace UnitBrains.Pathfinding
{
    public class Node {
        public readonly Vector2Int CoordinatePoint;
        public readonly int Cost;
        public readonly int Estimate;
        public int Value => Cost + Estimate;
        public readonly Node Parent;

        public Node(Vector2Int coord, int estimate, int cost = 10, Node parent = null) {
            CoordinatePoint = coord;
            Cost = cost;
            Estimate = estimate;
            Parent = parent;
        }
    }
}