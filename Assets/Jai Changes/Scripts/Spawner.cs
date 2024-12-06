using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Texture2D spawnImage;

    public GameObject objectToSpawn;
    //public GameObject objectToSpawnBlue;

    public int pixelSize;
    public float spacing;

    public float yScalar;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPointFromImage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPointFromImage()
    {
        if(spawnImage == null)
        {
            Debug.LogError("No input texture found");
            return;
        }
        bool[,] occupiedPixels = new bool[spawnImage.width, spawnImage.height];

        int count=0;

        for(int z=0; z<spawnImage.height; z+=pixelSize)
        {
            for(int x=0; x<spawnImage.width; x+=pixelSize)
            {
                if(CanSpawn(spawnImage, x, z, occupiedPixels))
                {
                    MarkOccupied(x, z, occupiedPixels);

                    //0 should be blue channel
                    Vector3 spawnPosition = new Vector3(x*spacing, PixHeight(x,z) * yScalar, z * spacing) + transform.position - (transform.right * spawnImage.width/2);

                    GameObject obj = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                    obj.transform.parent = transform;

                    //obj.GetComponent<MeshRenderer>().material = new Material(obj.GetComponent<MeshRenderer>().material);
                    //obj.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

                    count++;
                }

                //print(count);
            }
        }
    }

    bool CanSpawn(Texture2D image, int startX, int startY, bool[,] occupied)
    {
        int redCount=0;

        for (int y = 0; y < pixelSize; y ++)
        {
            for (int x = 0; x < pixelSize; x++)
            {
                int pixelX = startX + x;
                int pixelY = startY + y;

                if (pixelX >= spawnImage.width || pixelY >= spawnImage.height)
                {
                    continue;
                }

                if(occupied[pixelX, pixelY])
                {
                    return false;
                }

                Color pixelColour = image.GetPixel(pixelX, pixelY);
                if (IsRed(pixelColour))
                {
                    //print("pixel at [" + pixelX+","+pixelY+"] is red with rgb [" + pixelColour.r+","+pixelColour.g+","+pixelColour.b+"]");
                    redCount++;
                }
            }
        }


        return redCount>=pixelSize;
    }

    float PixHeight(int pixelX, int pixelY)
    {
        float height = 0;
        Texture2D image = spawnImage;

        Color pixelColour = image.GetPixel(pixelX, pixelY);
        height = pixelColour.g;
        return height;
    }

    void MarkOccupied(int startX, int startY, bool[,] occupied)
    {
        for (int y = 0; y < pixelSize; y++)
        {
            for (int x = 0; x < pixelSize; x++)
            {
                int pixelX = startX + x;
                int pixelY = startY + y;

                if(pixelX>=occupied.GetLength(0) || pixelY >= occupied.GetLength(1))
                {
                    continue;
                }

                occupied[pixelX, pixelY] = true;
            }
        }
    }

    bool IsRed(Color colour)
    {
        return colour.r > 0.5f;
    }
}
