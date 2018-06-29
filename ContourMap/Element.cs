using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ContourMap
{
	public class Element
	{
		int id;
		int[] vertIndices;
		int[] neighborIndices;
		Vector3[] intersections;
		ContourType type;
		bool flag;

		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		public int[] VertIndices
		{
			get { return vertIndices; }
			set { vertIndices = value; }
		}

		public int[] NeighborIndices
		{
			get { return neighborIndices; }
			set { neighborIndices = value; }
		}

		public Vector3[] Intersections
		{
			get { return intersections; }
			set { intersections = value; }
		}

		public ContourType Type
		{
			get { return type; }
			set { type = value; }
		}

		public bool Flag
		{
			get { return flag; }
			set { flag = value; }
		}

		public Element(int i)
		{
			id = i;
			vertIndices = new int[3] { -1, -1, -1 };
			neighborIndices = new int[3] { -1, -1, -1 };
			intersections = new Vector3[3];
			type = ContourType.Unknow;
			flag = false;
		}

		public void Initialize()
		{
			type = ContourType.Unknow;
		}
	}

	public enum ContourType
	{
		T000 = 0,
		T001,
		T010,
		T011,
		T100,
		T101,
		T110,
		T111,
		Unknow
	}
}
