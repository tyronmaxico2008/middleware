using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{

    public class clsMsg
    {
        public string Message { get; set; }
        public object Obj { get; set; }
        public string MessageType = "Message";
        public string Info { get; set; }

        public clsMsg()
        {
            this.Message = "";
        }
        public clsMsg(string msg)
        {

            Message = msg;
        }

        public clsMsg(string msg, object val)
        {
            this.Message = msg;
            this.Obj = val;
        }

        public bool Validated
        {
            get
            {
                return Message.isEmpty() ? true : false;
            }
        }
    }


}
