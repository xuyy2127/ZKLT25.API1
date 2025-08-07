namespace ZKLT25.API.Helper.TokenModule
{
    public class JwtTokenModel
    {
        public string UserId { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expires { get; set; }
        public string Security { get; set; }
        public string UserName { get; set; }
        public string DepartId { get; set; }
        public string Leader { get; set; }
        public int? IsManager { get; set; }
        public string? DepartName { get; set; }
        public int IsCustomer { get; set; } = 0;
    }
}
