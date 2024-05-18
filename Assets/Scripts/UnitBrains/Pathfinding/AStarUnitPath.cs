using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using Utilities;

namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };
        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            Node startNode = new(startPoint);
            Node endNode = new(endPoint);
            List<Node> openList = new() { startNode };
            List<Node> closedList = new();
            while (openList.Any()) {
                Node currentNode = openList[0];
                foreach (var node in openList) {
                    if (node.Value < currentNode.Value) {
                        currentNode = node;
                    }
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == endNode) {
                    List<Vector2Int> _path = new();
                    while(currentNode != null) {
                        _path.Add(currentNode.CoordinatePoint);
                        currentNode = currentNode.Parent;
                    }

                    _path.Reverse();
                    path = _path.ToArray();
                    return;
                }

                //
                for (int i = 0; i < dx.Length; i++) {
                    int newX = currentNode.CoordinatePoint.x + dx[i];
                    int newY = currentNode.CoordinatePoint.y + dy[i];
                    Vector2Int newPoint = new Vector2Int(newX, newY);

                    if (runtimeModel.IsTileWalkable(newPoint)) {
                        Node neighbor = new(newPoint);
                        if (closedList.Contains(neighbor))
                            continue;
                        neighbor.Parent = currentNode;
                        neighbor.CalculateEstimate(endNode.CoordinatePoint);
                        neighbor.CalculateValue();
                        openList.Add(neighbor);
                    }
                }
            }
        }
    }
}