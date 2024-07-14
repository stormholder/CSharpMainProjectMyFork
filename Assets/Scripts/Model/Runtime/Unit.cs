﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UnitBrains;
using Controller;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnitBrains.Buff;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;

namespace Model.Runtime
{
    public class Unit : IReadOnlyUnit
    {
        public UnitConfig Config { get; }
        public Vector2Int Pos { get; private set; }
        public int Health { get; private set; }
        public bool IsDead => Health <= 0;
        public BaseUnitPath ActivePath => _brain?.ActivePath;
        public IReadOnlyList<BaseProjectile> PendingProjectiles => _pendingProjectiles;

        private readonly List<BaseProjectile> _pendingProjectiles = new();
        private IReadOnlyRuntimeModel _runtimeModel;
        private BaseUnitBrain _brain;
        private BuffController _buffController;

        private float _nextBrainUpdateTime = 0f;
        private float _nextMoveTime = 0f;
        private float _nextAttackTime = 0f;
        
        public Unit(UnitConfig config, Vector2Int startPos, Coordinator coordinator)
        {
            Config = config;
            Pos = startPos;
            Health = config.MaxHealth;
            _brain = UnitBrainProvider.GetBrain(config);
            _brain.SetUnit(this);
            _brain.SetCoordinator(coordinator);
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            _buffController = ServiceLocator.Get<BuffController>();
        }

        public void Update(float deltaTime, float time)
        {
            if (IsDead)
                return;
            
            if (_nextBrainUpdateTime < time)
            {
                _nextBrainUpdateTime = time + Config.BrainUpdateInterval;
                _brain.Update(deltaTime, time);
            }

            var buffs = _buffController.GetUnitBuffs(this);
            
            if (_nextMoveTime < time)
            {
                float speedModifier = 1.0f;
                var speedBuff = buffs.FirstOrDefault(b => b.GetType().Equals(typeof(SpeedBuff)));
                if (speedBuff != null)
                {
                    speedModifier = speedBuff.Modifier;
                }
                _nextMoveTime = time + Config.MoveDelay / speedModifier;
                Move();
            }
            
            if (_nextAttackTime < time && Attack())
            {
                // TODO
                float attackModifier = 1.0f;
                var attackBuff = buffs.FirstOrDefault(b => b.GetType().Equals(typeof(AttackPowerBuff)));
                if (attackBuff != null)
                {
                    attackModifier = attackBuff.Modifier;
                }
                _nextAttackTime = time + Config.AttackDelay / attackModifier;
            }
        }

        private bool Attack()
        {
            var projectiles = _brain.GetProjectiles();
            if (projectiles == null || projectiles.Count == 0)
                return false;
            
            _pendingProjectiles.AddRange(projectiles);
            return true;
        }

        private void Move()
        {
            var targetPos = _brain.GetNextStep();
            var delta = targetPos - Pos;
            if (delta.sqrMagnitude > 2)
            {
                Debug.LogError($"Brain for unit {Config.Name} returned invalid move: {delta}");
                return;
            }

            if (_runtimeModel.RoMap[targetPos] ||
                _runtimeModel.RoUnits.Any(u => u.Pos == targetPos))
            {
                return;
            }
            
            Pos = targetPos;
        }

        public void ClearPendingProjectiles()
        {
            _pendingProjectiles.Clear();
        }

        public void TakeDamage(int projectileDamage)
        {
            Health -= projectileDamage;
        }
    }
}