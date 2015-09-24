using MIPSCoreUI.Bootstrapper;
using System.Windows.Data;
using System.Windows.Media;
namespace MIPSCoreUI.Converter
{
    class InstructionToColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                value = System.Convert.ToUInt32((string)value, 16);
                if ((uint)value == CBootstrapper.Core.InstructionMemory.GetProgramCounter.UnsignedDecimal)
                    return 1;
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush(Colors.Black);
        }
    }
}
