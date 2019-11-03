using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class testOutcode : MonoBehaviour
{
    Texture2D texture;

    Vector3[] cube1;

    float rotationAngle;
    Vector3 startingAxis;
    Quaternion rotation;
    Vector3 scale;
    Vector3 translate;

    Matrix4x4 viewMatrix;
    Matrix4x4 perspMatrix;

    Matrix4x4 rotationMatrix;
    Matrix4x4 scaleMatrix;
    Matrix4x4 translateMatrix;


    Matrix4x4 single_matrix_of_transformations;
    Matrix4x4 superMatrix;

    Vector3[] finalImage;
    Vector2[] finalPostDivImage;

    Vector2 start;
    Vector2 finish;


    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height);
        GetComponent<Renderer>().material.mainTexture = texture;

        cube1 = new Vector3[8];
        cube1[0] = new Vector3(1, 1, 1);
        cube1[1] = new Vector3(-1, 1, 1);
        cube1[2] = new Vector3(-1, -1, 1);                   //define the initial cube
        cube1[3] = new Vector3(1, -1, 1);
        cube1[4] = new Vector3(1, 1, -1);
        cube1[5] = new Vector3(-1, 1, -1);
        cube1[6] = new Vector3(-1, -1, -1);
        cube1[7] = new Vector3(1, -1, -1);

        
        Vector3 cameraPosition = new Vector3(16,5,52);  
        Vector3 cameraLookAt = new Vector3(2,14,2);
        Vector3 cameraUp = new Vector3(-1,2,14);

        Vector3 lookRotDir = cameraLookAt - cameraPosition;
        Quaternion cameraRot = Quaternion.LookRotation(lookRotDir.normalized, cameraUp.normalized);

        viewMatrix = Matrix4x4.TRS(-cameraPosition, cameraRot, Vector3.one);

        perspMatrix = Matrix4x4.Perspective(90, Screen.width / Screen.height, 1, 1000);

        startingAxis = new Vector3(14, 2, 2);           //The axis it initially starts on
        startingAxis.Normalize();

        rotationAngle = -22;

        scale = new Vector3(14, 5, 2);

        translate = new Vector3(5, -3, 4);

        drawCube();
    }

    private void plot(List<Vector2Int> list)
    {
        foreach (Vector2Int v in list)
        {
            Color color = Color.red;
            texture.SetPixel(v.x, v.y, color);
            Debug.Log(v.x + " " + v.y);
        }
        texture.Apply();
    }

    private Vector2Int convertToScreenSpace(Vector2 start)
    {
        int x = (int)Math.Round(((start.x + 1)/2) * (Screen.width - 1));

        int y = (int)Math.Round((1 - start.y)/2 * (Screen.height - 1));

        return new Vector2Int(x, y);
    }

    // Update is called once per frame
    void Update()
    {
        translate += (Vector3.one) * Time.deltaTime;
        drawCube();
    }

    void drawCube()
    {
        rotation = Quaternion.AngleAxis(rotationAngle, startingAxis);
        rotationMatrix = Matrix4x4.TRS(new Vector3(0,0,0),rotation,Vector3.one);                       //Scale is not accounted for.

        scaleMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, scale);

        translateMatrix = Matrix4x4.TRS(translate, Quaternion.identity, new Vector3(1,1,1));

        single_matrix_of_transformations = translateMatrix * scaleMatrix *  rotationMatrix;

        superMatrix = perspMatrix * viewMatrix * single_matrix_of_transformations;

        finalImage = MatrixTransform(cube1, superMatrix);

        finalPostDivImage = divideByz(finalImage);

         
        start = finalPostDivImage[0];
        finish = finalPostDivImage[1];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }
        

        //1 - 2
        start = finalPostDivImage[1];

        finish = finalPostDivImage[2];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[2];

        finish = finalPostDivImage[3];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[3];

        finish = finalPostDivImage[0];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }
        start = finalPostDivImage[1];

        finish = finalPostDivImage[5];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[0];

        finish = finalPostDivImage[4];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[2];

        finish = finalPostDivImage[6];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[3];

        finish = finalPostDivImage[7];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[6];

        finish = finalPostDivImage[5];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[5];

        finish = finalPostDivImage[4];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[4];

        finish = finalPostDivImage[7];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

        start = finalPostDivImage[7];

        finish = finalPostDivImage[6];

        if (lineClip(ref start, ref finish))
        {
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
        }

    }


    //Below this is ok
    private Vector3[] MatrixTransform(
        Vector3[] meshVertices,
        Matrix4x4 transformMatrix)
    {
        Vector3[] output = new Vector3[meshVertices.Length];
        for (int i = 0; i < meshVertices.Length; i++)
            output[i] = transformMatrix *
                new Vector4(
                meshVertices[i].x,
                meshVertices[i].y,
                meshVertices[i].z,
                    1);

        return output;
    }

    private Vector2[] divideByz(Vector3[] finalImage)
    {
        List<Vector2> output_list = new List<Vector2>();
        foreach (Vector3 v in finalImage)
            output_list.Add(new Vector2(v.x / v.z, v.y / v.z));

        return output_list.ToArray();

    }

    public static bool lineClip(ref Vector2 v, ref Vector2 u)
    {
        Outcode inViewPort = new Outcode();
        Outcode vO = new Outcode(v);
        Outcode uO = new Outcode(u);
        //detect trivial acceptance
        if ((vO + uO) == inViewPort)
            return true;

        //detect trivial rejection
        if ((vO * uO) != inViewPort)
            return false;

        /*check vO is in. If not, flip the u and the v around and check method again.
         * When this gets hit again, it is now checking u instead of v beacause they flipped.
         * This is only hit if one point is inViewPort and the other isnt. (wasnt TA or TR).
        */
        if (vO == inViewPort)                   
            return lineClip(ref u,ref v);

        //only called if one point in viewport
        //No need for uO methods, as the point will be flipped to vO in previous methods.
        Vector2 v2 = v;     //defined here so i dont have to keep defining it
        if (vO.up)
        {
            v = intercept(u, v, 0);
            return false;      
        }
        if (vO.down)
        {
            v = intercept(u, v, 1);
            return false;
        }
        if (vO.left)
        {
            v = intercept(u, v, 2);
            return false;
        }
            v = intercept(u, v, 3);
            return false;
    }
    /// <summary>
    /// calculates the intercept point on the line
    /// <param name= "p1">point 1</param>
    /// <param name= "p2">point 2</param>
    /// <param name= "v2">viewport side UDLR</param>
    /// </summary>
    /// <returns>returns the clipped point on the line</returns>
    private static Vector2 intercept(Vector2 p1, Vector2 p2, int v2)
    {
        float slope = (p2.y - p1.y) / (p2.x - p1.x);
        if (v2 == 0)
            return new Vector2(p1.x + (1/slope) * (1- p1.y), 1);

        if (v2 == 1)
            return new Vector2(p1.x + (1 / slope) * (-1 - p1.y), -1);

        if (v2 == 2)
            return new Vector2( -1, p1.y + (slope) * (-1 - p1.x));

        return new Vector2(1, p1.y + (slope) * (1 - p1.x));

    } 

    public List<Vector2Int>Breshenham(Vector2Int start, Vector2Int finish)
    {
        int dx = finish.x - start.x;

        if(dx < 0)
        {
            return Breshenham(finish, start);
        }
        int dy = (int)finish.y - (int)start.y;

        if(dy<0)//neg slope
        {
            return negateY(Breshenham(negateY(start), negateY(finish)));
        }
        if(dy>dx)//slope>1
        {
            return swapXY(Breshenham(swapXY(start),swapXY(finish)));
        }
        int A = 2 * dy;
        int B = 2 * (dy - dx);
        int p = 2 * dy - dx;
        List<Vector2Int> outputList = new List<Vector2Int>();

        int y = (int)start.y;
        for(int x = (int)start.x; x<=(int)finish.x; x++)
        {
            outputList.Add(new Vector2Int(x, y));
            if(p>0)
            {
                y++;
                p += B;
            }
            else
            {
                p += A;
            }
        }
        Debug.Log(outputList);
        return outputList;
    }

    private Vector2Int swapXY(Vector2Int start)
    {
        return new Vector2Int(start.y, start.x);
    }

    private List<Vector2Int> swapXY(List<Vector2Int> list)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();
        foreach (Vector2Int v in list)
            outputList.Add(swapXY(v));

        return outputList;
    }

    private List<Vector2Int> negateY(List<Vector2Int> list)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();
        foreach (Vector2Int finish in list)
            outputList.Add(negateY(finish));

        return outputList;
    }

    private Vector2Int negateY(Vector2Int finish)
    {
        return new Vector2Int(finish.x,-finish.y);
    }
}
    
    

    
         

        
