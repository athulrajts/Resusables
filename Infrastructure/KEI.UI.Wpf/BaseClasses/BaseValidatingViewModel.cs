using System;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using KEI.Infrastructure.Validation;
using KEI.Infrastructure.Validation.Attributes;

namespace KEI.UI.Wpf
{
    public abstract class BaseValidatingViewModel : BaseViewModel, IDataErrorInfo
    {
        #region Private Fields

        protected Dictionary<string, bool> _errors = new Dictionary<string, bool>();
        protected Dictionary<string, ValidatorGroup> _validators = new Dictionary<string, ValidatorGroup>();
        private PropertyInfo[] _props;

        #endregion

        public BaseValidatingViewModel()
        {
            _props = GetType().GetProperties();

            foreach (var prop in _props)
            {
                var validations = prop.GetCustomAttributes<ValidationAttribute>(true);

                foreach (var validation in validations)
                {
                    if (!_validators.ContainsKey(prop.Name))
                    {
                        _validators.Add(prop.Name, new ValidatorGroup());
                    }

                    _validators[prop.Name].Rules.Add(validation.GetValidator());
                }
            }
        }

        public bool HasError
        {
            get
            {
                if (_errors.Count == 0)
                    return true;

                foreach (var item in _errors)
                {
                    if (item.Value == false)
                        return false;
                }

                return true;
            }
        }

        protected ValidationResult Validate(string name)
        {
            foreach (var prop in _props)
            {
                if (prop.Name == name)
                {
                    if (_validators.ContainsKey(name))
                    {
                        return _validators[name].Validate(prop.GetValue(this));
                    }
                }
            }

            return new ValidationResult(true);
        }

        #region Protected Functions

        protected ValidationBuilder ValidationRuleFor<T>(Expression<Func<T>> expr)
        {
            var member = ParseMemberName(expr);

            if (!string.IsNullOrEmpty(member))
            {
                if (!_validators.ContainsKey(member))
                {
                    var validation = new ValidatorGroup();
                    _validators.Add(member, validation);
                }

                return new ValidationBuilder(_validators[member]);
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

        #endregion


        #region IDataErrorInfo Members
        
        public string this[string name]
        {
            get
            {
                ValidationResult result = Validate(name);

                if (_errors.ContainsKey(name))
                {
                    _errors[name] = result.IsValid;
                }
                else
                {
                    _errors.Add(name, result.IsValid);
                }

                RaisePropertyChanged(nameof(HasError));

                return result.ErrorMessage;
            }
        }
        
        public virtual string Error => null;

        #endregion
    }
}
