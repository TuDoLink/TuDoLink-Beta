using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TudolinkWeb.Models
{
    [TableName("TB_MailTask")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class TB_MailTask : Service.SQLiteDababase.Record<TB_MailTask>
    {
        public int Id { get; set; }

        public string Target { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get; set; }

        public string Detail { get; set; }


        public DateTime? SendAt { get; set; }

        public DateTime CreateAt { get; set; }
    }
}