using UnityEngine;
using System.Collections;

public class Circuit : MonoBehaviour
{
    public GameObject[] waypoints;

    private void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    private void DrawGizmos(bool selected)
    {
        if (!selected || waypoints.Length <= 0) return;

        Vector3 prev = waypoints[0].transform.position;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 next = waypoints[i].transform.position;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
        Gizmos.DrawLine(prev, waypoints[0].transform.position);
    }

}
