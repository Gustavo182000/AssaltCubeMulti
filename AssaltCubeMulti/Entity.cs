using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AssaltCubeMulti
{
    public class Entity
    {
        public IntPtr baseAddress;
        public Vector3 feet, head;
        public Vector3 viewAngles;
        public float mag, viewOffset;
        public int health;
        public string name;
        public int rifleAmmo { get; set; }
        public int grenadeAmmo { get; set; }
    }
}
