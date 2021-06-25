using System;

namespace ProjektProcesor
{
    class Register
    {
        sRegister High;
        sRegister Low;
        public Register(ushort value = 0)
        {
            High = new();
            Low = new();
            this.setValue(value);
        }
        public Register(sRegister high, sRegister low)
        {
            High = high;
            Low = low;
        }
        public void setValue(ushort val)
        {
            High.Value = (byte)(val >> 8);
            Low.Value = (byte)(val & 255);
        }
        public ushort getValue()
        {
            byte[] bytes = new byte[] { Low.Value, High.Value };
            return BitConverter.ToUInt16(bytes);
        }
    }
}
