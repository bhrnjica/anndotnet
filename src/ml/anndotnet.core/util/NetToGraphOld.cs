//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CNTK;
using DotNetGraph;
using System.Globalization;

namespace ANNdotNET.Core
{
     
    public class NetToGraph
    {
        
        DotGraph m_graph = null;
        Stack<Function> funStack = new Stack<Function>();
        Stack<Variable> varStack = new Stack<Variable>();
        Stack<CntkType> typeStack = new Stack<CntkType>();
        List<string> visitedNodes = new List<string>();
        public NetToGraph()
        {

        }

        /// <summary>
        /// Parses CNTK Function and constrict the GraphViz DOT string
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public string ToGraph(Function z)
        {

            m_graph = new DotGraph($"network_graph", true);

            var root = z.RootFunction;
            funStack.Push(root);
            typeStack.Push(CntkType.Function);

            while (typeStack.Count>0)
            {
                var type = typeStack.Pop();
                if(type== CntkType.Function)
                {
                    Function node = funStack.Pop();
                    if (node == null)
                        continue;

                    //print function

                    node = node.RootFunction;
                    string id = node.Uid;
                    if (visitedNodes.Count > 0 && visitedNodes.Contains(id))
                        continue;

                    processFunction(node, root);
                }
                else
                {
                    var v = varStack.Pop();
                    funStack.Push(v.Owner);
                    typeStack.Push(CntkType.Function);
                }
            }

            // Minified version
            var dot = m_graph.Compile(false);
            return dot;
        }

        private void processFunction(Function node, Function rootNode)
        {
            string id = node.Uid;

            var currentVertex = LazyCreateNode(node);

            foreach (var input in node.RootFunction.Inputs)
            {
                varStack.Push(input);
                typeStack.Push(CntkType.Variable);
            }

            //add node's inputs
            var inputs = node.Inputs;
            for (int i = 0; i < inputs.Count; i++)
            {
                Variable input = inputs[i];
                if (node.IsBlock && input.IsConstant)
                    continue;

                DotNode vertex = null;
                if (!input.IsOutput)
                {
                    var name = input.Kind.ToString();
                    if (!string.IsNullOrEmpty(input.Name))
                    {
                        if (name.Equals("Parameter"))
                            name = input.Name;
                        else
                            name += "\n" + input.Name;
                    }
                    name += "\n" + ShapeDescription(input);

                    vertex = createVertex(input);
                    
                    if (input.IsInput || input.IsPlaceholder)
                    {
                        vertex.Label = name;
                        // 
                        vertex.FixedSize = true;
                        vertex.Height = 1f;
                        vertex.Width = 1.3f;
                        vertex.PenWidth = 4;
                    }
                    else if (string.IsNullOrEmpty(input.Name) && input.IsConstant && (input.Shape.Dimensions.Count == 0 || input.Shape.Dimensions[0] == 1))
                    {
                        string label1 = "";
                        var contView = new Constant(input).Value();
                        var value = new Value(contView);
                        switch (input.DataType)
                        {
                            case DataType.Float:
                                label1 = value.GetDenseData<float>(input)[0][0].ToString("N4",CultureInfo.InvariantCulture);
                                break;
                            case DataType.Double:
                                label1 = value.GetDenseData<double>(input)[0][0].ToString("N4", CultureInfo.InvariantCulture);
                                break;
                            case DataType.Float16:
                                label1 = (value.GetDenseData<float16>(input)[0][0]).ToString();
                                break;
                            default:
                                break;
                        }

                        vertex.Label = label1;
                        // 
                        vertex.Height = 0.6f;
                        vertex.Width = 1f;
                    }
                    else
                    {
                        vertex.Label = name;
                       //  
                        vertex.Height = 0.6f;
                        vertex.Width = 1f;
                    }
                }
                else 
                {
                    vertex = LazyCreateNode(input.Owner);
                }

                string label = string.IsNullOrEmpty(input.Name) ? input.Uid : input.Name;
                label += "\n" + ShapeDescription(input);

                DotArrow edge = new DotArrow(vertex, currentVertex);
                edge.Label = label;
                m_graph.Add(vertex);
                m_graph.Add(currentVertex);
                m_graph.Add(edge);

            }

            foreach(var output in node.Outputs)
            {
                //only final network outputs are drawn
                if (node.Uid==rootNode.Uid)
                {
                    var finalVertex =createVertex(output);
                    finalVertex.Label = output.Name + "\n" + ShapeDescription(output);
                    finalVertex.Shape = DotNodeShape.Egg;
                    finalVertex.FillColor = DotColor.Purple;
                    finalVertex.FixedSize = true;
                    finalVertex.Height = 1f;
                    finalVertex.Width = 1.3f;
                    finalVertex.PenWidth = 4;
                    //add nodes                    
                    m_graph.Add(currentVertex);
                    m_graph.Add(finalVertex);
                }
            }
            //mark current node as visited
            visitedNodes.Add(id);
        }

        private DotNode createVertex(Function fun)
        {
            var n = new DotNode(fun.Uid);
            Variable node = null;
            if (fun.Outputs.Count == 1)
                node = fun;
            else
                node = fun.Outputs.Last();

            if (node.IsInput)
            {
                n.Style = DotNodeStyle.Filled;
                n.Shape = DotNodeShape.Egg;
                n.FillColor = DotColor.Forestgreen;
            }
            else if (node.IsPlaceholder)
            {
                n.Style = DotNodeStyle.Filled;
                n.Shape = DotNodeShape.Invhouse;
                n.FillColor = DotColor.Grey;
            }
            else if (node.IsParameter)
            {
                n.Style = DotNodeStyle.Filled;
                n.Shape = DotNodeShape.Note;
                n.FillColor = DotColor.Palegreen;
            }
            else if (node.IsConstant)
            {
                n.Style = DotNodeStyle.Filled;
                n.Shape = DotNodeShape.Rect;
                n.FillColor = DotColor.Grey;
            }
            else
            {
                n.Style = DotNodeStyle.Filled;
                n.Shape = DotNodeShape.Rectangle;
                n.FillColor = DotColor.Lemonchiffon1;
            }
            return n;
        }

        DotNode LazyCreateNode(Function node)
        {
            var primitiveOperationsMap = PrimitiveOperationsMap();
            DotNode vertex= null;

            if (node.IsPrimitive && !node.IsBlock && node.Outputs.Count == 1 && node.Output.Name == node.Name)
            {
                //
                var operationName = primitiveOperationsMap.ContainsKey(node.OpName)? primitiveOperationsMap[node.OpName]: node.OpName;

                bool renderAsPrimitive = operationName.Count() <= 4;
                var size = renderAsPrimitive ? 0.4f : 0.6f;

                vertex = createVertex(node);
                vertex.Label = operationName;
                // 
                vertex.FixedSize = renderAsPrimitive ? true : false;
                vertex.Height = size;
                vertex.Width = size;
                vertex.FontSize = renderAsPrimitive && operationName.Length == 1 ? 20 : 12;
                vertex.PenWidth = node.OpName != "Pass" && node.OpName != "ParameterOrder" ? 4 : 1;

            }
            else
            {
                string functionName = string.IsNullOrEmpty(node.Name) ? "" : "\n" + node.Name + "()";
                vertex = createVertex(node);

                //
                vertex.Label = node.OpName + functionName;
                vertex.FixedSize = true;
                vertex.Height = 1;
                vertex.Width = 1.3f;
                vertex.PenWidth = node.OpName != "Pass" && node.OpName != "ParameterOrder" ? 4 : 1;
            }

            return vertex;
        }

        Dictionary<string, string> PrimitiveOperationsMap()
        {
            return new Dictionary<string, string>()
            {
                { "Plus", "+" },
                { "Minus", "-" },
                { "ElementTimes", "*" },
                { "Times", "@" }
            };
        }

        /// <summary>
        /// construct the string for node representation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ShapeDescription(Variable input)
        {
            var retValue = "";
            if(input.DynamicAxes.Count < 0)
            {
                //# the '#' indicates the batch axis, while * indicate dynamic axes (which can be sequences)
                var numDynAxes = input.DynamicAxes.Where(x => x.IsDynamic).Count();
                var numStytAxes = input.DynamicAxes.Count - numDynAxes;
                if (numDynAxes == 0)
                    retValue += $"[";
                else if (numDynAxes > 1)
                    retValue += $"[{numDynAxes}#";
                else if (numDynAxes == 1)
                    retValue += $"[#";

                if (numStytAxes <= 0)
                    retValue += "]";
                else if (numStytAxes == 1)
                    retValue += ",*]";
                else
                    retValue += $",{numStytAxes}*]";
            }
            //
            if(input.Shape.Dimensions.Count>0)
                retValue += $"({string.Join(", ", input.Shape.Dimensions.Reverse())})";

            return retValue;
        }
    }
}
