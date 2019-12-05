using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;


public class testOutcode : MonoBehaviour
{
    Texture2D texture;
    static int resWidth = Screen.width;
    static int resHeight = Screen.height;

    public Light light;
    Color defaultColour;

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
    Vector3[] finalPostDivImage;

    Vector2 start;
    Vector2 finish;

    List<Vector2Int> pixelsDrawn;

    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height);
        GetComponent<Renderer>().material.mainTexture = texture;
        pixelsDrawn = new List<Vector2Int>();
        defaultColour = new Color(texture.GetPixel(1, 1).r, texture.GetPixel(1, 1).g, texture.GetPixel(1, 1).b, texture.GetPixel(1, 1).a);

        cube1 = new Vector3[8];
        cube1[0] = new Vector3(1, 1, 1);
        cube1[1] = new Vector3(-1, 1, 1);
        cube1[2] = new Vector3(-1, -1, 1);                   //define the initial cube
        cube1[3] = new Vector3(1, -1, 1);
        cube1[4] = new Vector3(1, 1, -1);
        cube1[5] = new Vector3(-1, 1, -1);
        cube1[6] = new Vector3(-1, -1, -1);
        cube1[7] = new Vector3(1, -1, -1);


        Vector3 cameraPosition = new Vector3(0, 0, 25);
        Vector3 cameraLookAt = new Vector3(0, 0, 0);
        Vector3 cameraUp = new Vector3(0, 1, 0);

        Vector3 lookRotDir = cameraLookAt - cameraPosition;
        Quaternion cameraRot = Quaternion.LookRotation(lookRotDir.normalized, cameraUp.normalized);

        viewMatrix = Matrix4x4.TRS(-cameraPosition, cameraRot, Vector3.one);

        perspMatrix = Matrix4x4.Perspective(90, Screen.width / Screen.height, 1, 1000);

        startingAxis = new Vector3(14, 2, 2);           //The axis it initially starts on
        startingAxis.Normalize();

        rotationAngle = -90;

        scale = new Vector3(14, 5, 2);

        translate = new Vector3(5, -3, 0);

        drawPoints();

    }
    public static Vector3[] vertices =
    {
        new Vector3(1,1,1),
        new Vector3(-1,1,1),
        new Vector3(-1,-1,1),
        new Vector3(1,-1,1),
        new Vector3(-1,1,-1),
        new Vector3(1,1,-1),
        new Vector3(1,-1,-1),
        new Vector3(-1,-1,-1)
    };
    public static int[][] faceTriangles =
    {
        new int[]{0,1,2,3},
        new int[]{5,0,3,6},
        new int[]{4,5,6,7},
        new int[]{1,4,7,2},
        new int[]{5,4,1,0},
        new int[]{3,2,7,6}
    };

    public static Vector3[] faceVertices(int dir)
    {
        Vector3[] fv = new Vector3[4];
        for (int i = 0; i < fv.Length; i++)
        {
            fv[i] = vertices[faceTriangles[dir][i]];
        }
        return fv;
    }

    private void plot(List<Vector2Int> list)
    {

        foreach (Vector2Int v in list)
        {
            Color color = Color.blue;
            texture.SetPixel(v.x, v.y, color);
            pixelsDrawn.Add(v);
        }
        // Destroy(texture);
        texture.Apply();

    }

    private Vector2Int convertToScreenSpace(Vector2 start)
    {
        int x = (int)Math.Round(((start.x + 1) / 2) * (Screen.width - 1));

        int y = (int)Math.Round((1 - start.y) / 2 * (Screen.height - 1));

        return new Vector2Int(x, y);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(texture);
        rotationAngle += 1;
        texture = new Texture2D(Screen.width, Screen.height);
        GetComponent<Renderer>().material.mainTexture = texture;
        startingAxis = Vector3.up;
        translate = new Vector3(10, 0, 0);
        cubeDrawing(divideByz(MatrixTransform(cube1, superMatrix)));
        drawPoints();
    }
    
    void flood_fill(int pos_x, int pos_y, Color plane_color, Color fill_color)
    {
        
        if (texture.GetPixel(pos_x, pos_y) == plane_color) //if i haven't been there already go back
            return;

        if (texture.GetPixel(pos_x, pos_y) != fill_color) // if it's not color go back
            return;


        texture.SetPixel(pos_x, pos_y, fill_color); // mark the point so that I know if I passed through it. 

        flood_fill(pos_x + 1, pos_y, plane_color, fill_color);  // then i can either go south
        flood_fill(pos_x - 1, pos_y, plane_color, fill_color);  // or north
        flood_fill(pos_x, pos_y + 1, plane_color, fill_color);  // or east
        flood_fill(pos_x, pos_y - 1, plane_color, fill_color);  // or west

        return;

    }

    void drawPoints()
    {
        rotation = Quaternion.AngleAxis(rotationAngle, startingAxis.normalized);
        rotationMatrix = Matrix4x4.TRS(new Vector3(0,0,0),rotation,Vector3.one);                       //Scale is not accounted for.

        scaleMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one);

        translateMatrix = Matrix4x4.TRS(translate, Quaternion.identity, new Vector3(3,3,3));

        single_matrix_of_transformations = rotationMatrix * scaleMatrix * translateMatrix;

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

    
    private Vector3[] divideByz(Vector3[] finalImage)
    {
        List<Vector3> output_list = new List<Vector3>();
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
            return true;      
        }
        if (vO.down)
        {
            v = intercept(u, v, 1);
            return true;
        }
        if (vO.left)
        {
            v = intercept(u, v, 2);
            return true;
        }
            v = intercept(u, v, 3);
            return true;
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

    public void cubeDrawing(Vector3[] cubeDrawn)
    {
        //Front
        //t1
        faceDrawn(cubeDrawn[0], cubeDrawn[1], cubeDrawn[2]);

        //t2
        faceDrawn(cubeDrawn[0], cubeDrawn[2], cubeDrawn[3]);

        //Right
        //t1
        faceDrawn(cubeDrawn[4], cubeDrawn[0], cubeDrawn[3]);
        //t2
        faceDrawn(cubeDrawn[4], cubeDrawn[3], cubeDrawn[7]);


        //Top
        //t1
        faceDrawn(cubeDrawn[4], cubeDrawn[5], cubeDrawn[1]);

        //t2
        faceDrawn(cubeDrawn[4], cubeDrawn[1], cubeDrawn[0]);

        //Back
        //t1
        faceDrawn(cubeDrawn[5], cubeDrawn[4], cubeDrawn[7]);

        //t2
        faceDrawn(cubeDrawn[5], cubeDrawn[7], cubeDrawn[6]);

        //Left
        //t1
        faceDrawn(cubeDrawn[1], cubeDrawn[5], cubeDrawn[6]);

        //t2
        faceDrawn(cubeDrawn[1], cubeDrawn[6], cubeDrawn[2]);

        //Bottom
        //t1
        faceDrawn(cubeDrawn[6], cubeDrawn[7], cubeDrawn[3]);

        //t2
        faceDrawn(cubeDrawn[6], cubeDrawn[3], cubeDrawn[2]);
    }

    public void faceDrawn(Vector2 i, Vector2 j, Vector2 k)
    {
        float iter = (j.x - i.x) * (k.y - j.y) - (j.y - i.y) * (k.x - j.x);

        if (iter >= 0)
        {
            lineDrawing(i, j, texture);
            lineDrawing(j,k,texture);
            lineDrawing(k,i,texture);

            Vector2 Center = getCenter(i, j, k);
            Center = convertToScreenSpace(Center);
            Vector3 normal = getNormal(i, j, k);
            Vector3 lightBeam = getLightDirection(Center);
            float dotProd = Vector3.Dot(lightBeam, normal);
            Color reflection = new Color(dotProd * Color.cyan.r * light.intensity, dotProd * Color.cyan.g * light.intensity, dotProd * Color.cyan.b * light.intensity, 1);

            floodFillStack((int)Center.x, (int)Center.y, reflection, defaultColour);
        }
    }

    public Vector2 getCenter(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        return new Vector2((point1.x + point2.x + point3.x) / 3, (point1.y + point2.y + point3.y) / 3);
    }

    public Vector3 getNormal(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        return Vector3.Normalize(Vector3.Cross(point2 - point1, point3 - point1));
    }

    public Vector3 getLightDirection(Vector3 center)
    {
        return Vector3.Normalize((center - light.transform.position));
    }

    public void floodFillStack(int x, int y, Color newColour, Color oldColour)
    {
        Stack<Vector2> pixels = new Stack<Vector2>();
        pixels.Push(new Vector2(x, y));

        while (pixels.Count > 0)
        {
            Vector2 p = pixels.Pop();
            if (checkBounds(p))
            {
                if (texture.GetPixel((int)p.x, (int)p.y) == oldColour)
                {
                    texture.SetPixel((int)p.x, (int)p.y, newColour);
                    pixels.Push(new Vector2(p.x + 1, p.y));
                    pixels.Push(new Vector2(p.x - 1, p.y));
                    pixels.Push(new Vector2(p.x, p.y + 1));
                    pixels.Push(new Vector2(p.x, p.y - 1));
                }
            }
        }
    }
    public bool checkBounds(Vector2 pixel)
    {
        if ((pixel.x < 0) || (pixel.x >= resWidth - 1))
        {
            print("pixel out of bounds");
            return false;
        }

        if ((pixel.y < 0) || (pixel.y >= resHeight - 1))
        {
            print("pixel out of bounds");
            return false;
        }

        return true;
    }

    private void lineDrawing(Vector2 v1, Vector2 v2, Texture2D screen)
        {
            Vector2 start = v1, end = v2;

            if(lineClip(ref start, ref end))
            {
                List<Vector2Int> breshenham1 = Breshenham(convertToScreenSpace(start), convertToScreenSpace(end));
                breshenhamShow(texture, breshenham1);
            }
            else
            {
                print("wrong" + start + " end" + end);
            }
        }

    public static void breshenhamShow(Texture2D screen, List<Vector2Int> breshehamAll)
    {
        foreach(Vector2Int point in breshehamAll)
        {
            screen.SetPixel(point.x, point.y, Color.magenta);
        }
    }
}
    
    

    
         

        
