using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TudolinkWeb.Models
{
    [TableName("TB_Recharge")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class TB_Recharge : Service.SQLiteDababase.Record<TB_Recharge>
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string TelegramHandle { get; set; }

        public string DesiredContribution { get; set; }

        public string ETHaddress { get; set; }

        public DateTime CreateAt { get; set; }
    }
}