using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetworkModel
{
    public class NonHierarchicContainer
    {
        public UInt32 Size
        {
            get
            {
                return (UInt32)size;
            }
            set
            {
                size = (int)value;
                if (size > maxSize)
                {
                    maxSize = (size > MAX_SIZE) ? size : MAX_SIZE;
                    data = new BitArray[maxSize];
                    for (int i = 1; i < maxSize; i++)
                    {
                        data[i] = new BitArray(i, false);
                    }
                }
                else
                {
                    for (int i = 1; i < maxSize; i++)
                    {
                        data[i].SetAll(false);
                    }
                }

                degrees.Clear();
                for (int i = 0; i < size; ++i)
                {
                    degrees.Add(0);
                }
                numberOfEdges = 0;
            }
        }

        /// <summary>
        /// Gets adjacency matrix for the network.
        /// </summary>
        /// <returns>Adjacency matrix.</returns>
        public BitArray[] GetMatrix()
        {
            BitArray[] matrix = new BitArray[Size];
            for (int i = 0; i < Size; ++i)
            {
                matrix[i] = new BitArray((int)Size);
                matrix[i][i] = false;
                for (int j = 0; j < i; ++j)
                {
                    matrix[i][j] = matrix[j][i] = AreConnected(i, j);
                }
            }
            return matrix;
        }

        /// <summary>
        /// Sets given adjacency matrix on the network.
        /// </summary>
        /// <param name="matrix">Given adjacency matrix.</param>
        /// <note>Specific for each network model type.</note>
        public void SetMatrix(BitArray[] matrix)
        {
            Size = (uint)matrix.Length;
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i][j])
                    {
                        AddConnection(i, j);
                    }
                }
            }
        }

        public NonHierarchicContainer Clone()
        {
            NonHierarchicContainer clone = (NonHierarchicContainer)this.MemberwiseClone();

            clone.data = new BitArray[maxSize];
            for (int i = 1; i < maxSize; i++)
            {
                clone.data[i] = new BitArray(i, false);
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = i + 1; j < size; j++)
                {                  
                    if (AreConnected(i, j))
                    {
                        clone.AddConnection(i, j);
                    }
                }
            }
            clone.degrees = new List<int>(this.degrees);

            return clone;
        }

        private const int MAX_SIZE = 1024;
        private int size;
        private int maxSize;
        private int numberOfEdges;
        private BitArray[] data;
        private List<int> degrees;

        public List<int> Degrees
        {
            get { return degrees; }
        }

        public NonHierarchicContainer()
        {
            size = 0;
            maxSize = MAX_SIZE;
            data = new BitArray[maxSize];
            for (int i = 1; i < maxSize; i++)
            {
                data[i] = new BitArray(i, false);
            }
            degrees = new List<int>();
            numberOfEdges = 0;
        }

        public int AddVertex()
        {
            if (maxSize == 0)
            {
                maxSize = 1;
                data = new BitArray[maxSize];
            }
            if (size < maxSize)
            {
                data[size] = new BitArray(size, false);
                ++size;
            }
            else
            {
                maxSize *= 2;
                BitArray[] BiggerData = new BitArray[maxSize];
                for (int i = 1; i < maxSize / 2; i++)
                {
                    BiggerData[i] = new BitArray(i, false);
                    for (int j = 0; j < i; ++j)
                    {
                        BiggerData[i][j] = data[i][j];
                    }
                }
                BiggerData[size] = new BitArray(size, false);
                data = BiggerData;
                ++size;
            }
            degrees.Add(0);
            return size - 1;
        }

        public void AddConnection(int firstVertex, int secondVertex)
        {
            Debug.Assert(firstVertex >= 0 && firstVertex < size, "First vertex is out of range!");
            Debug.Assert(secondVertex >= 0 && secondVertex < size, "Second vertex is out of range!");
            Debug.Assert(firstVertex != secondVertex, "Vertices are equal!");

            if (firstVertex < secondVertex)
            {
                int temp = firstVertex;
                firstVertex = secondVertex;
                secondVertex = temp;
            }
            if (!data[firstVertex][secondVertex])
            {
                data[firstVertex][secondVertex] = true;
                numberOfEdges++;
                ++degrees[firstVertex];
                ++degrees[secondVertex];
            }
        }

        public void RemoveConnection(int firstVertex, int secondVertex)
        {
            Debug.Assert(firstVertex >= 0 && firstVertex < size, "First vertex is out of range!");
            Debug.Assert(secondVertex >= 0 && secondVertex < size, "Second vertex is out of range!");
            Debug.Assert(firstVertex != secondVertex, "Vertices are equal!");

            if (firstVertex < secondVertex)
            {
                int temp = firstVertex;
                firstVertex = secondVertex;
                secondVertex = temp;
            }
            if (data[firstVertex][secondVertex])
            {
                data[firstVertex][secondVertex] = false;
                numberOfEdges--;
                --degrees[firstVertex];
                --degrees[secondVertex];
            }
        }

        public bool AreConnected(int firstVertex, int secondVertex)
        {
            Debug.Assert(firstVertex >= 0 && firstVertex < size, "First vertex is out of range!");
            Debug.Assert(secondVertex >= 0 && secondVertex < size, "Second vertex is out of range!");
            if (firstVertex == secondVertex)
            {
                return false;
            }
            if (firstVertex < secondVertex)
            {
                int temp = firstVertex;
                firstVertex = secondVertex;
                secondVertex = temp;
            }

            return data[firstVertex][secondVertex] == true;
        }
        
        public List<int> GetAdjacentEdges(int vertex)
        {
            Debug.Assert(vertex < size && vertex >= 0, "Vertex is out of range!");
            List<int> list = new List<int>();
            for (int i = 0; i < vertex; i++)
            {
                if (data[vertex][i] == true)
                {
                    list.Add(i);
                }
            }

            for (int i = vertex + 1; i < size; i++)
            {
                if (data[i][vertex] == true)
                {
                    list.Add(i);
                }
            }
            return list;

        }

        public int GetVertexDegree(int vertex)
        {
            Debug.Assert(vertex < size && vertex >= 0, "Vertex is out of range!");
            return (int)Degrees[vertex];
        }

        public int GetVertexDegreeSum()
        {
            int sum = 0;
            for (int i = 0; i < size; ++i)
                sum += GetVertexDegree(i);

            return sum;
        }

        public int CalculateNumberOfEdges()
        {
            return numberOfEdges;
        }
    }
}
