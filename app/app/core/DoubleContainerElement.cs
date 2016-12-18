using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core
{
    public class DoubleContainerElement : IContainerElement
    {
        public double element { get; set; }


        public DoubleContainerElement()
        {
            this.element = 0;
        }

        public DoubleContainerElement(double value)
        {
            this.element = value;
        }

        IContainerElement IContainerElement.getNeutralElememt()
        {
            return new DoubleContainerElement(0);
        }

        IContainerElement IContainerElement.getTransposed()
        {
            return this;
        }

        bool IContainerElement.isNeutralElement()
        {
            return 0 == element;
        }

        public override string ToString()
        {
            return element.ToString();
        }

        public static DoubleContainerElement operator +(DoubleContainerElement a, DoubleContainerElement b)
        {
            return new DoubleContainerElement(a.element + b.element);
        }

        public static DoubleContainerElement operator -(DoubleContainerElement a, DoubleContainerElement b)
        {
            return new DoubleContainerElement(a.element - b.element);
        }

        public static DoubleContainerElement operator *(DoubleContainerElement a, DoubleContainerElement b)
        {
            return new DoubleContainerElement(a.element * b.element);
        }

        public static DoubleContainerElement operator /(DoubleContainerElement a, DoubleContainerElement b)
        {
            if (b.element == 0)
            {
                throw new ArithmeticException("Division to 0 error!");
            }
            return new DoubleContainerElement(a.element / b.element);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!obj.GetType().Equals(typeof(DoubleContainerElement)))
            {
                return false;
            }

            DoubleContainerElement anotherElement = obj as DoubleContainerElement;

            return this.element == anotherElement.element;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
