//using KEI.Infrastructure.Validation;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;

//namespace KEI.UI.Wpf.AttachedProperties
//{
//    public class TextBoxBehaviours
//    {
//        public static ValidatorGroup GetValidators(DependencyObject obj)
//        {
//            var collection = (ValidatorGroup)obj.GetValue(ValidatorsProperty);
//            if (collection == null)
//            {
//                collection = new ValidatorGroup();
//                obj.SetValue(ValidatorsProperty, collection);
//            }
//            return collection;
//        }

//        public static void SetValidators(DependencyObject obj, ValidatorGroup value)
//        {
//            obj.SetValue(ValidatorsProperty, value);
//        }

//        // Using a DependencyProperty as the backing store for Validators.  This enables animation, styling, binding, etc...
//        public static readonly DependencyProperty ValidatorsProperty =
//            DependencyProperty.RegisterAttached("ValidatorsInternal", typeof(ValidatorGroup), typeof(TextBoxBehaviours), new PropertyMetadata(null, ValidatorsChanged));

//        private static void ValidatorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (d is TextBox tb)
//            {
//                tb.TextChanged -= Tb_TextChanged;
//                tb.TextChanged += Tb_TextChanged;

//                validBrush = tb.Background;
//                ValidateText(tb);
//            }
//        }

//        private static Brush validBrush;
//        private static void Tb_TextChanged(object sender, TextChangedEventArgs e)
//        {
//            ValidateText(sender as TextBox);
//        }

//        private static void ValidateText(TextBox tb)
//        {
//            IList<IValidationRule> validators = GetValidators(tb);

//            foreach (var validator in validators)
//            {
//                var result = validator.Validate(tb.Text);
//                tb.ToolTip = result.IsValid ? null : result.ErrorMessage;
//                tb.Background = result.IsValid ? validBrush : Brushes.Red;
//            }
//        }
//    }
//}
