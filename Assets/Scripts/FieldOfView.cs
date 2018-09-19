using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TargetsVisibilityChange(List<Transform> newTargets);

[ExecuteInEditMode]
public class FieldOfView : MonoBehaviour
{
    public float aimingHeight = 0.75f;                  // 眼睛距地高度
    public float viewRadius = 1f;                       // 可视距离
    [Range(0, 360)] public float viewAngle = 360f;      // 可视扇形区域夹角
    public float meshDegreeResolution = 1f;             // 扇形区域检测用的Mesh的生成三角形个数/度
    public int edgeResolveIterations = 4;               // 计算墙壁边缘的时候二分法逼近的精度
    public float edgeDistanceThreshold = 0.5f;          // 计算墙壁边缘的时候的阈值
    public float checkTimeGap = 0.2f;                   // 进行检测的时间间隔

    public LayerMask targetMask;                        // 能被探测到的物体
    public LayerMask obstacleMask;                      // 障碍物

    [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public static event TargetsVisibilityChange OnTargetsVisibilityChange;
    public FogProjector fogProjector;

    private void Start()
    {
        viewMesh = new Mesh() { name = "View Mesh" };
        viewMeshFilter.mesh = viewMesh;

        fogProjector = fogProjector ?? FindObjectOfType<FogProjector>();

        StartCoroutine("FindTargetsWithDelay", checkTimeGap);
    }

    void FindVisibleTargets()
    {
        Vector3 aimingPosition = transform.position + transform.up * aimingHeight;
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapCapsule(transform.position, transform.position - transform.up * 20, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            Vector3 dirToTarget = (target.position - aimingPosition).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(aimingPosition, dirToTarget, dstToTarget, obstacleMask))
                {
                    if (target.position.y - transform.position.y < 0.75f)
                        visibleTargets.Add(target);
                }
            }
        }

        if (OnTargetsVisibilityChange != null) OnTargetsVisibilityChange(visibleTargets);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshDegreeResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i>0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero + aimingHeight * transform.up)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero + aimingHeight * transform.up)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;

            // Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero + aimingHeight * transform.up;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero + transform.up * aimingHeight;
        Vector3 maxPoint = Vector3.zero + transform.up * aimingHeight;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 aimingPosition = transform.position + transform.up * aimingHeight;
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(aimingPosition, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, aimingPosition + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
        fogProjector.UpdateFog();
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}
