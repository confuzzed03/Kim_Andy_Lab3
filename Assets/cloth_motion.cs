using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cloth_motion: MonoBehaviour {

	float 		t;
	int[] 		edge_list;
	float 		mass;
	float		damping;
	float 		stiffness;
	float[] 	L0;
	Vector3[] 	velocities;


	// Use this for initialization
	void Start () 
	{
		t 			= 0.075f;
		mass 		= 1.0f;
		damping 	= 0.99f;
		stiffness 	= 1000.0f;

		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		int[] 		triangles = mesh.triangles;
		Vector3[] 	vertices = mesh.vertices;

		//Construct the original edge list
		int[] original_edge_list = new int[triangles.Length*2];
		for (int i=0; i<triangles.Length; i+=3) 
		{
			original_edge_list[i*2+0]=triangles[i+0];
			original_edge_list[i*2+1]=triangles[i+1];
			original_edge_list[i*2+2]=triangles[i+1];
			original_edge_list[i*2+3]=triangles[i+2];
			original_edge_list[i*2+4]=triangles[i+2];
			original_edge_list[i*2+5]=triangles[i+0];
		}
		//Reorder the original edge list
		for (int i=0; i<original_edge_list.Length; i+=2)
			if(original_edge_list[i] > original_edge_list[i + 1]) 
				Swap(ref original_edge_list[i], ref original_edge_list[i+1]);
		//Sort the original edge list using quicksort
		Quick_Sort (ref original_edge_list, 0, original_edge_list.Length/2-1);

		int count = 0;
		for (int i=0; i<original_edge_list.Length; i+=2)
			if (i == 0 || 
			    original_edge_list [i + 0] != original_edge_list [i - 2] ||
			    original_edge_list [i + 1] != original_edge_list [i - 1]) 
					count++;

		edge_list = new int[count * 2];
		int r_count = 0;
		for (int i=0; i<original_edge_list.Length; i+=2)
			if (i == 0 || 
			    original_edge_list [i + 0] != original_edge_list [i - 2] ||
				original_edge_list [i + 1] != original_edge_list [i - 1]) 
			{
				edge_list[r_count*2+0]=original_edge_list [i + 0];
				edge_list[r_count*2+1]=original_edge_list [i + 1];
				r_count++;
			}


		L0 = new float[edge_list.Length/2];
		for (int e=0; e<edge_list.Length/2; e++) 
		{
			int v0 = edge_list[e*2+0];
			int v1 = edge_list[e*2+1];
			L0[e]=(vertices[v0]-vertices[v1]).magnitude;
		}

		velocities = new Vector3[vertices.Length];
		for (int v=0; v<vertices.Length; v++)
			velocities [v] = new Vector3 (0, 0, 0);

		//for(int i=0; i<edge_list.Length/2; i++)
		//	Debug.Log ("number"+i+" is" + edge_list [i*2] + "and"+ edge_list [i*2+1]);
	}

	void Quick_Sort(ref int[] a, int l, int r)
	{
		int j;
		if(l<r)
		{
			j=Quick_Sort_Partition(ref a, l, r);
			Quick_Sort (ref a, l, j-1);
			Quick_Sort (ref a, j+1, r);
		}
	}

	int  Quick_Sort_Partition(ref int[] a, int l, int r)
	{
		int pivot_0, pivot_1, i, j;
		pivot_0 = a [l * 2 + 0];
		pivot_1 = a [l * 2 + 1];
		i = l;
		j = r + 1;
		while (true) 
		{
			do ++i; while( i<=r && (a[i*2]<pivot_0 || a[i*2]==pivot_0 && a[i*2+1]<=pivot_1));
			do --j; while(  a[j*2]>pivot_0 || a[j*2]==pivot_0 && a[j*2+1]> pivot_1);
			if(i>=j)	break;
			Swap(ref a[i*2], ref a[j*2]);
			Swap(ref a[i*2+1], ref a[j*2+1]);
		}
		Swap (ref a [l * 2 + 0], ref a [j * 2 + 0]);
		Swap (ref a [l * 2 + 1], ref a [j * 2 + 1]);
		return j;
	}

	void Swap(ref int a, ref int b)
	{
		int temp = a;
		a = b;
		b = temp;
	}


	void Strain_Limiting()
	{
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] temp_X = new Vector3[vertices.Length];
        Vector3[] x_new = new Vector3[vertices.Length];
        int[] temp_N = new int[vertices.Length];
        HashSet<int> vi_list = new HashSet<int>();
        HashSet<int> vj_list = new HashSet<int>();
        for (int e = 0; e < edge_list.Length / 2; e++)
        {
            int v0 = edge_list[e * 2];
            int v1 = edge_list[e * 2 + 1];

            Vector3 v_i = vertices[v0];
            Vector3 v_j = vertices[v1];

            float length = (v_i - v_j).magnitude;

            if (length > L0[e])
            {
                vi_list.Add(v0);
                vj_list.Add(v1);

                Vector3 x_i = 0.5f * (v_i + v_j + L0[e] * (v_i - v_j) / (v_i - v_j).magnitude);
                Vector3 x_j = 0.5f * (v_j + v_i + L0[e] * (v_j - v_i) / (v_j - v_i).magnitude);

                temp_X[v0] += x_i;
                temp_X[v1] += x_j;
                temp_N[v0]++;
                temp_N[v1]++;
            }
        }
        foreach(int i in vi_list)
        {
            Vector3 xi_new = (0.2f * vertices[i] + temp_X[i]) / (0.2f + temp_N[i]);
            velocities[i] += (xi_new - vertices[i]) / t;
            vertices[i] = xi_new;
        }
        foreach (int j in vj_list)
        {
            Vector3 xj_new = (0.2f * vertices[j] + temp_X[j]) / (0.2f + temp_N[j]);
            velocities[j] += (xj_new - vertices[j]) / t;
            vertices[j] = xj_new;
        }
    }


	void Collision_Handling()
	{
        GameObject sphere = GameObject.Find("Sphere");
        Vector3 center = sphere.transform.TransformPoint(0,0,0);
	}

	// Update is called once per frame
	void Update () 
	{
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices = mesh.vertices;
        for (int x = 0; x < vertices.Length; x++)
        {
            if(x != 0 && x != 10)
            {
                velocities[x] += t * new Vector3(0, -9.8f, 0);
                velocities[x] *= damping;
                vertices[x] += t * velocities[x];
            }
        }
        mesh.vertices = vertices;

		mesh.RecalculateNormals ();

        for(int i = 0; i < 64; i++)
        {
            Strain_Limiting();
        }

        Collision_Handling();

	}
}
