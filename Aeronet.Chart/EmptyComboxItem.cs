using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aeronet.Chart
{
    public class ComboBoxItem
    {
        public static string DisplayName = "Text";
        public static string ValueName = "Value";

        public static EmptyItem EmptyItem = new EmptyItem();
    }

    public class EmptyItem
    {
        public string Text = "- Select -";
        public string Value = "";

        public dynamic ToItem()
        {
            return new { Text = Text, Value = Value };
        }
    }
}