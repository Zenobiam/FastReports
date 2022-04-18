using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FastReport.Design.PageDesigners.Page
{
    internal class EventIndicator
    {
        GraphicsPath indicatorPath;

        Dictionary<Type, PropertyDescriptorCollection> propertiesCollection;

        public bool HaveToDraw(ComponentBase obj)
        {
            Type type = obj.GetType();
            if (!propertiesCollection.ContainsKey(type))
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);

                PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
                foreach (PropertyDescriptor prop in props)
                {
                    BrowsableAttribute attr = prop.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
                    // skip nonbrowsable properties
                    if (attr != null && attr.Browsable == false) continue;
                    // check if property is an event
                    if (prop.Name.EndsWith("Event"))
                        properties.Add(prop);
                }
                propertiesCollection[type] = properties;
            }

            foreach(PropertyDescriptor prop in propertiesCollection[type])
            {
                string value = prop.GetValue(obj) as string;

                if (value != null && value != "")
                    return true;
            }
            return false;
        }

        public void DrawIndicator(ComponentBase obj, FRPaintEventArgs paintArgs)
        {
            IGraphics g = paintArgs.Graphics;
            IGraphicsState state = g.Save();
            g.TranslateTransform(obj.AbsLeft * paintArgs.ScaleX, obj.AbsTop * paintArgs.ScaleY);
            g.ScaleTransform(paintArgs.ScaleX, paintArgs.ScaleY);
            g.FillPath(Brushes.Red, indicatorPath);
            g.Restore(state);
        }

        public EventIndicator()
        {
            propertiesCollection = new Dictionary<Type, PropertyDescriptorCollection>();
            indicatorPath = new GraphicsPath();
            indicatorPath.StartFigure();
            indicatorPath.AddPolygon(new PointF[] { new PointF(3, 3), new PointF(10, 8), new PointF(3, 13) });
            indicatorPath.CloseFigure();
        }
    }
}
