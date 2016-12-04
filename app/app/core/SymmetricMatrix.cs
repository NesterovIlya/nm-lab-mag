using System;
using System.Collections.Generic;

namespace app.core
{
    public class SymmetricMatrix<T> : IContainerElement where T : IContainerElement
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

        public SymmetricMatrix(int dimension, T neutralElement)
        {
            _dimension = dimension;
            _elements = new List<List<T>>(dimension);
            for (int i = 0; i < _dimension; i++)
            {
                _elements.Add(new List<T>());
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
                return transpose ? (T) value.getTransposed() : value;
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
                value = (T) value.getTransposed();
            }

            normalizeIndexes(ref row, ref column);
            if (_elements[row].Count <= column)
            {
                if (!value.isNeutralElement())
                {
                    for (int i = _elements[row].Count; i < column; i++)
                    {
                        _elements[row].Add(_neutralElement);
                    }
                    _elements[row].Add(value);
                    if (_elements[row].Count > _bandWidth)
                        _bandWidth = _elements[row].Count;
                }
            }
            else
            {
                _elements[row][column] = value;
                if (value.isNeutralElement())
                {
                    normalizeRow(row);
                    calculateBandWidth();
                }
            }
        }

        private void calculateBandWidth()
        {
            _bandWidth = 0;
            foreach (List<T> row in _elements)
            {
                if (row.Count > _bandWidth)
                {
                    _bandWidth = row.Count;
                }
            }
        }

        private void normalizeRow(int row)
        {
            for (int i = _elements[row].Count - 1; i >= 0; i--)
            {
                if (_elements[row][i].isNeutralElement())
                {
                    _elements[row].RemoveAt(i);
                }
                else
                {
                    return;
                }
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

            j = j - i;
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

        public IContainerElement getTransposed() { return this; }

        public IContainerElement getNeutralElememt()
        {
            return new SymmetricMatrix<T>(_dimension, _neutralElement);
        }

        public int getBandWidth()
        {
            return _bandWidth;
        }

    }
}
