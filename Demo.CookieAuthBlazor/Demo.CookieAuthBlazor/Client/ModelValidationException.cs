using System;
using System.Collections.Generic;

namespace Demo.CookieAuthBlazor.Client
{
    public class ModelValidationException: Exception
    {
        public Dictionary<string, List<string>> ModelErrors { get; }

        public ModelValidationException(Dictionary<string, List<string>> modelErrors)
        {
            ModelErrors = modelErrors;
        }
    }
}
