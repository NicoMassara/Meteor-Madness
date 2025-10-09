namespace _Main.Scripts.Utilities
{
    public struct MathfCalculations
    {
        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
        {
            // Normaliza value entre 0 y 1 respecto al rango de entrada
            float t = (value - inMin) / (inMax - inMin);
            // Escala al rango de salida
            return outMin + t * (outMax - outMin);
        }
    }
}