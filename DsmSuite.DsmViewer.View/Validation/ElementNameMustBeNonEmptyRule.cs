using System.Windows.Controls;

namespace DsmSuite.DsmViewer.View.Validation
{
    public class ElementNameMustBeNonEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult result;
            string name = value.ToString();
            result = name.Length == 0 ? new ValidationResult(false, "Please enter non empty string") : new ValidationResult(true, null);
            return result;
        }
    }
}
