using System.ComponentModel.DataAnnotations;

namespace JwtAuthSample
{
    public class LoginViewModel
    {
        //用户名
        [Required]
        public string User { get; set; }
        //密码
        [Required]
        public string Password { get; set; }

    }
}