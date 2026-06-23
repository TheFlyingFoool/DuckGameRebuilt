using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.XnaToFna
{
    public class GenericRelinkEntry
    {
        public string TargetType;
        public string TargetMethod;

        public GenericRelinkEntry(string targetType, string targetMethod)
        {
            TargetType = targetType;
            TargetMethod = targetMethod;
        }
    }
}
