using System.Collections.Generic;

namespace ABPHelper.Models.HelperModels
{
    public class AddNewServiceMethodModel
    {
        public IEnumerable<string> Names { get; set; }

        public bool IsAsync { get; set; } 
    }
}