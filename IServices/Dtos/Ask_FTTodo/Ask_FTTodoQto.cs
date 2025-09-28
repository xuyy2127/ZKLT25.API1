using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_FTTodoQto : BasePageParams
    {
        public int BillID { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
    }
}


