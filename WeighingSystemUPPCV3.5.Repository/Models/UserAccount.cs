using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class UserAccount
    {
        [Key]
        public string UserAccountId { get; set; }

        public string UserAccountIdOld { get; set; }


        [Required(ErrorMessage = "User name is required.")]
        [MaxLength(50, ErrorMessage = "User name must not exceed to 50 characters.")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(50, ErrorMessage = "Password must not exceed to 50 characters.")]
        public string UserPwd { get; set; }

        [Compare(nameof(UserAccount.UserPwd), ErrorMessage = "Password confirmation is invalid")]
        [NotMapped]
        public string UserConfirmPwd { get; set; }


        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(60, ErrorMessage = "Full name must not exceed to 60 characters.")]
        public string FullName { get; set; }


        [MaxLength(50, ErrorMessage = "Position must not exceed to 50 characters.")]
        public string Position { get; set; }


        [DefaultValue(true)]
        public Nullable<bool> IsActive { get; set; }


        public virtual UserAccountPermission UserAccountPermission { get; set; }
    }
}
