namespace Pulsarc.Utils.Maths
{
    static public class PulsarcMath
    {
        static public float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat + (secondFloat - firstFloat) * by;
        }
    }
}
