using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HorseSpot.Infrastructure.Validators
{
    public static class ValidationHelper
    {
        public static void ValidateModelAttributes<T>(T model)
        {
            var context = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);

            if (!isValid)
            {
                var errorMessages = new StringBuilder();

                foreach (var msg in validationResults)
                {
                    errorMessages.Append(msg.ErrorMessage);
                }

                throw new Exceptions.ValidationException(errorMessages.ToString());
            }
        }
    }
}
