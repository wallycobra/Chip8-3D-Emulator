using UnityEngine;

public class CubeDisplay
{
    public GameObject[,] cubes = new GameObject[64, 32];
    public float offset = 1.05f;
    public void Initialize()
    {
        CreateCubes();
    }
    private void CreateCubes()
    {
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                cubes[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubes[x, y].transform.position = new Vector3(x * offset, - y * offset, 0);
                Renderer renderer = cubes[x, y].GetComponent<Renderer>();
                renderer.material.color = Color.green;
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", Color.green * .5f);
                Render(x, y, true); // Start with all pixels off
            }
        }
    }
    public void Render(int x, int y, bool isOn)
    {
        cubes[x, y].SetActive(isOn);
    }   
    public void RenderFullDisplay(bool[,] display)
    {
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                cubes[x, y].SetActive(display[x, y]);
            }   
        }
    }
}