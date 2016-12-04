using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace app.core
{
    public class Node
    {
        public Point3D point{ get; set;}
        public double coefB { get; set; }
        public double coefC { get; set; }
        public double coefD { get; set; }
        public int id; 

        /*public Node clone()
        {
            Node cloned = new Node();
            cloned.point = new Point3D(this.point.X, this.point.Y, this.point.Z);
            return cloned;
        }*/
    }
}
