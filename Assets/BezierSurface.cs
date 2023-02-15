using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSurface : MonoBehaviour
{
    //testing
    [SerializeField] int numberOfSampePoints;
    [SerializeField] bool drawSurfaceNormals;
    [SerializeField] float normalDrawLength;
    [SerializeField] Material newmat;
    [SerializeField] GameObject camera;
    private Texture2D noiseTex;
    //mesh filter for grabbing the mesh
    [SerializeField] private MeshFilter meshFilter;
    private Mesh mesh;
    private int nospsquared;
    //The control points
    const int NUMBER_OF_CONTROL_POINTS = 16;
    [Tooltip("4x4 matrix of control points in row major order")]
    [SerializeField] private Transform[] surfaceControlTransforms = new Transform[NUMBER_OF_CONTROL_POINTS];
    private Vector3[] surfaceControlPoints = new Vector3[NUMBER_OF_CONTROL_POINTS];

    private void Awake()
    {
        for (int index = 0; index < NUMBER_OF_CONTROL_POINTS; index++)
        {
            surfaceControlPoints[index] = surfaceControlTransforms[index].position;
        }
    }

    private void Start()
    {
        //get the mesh from the mesh filter
        // mesh = meshFilter.mesh;
        // mesh.Clear();
        // Debug.Log(mesh);
        // //drawSurface(surfaceControlPoints, Color.yellow, Color.green, drawSurfaceNormals);
        // nospsquared = numberOfSampePoints* numberOfSampePoints;
        
        // Vector3 [] vertexarray;
        // Vector3 [] normalarray;
        //  Vector4 [] uvarray;
        // int [] triangles = renderSurface(surfaceControlPoints,out normalarray, out vertexarray,out uvarray);
        // for(int i = 0; i < nospsquared ; i++){
        //     Debug.Log(vertexarray[i]);
        // } 
        // int length = calcTrianglespoints();
        // for(int i = 0; i < length; i++){
        //     Debug.Log($"triangles{triangles[i]}");
        // }
        // mesh.vertices = vertexarray;
        // mesh.normals = normalarray;
        // mesh.triangles = triangles;
        // meshFilter.mesh = mesh;
    
        // newmat.SetFloat("_exponent",9.0f);
        // newmat.SetVectorArray("uvCoordinates",uvarray);
        // //float a = newmat.shader.Find("GLSL basic111 shader").expo;
        // //
        //  //noiseTex = new Texture2D(1,1);
        //   //newmat.mainTexture = noiseTex;
    }
    

    private void Update()
    {
        Awake();
    
        

        //draw the bezier surface 
        drawSurface(surfaceControlPoints, Color.yellow, Color.green, drawSurfaceNormals);
    }

    private Vector3[] Slice(Vector3[] data, int start, int end)
    {
        Vector3[] subArray = new Vector3[end - start];

        for (int index = start; index < end; index++)
        {
            subArray[index - start] = new Vector3(data[index].x, data[index].y, data[index].z);
        }

        return subArray;
    }

    private void drawCurve(Vector3[] controlPoints, Color color)
    {
        List<Vector3> samplePoints = new List<Vector3>();
        for (int sampleIndex = 0; sampleIndex < numberOfSampePoints; sampleIndex++)
        {
            float t = (float)sampleIndex / (numberOfSampePoints - 1.0f);
            Vector3 tangent;
            samplePoints.Add(computePointOnBezierCurve(t, controlPoints, out tangent));
        }

        for (int sampleIndex = 0; sampleIndex < samplePoints.Count - 1; sampleIndex++)
        {
            Debug.DrawLine(samplePoints[sampleIndex], samplePoints[sampleIndex + 1], color);
        }
    }

    private void drawSurface(Vector3[] controlPoints, Color uColor, Color vColor, bool drawNormals=false)
    {
        List<List<Vector3>> samplePoints = new List<List<Vector3>>();
        List<List<Vector3>> sampleNormals = new List<List<Vector3>>();

        for (int vSampleIndex = 0; vSampleIndex < numberOfSampePoints; vSampleIndex++)
        {
            float v = (float)vSampleIndex / (numberOfSampePoints - 1.0f);
            samplePoints.Add(new List<Vector3>());
            sampleNormals.Add(new List<Vector3>());

            for (int uSampleIndex = 0; uSampleIndex < numberOfSampePoints; uSampleIndex++)
            {
                float u = (float)uSampleIndex / (numberOfSampePoints - 1.0f);
                Vector3 normal;
                samplePoints[vSampleIndex].Add(computePointOnBezierSurface(u, v, controlPoints, out normal));
                sampleNormals[vSampleIndex].Add(normal);
            }
        }

        for (int vSampleIndex = 0; vSampleIndex < numberOfSampePoints; vSampleIndex++)
        {
            for (int uSampleIndex = 0; uSampleIndex < numberOfSampePoints; uSampleIndex++)
            {
                //draw a line to the next u point
                if (uSampleIndex != numberOfSampePoints - 1)
                {
                    Debug.DrawLine(samplePoints[vSampleIndex][uSampleIndex], samplePoints[vSampleIndex][uSampleIndex + 1], uColor);
                }

                //draw a line to the next v point
                if (vSampleIndex != numberOfSampePoints - 1)
                {
                    Debug.DrawLine(samplePoints[vSampleIndex][uSampleIndex], samplePoints[vSampleIndex + 1][uSampleIndex], vColor);
                }

                //if drawNormal is true then draw a line to show the normals
                if (drawNormals)
                {
                    //note that we cannot set a color in the inspector, doing so result in DrawLine drawing nothing
                    Debug.DrawLine(samplePoints[vSampleIndex][uSampleIndex], samplePoints[vSampleIndex][uSampleIndex] + (normalDrawLength*sampleNormals[vSampleIndex][uSampleIndex]), Color.blue);
                }
            }
        }
    }
    //return a list of sample points and normals
    private int[] renderSurface(Vector3[] controlPoints,out Vector3[] sampleNormals, out Vector3[] samplePoints,out Vector4[] uv)
    {
        //List<List<Vector3>> samplePoints = new List<List<Vector3>>();
       // List<List<Vector3>> sampleNormals = new List<List<Vector3>>();
        sampleNormals = new Vector3[nospsquared];
        samplePoints = new Vector3[nospsquared];
        uv = new Vector4[nospsquared];
        int[] triangles = new int[calcTrianglespoints()];
        Debug.Log($"nospsquared{nospsquared}");
        int index = 0;
        for (int vSampleIndex = 0; vSampleIndex < numberOfSampePoints; vSampleIndex++)
        {
            float v = (float)vSampleIndex / (numberOfSampePoints - 1.0f);
            
            //samplePoints.Add(new List<Vector3>());
            //sampleNormals.Add(new List<Vector3>());

            for (int uSampleIndex = 0; uSampleIndex < numberOfSampePoints; uSampleIndex++)
            {
                float u = (float)uSampleIndex / (numberOfSampePoints - 1.0f);
                uv[vSampleIndex*numberOfSampePoints+uSampleIndex] =  new Vector4(u,v,0.0f,0.0f);
                Vector3 normal;

                samplePoints[(vSampleIndex*numberOfSampePoints) + uSampleIndex] = (computePointOnBezierSurface(u, v, controlPoints, out normal));
                sampleNormals[(vSampleIndex* numberOfSampePoints) + uSampleIndex] = (normal);
                if(vSampleIndex > 0){
                    if(uSampleIndex < numberOfSampePoints-1){
                    triangles[index] = (vSampleIndex*numberOfSampePoints) + uSampleIndex;
                    index += 1;
                   
                    triangles[index] = ((vSampleIndex-1)*numberOfSampePoints) + uSampleIndex +1 ; 
                    index += 1;
                     triangles[index] = (vSampleIndex*numberOfSampePoints) + uSampleIndex +1 ; 
                    index += 1;
                    // upside down triangle
                      triangles[index] = (vSampleIndex*numberOfSampePoints) + uSampleIndex;
                    index += 1;
                    triangles[index] = ((vSampleIndex-1)*numberOfSampePoints) + uSampleIndex ; 
                    index += 1;
                    triangles[index] = ((vSampleIndex-1)*numberOfSampePoints) + uSampleIndex +1 ; 
                    index += 1;
                    Debug.Log($"triindex{triangles[index-1]}");

                    }
                }
            }
        }
        return triangles;

      //return samplePoints;
    }
    int calcTrianglespoints(){
       int sidecount = 1+2 + ((numberOfSampePoints-2)*3);
       int middlecount = 2*3 + ((numberOfSampePoints-2)*6);
       return (2*sidecount) + ((numberOfSampePoints-2)*middlecount);  
    }

    /*Function to compute the position and tangent on a Bezier surface
    * 
    * u - the parameter u of the Bezier surface
    * v - the parameter v of the Bezier surface
    * controlPoints - list of control points to define the curve, row major order
    * 
    * return:
    *  Value - the position of the point on the surface
    *  normal - the normal vector of the Bezier surface at the point
    */
    private Vector3 computePointOnBezierSurface(float u, float v, Vector3[] controlPoints, out Vector3 normal)
    {
        const int NUMBER_OF_CURVES = 4; //The total number of curves going on direction, it is the same
                                        //as the number of control points for any curve, and is the square
                                        //root of NUMBER_OF_CONTROL_POINTS

        //declare arrays to store the control points for the v direction and tangents in u direction
        Vector3[] vControlPoints = new Vector3[NUMBER_OF_CURVES];
        Vector3[] uTangents = new Vector3[NUMBER_OF_CURVES];

        Vector3 finalPoint;
        Vector3 vTangent;
        Vector3 uTangent;

        //compute the control points for v direction and tangents in u direction
        for (int uCurveIndex = 0; uCurveIndex < NUMBER_OF_CURVES; uCurveIndex++)
        {
            vControlPoints[uCurveIndex] = computePointOnBezierCurve(u, Slice(controlPoints, uCurveIndex*NUMBER_OF_CURVES, (uCurveIndex + 1)*NUMBER_OF_CURVES), out uTangents[uCurveIndex]);
        }

        //use interpolation to compute u tangent

        //find which two u curve the point is between
        float uScaledByNumberOfCurves = u * (NUMBER_OF_CURVES - 1); //first scale u by the number of curve, u is now in
                                                              //the form of x.y where x is the index of the curve on
                                                              //the left, and y is the t parameter for interpolation
        int leftUCurveIndex = (int)Mathf.Floor(uScaledByNumberOfCurves); //get index (x) by flooring u
        float uInterpolationParameter = uScaledByNumberOfCurves - leftUCurveIndex; //get the t parameter (y) using x.y - x

        if (leftUCurveIndex == NUMBER_OF_CURVES - 1)
        {
            //On the last control point, this is an edge case that causes an error, so we are handling tis case seperately
            //if you are on a control point the tangent is just the tangent for that control point
            uTangent = uTangents[leftUCurveIndex];
        }
        else
        {
            //perform the linear interpolation of the tangents, this is the same as using Mathf.Lerp
            uTangent = ((1-uInterpolationParameter)*uTangents[leftUCurveIndex]) + (uInterpolationParameter*uTangents[leftUCurveIndex + 1]);
        }


        //compute final point and tangent in v direction
        finalPoint = computePointOnBezierCurve(v, vControlPoints, out vTangent);

        //compute the normal vector
        normal = Vector3.Cross(uTangent, vTangent);

        //return final point
        //Debug.Log($"final point {finalPoint}, normal {normal}, v tangent {vTangent}, u tangent {uTangent}");
        return finalPoint;
    }

    /*Function to compute the position and tangent on a Bezier curve
    * 
    * t - the parameter t of the Bezier curve
    * controlPoints - list of control points to define the curve
    * 
    * return:
    *  Value - the position of the point on the curve
    *  tangent - the tangent vector of the Bezier curve at the point
    */
    private Vector3 computePointOnBezierCurve(float t, Vector3[] controlPoints, out Vector3 tangent)
    {
        //make a deep copy of the control points so we can edit them
        List<Vector3> intermmediateControlPoints = new List<Vector3>(controlPoints);

        //the order of the Bezier curve (number of control points - 1),
        //loop until this value is 1
        int bezierOrder = intermmediateControlPoints.Count - 1;

        //loop until we have 2 controls points (when order is one),
        //at that point we are able to compute tangent and position
        while (bezierOrder > 1)
        {
            //iterate through control points
            for (int bezierIndex = 0; bezierIndex < bezierOrder; bezierIndex++)
            {
                //compute the control point one level down
                intermmediateControlPoints[bezierIndex] = ((1 - t)*intermmediateControlPoints[bezierIndex]) + (t*intermmediateControlPoints[bezierIndex + 1]);
            }

            bezierOrder--;
        }

        //now the first two elements are of interest

        //compute unit tangent
        tangent = (intermmediateControlPoints[1] - intermmediateControlPoints[0]).normalized;

        //compute and return the position
        return ((1 - t)*intermmediateControlPoints[0]) + (t*intermmediateControlPoints[1]);
    }
}
