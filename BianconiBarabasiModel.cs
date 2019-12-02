using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NetworkModel;

namespace bianconi_barabasi_model
{
    class BaseModel
    {
        public BaseModel(int vertexcount)
        {
            vertexcount_ = vertexcount;
            edgespervertex_ = 1;
            container_ = new NonHierarchicContainer();
            fitnesses_ = new List<double>(vertexcount_);

        }

        public BaseModel(int vertexcount, int edgespervertex, string function_str)
        {
            vertexcount_ = vertexcount;
            edgespervertex_ = edgespervertex;
            distributed_random_ = new DistributedRandom(function_str);
            container_ = new NonHierarchicContainer();
            fitnesses_ = new List<double>(vertexcount_);
        }

        public void generate_network()
        {
            for (int current_vertex = 0; current_vertex != vertexcount_; ++current_vertex)
            {
                fitnesses_.Add(get_distributed_probablity());
                container_.AddVertex();
                for (int i = 0; i < Math.Min(current_vertex, edgespervertex_); ++i)
                {
                    int vertex_to_connect = get_vertex_to_connect(current_vertex);
                    Debug.Assert(vertex_to_connect >= 0, "internal error");
                    container_.AddConnection(current_vertex, vertex_to_connect);
                }
            }
        }

        public void continue_generation(int additional_vertex_count)
        {
            for (int current_vertex = vertexcount_; current_vertex != additional_vertex_count + vertexcount_; ++current_vertex)
            {
                fitnesses_.Add(get_distributed_probablity());
                container_.AddVertex();
                for (int i = 0; i < Math.Min(current_vertex, edgespervertex_); ++i)
                {
                    int vertex_to_connect = get_vertex_to_connect(current_vertex);
                    Debug.Assert(vertex_to_connect >= 0, "internal error");
                    container_.AddConnection(current_vertex, vertex_to_connect);
                }
            }
            vertexcount_ += additional_vertex_count;
        }

        public void add_internal_links(int internal_links_count)
        {
            if(vertexcount_ <= 1)
            {
                return;
            }

            for(int i=0; i<internal_links_count; ++i)
            {
                int first_vertex;
                int second_vertex;
                vertexes_to_connect(out first_vertex, out second_vertex);
                Debug.Assert(first_vertex >= 0 && second_vertex >=0, "internal error");
                container_.AddConnection(first_vertex, second_vertex);
            }
            

        }

        public BitArray[] get_network()
        {
            return container_.GetMatrix();
        }

        public double[] get_fitnesses()
        {
            double[] result = new double[vertexcount_];

            for (int i = 0; i < vertexcount_; ++i)
            {
                result[i] = fitnesses_[i];
            }

            return result;
        }
        private double get_distributed_probablity()
        {
            return distributed_random_.get_distributed_probability();
        }
        private int get_vertex_to_connect(int current_vertex)
        {
            double sum = 0;
            for (int i = 0; i < current_vertex; ++i)
            {
                sum += (container_.GetVertexDegree(i) * fitnesses_[i]);
            }

            double probabilty = distributed_random_.get_probability() * sum;

            sum = 0;
            for (int i = 0; i < current_vertex; ++i)
            {
                sum += (container_.GetVertexDegree(i) * fitnesses_[i]);
                if (probabilty <= sum)
                {
                    return i;
                }
            }

            return -1;
        }

        private void vertexes_to_connect(out int first, out int second)
        {
            double sum = 0;
            for (int i = 0; i < vertexcount_; ++i)
            {
                for (int j = i + 1; j < vertexcount_; ++j)
                {
                    sum += (container_.GetVertexDegree(i) * fitnesses_[i]) * (container_.GetVertexDegree(j) * fitnesses_[j]);
                }
            }

            double probabilty = distributed_random_.get_probability() * sum;

            sum = 0;
            for (int i = 0; i < vertexcount_; ++i)
            {
                for (int j = i + 1; j < vertexcount_; ++j)
                {
                    sum += (container_.GetVertexDegree(i) * fitnesses_[i]) * (container_.GetVertexDegree(j) * fitnesses_[j]);
                    if (sum <= probabilty)
                    {
                        first = i;
                        second = j;
                        return;
                    }
                }
            }
            first = 0;
            second = 1;
        }

        private int vertexcount_;
        private int edgespervertex_;
        private DistributedRandom distributed_random_;
        private NonHierarchicContainer container_;
        private List<double> fitnesses_;

    }

    class BianconiBarabasiModel
    {
        public BianconiBarabasiModel(int vertexcount, int edgespervertex, string function_str)
        {
            base_model_ = new BaseModel(vertexcount, edgespervertex, function_str);
        }

        public void generate_network()
        {
            base_model_.generate_network();
        }

        public BitArray[] get_network()
        {
            return base_model_.get_network();
        }

        public double[] get_fitnesses()
        {
            return base_model_.get_fitnesses();
        }


        private BaseModel base_model_;
    }

    class BBModelExtentionInternalLinks
    {
        public BBModelExtentionInternalLinks(int vertexcount, int edgespervertex, string function_str, int vertex_per_step, int internal_edge_per_step) 
        {
            base_model_ = new BaseModel(0, edgespervertex, function_str);
            vertex_count_ = vertexcount;
            vertex_per_step_ = vertex_per_step;
            internal_edge_per_step_ = internal_edge_per_step;
        }

        public void generate_network()
        {
            int steps = vertex_count_ / vertex_per_step_ + 1;
            for(int i=0; i<steps; ++i)
            {
                base_model_.continue_generation(vertex_per_step_);
                base_model_.add_internal_links(internal_edge_per_step_);
            }
        }

        public double[] get_fitnesses()
        {
            return base_model_.get_fitnesses();
        }

        public BitArray[] get_network()
        {
            return base_model_.get_network();
        }

        private BaseModel base_model_;
        private int vertex_count_;
        private int vertex_per_step_;
        private int internal_edge_per_step_;

    }
}
