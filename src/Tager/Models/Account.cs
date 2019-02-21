using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TagManage.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("邮箱")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage ="{0}为必填项!")]
        [MaxLength(255, ErrorMessage ="{0}不能超过{1}位!")]
        public string Email { get; set; }

        [DisplayName("密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(40, ErrorMessage ="{0}应该在{2}-{1}之间!", MinimumLength =6)]
        public string Password { get; set; }

        public string RCode { get; set; }

        public string FCode { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }

    public class Login
    {
        [DisplayName("邮箱")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [MaxLength(255, ErrorMessage = "{0}不能超过{1}位!")]
        public string Email { get; set; }

        [DisplayName("密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class Register
    {
        [DisplayName("邮箱")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [MaxLength(255, ErrorMessage = "{0}不能超过{1}位!")]
        [Remote("CheckEmail", "Account", ErrorMessage ="该邮箱已注册!", HttpMethod ="POST")]
        public string Email { get; set; }

        [DisplayName("密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        public string Password { get; set; }

        [DisplayName("确认密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage ="密码与确认密码不一致!")]
        public string RePassword { get; set; }
    }

    public class ForgotPassword
    {
        [DisplayName("邮箱")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [MaxLength(255, ErrorMessage = "{0}不能超过{1}位!")]
        public string Email { get; set; }
    }

    public class ResetPassword
    {
        [DisplayName("密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        public string Password { get; set; }

        [DisplayName("确认密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密码与确认密码不一致!")]
        public string RePasswprd { get; set; }
    }
}