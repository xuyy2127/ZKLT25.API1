using System.ComponentModel;
using System.Reflection;

namespace ZKLT25.API.Helper
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 根据枚举名称获取枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetEnumByName<T>(this string name) where T : System.Enum
        {
            try
            {
                return (T)System.Enum.Parse(typeof(T), name);
            }
            catch
            {
                throw new Exception($"名称{name}转换为枚举{typeof(T).Name}失败，请检查枚举中是否存在！");
            }
        }

        /// <summary>
        /// 根据枚举名称获取枚举备注
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetDescriptionByName<T>(this string name) where T : System.Enum
        {
            return GetEnumByName<T>(name).GetDescription();
        }

        /// <summary>
        /// 获取枚举信息集合
        /// </summary>
        /// <typeparam name="T">枚举类名</typeparam>
        /// <returns></returns>
        public static List<EnumModel> EnumToList<T>() where T : System.Enum
        {
            List<EnumModel> list = new List<EnumModel>();
            foreach (System.Enum @enum in System.Enum.GetValues(typeof(T)))
            {
                list.Add(new EnumModel
                {
                    Value = Convert.ToInt32(@enum),
                    Name = @enum.ToString(),
                    Description = @enum.GetDescription()
                });
            }
            return list;
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="@enum"></param>
        /// <returns></returns>
        public static string GetDescription(this System.Enum @enum)
        {
            Type type = @enum.GetType();
            MemberInfo[] memInfo = type.GetMember(@enum.ToString());
            if (memInfo == null || !memInfo.Any())
            {
                return string.Empty;
            }

            object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs == null || !attrs.Any())
            {
                return string.Empty;
            }
            return ((DescriptionAttribute)attrs[0]).Description;
        }

        /// <summary>
        /// 根据枚举备注获取枚举
        /// </summary>
        /// <param name="@enum"></param>
        /// <returns></returns>
        public static T GetEnumByDescription<T>(this string description) where T : System.Enum
        {
            var list = EnumToList<T>();

            var model = list.Where(x => x.Description == description).FirstOrDefault();
            if (model == null)
            {
                throw new BizException($"枚举{nameof(T)}中未找到备注为{description}的数据！");
            }

            return GetEnumByName<T>(model.Name);
        }
    }
}
