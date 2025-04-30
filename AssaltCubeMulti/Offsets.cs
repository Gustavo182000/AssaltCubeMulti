using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaltCubeMulti
{
    public static class Offsets
    {
        public static int
            iViewMatrix = 0x17DFFC - 0x6C + 0x4 * 16,
            iLocalPlayer = 0x0018AC00,
            iEntityList = 0x00191FCC,
            vHead = 0x4, //Head Position X
            vFeet = 0x28, //Position Z
            vAngle = 0x34,
            vHealth = 0xEC,
            vName = 0x205,
            vRifleAmmo = 0x140,
            vGrenadeAmmo = 0x144;
    }
}
