using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DefaultsExample {
    public class DemoDataTemplateSelector : DataTemplateSelector {
         public DataTemplate TemplateEven { get; set; }
         public DataTemplate TemplateOdd { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            var str = (string)item;
            Debug.Assert(str.StartsWith("Item"));
            var number = Int32.Parse(str.Substring(4));
            return (number & 0x01) == 0 ? TemplateEven : TemplateOdd;
        }
    }
}