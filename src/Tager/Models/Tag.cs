using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TagManage.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("标签名称")]
        [Required(ErrorMessage ="{0}位必填项!")]
        [MaxLength(50, ErrorMessage ="{0}不能超过{1}位!")]
        [Remote("CheckTag", "Home", ErrorMessage ="已有该标签,请勿重复添加!", HttpMethod ="POST")]
        public string Name { get; set; }

        [DisplayName("链接")]
        [DataType(DataType.Url)]
        [Required(ErrorMessage = "{0}位必填项!")]
        [MaxLength(1000, ErrorMessage = "{0}不能超过{1}位!")]
        public string Url { get; set; }

        public int ClickNum { get; set; }

        public virtual User User { get; set; }
    }
}