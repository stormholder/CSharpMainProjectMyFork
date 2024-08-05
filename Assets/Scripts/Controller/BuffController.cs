using Model.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitBrains.Buff;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace Controller
{
    public class BuffController : IDisposable
    {
        Dictionary<Unit, List<BaseBuff>> _unitBuffs = new();
        private TimeUtil _time = ServiceLocator.Get<TimeUtil>();

        public BuffController()
        {
            _time.AddFixedUpdateAction(UpdateCoroutine);
        }

        public void AddUnitBuff(Unit unit, BaseBuff buff)
        {
            if (!_unitBuffs.ContainsKey(unit))
            {
                _unitBuffs.Add(unit, new List<BaseBuff>());
            }
            _unitBuffs[unit].Add(buff);
        }

        public List<BaseBuff> GetUnitBuffs(Unit unit)
        {
            return (_unitBuffs.ContainsKey(unit)) ? _unitBuffs[unit] : new();
        }
        private void UpdateCoroutine(float timeDelta)
        {
            if (_unitBuffs.Values.Count == 0)
                return;
            foreach (var unitBuffList in _unitBuffs.Values)
            {
                for (int i = 0; i < unitBuffList.Count; i++)
                {
                    BaseBuff unitBuff = unitBuffList[i];
                    unitBuff.Duration -= timeDelta;
                    if (unitBuff.Duration <= 0)
                        unitBuffList.Remove(unitBuff);
                }
            }
        }

        public void Dispose()
        {
            _time.RemoveFixedUpdateAction(UpdateCoroutine);
            //if (_updateCoroutine != null)
            //{
            //    _time.StopCoroutine(_updateCoroutine);
            //    _updateCoroutine = null;
            //}
        }
    }
}
