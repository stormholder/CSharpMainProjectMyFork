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
        // public static int UnitCounter = 0;
        // private int UnitID = UnitCounter++;
        private float StateSwitchCooldownTimeSeconds = 1f;
        private float _stateSwitchCooldownTime = 0f;
        private UnitState _prevUnitState = UnitState.Move;
        private UnitState _unitState = UnitState.Move;

        private void ToggleState(UnitState state) {
            if (state == _unitState) {
                return;
            }
            // if (state != UnitState.Switch) {
            //     Debug.Log($"Unit#{UnitID} toggling to State \"{state}\"");
            // }
            _prevUnitState = _unitState;
            _unitState = state;
        }

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);
            if (_unitState == UnitState.Switch) {
                _stateSwitchCooldownTime += Time.deltaTime;
                if (_stateSwitchCooldownTime >= StateSwitchCooldownTimeSeconds) {
                    _stateSwitchCooldownTime = 0;
                    ToggleState( (_prevUnitState == UnitState.Move) ? UnitState.Attack : UnitState.Move );
                }
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            var targets = base.SelectTargets();
            if (_unitState != UnitState.Attack) {
                return new();
            }
            if (!targets.Any()) {
                ToggleState(UnitState.Switch);
            }
            return targets;
        }

        public override Vector2Int GetNextStep() {
            var nextStep = base.GetNextStep();
            if (_unitState != UnitState.Move) {
                return unit.Pos;
            }
            if (nextStep == unit.Pos) {
                ToggleState(UnitState.Switch);
            }
            return nextStep;
        }
    }
}