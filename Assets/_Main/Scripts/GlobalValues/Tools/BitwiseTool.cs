using System;

namespace MeteorMadness.GlobalValues.Tools
{
    public static class BitwiseTool
    {
        public static byte Set(byte word, int n)
        {
            ValidateRange(n);
            return (byte)(word | (1 << n));
        }

        public static byte Clear(byte word, int n)
        {
            ValidateRange(n);
            return (byte)(word & ~(1 << n));
        }

        public static byte Toggle(byte word, int n)
        {
            ValidateRange(n);
            return (byte)(word ^ (1 << n));
        }

        public static bool Read(byte word, int n)
        {
            ValidateRange(n);
            return ((word >> n) & 1) != 0;
        }
        
        public static byte Write(byte word, int n, bool value)
        {
            ValidateRange(n);
            return value
                ? (byte)(word | (1 << n))
                : (byte)(word & ~(1 << n));
        }

        private static void ValidateRange(int n)
        {
            if (n < 0 || n > 7)
                throw new ArgumentOutOfRangeException(nameof(n), "Bit index must be between 0 and 7.");
        }
    }
    
    public static class BitwiseGroupTool
    {
        private const int GroupCount = 2;
        private const int GroupSize = 4;

        // Setea un grupo a un valor (0-3)
        public static byte Set(byte word, int groupIndex, byte value)
        {
            if (!IsGroupInRange(groupIndex)) return word;
            if (!IsValueInRange(value)) throw new ArgumentOutOfRangeException(nameof(value), "Value must be 0-3.");

            int shift = groupIndex * GroupSize;
            word &= (byte)~(0b11 << shift);   // Limpiar el grupo
            word |= (byte)(value << shift);   // Poner el nuevo valor
            return word;
        }

        // Limpia (pone a 0) un grupo
        public static byte Clear(byte word, int groupIndex)
        {
            if (!IsGroupInRange(groupIndex)) return word;

            int shift = groupIndex * GroupSize;
            word &= (byte)~(0b11 << shift);
            return word;
        }

        // Alterna el valor de un grupo (ejemplo: 0->3, 1->2, etc.)
        public static byte Toggle(byte word, int groupIndex)
        {
            if (!IsGroupInRange(groupIndex)) return word;

            int shift = groupIndex * GroupSize;
            byte currentValue = (byte)((word >> shift) & 0b11);
            byte toggledValue = (byte)(currentValue ^ 0b11); // Simple XOR para alternar bits
            word &= (byte)~(0b11 << shift);                  // Limpiar grupo
            word |= (byte)(toggledValue << shift);          // Poner valor alternado
            return word;
        }

        // Lee el valor de un grupo
        public static byte Read(byte word, int groupIndex)
        {
            if (!IsGroupInRange(groupIndex)) return 0;

            int shift = groupIndex * GroupSize;
            return (byte)((word >> shift) & 0b11);
        }

        private static bool IsGroupInRange(int groupIndex)
        {
            if (groupIndex is >= 0 and < GroupCount) return true;
            throw new ArgumentOutOfRangeException(nameof(groupIndex), $"Group index must be between 0 and {GroupCount - 1}.");
        }

        private static bool IsValueInRange(byte value)
        {
            return value <= 3;
        }
    }
    
}