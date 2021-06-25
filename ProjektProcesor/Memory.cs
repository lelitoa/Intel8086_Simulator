using System;

namespace ProjektProcesor
{
    class Memory
    {
        byte[] mem = new byte[65536];
        public Memory()
        {
            for (int i = 0; i < 65536; i++)
            {
                mem[i] = 0x00;
            }
        }
        public void setByte(ushort add, byte b)
        {
            mem[add] = b;
        }
        public byte getByte(ushort add)
        {
            return mem[add];
        }
        public void setBytes(ushort add, ushort val)
        {
            mem[add] = (byte)(val >> 8);
            mem[add + 1] = (byte)(val & 255);
        }
        public ushort getBytes(ushort add)
        {
            byte[] bytes = new byte[] { mem[add + 1], mem[add] };
            return BitConverter.ToUInt16(bytes);
        }
    }
}
