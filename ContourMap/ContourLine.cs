using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ContourMap
{
	public class ContourLine
	{
		List<Vector3> points;
		Color color;
		VertexPositionColor[] verts; 

		public List<Vector3> Points
		{
			get
			{
				return points;
			}

			set
			{
				points = value;
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}

			set
			{
				color = value;
			}
		}

		public VertexPositionColor[] Verts
		{
			get
			{
				return verts;
			}

			set
			{
				verts = value;
			}
		}

		public ContourLine(Color c)
		{
			points = new List<Vector3>();
			color = c;
		}

		public void GeneVertList()
		{
			List<VertexPositionColor> vertList = new List<VertexPositionColor>();
			vertList.Add(new VertexPositionColor(points[0], color));
			for(int i=1;i<points.Count;i++)
			{
				//if (points[i] != vertList[vertList.Count - 1].Position)
					vertList.Add(new VertexPositionColor(points[i], color));
			}
			verts = vertList.ToArray();
		}

		
	}
}
