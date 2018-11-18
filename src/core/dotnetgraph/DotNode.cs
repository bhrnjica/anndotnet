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

namespace DotNetGraph
{
    public class DotNode : DotElement
    {
        public string Name { get; }

        [DotAttribute("shape", DotNodeShape.Ellipse)]
        public DotNodeShape Shape { get; set; } = DotNodeShape.Ellipse;

        [DotAttribute("label", "\n")]
        public string Label { get; set; } = "\n";

        [DotAttribute("fillcolor", DotColor.Lightgrey)]
        public DotColor FillColor { get; set; } = DotColor.Lightgrey;

        [DotAttribute("fontcolor", DotColor.Black)]
        public DotColor FontColor { get; set; } = DotColor.Black;

        [DotAttribute("style", DotNodeStyle.Default)]
        public DotNodeStyle Style { get; set; } = DotNodeStyle.Default;

        [DotAttribute("height", 0.5f)]
        public float Height { get; set; } = 0.5f;
        [DotAttribute("width", 0.5f)]
        public float Width { get; set; } = 0.5f;

        [DotAttribute("fixedsize", true)]
        public bool FixedSize { get; set; } = true;

        [DotAttribute("fontsize", 10)]
        public int FontSize { get; set; } = 10;

        [DotAttribute("penwidth", 0.9f)]
        public float PenWidth { get; set; } = 0.9f;

        public DotNode(string name)
        {
            Name = name;
        }
    }
}