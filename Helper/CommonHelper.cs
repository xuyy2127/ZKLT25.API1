using System.Linq.Expressions;

namespace ZKLT25.API.Helper
{
    public static class CommonHelper
    {
        public static void UpdateEntity<T>(T entity, object dto)
        {
            var dtoProperties = dto.GetType().GetProperties();
            foreach (var prop in dtoProperties)
            {
                var value = prop.GetValue(dto);
                if (value != null)
                {
                    var entityProp = entity.GetType().GetProperty(prop.Name);
                    entityProp?.SetValue(entity, value);
                }
            }
        }

        /// <summary>
        /// 任务状态存储过程
        /// </summary>
        public static Dictionary<string, string> Func_TransProcState = new Dictionary<string, string>
        {
            { "-99","已合格"},
            { "0","保存"},
            { "1","基础数据不全"},
            { "2","缺料"},
            { "20","替料完成"},
            { "30","调整未生效"},
            { "3","申请生效"},
            { "4","已打印"},
            { "5","计划备料"},
            { "6","已开工"},
            { "7","已完工"},
            { "8","已报入库"},
            { "-2","申请作废"},
            { "-1","作废"},
            { "88","已绑卡"},
            { "1011","发料调度"},
            { "-1011","生产退料"},
            { "1109","备料开始"},
            { "1110","备料完成"},
            { "1111","领料确认"},
            { "-21","作废审批通过"},
            { "-22","作废检验员确认"},
            { "-23","申请作废驳回"},
            { "10","生效"},
            { "11","领料出库"},
            { "12","申请制造主任审批"},
            { "13","供应商已确认"}
        };
    }
}
