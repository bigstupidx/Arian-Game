using UnityEngine;
using System.Collections;

public class bl_CombineMesh : MonoBehaviour {

    [ContextMenu("Combine")]
    void DoCombine()
    {
        Combine();
    }

	public void Combine()
    {
        Vector3 position = transform.position;
        transform.position = Vector3.zero;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine,true,true);
        transform.gameObject.SetActive(true);

        //Reset position
        transform.position = position;
    }
}