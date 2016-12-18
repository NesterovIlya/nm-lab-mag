﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core
{
    /**
     * Тетраэдр
     */
    public class Element
    {
        // Номер элемента (тетраэдра)
        public int id { get; set; }

        // Узлы элемента (тетраэдра)
        public Node nodeI { get; set; }
        public Node nodeJ { get; set; }
        public Node nodeK { get; set; }
        public Node nodeP { get; set; }
      

        public Element(int id, Node nodeI, Node nodeJ, Node nodeK, Node nodeP)
        {
            this.id = id;
            this.nodeI = nodeI;
            this.nodeJ = nodeJ;
            this.nodeK = nodeK;
            this.nodeP = nodeP;
        }
    }
}
