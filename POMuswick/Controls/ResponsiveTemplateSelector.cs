using System;
using System.Collections.Generic;
using System.Text;

namespace POMuswick.Controls
{
    public class ResponsiveTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SmallTemplate { get; set; }
        public DataTemplate LargeTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            // Get the current screen width in device-independent units
            var width = DeviceDisplay.Current.MainDisplayInfo.Width;

            // Choose template based on width (e.g., 600 units for tablet/desktop)
            return width < 721 ? SmallTemplate : LargeTemplate;
        }
    }
}