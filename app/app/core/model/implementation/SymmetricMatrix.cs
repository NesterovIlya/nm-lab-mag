using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core.model.implementation
{
    class SymmetricMatrix<T> : IContainerElement where T : IContainerElement
    {
        private List<List<T>> _elements;
        private int _dimension;
        private int _bandWidth;
        private T _neutralElement;

        public T this[int row, int column]
        {
            get
            {
                return getElement(row, column);
            }

            set
            {
                setElement(row, column, value);
            }
        }

        public SymmetricMatrix(int dimention, T neutralElement)
        {
            _dimension = dimention;
            _elements = new List<List<T>>(dimention);
            for (int i = 0; i < _dimension; i++)
            {
                _elements[i] = new List<T>();
            }
            _neutralElement = neutralElement;
        }

        private T getElement(int row, int column)
        {
            if ((row >= _dimension) || (column >= _dimension))
            {
                throw new IndexOutOfRangeException();
            }

            bool transpose = row > column;
            normalizeIndexes(ref row, ref column);
            if (_elements[row].Count > column)
            {
                T value = _elements[row][column];
                if (transpose)
                {
                    value.transpose();
                }
                return value;
            }
            else
            {
                return _neutralElement;
            }
        }

        public void setElement(int row, int column, T value)
        {
            if ((row >= _dimension) || (column >= _dimension))
            {
                throw new IndexOutOfRangeException();
            }

            if (row > column)
            {
                value.transpose();
            }

            normalizeIndexes(ref row, ref column);
            if (_elements[row].Count <= column)
            {
                for (int i = _elements[row].Count; i < column; i++)
                {
                    _elements[row].Add(_neutralElement);
                }
                _elements[row].Add(value);
                if (_elements[row].Count > _bandWidth)
                    _bandWidth = _elements[row].Count;
            }
            else
            {
                _elements[row][column] = value;
            }
        }

        private void normalizeIndexes(ref int i, ref int j)
        {
            if (i > j)
            {
                int temp = i;
                i = j;
                j = temp;
            }

            j = j - i + 1;
        }

        public bool isNeutralElement()
        {
            foreach (List<T> row in _elements)
            {
                if (row.Count != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void transpose() { }

        public IContainerElement getNeutralElememt()
        {
            return _neutralElement;
        }

        public int getBandWidth()
        {
            return _bandWidth;
        }
    }
}
