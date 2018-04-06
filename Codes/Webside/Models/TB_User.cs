using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TudolinkWeb.Models
{
    [TableName("TB_User")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class TB_User : Service.SQLiteDababase.Record<TB_User>
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }


        public string Email { get; set; }

        /// <summary>
        /// 邮箱已认证
        /// </summary>
        public bool EmailValid { get; set; }

        public DateTime? EmailValidAt { get; set; }


        /// <summary>
        /// 推荐用户Id
        /// </summary>
        public int? RecommendBy { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}