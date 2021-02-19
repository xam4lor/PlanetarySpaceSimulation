using System.Collections.Generic;
using UnityEngine;

namespace QuadTree {
	public class Chunk {
		public int depth;

		private Chunk parentChunk;
		
		private GameObject gameObject;
		private MeshFilter meshFilter;
		private PlanetChunks handler;

		private BoundingBox bounds;
		private Chunk[] cells;
		private string name;
		private bool subdivided;
		

		public Chunk(PlanetChunks handler, BoundingBox bounds, int depth, int id, GameObject parent, Chunk parentChunk, string name) {
			this.parentChunk = parentChunk;
			this.handler = handler;
			this.bounds  = bounds;
			this.cells   = new Chunk[4];
			this.depth   = depth;
			this.name    = name;
			this.subdivided = false;
			
			this.generateMesh(id, parent);
		}

		public void subdivide() {
			if (this.subdivided)
				return;

			// Subdivides chunks
			float x = this.bounds.pos.x;
			float y = this.bounds.pos.y;
			float subWidth  = this.bounds.dim / 2f;
			float subHeight = this.bounds.dim / 2f;

			// 0 = top right
			this.cells[0] = new Chunk(handler, new BoundingBox(x + subWidth, y, subWidth), this.depth + 1, 0, this.gameObject, this, this.name + 0);
			// 1 = top left
			this.cells[1] = new Chunk(handler, new BoundingBox(x, y, subWidth), this.depth + 1, 1, this.gameObject, this, this.name + 1);
			// 2 = bottom left
			this.cells[2] = new Chunk(handler, new BoundingBox(x, y + subHeight, subWidth), this.depth + 1, 2, this.gameObject, this, this.name + 2);
			// 3 = bottom right
			this.cells[3] = new Chunk(handler, new BoundingBox(x + subWidth, y + subHeight, subWidth), this.depth + 1, 3, this.gameObject, this, this.name + 3);

			// Clear Mesh
			this.gameObject.GetComponent<MeshRenderer>().enabled = false;
			this.gameObject.GetComponent<BoxCollider>().enabled = false;

			this.subdivided = true;
		}

		private void generateMesh(int id, GameObject parentGo) {
			// GameObject
			GameObject chunkGo = new GameObject(this.name);
			chunkGo.transform.parent = parentGo.transform;
			chunkGo.transform.position = new Vector3(0, 0, 0);
			chunkGo.tag = "Chunk";

			// Mesh
			chunkGo.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
			MeshFilter meshFilter = chunkGo.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = this.constructMesh();

			// Colliders
			chunkGo.AddComponent<BoxCollider>();

			this.gameObject = chunkGo;
			this.meshFilter = meshFilter;
		}

		private Mesh constructMesh() {
			Mesh mesh = new Mesh();
			float s = this.bounds.dim;
			float globalS = handler.getScale();
			int density = Planet.chunkDensity;

			Vector3 trans = handler.getTranslation();
			Vector3 off = new Vector3(globalS / 2, -globalS / 2, globalS / 2);


			Vector3[] vertices = new Vector3[(density + 1) * (density + 1)];
			Vector3[] normals  = new Vector3[(density + 1) * (density + 1)];
			int[] triangles    = new int[6 * (density-1)*(density-1)];


			int indexTr = 0;
			for (int j = 0; j < density; j++) {
				for (int i = 0; i < density; i++) {
					float x = this.bounds.pos.x + s / (density - 1) * i;
					float y = this.bounds.pos.y + s / (density - 1) * j;

					vertices[i + j * density] = this.handler.localRotation * ((new Vector3(x, 0, y) - off).normalized * globalS) + trans;
					normals [i + j * density] = vertices[i + j * density].normalized;

					if (i != density - 1 && j != density - 1) {
						triangles[indexTr + 0] = i + (j + 1) * density;
						triangles[indexTr + 1] = i + j * density + 1;
						triangles[indexTr + 2] = i + j * density;
						indexTr += 3;
					}
					if (i != 0 && j != density - 1) {
						triangles[indexTr + 0] = i + j * density;
						triangles[indexTr + 1] = i - 1 + (j + 1) * density;
						triangles[indexTr + 2] = i + (j + 1) * density;
						indexTr += 3;
					}
				}
			}

			// Generate Mesh
			mesh.Clear();
			mesh.vertices  = vertices;
			mesh.triangles = triangles;
			mesh.normals   = normals;

			return mesh;
		}

/* 		public void quadDivide(float playerDistance) {
			int[] depthsTable = new int[] { 10, 100, 1000, 10000, 100000, 1000000 };
			int computedTargetDepth = 15; // max value

			for (int i = 0; i < depthsTable.Length; i++) {
				if (playerDistance < depthsTable[i]) {
					break;
				}
				computedTargetDepth -= 1;
			}

			if (this.depth < computedTargetDepth) {
				this.subdivide();

				for (int i = 0; i < this.cells.Length; i++) {
					if (this.cells[i] != null) {
						this.cells[i].quadDivide(playerDistance);
					}
				}
			}
			else if (this.depth > computedTargetDepth) {
				this.killChildren();
			}
		} */


		public RecursiveTree getDividedChunksFromCenter(Vector3 playerCenterPosition, ref RecursiveTree parent) {
            float[] threshold = this.handler.planet.threshold;

            float globalS = handler.getScale();
            Vector3 trans = handler.getTranslation();
            Vector3 off = new Vector3(globalS / 2, -globalS / 2, globalS / 2);

            float x = this.bounds.pos.x + this.bounds.dim / 2;
            float y = this.bounds.pos.y + this.bounds.dim / 2;

            Vector3 pos = this.handler.localRotation * ((new Vector3(x, 0, y) - off).normalized * globalS) + trans;

			// Test distance btw this chunk and projected player point
            float distancePlayerCenter = Vector3.Distance(playerCenterPosition, pos);
            if (depth < threshold.Length && distancePlayerCenter < threshold[depth] * this.handler.planet.scale) { // If < threshold
				// Divide 
                parent.divide();
				this.subdivide();

				for (int i = 0; i < this.cells.Length; i++) {
                    parent.childrens[i] = this.cells[i].getDividedChunksFromCenter(playerCenterPosition, ref parent.childrens[i]);
				}
            }
			return parent;
		}

		public void killUnreferencedChunks(RecursiveTree parent) {
			if (parent.hasChildren) {
                for (int i = 0; i < this.cells.Length; i++) {
                    this.cells[i].killUnreferencedChunks(parent.childrens[i]);
				}
			}
			else if (this.subdivided && !parent.hasChildren) {
				for (int i = 0; i < this.cells.Length; i++) {
                    this.cells[i].killChildren();
				}
                this.subdivided = false;
                this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                this.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
		}





		public void kill() {
            // KILLS HIMSELF
			this.subdivided = false;
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;

			this.handler.destroyGameObject(this.gameObject);
		}

		public void killChildren() {
			if (this.subdivided) {
                for (int i = 0; i < this.cells.Length; i++) {
                    this.cells[i].killChildren();
                }
			}
			this.kill();
		}


		public void drawDebug() {
            /*
 			Gizmos.DrawLine(new Vector3(this.bounds.pos.x, 0, this.bounds.pos.y), new Vector3(this.bounds.pos.x, 0, this.bounds.pos.y + this.bounds.dim));
			Gizmos.DrawLine(new Vector3(this.bounds.pos.x, 0, this.bounds.pos.y), new Vector3(this.bounds.pos.x + this.bounds.dim, 0, this.bounds.pos.y));
			Gizmos.DrawLine(new Vector3(this.bounds.pos.x + this.bounds.dim, 0, this.bounds.pos.y), new Vector3(this.bounds.pos.x + this.bounds.dim, 0, this.bounds.pos.y + this.bounds.dim));
			Gizmos.DrawLine(new Vector3(this.bounds.pos.x, 0, this.bounds.pos.y + this.bounds.dim), new Vector3(this.bounds.pos.x + this.bounds.dim, 0, this.bounds.pos.y + this.bounds.dim));

			for (int i = 0; i < this.cells.Length; i++) {
				if (this.cells[i] != null) {
					this.cells[i].drawDebug();
				}
			}
			*/


            float globalS = handler.getScale();
            Vector3 trans = handler.getTranslation();
            Vector3 off = new Vector3(globalS / 2, -globalS / 2, globalS / 2);

            float x = this.bounds.pos.x + this.bounds.dim / 2;
            float y = this.bounds.pos.y + this.bounds.dim / 2;

            Vector3 pos = this.handler.localRotation * ((new Vector3(x, 0, y) - off).normalized * globalS) + trans;
			Gizmos.DrawSphere(pos, 1);
		}

		public Chunk getChunkWithName(string name) {
			if (name.Equals(this.name.Substring(6)))
				return this;
			if (this.name.Substring(6).Equals(""))
				return this.cells[int.Parse(name.ToCharArray()[this.depth].ToString())].getChunkWithName(name);
			return this.cells[int.Parse(name.ToCharArray()[this.depth].ToString())].getChunkWithName(name);
		}


		public Chunk getChunk(int id) {
			if (cells[id] != null) {
				return cells[id];
			}
			return null;
		}

		public string getName() {
			return this.name;
		}

		public int getDepth() {
			return this.depth;
		}
	}


	public struct RecursiveTree {
		public bool hasChildren;
		public RecursiveTree[] childrens;

		public RecursiveTree(bool hasChildren) {
			this.hasChildren = hasChildren;
			this.childrens = new RecursiveTree[4];
		}

		public void divide() {
			this.hasChildren = true;
			for (int i = 0; i < childrens.Length; i++) {
                childrens[i] = new RecursiveTree(false);
			}
		}
	}

	public struct BoundingBox {
		public Vector3 pos;
		public float dim;

		public BoundingBox(float x, float y, float dimension) {
			this.pos = new Vector3(x, y);
			this.dim = dimension;
		}
	}
}