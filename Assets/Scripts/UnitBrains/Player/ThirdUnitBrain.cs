using System.Collections.Generic;
using System.Linq;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{

    public enum UnitState {
        Move,
        Attack,
        Switch
    }

    public class ThirdUnitBrain : DefaultPlayerUnitBrain {
        public override string TargetUnitName => "Ironclad Behemoth";
        private float StateSwitchCooldownTimeSeconds = 1f;
        private float _stateSwitchCooldownTime = 0f;
        private UnitState _prevUnitState = UnitState.Move;
        private UnitState _unitState = UnitState.Move;

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);

            if (_unitState == UnitState.Switch) {
                _stateSwitchCooldownTime += Time.deltaTime;
                float t = _stateSwitchCooldownTime / (StateSwitchCooldownTimeSeconds/10);
                if (t >= 1) {
                    _stateSwitchCooldownTime = 0;
                    if (_prevUnitState == UnitState.Move) {
                        _prevUnitState = _unitState;
                        _unitState = UnitState.Attack;
                    } else if (_prevUnitState == UnitState.Attack) {
                        _prevUnitState = _unitState;
                        _unitState = UnitState.Move;
                    }
                }
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            // TODO: block if switching action or attacking
            return base.SelectTargets();
        }

        public override Vector2Int GetNextStep() {
            // TODO: block if switching action or moving
            return base.GetNextStep();
        }
    }
}