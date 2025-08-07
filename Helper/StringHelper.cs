using System.Collections.Frozen;

namespace ZKLT25.API.Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static int ToInt32(this string? soure)
        {
            if (string.IsNullOrEmpty(soure))
            {
                return 0;
            }

            try
            {
                var num = soure.Split('.')[0];
                return Convert.ToInt32(num);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string? soure)
        {
            if (string.IsNullOrEmpty(soure))
            {
                return 0;
            }

            try
            {
                return Convert.ToDecimal(soure);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static float ToFloat(this string? soure)
        {
            if (string.IsNullOrEmpty(soure))
            {
                return 0;
            }

            try
            {
                return float.Parse(soure);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string? soure)
        {
            if (string.IsNullOrEmpty(soure))
            {
                return null;
            }

            try
            {
                return Convert.ToDateTime(soure);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static bool ToBool(this string? soure)
        {
            if (string.IsNullOrEmpty(soure))
            {
                return false;
            }

            try
            {
                return Convert.ToBoolean(soure);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 转换DN码
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static string ToDN(this string soure)
        {
            var keys = new Dictionary<string, string>()
            {
                { "1/2\"","DN15"},
                { "3/4\"","DN20"},
                { "1\"" ,"DN25"},
                { "1-1/4\"","DN32" },
                { "1-1/2\"","DN40"},
                { "2\"","DN50" },
                { "2-1/2\"","DN65" },
                { "3\"","DN80"},
                { "4\"","DN100"},
                { "5\"","DN125"},
                { "6\"","DN150" },
                { "8\"","DN200"},
                { "10\"","DN250"},
                { "12\"","DN300"},
                { "14\"","DN350"},
                { "16\"", "DN400"}
            }
            .ToFrozenDictionary();

            if (!keys.ContainsKey(soure))
            {
                return soure;
            }

            return keys[soure];
        }

        /// <summary>
        /// 转换PN码
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static string ToPN(this string soure)
        {
            var keys = new Dictionary<string, string>()
            {
               { "CL150" ,"PN20"},
               { "CL300","PN50"},
               { "CL600","PN110"},
               { "CL900","PN150" },
               { "CL1500","PN260"},
               { "CL2500","PN420"}
            }
            .ToFrozenDictionary();

            if (!keys.ContainsKey(soure))
            {
                return soure;
            }

            return keys[soure];
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static string ToHalfWidth(this string soure)
        {
            var val = soure ?? string.Empty;

            return new string(val.Select(c =>
            {
                if (c >= 65281 && c <= 65374)
                {
                    // 全角字符（除空格）转换为半角字符
                    return (char)(c - 65248);
                }
                else if (c == 12288)
                {
                    // 全角空格转换为半角空格
                    return (char)32;
                }
                return c;
            }).ToArray());
        }

        /// <summary>
        /// 使用非加密的 FNV-1a 算法 获取hashCode
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static int ToStableHashCode(this string soure)
        {
            if (string.IsNullOrEmpty(soure)) return 0;

            const long fnvPrime = 16777619;
            long hash = 2166136261; // 使用long避免溢出

            foreach (char c in soure)
            {
                hash ^= c;
                hash *= fnvPrime;
            }

            return (int)hash;
        }
    }
}
