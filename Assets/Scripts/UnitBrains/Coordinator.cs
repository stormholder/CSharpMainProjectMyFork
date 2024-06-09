using Model;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Assets.Scripts.UnitBrains
{
    public class Coordinator
    {
        private static Coordinator? instance;

        private Coordinator() {
            timeUtil.AddFixedUpdateAction(FixedUpdate);
        }

        public static Coordinator GetInstance()
        {
            if (instance == null)
            {
                instance = new Coordinator();
            }
            return instance;
        }

        protected IReadOnlyRuntimeModel runtimeModel => ServiceLocator.Get<IReadOnlyRuntimeModel>();
        protected TimeUtil timeUtil => ServiceLocator.Get<TimeUtil>();

        private void FixedUpdate(float timeDelta)
        {
            // TODO
        }

    }
}
