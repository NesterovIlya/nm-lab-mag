﻿using System;
using System.Windows.Media.Media3D;

namespace app.core.model.implementation
{
    class MatrixDimension3 : IContainerElement
    {
        private double[,] _elements;

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

        public MatrixDimension3()
        {
            _elements = new double[3, 3];
        }

        public void transpose()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 3; j++)
                {
                    swap(ref _elements[i, j], ref _elements[j, i]);
                }
            }
        }

        public bool isNeutralElement()
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    if (_elements[i, j] != 0)
                        return false;
            return true;
        }

        private void swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
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

            if (!obj.GetType().Equals(typeof(MatrixDimension3)))
            {
                return false;
            }

            MatrixDimension3 matrix = obj as MatrixDimension3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
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
            return new MatrixDimension3();
        }

        public MatrixDimension3 addMatrix(MatrixDimension3 matrix)
        {
            if (matrix == null)
            {
                return null;
            }

            MatrixDimension3 result = new MatrixDimension3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_elements[i, j] != matrix._elements[i, j])
                    {
                        result[i,j] = _elements[i,j] + matrix[i,j]; 
                    }
                }
            }
            return result;
        }

        public MatrixDimension3 subtractionMatrix(MatrixDimension3 matrix)
        {
            if (matrix == null)
            {
                return null;
            }

            MatrixDimension3 result = new MatrixDimension3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_elements[i, j] != matrix._elements[i, j])
                    {
                        result[i, j] = _elements[i, j] - matrix[i, j];
                    }
                }
            }
            return result;
        }

        public static MatrixDimension3 operator+ (MatrixDimension3 a, MatrixDimension3 b)
        {
            return a.addMatrix(b);
        }

        public static MatrixDimension3 operator- (MatrixDimension3 a, MatrixDimension3 b)
        {
            return a.subtractionMatrix(b);
        }

        public Vector3D vectorMultiplication(Vector3D vector)
        {
            Vector3D result = new Vector3D();

            for (int i = 0; i < 3; i++)
            {
                vector.X += _elements[0, i];
            }

            for (int i = 0; i < 3; i++)
            {
                vector.Y += _elements[1, i];
            }

            for (int i = 0; i < 3; i++)
            {
                vector.Z += _elements[2, i];
            }

            return result;
        }

        public static Vector3D operator* (MatrixDimension3 matrix, Vector3D vector)
        {
            return matrix.vectorMultiplication(vector);
        }

    }
}