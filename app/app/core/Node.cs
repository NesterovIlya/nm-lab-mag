using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace app.core
{
    /*
     * Узел сетки
     */
    public class Node
    {
        // Номер узла
        public int id { get; set; }

        // Координаты узла
        public Point3D point { get; set;}

        // Коэффициенты функции формы (коэффициент А не нужен, потому его здесь нет)
        public double coefB { get; set; }
        public double coefC { get; set; }
        public double coefD { get; set; }

        public Node() {}

        public Node(int id, Point3D point)
        {
            this.id = id;
            this.point = point;
        }

        public Node(int id, Point3D point, double coefB, double coefC, double coefD)
        {
            this.id = id;
            this.point = point;
            this.coefB = coefB;
            this.coefC = coefC;
            this.coefD = coefD;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!obj.GetType().Equals(typeof(Node)))
            {
                return false;
            }

            Node anotherNode = obj as Node;

            return this.id.Equals(anotherNode.id)
                && this.point.Equals(anotherNode.point)
                && this.coefB.Equals(anotherNode.coefB)
                && this.coefC.Equals(anotherNode.coefC)
                && this.coefD.Equals(anotherNode.coefD);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return id + "|(" + point + ")|" + coefB + "|" + coefC + "|" + coefD;
        }
    }
}
