using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomServiceTestUtil.Classes
{
    public class ResponseResult
    {
        public string Schema { get; set; }
        public Direction ValidationDirection { get; set; }
        public string CallPath { get; set; }
        public string Machine { get; set; }
    }
}
