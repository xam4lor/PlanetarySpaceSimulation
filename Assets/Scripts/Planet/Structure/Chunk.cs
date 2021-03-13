using System.Collections.Generic;
using UnityEngine;



// Thanks to https://www.youtube.com/watch?v=mXTxQko-JH0, https://www.youtube.com/watch?v=QN39W020LqU, https://www.youtube.com/watch?v=YueAtA_YnSY
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

        // First digit : Id, Second digit : direction : R : 0, L : 1, D : 2, U : 3, RU : 4, RD : 5, LD : 6, LU : 7, 8 : halt
        private int[,,] neighborArray = {
			// Quadrant 0 : Top left
			{{1, 8}, {1, 1}, {2, 8}, {2, 3}, {3, 3}, {3, 8}, {3, 1}, {3, 7}},
			// Quadrant 1 : Top right
			{{0, 0}, {0, 8}, {3, 8}, {3, 3}, {2, 4}, {2, 0}, {2, 8}, {2, 3}},
			// Quadrant 2 : Bottom left
			{{3, 8}, {3, 1}, {0, 2}, {0, 8}, {1, 8}, {1, 2}, {1, 6}, {1, 1}},
			// Quadrant 3 : Bottom right
			{{2, 0}, {2, 8}, {1, 2}, {1, 8}, {0, 0}, {0, 5}, {0, 2}, {0, 8}}
		};
		

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

            // 0 = top left
            this.cells[0] = new Chunk(handler, new BoundingBox(x, y, subWidth), this.depth + 1, 0, this.gameObject, this, this.name + 0);
			// 1 = top right
			this.cells[1] = new Chunk(handler, new BoundingBox(x + subWidth, y, subWidth), this.depth + 1, 1, this.gameObject, this, this.name + 1);
			// 2 = bottom left
			this.cells[2] = new Chunk(handler, new BoundingBox(x, y + subHeight, subWidth), this.depth + 1, 2, this.gameObject, this, this.name + 2);
			// 3 = bottom right
			this.cells[3] = new Chunk(handler, new BoundingBox(x + subWidth, y + subHeight, subWidth), this.depth + 1, 3, this.gameObject, this, this.name + 3);

			// Clear Mesh
			this.gameObject.GetComponent<MeshRenderer>().enabled = false;
			this.gameObject.GetComponent<MeshCollider>().enabled = false;

			this.subdivided = true;
		}

		private void generateMesh(int id, GameObject parentGo) {

			// GameObject
			GameObject chunkGo = new GameObject(this.name);
			chunkGo.transform.parent = parentGo.transform;
			chunkGo.transform.position = new Vector3(0, 0, 0);
			chunkGo.tag = "Chunk";

            // Renderer & Material
            MeshRenderer renderer = chunkGo.AddComponent<MeshRenderer>();
			Material mat = new Material(Shader.Find("Sprites/Diffuse")); // Shader.Find("Standard")
            renderer.sharedMaterial = mat;

            // Mesh
            Mesh mesh = this.constructMesh();
            MeshFilter meshFilter = chunkGo.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;

            // Colliders
            MeshCollider meshCollider = chunkGo.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.sharedMesh = mesh;

			this.gameObject = chunkGo;
			this.meshFilter = meshFilter;
		}

		private Mesh constructMesh() {
			Mesh mesh = new Mesh();
			float s = this.bounds.dim;
			float globalS = handler.getScale();
			int density = handler.planet.chunkDensity;

			Vector3 trans = handler.getTranslation();
			Vector3 off = new Vector3(globalS / 2, -globalS / 2, globalS / 2);

			Vector3[] vertices = new Vector3[(density + 1) * (density + 1)];
			Vector3[] normals  = new Vector3[(density + 1) * (density + 1)];
			Color[] colors     = new Color[vertices.Length];
			int[] triangles    = new int[6 * (density - 1) * (density - 1)];

            // Fix edges problems
            /* GameObject neighbor1 = null;
            GameObject neighbor2 = null;
            if (!name.Substring(1).Equals("hunk ")) {
                char[] numberDigits = name.Substring(6).ToCharArray();

                int[] targetReverseID1 = new int[numberDigits.Length];
                int[] targetReverseID2 = new int[numberDigits.Length];

				int direction1 = 0;
                int direction2 = 0;
                int digit = int.Parse(numberDigits[numberDigits.Length - 1].ToString());

                if (digit == 0) { // Top left quadrant
                    direction1 = 1; // L
					direction2 = 3; // U
                }
                else if (digit == 1) { // Top right quadrant
					direction1 = 0; // R
                    direction2 = 3; // U
                }
                else if (digit == 2) { // Bottom left quadrant
					direction1 = 1; // L
                    direction2 = 2; // D
                }
                else if (digit == 3) { // Bottom right quadrant
                    direction1 = 0; // R
                    direction2 = 2; // D
                }
                targetReverseID1[numberDigits.Length - 1] = direction1;
                targetReverseID2[numberDigits.Length - 1] = direction2;

                for (int i = numberDigits.Length - 2; i > 0; i--) {
                    digit = int.Parse(numberDigits[i].ToString());
					
					if (direction1 != 8) {
                        targetReverseID1[i] = neighborArray[digit, direction1, 0];
                        direction1 = neighborArray[digit, direction1, 1];
					}
                    if (direction2 != 8) {
                        targetReverseID2[i] = neighborArray[digit, direction2, 0];
                        direction2 = neighborArray[digit, direction2, 1];
					}

					if (direction1 == 8 && direction2 == 8)
						break;
                }

				string val1 = "";
                string val2 = "";
				for (int i = 0; i < numberDigits.Length; i++) {
                    val1 += targetReverseID1[i];
                    val2 += targetReverseID2[i];
				}

                // Suppose object in same Main Chunk
                val1 += " ";
                val2 += " ";
				string initStr = this.name.Substring(0, 1) + "hunk "; // e.g. : 0hunk

				for (int i = 0; i < val1.Length; i++) {
					if (neighbor1 == null) {
                        val1 = val1.Substring(0, val1.Length - 1); // remove last digit
                        neighbor1 = GameObject.Find(initStr + val1);
					}
                    if (neighbor2 == null) {
                        val2 = val2.Substring(0, val2.Length - 2); // remove last digit
                        neighbor2 = GameObject.Find(initStr + val2);
                    }

					if (neighbor1 != null && neighbor2 != null) // Found neighbor
						break;
				} // Else : TODO

				if (neighbor1 != null) {
                	Debug.Log("Neighbor 1 : " + neighbor1.name);
                    neighbor1.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
                }
                if (neighbor2 != null) {
                    Debug.Log("Neighbor 2 : " + neighbor2.name);
                    neighbor2.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
                }
            } */

            int indexTr = 0;
			for (int j = 0; j < density; j++) {
				for (int i = 0; i < density; i++) {
					float x = this.bounds.pos.x + s / (density - 1) * i;
					float y = this.bounds.pos.y + s / (density - 1) * j;
                    float z = 0;

					Vector3 sphereUnitPosition = this.handler.localRotation * (new Vector3(x, z, y) - off).normalized;

					// Default
                    z = this.handler.planet.getAltitudeAt(sphereUnitPosition);

					vertices[i + j * density] = sphereUnitPosition * (this.handler.planet.planetScale + z) + trans;
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

					// Color
					colors[i + j * density] = this.handler.getColorAtAltitude(z);
                }
			}

			// Generate Mesh
			mesh.Clear();
			mesh.vertices  = vertices;
			mesh.colors    = colors;
			mesh.triangles = triangles;
			mesh.normals   = normals;

			return mesh;
		}


		public RecursiveTree getDividedChunksFromCenter(Vector3 playerPos, Vector3 collisionPos, float collisionDist, ref RecursiveTree parent) {
            float[] threshold = this.handler.planet.threshold;

            float globalS = handler.getScale();
            Vector3 trans = handler.getTranslation();
            Vector3 off = new Vector3(globalS / 2, -globalS / 2, globalS / 2);

            float x = this.bounds.pos.x + this.bounds.dim / 2;
            float y = this.bounds.pos.y + this.bounds.dim / 2;

            Vector3 pos = this.handler.localRotation * ((new Vector3(x, 0, y) - off).normalized * globalS) + trans;

            // Test distance btw this chunk and projected player point
            float distancePlayerCenter = Vector3.Distance(playerPos, pos);
            if (depth < 4) {
            /* if (depth < threshold.Length && distancePlayerCenter < threshold[depth] * this.handler.getScale()) { // If < threshold */
				// Divide 
                parent.divide();
				this.subdivide();

				for (int i = 0; i < this.cells.Length; i++) {
                    parent.childrens[i] = this.cells[i].getDividedChunksFromCenter(playerPos, collisionPos, collisionDist, ref parent.childrens[i]);
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
                this.gameObject.GetComponent<MeshCollider>().enabled = true;
			}
		}





		public void kill() {
            // KILLS HIMSELF
			this.subdivided = false;
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<MeshCollider>().enabled = false;

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