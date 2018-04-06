using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TudolinkWeb.Models
{
    [TableName("TB_ValidCode")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class TB_ValidCode : Service.SQLiteDababase.Record<TB_ValidCode>
    {
        public int Id { get; set; }

        public string Target { get; set; }

        public string Code { get; set; }

        public string IP { get; set; }

        public DateTime CreateAt { get; set; }
    }
}