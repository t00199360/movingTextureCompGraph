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

    List<Vector2Int> pixelsDrawn;

    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height);
        GetComponent<Renderer>().material.mainTexture = texture;
        pixelsDrawn = new List<Vector2Int>();
        

        cube1 = new Vector3[8];
        cube1[0] = new Vector3(1, 1, 1);
        cube1[1] = new Vector3(-1, 1, 1);
        cube1[2] = new Vector3(-1, -1, 1);                   //define the initial cube
        cube1[3] = new Vector3(1, -1, 1);
        cube1[4] = new Vector3(1, 1, -1);
        cube1[5] = new Vector3(-1, 1, -1);
        cube1[6] = new Vector3(-1, -1, -1);
        cube1[7] = new Vector3(1, -1, -1);

        
        Vector3 cameraPosition = new Vector3(0,0,10);  
        Vector3 cameraLookAt = new Vector3(0,0,0);
        Vector3 cameraUp = new Vector3(0,1,0);

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
        int x = (int)Math.Round(((start.x + 1)/2) * (Screen.width - 1));

        int y = (int)Math.Round((1 - start.y)/2 * (Screen.height - 1));

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
        translate = new Vector3(10,0,0);
        drawPoints();
        flood_fill(convertToScreenSpace(cube1[1]).x, convertToScreenSpace(cube1[1]).y, texture.GetPixel(0,0), Color.blue);
        //FloodFillBorder(convertToScreenSpace(cube1[1]).x, convertToScreenSpace(cube1[1]).y, texture.GetPixel(0, 0), Color.red);
    }
    


    void flood_fill(int pos_x, int pos_y, Color plane_color, Color fill_color)
    {
        //int w = texture.width;
        //int h = texture.height;
        //Color[] colors = texture.GetPixels();
        //Color refCol = colors[pos_x + pos_y * w];
        //Queue<Point> nodes = new Queue<Point>();
        //nodes.Enqueue(new Point(pos_x, pos_y));
        //while (nodes.Count > 0)
        //{
        //    Point current = nodes.Dequeue();
        //    for (int i = current.X; i < w; i++)
        //    {
        //        Debug.Log(colors.Length);
        //        Color C = colors[i + current.Y * w];
        //        if (C != refCol || C == plane_color)
        //            break;
        //        colors[i + current.Y * w] = plane_color;
        //        if (current.Y + 1 < h)
        //        {
        //            C = colors[i + current.Y * w + w];
        //            if (C == refCol && C != plane_color)
        //                nodes.Enqueue(new Point(i, current.Y - 1));
        //        }
        //    }

        //    for (int i = current.X - 1; i >= 0; i--)
        //    {
        //        Color C = colors[i + current.Y * w];
        //        if (C != refCol || C == plane_color)
        //            break;
        //        colors[i + current.Y * w] = plane_color;
        //        if (current.Y + 1 < h)
        //        {
        //            C = colors[i + current.Y * w + w];
        //            if (C == refCol && C != plane_color)
        //                nodes.Enqueue(new Point(i, current.Y + 1));
        //        }
        //        if (current.Y - 1 >= 0)
        //        {
        //            C = colors[i + current.Y * w - w];
        //            if (C == refCol && C != plane_color)
        //                nodes.Enqueue(new Point(i, current.Y - 1));
        //        }
        //    }

        //}
        //texture.SetPixels(colors);

        
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

    void FloodFillBorder(int aX, int aY, Color aFillColor, Color aBorderColor)
    {
        int w = texture.width;
        int h = texture.height;
        Color[] colors = texture.GetPixels();
        byte[] checkedPixels = new byte[colors.Length];
        Color refCol = aBorderColor;
        Queue<Point> nodes = new Queue<Point>();
        nodes.Enqueue(new Point(aX, aY));
        while (nodes.Count > 0)
        {
            Point current = nodes.Dequeue();

            for (int i = current.X; i < w; i++)
            {
                if (checkedPixels[i + current.Y * w] > 0 || colors[i + current.Y * w] == refCol)
                    break;
                colors[i + current.Y * w] = aFillColor;
                checkedPixels[i + current.Y * w] = 1;
                if (current.Y + 1 < h)
                {
                    if (checkedPixels[i + current.Y * w + w] == 0 && colors[i + current.Y * w + w] != refCol)
                        nodes.Enqueue(new Point(i, current.Y + 1));
                }
                if (current.Y - 1 >= 0)
                {
                    if (checkedPixels[i + current.Y * w - w] == 0 && colors[i + current.Y * w - w] != refCol)
                        nodes.Enqueue(new Point(i, current.Y - 1));
                }
            }
            for (int i = current.X - 1; i >= 0; i--)
            {
                if (checkedPixels[i + current.Y * w] > 0 || colors[i + current.Y * w] == refCol)
                    break;
                colors[i + current.Y * w] = aFillColor;
                checkedPixels[i + current.Y * w] = 1;
                if (current.Y + 1 < h)
                {
                    if (checkedPixels[i + current.Y * w + w] == 0 && colors[i + current.Y * w + w] != refCol)
                        nodes.Enqueue(new Point(i, current.Y + 1));
                }
                if (current.Y - 1 >= 0)
                {
                    if (checkedPixels[i + current.Y * w - w] == 0 && colors[i + current.Y * w - w] != refCol)
                        nodes.Enqueue(new Point(i, current.Y - 1));
                }
            }
        }
        texture.SetPixels(colors);
    }


void drawPoints()
    {
        rotation = Quaternion.AngleAxis(rotationAngle, startingAxis);
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
}
    
    

    
         

        
