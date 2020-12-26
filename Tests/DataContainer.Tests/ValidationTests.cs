using KEI.Infrastructure.Validation;
using System.IO;
using Xunit;

namespace DataContainer.Tests
{
    public class ValidationTests
    {

        #region LengthValidator
        
        [Fact]
        public void LengthValidator_ValidateExact()
        {
            IValidationRule rule = ValidationBuilder.Create().Length(3).Validator;

            var result1 = rule.Validate("124");
            var result2 = rule.Validate("1234");
            var result3 = rule.Validate(123);
            var result4 = rule.Validate("1");

            Assert.True(result1.IsValid);
            Assert.False(result2.IsValid);
            Assert.False(result3.IsValid);
            Assert.False(result4.IsValid);
        }

        [Fact]
        public void LengthValidator_ValidateRange()
        {
            IValidationRule rule = ValidationBuilder.Create().Length(3, 5).Validator;

            var result1 = rule.Validate("12");
            var result2 = rule.Validate("123");
            var result3 = rule.Validate("1234");
            var result4 = rule.Validate("12345");
            var result5 = rule.Validate("123456");

            Assert.False(result1.IsValid);
            Assert.True(result2.IsValid);
            Assert.True(result3.IsValid);
            Assert.True(result4.IsValid);
            Assert.False(result5.IsValid);
        }

        #endregion

        [Fact]
        public void NotNullOrEmptyValidator_FailsForNullOrEmpty()
        {
            IValidationRule rule = ValidationBuilder.Create().NotNullOrEmpty().Validator;

            var result1 = rule.Validate(null);
            var result2 = rule.Validate(string.Empty);
            var result3 = rule.Validate("");

            Assert.False(result1.IsValid);
            Assert.False(result2.IsValid);
            Assert.False(result3.IsValid);
        }

        [Fact]
        public void NotNullOrEmptyValidator_PassesNotNull()
        {
            IValidationRule rule = ValidationBuilder.Create().NotNullOrEmpty().Validator;

            var result1 = rule.Validate("dsfj;kdsf");
            var result2 = rule.Validate(new object());

            Assert.True(result1.IsValid);
            Assert.True(result2.IsValid);
        }


        [Fact]
        public void PathValidator_FileValidation()
        {
            IValidationRule rule = ValidationBuilder.Create().File().Validator;

            string filename = "test.txt";
            
            var stream = File.Create(filename);
            stream.Close();
            stream.Dispose();
            var result1 = rule.Validate(filename);
            Assert.True(result1.IsValid);


            File.Delete(filename);
            var result2 = rule.Validate(filename);
            Assert.False(result2.IsValid);
        }

        [Fact]
        public void PathValidator_DirectoryValidation()
        {
            IValidationRule rule = ValidationBuilder.Create().Directory().Validator;

            string dir = "TestDirectory";

            Directory.CreateDirectory(dir);
            var result1 = rule.Validate(dir);
            Assert.True(result1.IsValid);


            Directory.Delete(dir);
            var result2 = rule.Validate(dir);
            Assert.False(result2.IsValid);
        }
    }
}
