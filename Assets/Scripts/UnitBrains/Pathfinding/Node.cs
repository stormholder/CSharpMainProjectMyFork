using System;
using System.Collections.Generic;
using Model;
using UnityEngine;
using Utilities;

namespace UnitBrains.Pathfinding
{
    public class Node {
        public Vector2Int CoordinatePoint;
        public int Cost = 10;
        public int Estimate;
        public int Value;
        public Node Parent;

        public Node(Vector2Int coord) {
            CoordinatePoint = coord;
        }

        public void CalculateEstimate(Vector2Int targetCoordinatePoint) {
            Estimate = Math.Abs(CoordinatePoint.x - targetCoordinatePoint.x) + Math.Abs(CoordinatePoint.y - targetCoordinatePoint.y);
        }

        public void CalculateValue() {
            Value = Cost + Estimate;
        }

        public override bool Equals(object? obj) => obj is Node node && CoordinatePoint == node.CoordinatePoint;
    }
}