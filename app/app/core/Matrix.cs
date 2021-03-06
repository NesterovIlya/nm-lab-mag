﻿using System;
using System.Collections.Generic;
using System.IO;

namespace app.core
{
    public class Matrix : IContainerElement
    {
        private int _rowsCount;
        public int RowsCount
        {
            get { return _rowsCount; }
            private set { }
        }

        private int _columnsCount;
        public int ColumnsCount
        {
            get { return _columnsCount; }
            private set { }
        }
       
        private double[,] _elements;

        public Matrix(int rowsCount, int columnsCount)
        {
            _rowsCount = rowsCount;
            _columnsCount = columnsCount;
            _elements = new double[rowsCount, columnsCount];
        }

        public double this[int row, int column]
        {
            get
            {
                return _elements[row, column];
            }

            set
            {
                _elements[row, column] = value;
            }
        }

        public IContainerElement getTransposed()
        {
            MatrixDimension3 result = new MatrixDimension3();
            for (int i = 0; i < _rowsCount; i++)
            {
                for (int j = 0; j < _columnsCount; j++)
                {
                    result[i, j] = _elements[j, i];
                }
            }
            return result;
        }

        public bool isNeutralElement()
        {
            for (int i = 0; i < _rowsCount; i++)
                for (int j = 0; j < _columnsCount; j++)
                    if (_elements[i, j] != 0)
                        return false;
            return true;
        }

        public double getElement(int row, int column)
        {
            return _elements[row, column];
        }

        public void setElement(int row, int column, double value)
        {
            _elements[row, column] = value;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!obj.GetType().Equals(typeof(Matrix)))
            {
                return false;
            }

            Matrix matrix = obj as Matrix;

            if ((_rowsCount != matrix._rowsCount) || (_columnsCount != matrix._columnsCount))
            {
                return false;
            }

            for (int i = 0; i < _rowsCount; i++)
            {
                for (int j = 0; j < _columnsCount; j++)
                {
                    if (_elements[i, j] != matrix._elements[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public IContainerElement getNeutralElememt()
        {
            return new Matrix(_rowsCount, _columnsCount);
        }

        public Matrix multiply(Matrix matrix)
        {
            if (_columnsCount != matrix._rowsCount)
            {
                return null;
            }
            Matrix result = new Matrix(_rowsCount, matrix._columnsCount);
            double element;
            for (int i = 0; i < _rowsCount; i++)
            {
                for (int j = 0; j < matrix._columnsCount; j++)
                {
                    element = 0;
                    for (int k = 0; k < _columnsCount; k++)
                    {
                        element += _elements[i, k] * matrix._elements[k, j];
                    }
                    result._elements[i, j] = element;
                }
            }

            return result;
        }

        public static Matrix operator* (Matrix a, Matrix b)
        {
            return a.multiply(b);
        }

        public static Matrix operator *(Matrix a, double b)
        {
            Matrix result = new Matrix(a.RowsCount, a.ColumnsCount);
            for (int i = 0; i < a.RowsCount; i++)
            {
                for (int j = 0; j < a.ColumnsCount; j++)
                {
                    result[i, j] = a[i, j] * b;
                }
            }
            return result;
        }

        //TODO удалить после тестирования
        public IList<double> SumByRow()
        {
            IList<double> result = new List<double>();

            for (int rowInd = 0; rowInd < RowsCount; rowInd++)
            {
                double sum = 0;
                for (int colInd = 0; colInd < ColumnsCount; colInd++)
                {
                    sum += this[rowInd, colInd];
                }
                result.Add(sum);
            }
            return result;
        }

        public void printToFile(String path)
        {
            StreamWriter sw = null;
            try
            {
                sw  = new StreamWriter(path);
                for (int rowInd = 0; rowInd < RowsCount; rowInd++)
                {
                    String line = "";
                    for (int colInd = 0; colInd < ColumnsCount; colInd++)
                    {
                        var prefix = _elements[rowInd, colInd] < 0 ? "" : " ";
                        line += prefix + String.Format("{0:0.000}", _elements[rowInd, colInd]) + " ";
                    }
                    sw.WriteLine(line);
                    sw.WriteLine();
                }
            } finally
            {
                sw.Close();
            }

        }

    }
}
