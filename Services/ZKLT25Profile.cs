using AutoMapper;
using ZKLT25.API.IServices.Dtos;
using ZKLT25.API.Models;

namespace ZKLT25.API.Services
{
    public class ZKLT25Profile : Profile
    {
        public ZKLT25Profile()
        {
            // Ask_FTList 映射配置
            CreateMap<Ask_FTList, Ask_FTListDto>()
                .ForMember(dest => dest.TypeText, opt => opt.MapFrom(src => GetTypeText(src.isWG)))
                .ForMember(dest => dest.IsAskText, opt => opt.MapFrom(src => GetIsAskText(src.isAsk)));
            
            CreateMap<Ask_FTListCto, Ask_FTList>();
            CreateMap<Ask_FTListUto, Ask_FTList>();

            // Ask_FTFJListLog 映射配置
            CreateMap<Ask_FTFJListLog, Ask_FTFJListLogDto>();

            // Ask_FJList 映射配置
            CreateMap<Ask_FJList, Ask_FJListDto>();

            // Ask_Supplier 映射配置
            CreateMap<Ask_Supplier, Ask_SupplierDto>();
            CreateMap<Ask_SupplierCto, Ask_Supplier>();
            CreateMap<Ask_SupplierUto, Ask_Supplier>();

            // Ask_SuppRangeFT 映射配置
            CreateMap<Ask_SuppRangeFT, Ask_SuppRangeFTDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SuppName : null))
                .ForMember(dest => dest.FTName, opt => opt.MapFrom(src => src.FTList != null ? src.FTList.FTName : null))
                .ForMember(dest => dest.FTVersion, opt => opt.MapFrom(src => src.FTList != null ? src.FTList.FTVersion : null));

            // Ask_Bill 映射配置
            CreateMap<Ask_Bill, Ask_BillDto>()
                .ForMember(dest => dest.BillStateText, opt => opt.MapFrom(src => GetBillStateText(src.BillState)));

            // Ask_DataFT 和 Ask_DataFTOut 共享映射配置
            CreateDataFTMapping<Ask_DataFT>();
            CreateDataFTMapping<Ask_DataFTOut>();
            
            // Ask_DataFJ 和 Ask_DataFJOut 共享映射配置
            CreateDataFJMapping<Ask_DataFJ>();
            CreateDataFJMapping<Ask_DataFJOut>();

            // Ask_CGPriceValue 映射配置
            CreateMap<Ask_CGPriceValue, Ask_CGPriceValueDto>()
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.ExpireTime == null || src.ExpireTime > DateTime.Now));
            CreateMap<Ask_CGPriceValueCto, Ask_CGPriceValue>();
            CreateMap<Ask_CGPriceValueUto, Ask_CGPriceValue>();
        }

        /// <summary>
        /// 创建DataFT相关的映射配置
        /// </summary>
        private void CreateDataFTMapping<T>() where T : class
        {
            CreateMap<T, Ask_DataFTDto>()
                .ForMember(dest => dest.IsPreProBindText, opt => opt.MapFrom(src => GetPreProBindText(GetIsPreProBind(src))))
                .ForMember(dest => dest.PriceStatusText, opt => opt.MapFrom(src => GetPriceStatusText(GetTimeout(src))))
                .ForMember(dest => dest.AvailableActions, opt => opt.MapFrom(src => GetAvailableActions(GetTimeout(src))));
        }

        /// <summary>
        /// 获取IsPreProBind值
        /// </summary>
        private static int GetIsPreProBind<T>(T source)
        {
            return source switch
            {
                Ask_DataFT dataFT => dataFT.IsPreProBind,
                Ask_DataFTOut dataFTOut => dataFTOut.IsPreProBind,
                _ => 0
            };
        }

        /// <summary>
        /// 获取timeout值
        /// </summary>
        private static int? GetTimeout<T>(T source)
        {
            return source switch
            {
                Ask_DataFT dataFT => dataFT.Timeout,
                Ask_DataFTOut dataFTOut => dataFTOut.Timeout,
                _ => null
            };
        }

        /// <summary>
        /// 创建DataFJ相关的映射配置
        /// </summary>
        private void CreateDataFJMapping<T>() where T : class
        {
            CreateMap<T, Ask_DataFJDto>()
                .ForMember(dest => dest.IsPreProBindText, opt => opt.MapFrom(src => GetPreProBindText(GetIsPreProBindFJ(src))))
                .ForMember(dest => dest.PriceStatusText, opt => opt.MapFrom(src => GetPriceStatusText(GetTimeoutFJ(src))))
                .ForMember(dest => dest.AvailableActions, opt => opt.MapFrom(src => GetAvailableActions(GetTimeoutFJ(src))))
                .ForMember(dest => dest.ProjDay, opt => opt.MapFrom(src => GetProjDayFJ(src)))
                .ForMember(dest => dest.Day1, opt => opt.MapFrom(src => GetDay1FJ(src)))
                .ForMember(dest => dest.Day2, opt => opt.MapFrom(src => GetDay2FJ(src)))
                .ForMember(dest => dest.Memo1, opt => opt.MapFrom(src => GetMemo1FJ(src)));
        }

        /// <summary>
        /// 获取DataFJ的IsPreProBind值
        /// </summary>
        private static int GetIsPreProBindFJ<T>(T source)
        {
            return source switch
            {
                Ask_DataFJ dataFJ => dataFJ.IsPreProBind,
                Ask_DataFJOut dataFJOut => dataFJOut.IsPreProBind,
                _ => 0
            };
        }

        /// <summary>
        /// 获取DataFJ的Timeout值
        /// </summary>
        private static int? GetTimeoutFJ<T>(T source)
        {
            return source switch
            {
                Ask_DataFJ dataFJ => dataFJ.Timeout,
                Ask_DataFJOut dataFJOut => dataFJOut.Timeout,
                _ => null
            };
        }

        /// <summary>
        /// 获取类型显示文本
        /// </summary>
        private static string GetTypeText(int? isWG)
        {
            return isWG switch
            {
                1 => "外购", // 1=是外购
                0 => "自制", // 0=否，即自制  
                _ => "未知"
            };
        }

        /// <summary>
        /// 获取是否询价显示文本
        /// </summary>
        private static string GetIsAskText(int? isAsk)
        {
            return isAsk switch
            {
                1 => "是",
                0 => "否",
                _ => "未设置"
            };
        }

        /// <summary>
        /// 获取询价状态显示文本
        /// </summary>
        public static string GetBillStateText(int? state)
        {
            return state switch
            {
                -1 => "已关闭",
                0 => "发起",
                1 => "询价供货商",
                2 => "已完成",
                _ => "未知状态"
            };
        }

        /// <summary>
        /// 获取项目绑定状态显示文本
        /// </summary>
        public static string GetPreProBindText(int isPreProBind)
        {
            return isPreProBind switch
            {
                1 => "绑定项目",
                0 => "不绑定项目",
                _ => "其他"
            };
        }

        /// <summary>
        /// 获取价格状态显示文本
        /// </summary>
        public static string GetPriceStatusText(int? timeout)
        {
            return (timeout.HasValue && timeout.Value > 0) ? "已过期" : "有效";
        }

        /// <summary>
        /// 获取可执行的价格状态操作
        /// </summary>
        public static string GetAvailableActions(int? timeout)
        {
            return (timeout.HasValue && timeout.Value > 0) ? "设置有效" : "延长有效期,设置过期";
        }

        /// <summary>
        /// DataFJ ProjDay值
        /// </summary>
        private static string? GetProjDayFJ<T>(T source)
        {
            return source switch
            {
                Ask_DataFJ dataFJ => dataFJ.ProjDay,
                Ask_DataFJOut dataFJOut => dataFJOut.ProjDay,
                _ => null
            };
        }

        /// <summary>
        /// DataFJ Day1值
        /// </summary>
        private static string? GetDay1FJ<T>(T source)
        {
            return source switch
            {
                Ask_DataFJ dataFJ => dataFJ.Day1,
                Ask_DataFJOut dataFJOut => dataFJOut.Day1,
                _ => null
            };
        }

        /// <summary>
        /// DataFJ Day2值
        /// </summary>
        private static string? GetDay2FJ<T>(T source)
        {
            return source switch
            {
                Ask_DataFJ dataFJ => dataFJ.Day2,
                Ask_DataFJOut dataFJOut => dataFJOut.Day2,
                _ => null
            };
        }

        /// <summary>
        /// DataFJ Memo1值
        /// </summary>
        private static string? GetMemo1FJ<T>(T source)
        {
            return source switch
            {
                Ask_DataFJ dataFJ => dataFJ.Memo1,
                Ask_DataFJOut dataFJOut => dataFJOut.Memo1,
                _ => null
            };
        }

    }
}
