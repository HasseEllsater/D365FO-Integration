using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomServiceTestUtil
{
    public class JSONValidation
    {
        public string Schema { get; set; }
        public string JSON { get; set; }
        public string ValidationResult { get; set; }

        public bool ValidationPerformed { get; set; }
    }
}
