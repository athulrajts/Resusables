using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace KEI.Infrastructure.Validation
{
    public abstract class AbstractValidator<T>
    {
        private readonly PropertyInfo[] properties;
        private readonly Dictionary<string, ValidatorGroup> validators = new Dictionary<string, ValidatorGroup>();

        public AbstractValidator()
        {
            properties = typeof(T).GetProperties();
        }

        protected ValidationBuilder ValidationRuleFor<TProperty>(Expression<Func<T, TProperty>> expr)
        {
            var member = ParseMemberName(expr);

            if (!string.IsNullOrEmpty(member))
            {
                var validation = new ValidatorGroup();
                validators.Add(member, validation);

                return new ValidationBuilder(validation);
            }

            return new ValidationBuilder(null);
        }

        private string ParseMemberName(Expression expr)
        {
            if (expr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)expr;
                var member = ParseMemberName(lambda.Body);

                if (!string.IsNullOrEmpty(member))
                    return member;
            }
            else if (expr.NodeType == ExpressionType.MemberAccess)
            {
                var memberAccess = (MemberExpression)expr;

                return memberAccess.Member.Name;
            }

            return string.Empty;
        }

        private PropertyInfo FindProperty(string name)
        {
            foreach (var prop in properties)
            {
                if (prop.Name == name)
                    return prop;
            }
            return null;
        }

        public ValidationResults Validate(T instance)
        {
            var validationResult = new ValidationResults();

            foreach (var item in validators)
            {
                var propInfo = FindProperty(item.Key);
                var result = item.Value.Validate(propInfo.GetValue(instance));

                if (!result.IsValid)
                {
                    validationResult.IsValid = false;
                    validationResult.Errors.Add(new ValidationFailure { Property = item.Key, Error = result.ErrorMessage });
                }
            }

            return validationResult;
        }
    }


    public class ValidationResults
    {
        public bool IsValid { get; set; }
        public List<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>(); 
    }

    public class ValidationFailure
    {
        public string Property { get; set; }
        public string Error { get; set; }
    }
}
