using Microsoft.EntityFrameworkCore;
using ZKLT25.API.Models;

namespace ZKLT25.API.EntityFrameworkCore
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysAuth> SysAuth { get; set; }
        public DbSet<SysDepart> SysDepart { get; set; }
        public DbSet<Favorite> Favorite { get; set; }
        public DbSet<Base_SysMenu> Base_SysMenu { get; set; }
        public DbSet<Ask_FTList> Ask_FTList { get; set; }
        public DbSet<Ask_FJList> Ask_FJList { get; set; }
        public DbSet<Ask_FTFJListLog> Ask_FTFJListLog { get; set; }
        public DbSet<Ask_Supplier> Ask_Supplier { get; set; }
        public DbSet<Ask_SuppRangeFT> Ask_SuppRangeFT { get; set; }
        public DbSet<Ask_SuppRangeFJ> Ask_SuppRangeFJ { get; set; }
        public DbSet<Ask_Bill> Ask_Bill { get; set; }
        public DbSet<Ask_DataFT> Ask_DataFT { get; set; }
        public DbSet<Ask_BillDetail> Ask_BillDetail { get; set; }
        public DbSet<Ask_BillPrice> Ask_BillPrice { get; set; }
        public DbSet<Ask_BillLog> Ask_BillLog { get; set; }
        public DbSet<Price_Bill> Price_Bill { get; set; }

        [DbFunction(Name = "func_TransState", Schema = "dbo")]
        public static string func_TransState(string state)
        {
            throw new NotSupportedException("此方法仅用于LINQ查询映射");
        }
    }
}
