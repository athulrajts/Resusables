using KEI.Infrastructure.Validation;

namespace KEI.Infrastructure
{
    public class PropertyObjectBuilder
    {
        private readonly PropertyObject prop;

        public PropertyObjectBuilder(PropertyObject obj)
        {
            prop = obj;
        }

        public PropertyObjectBuilder()
        {

        }

        public virtual PropertyObjectBuilder SetCategory(string category)
        {
            prop.Category = category;
            return this;
        }
        
        public virtual PropertyObjectBuilder SetDescription(string description)
        {
            prop.Description = description;
            return this;
        }

        public virtual PropertyObjectBuilder SetDisplayName(string displayName)
        {
            prop.DisplayName = displayName;
            return this;
        }

        public virtual PropertyObjectBuilder SetValidation(ValidatorGroup validation)
        {
            prop.Validation = validation;
            return this;
        }
    }

    public class NumericPropertyObjectBuilder : PropertyObjectBuilder
    {
        private readonly INumericPropertyObject prop;

        public NumericPropertyObjectBuilder(PropertyObject obj) : base(obj)
        {
            prop = (INumericPropertyObject)obj;
        }

        public override NumericPropertyObjectBuilder SetCategory(string category)
        {
            return (NumericPropertyObjectBuilder)base.SetCategory(category);
        }

        public override NumericPropertyObjectBuilder SetDescription(string description)
        {
            return (NumericPropertyObjectBuilder)base.SetDescription(description);
        }

        public override NumericPropertyObjectBuilder SetDisplayName(string displayName)
        {
            return (NumericPropertyObjectBuilder)base.SetDisplayName(displayName);
        }

        public override NumericPropertyObjectBuilder SetValidation(ValidatorGroup validation)
        {
            return (NumericPropertyObjectBuilder)base.SetValidation(validation);
        }

        public NumericPropertyObjectBuilder SetIncrement(object increment)
        {
            prop.Increment = increment;
            return this;
        }
    }

    public class FilePropertyObjectBuilder : PropertyObjectBuilder
    {
        private readonly IFileProperty prop;

        public FilePropertyObjectBuilder(PropertyObject obj) : base(obj)
        {
            prop = (IFileProperty)obj;
        }

        public override FilePropertyObjectBuilder SetCategory(string category)
        {
            return (FilePropertyObjectBuilder)base.SetCategory(category);
        }

        public override FilePropertyObjectBuilder SetDescription(string description)
        {
            return (FilePropertyObjectBuilder)base.SetDescription(description);
        }

        public override FilePropertyObjectBuilder SetDisplayName(string displayName)
        {
            return (FilePropertyObjectBuilder)base.SetDisplayName(displayName);
        }

        public override FilePropertyObjectBuilder SetValidation(ValidatorGroup validation)
        {
            return (FilePropertyObjectBuilder)base.SetValidation(validation);
        }

        public FilePropertyObjectBuilder AddFilter(string description, string extension)
        {
            prop.Filters.Add(new(description, extension));

            return this;
        }
    }

    public interface INumericPropertyObject
    {
        public object Increment { get; set; }
    }
}
