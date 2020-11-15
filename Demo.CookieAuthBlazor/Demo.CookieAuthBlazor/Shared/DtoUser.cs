using System.Collections.Generic;

namespace Demo.CookieAuthBlazor.Shared
{
    public class DtoUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
    }
}
