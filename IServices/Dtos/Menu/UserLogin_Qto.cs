namespace ZKLT25.API.IServices.Dtos
{
    public class UserLogin_Qto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int isExternal
        {
            get; set;
        }
    }
}
