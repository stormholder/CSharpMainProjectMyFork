using UnityEngine;

namespace UnitBrains.Pathfinding
{
    public class Node {
        public readonly Vector2Int Pos;
        public int G; // cost from start pos
        public int H; // estimate cost to end pos
        public int Value => G + H;
        public Node Parent;

        public Node(Vector2Int coord, int h = 0, int g = 0, Node parent = null) {
            Pos = coord;
            G = g;
            H = h;
            Parent = parent;
        }
    }
}