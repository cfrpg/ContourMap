using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ContourMap
{
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		//Graphics
		VertexPositionColor[] verts;
		VertexPositionColor[] lineverts;
		BasicEffect effect;
		List<ContourLine> lines;

		//Data
		List<Vector3> points;
		List<float> values;
		List<Element> elements;
		int pointCount;
		int elemCount;
		int edgeCount;
		float vmax;
		float vmin;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferWidth = 700;
			graphics.PreferredBackBufferHeight = 700;
		}

		protected override void Initialize()
		{
			graphics.GraphicsProfile = GraphicsProfile.HiDef;
			graphics.PreferMultiSampling = true;
			GraphicsDevice.PresentationParameters.MultiSampleCount = 2;
			graphics.ApplyChanges();
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			effect = new BasicEffect(GraphicsDevice);
			effect.World = Matrix.Identity;
			effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 2), new Vector3(0, 0, 0), Vector3.Up);
			effect.Projection = Matrix.CreateOrthographicOffCenter(-0.5f, 1.5f, -1f, 1f, -1, 100);
			effect.VertexColorEnabled = true;

			loadData("d:\\temp\\data2.dat");
			
			for(int i=1;i<=15;i++)
			{
				GetLines(vmin + (vmax - vmin) / 16.0f * i);
			}
		}

		private void loadData(string path)
		{
			StreamReader sr = new StreamReader(path);
			pointCount = int.Parse(sr.ReadLine());
			elemCount = int.Parse(sr.ReadLine());
			edgeCount = int.Parse(sr.ReadLine());
			vmin = float.MaxValue;
			vmax = float.MinValue;
			points = new List<Vector3>();
			values = new List<float>();
			for (int i = 0; i < pointCount; i++)
			{
				float x, y, v;
				x = float.Parse(sr.ReadLine());
				y = float.Parse(sr.ReadLine());
				v = float.Parse(sr.ReadLine());
				points.Add(new Vector3(x, y, 0));
				values.Add(v);
				if (v > vmax)
					vmax = v;
				if (v < vmin)
					vmin = v;
			}
			verts = new VertexPositionColor[elemCount * 3];
			elements = new List<Element>();
			for (int i = 0; i < elemCount; i++)
			{
				int t;
				Element e = new Element(i);
				for (int j = 0; j < 3; j++)
				{
					t = int.Parse(sr.ReadLine());
					t--;
					e.VertIndices[j] = t;
					verts[i * 3 + j] = new VertexPositionColor(points[t], getColor(values[t], vmin, vmax));
				}
				t = int.Parse(sr.ReadLine());
				elements.Add(e);
			}
			lines = new List<ContourLine>();
			for (int i = 0; i < edgeCount; i++)
			{
				int a, n, b;
				a = int.Parse(sr.ReadLine());
				n = int.Parse(sr.ReadLine());
				b = int.Parse(sr.ReadLine());
				if (n == 4)
					n--;
				n--;
				elements[a - 1].NeighborIndices[n] = b - 1;
			}
		}

		private void GetLines(float value)
		{
			//init all elements
			//ContourLine line = new ContourLine(Color.Black);
			foreach(var v in elements)
			{
				v.Flag = false;
				int t = 0;
				for (int i = 2; i>=0; i--)
				{
					t <<= 1;
					if (values[v.VertIndices[i]] > value)
						t += 1;
				}
				v.Type = (ContourType)t;
				if (v.Type == ContourType.T011 || v.Type == ContourType.T100)
				{
					v.Intersections[1] = Linear(points[v.VertIndices[1]], values[v.VertIndices[1]], points[v.VertIndices[2]], values[v.VertIndices[2]], value);
					v.Intersections[2] = Linear(points[v.VertIndices[0]], values[v.VertIndices[0]], points[v.VertIndices[2]], values[v.VertIndices[2]], value);
					//line.Points.Add(v.Intersections[1]);
					//line.Points.Add(v.Intersections[2]);
				}
				if (v.Type == ContourType.T101 || v.Type == ContourType.T010)
				{
					v.Intersections[1] = Linear(points[v.VertIndices[1]], values[v.VertIndices[1]], points[v.VertIndices[2]], values[v.VertIndices[2]], value);
					v.Intersections[0] = Linear(points[v.VertIndices[0]], values[v.VertIndices[0]], points[v.VertIndices[1]], values[v.VertIndices[1]], value);
					//line.Points.Add(v.Intersections[1]);
					//line.Points.Add(v.Intersections[0]);
				}
				if (v.Type == ContourType.T110 || v.Type == ContourType.T001)
				{
					v.Intersections[0] = Linear(points[v.VertIndices[0]], values[v.VertIndices[0]], points[v.VertIndices[1]], values[v.VertIndices[1]], value);
					v.Intersections[2] = Linear(points[v.VertIndices[0]], values[v.VertIndices[0]], points[v.VertIndices[2]], values[v.VertIndices[2]], value);
					//line.Points.Add(v.Intersections[0]);
					//line.Points.Add(v.Intersections[2]);
				}
			}
			//line.GeneVertList();
			//lines.Add(line);
			for (int l = 0; l < elemCount; l++)
			{
				if (elements[l].Flag)
					continue;
				Element e = elements[l];
				if (e.Type == ContourType.T000 || e.Type == ContourType.T111)
				{
					e.Flag = true;
					continue;
				}
				ContourLine line = new ContourLine(Color.Black);

				while (true)
				{
					e.Flag = true;
					int next = -1;
					if (e.Type == ContourType.T011 || e.Type == ContourType.T100)
					{
						line.Points.Add(e.Intersections[1]);
						line.Points.Add(e.Intersections[2]);
						next = e.NeighborIndices[2];
					}
					if (e.Type == ContourType.T101 || e.Type == ContourType.T010)
					{
						line.Points.Add(e.Intersections[0]);
						line.Points.Add(e.Intersections[1]);
						next = e.NeighborIndices[1];
					}
					if (e.Type == ContourType.T110 || e.Type == ContourType.T001)
					{
						line.Points.Add(e.Intersections[0]);
						line.Points.Add(e.Intersections[2]);
						next = e.NeighborIndices[2];
					}
					float v0, v1, v2;
					v0 = values[e.VertIndices[0]];
					v1 = values[e.VertIndices[1]];
					v2 = values[e.VertIndices[2]];
					if (next == -1 || elements[next].Flag)
						break;
					e = elements[next];
				}
				line.GeneVertList();
				if (line.Verts.Length >= 2)
					lines.Add(line);

			}
		}

		private Vector3 Linear(Vector3 p1,float v1,Vector3 p2,float v2,float v)
		{
			Vector3 res= p1 + ((v - v1) / (v2 - v1) * (p2 - p1));
			Vector3 vv1 = res - p1, vv2 = p2 - res;
			float f = Vector3.Dot(vv1, vv2);
			return res;
		}

		private Color getColor(float v, float minv, float maxv)
		{
			float[] threshold = new float[] { 0, 0.25f, 0.5f, 0.75f, 1 };
			Color[] colors = new Color[] { Color.Blue, Color.Cyan, new Color(0, 255, 0), Color.Yellow, Color.Red };
			float t = (v - minv) / (maxv - minv);
			for(int i=1;i<threshold.Length;i++)
			{
				if(t<threshold[i])
				{
					return Color.Lerp(colors[i - 1], colors[i], (t - threshold[i - 1]) / (threshold[i] - threshold[i - 1]));
				}
			}
			return colors[colors.Length - 1];
			
		}

		protected override void UnloadContent()
		{

		}

		protected override void Update(GameTime gameTime)
		{

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);
			RasterizerState rs = RasterizerState.CullNone;
			GraphicsDevice.RasterizerState = rs;
			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verts, 0, elemCount);
			}
			foreach(var l in lines)
			{
				foreach (EffectPass pass in effect.CurrentTechnique.Passes)
				{
					pass.Apply();
					GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, l.Verts, 0, l.Verts.Length/2);
				}
			}
			base.Draw(gameTime);
		}
	}
}
