using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMEL.Chart
{
    public class ComboBoxItem
    {
        public static string DisplayName = "Text";
        public static string ValueName = "Value";

        public static EmptyItem EmptyItem = new EmptyItem();
    }

    public class EmptyItem
    {
        public string Text = "- 选择 -";
        public string Value = "";

        public dynamic ToItem()
        {
            return new { Text = Text, Value = Value };
        }
    }
}