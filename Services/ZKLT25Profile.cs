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


    }
}
