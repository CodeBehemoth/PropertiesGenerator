using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace PropertiesGenerator
{
    /// <summary>
    /// Base converter class with provides using of converters without x:Key="..."
    /// </summary>
    public abstract class BaseConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue( IServiceProvider serviceProvider )
        {
            return this;
        }

        #region IValueConverter Members

        public abstract object Convert( object value, Type targetType, object parameter, CultureInfo culture );

        public abstract object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture );

        #endregion
    }
}
