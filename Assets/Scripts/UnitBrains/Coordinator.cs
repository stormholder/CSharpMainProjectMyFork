using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.UnitBrains
{
    public class Coordinator
    {
        private static Coordinator? instance;

        private Coordinator() { }

        public static Coordinator GetInstance()
        {
            if (instance == null)
            {
                instance = new Coordinator();
            }
            return instance;
        }
    }
}
