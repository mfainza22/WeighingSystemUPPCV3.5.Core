using System.ComponentModel.DataAnnotations;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required.", AllowEmptyStrings = false)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required.",AllowEmptyStrings = false)]
        public string UserPwd { get; set; }
    }
}
