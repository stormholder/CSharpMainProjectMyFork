using System;
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
        private const int MaxLength = 100;
        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            //var currentPoint = startPoint;
            //var result = new List<Vector2Int> { startPoint };
            var route = CalculateAStar();
            if (route != null)
            {
                path = route.ToArray();
            }
        }

        private List<Vector2Int> CalculateAStar()
        {
            var counter = 0;
            Node startNode = new(startPoint);
            startNode.CalculateEstimate(endPoint);
            startNode.CalculateValue();
            List<Node> openList = new() { startNode };
            HashSet<Vector2Int> closedList = new();
            Node currentNode = openList[0];
            while (openList.Count > 0 && counter++ < MaxLength) {
                //string debugOpenPoints = "";
                string debugClosedPoints = "";
                foreach (var node in openList) {
                    //debugOpenPoints += ($"[{node.CoordinatePoint.x},{node.CoordinatePoint.y}]({node.Value}) ; ");
                    if (node.Value < currentNode.Value) {
                        currentNode = node;
                    }
                }
                //Debug.Log($"current open list nodes: {openList.Count}: {debugOpenPoints}");
                foreach (var node in closedList)
                    debugClosedPoints += ($"[{node.x},{node.y}] ; ");
                Debug.Log($"current closed list nodes: {closedList.Count}: {debugClosedPoints}");

                openList.Remove(currentNode);
                //if (!closedList.Contains(currentNode.CoordinatePoint))
                closedList.Add(currentNode.CoordinatePoint);

                if (currentNode.CoordinatePoint == endPoint) {
                    Debug.Log($"Found an route!");
                    List<Vector2Int> _path = new();
                    while(currentNode != null) {
                        _path.Add(currentNode.CoordinatePoint);
                        currentNode = currentNode.Parent;
                    }

                    _path.Reverse();
                    return _path;
                }

                for (int i = 0; i < dx.Length; i++) {
                    Vector2Int newPoint = new Vector2Int(
                        currentNode.CoordinatePoint.x + dx[i],
                        currentNode.CoordinatePoint.y + dy[i]
                        );
                    if (closedList.Contains(newPoint))
                        continue;

                    if (runtimeModel.IsTileWalkable(newPoint)) {
                        Node neighbor = new(newPoint);
                        neighbor.Parent = currentNode;
                        neighbor.CalculateEstimate(endPoint);
                        neighbor.CalculateValue();
                        openList.Add(neighbor);
                    }
                }
            }
            return null;
        }
    }
}