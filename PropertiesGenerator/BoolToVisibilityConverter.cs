using System;
using System.Windows;
using System.Windows.Data;

namespace PropertiesGenerator
{
    /// <summary>
    /// Using BoolToVisibilityConverter as markup extension
    /// </summary>
    [ValueConversion( typeof( bool ), typeof( Visibility ) )]
    public class BoolToVisibilityConverter : BaseConverter
    {
        /// <summary>
        /// Should be default constructor. See https://msdn.microsoft.com/en-us/library/ee855815.aspx#naming_the_support_type
        /// </summary>
        public BoolToVisibilityConverter()
        {
        }

        public override object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;

        }

        public override object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            return Binding.DoNothing;
        }
    }

}
