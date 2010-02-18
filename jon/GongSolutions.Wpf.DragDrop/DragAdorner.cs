using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
    class DragAdorner : Adorner
    {
        public DragAdorner(UIElement adornedElement, UIElement adornment)
            : base(adornedElement)
        {
            m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            m_AdornerLayer.Add(this);
            m_Adornment = adornment;
            IsHitTestVisible = false;
        }

        public Point MousePosition 
        {
            get { return m_MousePosition; }
            set
            {
                if (m_MousePosition != value)
                {
                    m_MousePosition = value;
                    m_AdornerLayer.Update(AdornedElement);
                }
            }
        }

        public void Detatch()
        {
            m_AdornerLayer.Remove(this);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            m_Adornment.Arrange(new Rect(finalSize));
            return finalSize;
        }
        
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));

            return result;
        }

        protected override Visual GetVisualChild(int index)
        {
            return m_Adornment;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            m_Adornment.Measure(constraint);
            return m_Adornment.DesiredSize;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        AdornerLayer m_AdornerLayer;
        UIElement m_Adornment;
        Point m_MousePosition;
    }
}
