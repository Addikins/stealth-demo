using System;
using UnityEngine;

namespace StealthGame.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] Color waypointColor;

        private void OnDrawGizmos()
        {

            const float waypointGizmoRadius = 0.3f;

            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.color = waypointColor;
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}