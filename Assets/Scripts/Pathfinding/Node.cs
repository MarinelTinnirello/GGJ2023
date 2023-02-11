using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{

	/// <summary>
	/// The connections (neighbors).
	/// </summary>
	[SerializeField]
	protected List<Node> m_Connections = new List<Node> ();
	[Header("Sphere Cast Neighbor Controllers")]
	public Vector3 origin;
	public Vector3 direction;
	public float radius = 0.5f;
	public float maxDistance = 0.1f;
	public LayerMask mask;
	float currentHitDistance;

	/// <summary>
	/// Gets the connections (neighbors).
	/// </summary>
	/// <value>The connections.</value>
	public virtual List<Node> connections
	{
		get
		{
			return m_Connections;
		}
	}

	public Node this [int index]
	{
		get
		{
			return m_Connections[index];
		}
	}

	void OnValidate ()
	{
		// Removing duplicate elements
		m_Connections = m_Connections.Distinct().ToList();
	}

	void Awake()
	{
		FindObjectOfType<Graph>().AddNodeToList(this);

		origin = transform.position;
		direction = transform.forward;

		currentHitDistance = maxDistance;
		m_Connections.Clear();
		RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction, maxDistance, mask, QueryTriggerInteraction.UseGlobal);
		foreach (RaycastHit hit in hits)
		{
			Node hitNode = hit.transform.GetComponent<Node>();
			if (hitNode) m_Connections.Add(hitNode);
			currentHitDistance = hit.distance;
		}
	}

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.red;
		Debug.DrawLine(origin, origin + direction * currentHitDistance);
		Gizmos.DrawWireSphere(origin + direction * currentHitDistance, radius);
    }
}
