/*
 * Copyright (c) 2017 DotNetGraph https://github.com/vfrz/DotNetGraph
 *  This file is part of DotNetGraph.
 *
 *     DotNetGraph is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *
 *     DotNetGraph is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *
 *     You should have received a copy of the GNU General Public License
 *     along with DotNetGraph.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DotNetGraph
{
    public class DotGraph
    {
        public string Name { get; }

        public bool Directed { get; }

        private List<DotElement> _elements;

        public DotGraph(string name, bool directed = false)
        {
            Name = name;
            Directed = directed;
            _elements = new List<DotElement>();
        }

        public string Compile(bool minified = true)
        {
            var builder = new StringBuilder();

            builder.Append((Directed ? "di" : "") + $"graph {Name} {{" + (minified ? "" : "\n"));

            foreach (var element in _elements)
            {
                if (element is DotNode node)
                {
                    builder.Append((minified ? "" : "\t") + node.Name + " [ ");
                }
                else if (element is DotArrow arrow)
                {
                    builder.Append((minified ? "" : "\t") + arrow.StartNodeName + "->" + arrow.TargetNodeName + " [ ");
                }

                foreach (var p in element.GetType().GetProperties())
                {
                    if (p.CanRead && Attribute.IsDefined(p, typeof(DotAttributeAttribute)))
                    {
                        var attribute = p.GetCustomAttributes(typeof(DotAttributeAttribute), false)
                            .Cast<DotAttributeAttribute>()
                            .FirstOrDefault();

                        var value = p.GetValue(element);

                        if (value.Equals(attribute?.DefaultValue))
                        {
                            continue;
                        }

                        var isEnum = value.GetType().IsEnum;
                        var isString = value is string;
                        var isFloat = value is float;

                        var valueString = isEnum
                            ? value.ToString().ToLower()
                            : (isFloat
                                ? Convert.ToSingle(value).ToString(CultureInfo.InvariantCulture)
                                : value);
                        var strValue = attribute?.Name + "=" + (isString ? "\"" : "") +
                                       valueString + (isString ? "\" " : " ");
                        builder.Append(strValue);
                    }
                }

                builder.Append("];" + (minified ? "" : "\n"));
            }

            builder.Append("}");

            return builder.ToString();
        }

        public void Add(DotElement element)
        {
            _elements.Add(element);
        }
    }
}