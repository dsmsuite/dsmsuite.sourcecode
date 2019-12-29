using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DsmSuite.DsmViewer.View.Validation
{
    public class ElementNameMustBeNonEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult result;
            string name = value.ToString();
            if (name.Length == 0)
            {
                result = new ValidationResult(false, "Please enter non empty string");
            }
            else
            {
                result = new ValidationResult(true, null);
            }
            return result;
        }
    }
}
