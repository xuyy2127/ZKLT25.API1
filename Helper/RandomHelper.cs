using MathNet.Numerics.Random;

namespace ZKLT25.API.Helper
{
    public static class RandomHelper
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// 生成指定区间内的随机小数（指定小数位数）
        /// </summary>
        /// <param name="minValue">最小值（包含）</param>
        /// <param name="maxValue">最大值（包含）</param>
        /// <param name="decimalPlaces">保留的小数位数（0 ≤ decimalPlaces ≤ 4）</param>
        /// <returns>随机小数</returns>
        public static decimal GenRandomNumber(decimal minValue, decimal maxValue, int decimalPlaces)
        {
            if (decimalPlaces < 0 || decimalPlaces > 4)
            {
                throw new ArgumentException("小数位数必须在 0 到 4 之间", nameof(decimalPlaces));
            }

            // 生成 [0, 1) 之间的随机数，转换为目标区间
            decimal randomValue = _random.NextDecimal();
            decimal scaledValue = minValue + randomValue * (maxValue - minValue);

            // 保留指定小数位数（四舍五入）
            return Math.Round(scaledValue, decimalPlaces);
        }
    }
}
