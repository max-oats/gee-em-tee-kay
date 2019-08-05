using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Vector3 bounds;
	public Vector3 boundsOffset;

	public Vector3 lastHitNormal { get; private set; }
	
	private const float checkUpAmount = .25f;
	private const float checkDownAmount = 1.5f;

	private const float climbHeight = .25f;
	private const float climbSteps = 10;

	private int layerMaskObstruction;
	private int layerMaskGround;

	private void Awake()
	{
		layerMaskObstruction = LayerMask.GetMask("Default", "Obstruction", "Character", "Collider", "Outline");
		layerMaskGround = LayerMask.GetMask("Default", "Obstruction", "Terrain", "Collider", "Outline");
	}

	// INSPECTOR STUFF
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(transform.position + boundsOffset, bounds);
    }

	public void SetLayerMaskObstruction(int newLayerMask)
	{
		layerMaskObstruction = newLayerMask;
	}

	public float Forward {
		get {
			return transform.position.z + boundsOffset.z + bounds.z/2f;
		}
	}

	public float Back {
		get {
			return transform.position.z + boundsOffset.z - bounds.z/2f;
		}
	}

	public float Left {
		get {
			return transform.position.x + boundsOffset.x - bounds.x/2f;
		}
	}

	public float Right {
		get {
			return transform.position.x + boundsOffset.x + bounds.x/2f;
		}
	}

	public float Top {
		get {
			return transform.position.y + boundsOffset.y + bounds.y/2f;
		}
	}

	public Vector3 TopWorldPosition {
		get {
			return transform.position + transform.up * (boundsOffset.y + bounds.y / 2f);
		}
	}

	public float Bottom {
		get {
			return transform.position.y + boundsOffset.y - bounds.y/2f;
		}
	}

	public Vector3 GetForwardLeftAt(Vector3 pos)
	{
		return pos + boundsOffset + (bounds.x/2f) * Vector3.left + (bounds.z/2f) * Vector3.forward;
	}

	public Vector3 GetForwardRightAt(Vector3 pos)
	{
		return pos + boundsOffset + (bounds.x/2f) * Vector3.right + (bounds.z/2f) * Vector3.forward;
	}

	public Vector3 GetBackLeftAt(Vector3 pos)
	{
		return pos + boundsOffset + (bounds.x/2f) * Vector3.left + (bounds.z/2f) * Vector3.back;
	}

	public Vector3 GetBackRightAt(Vector3 pos)
	{
		return pos + boundsOffset + (bounds.x/2f) * Vector3.right + (bounds.z/2f) * Vector3.back;
	}

	public bool IsCollidingAt(Vector3 pos)
	{
		return Physics.CheckBox(pos + boundsOffset, bounds/2f, Quaternion.identity, layerMaskObstruction);
	}

	public bool IsCollidingFromTo(Vector3 startPos, Vector3 endPos)
	{
		var diff = endPos - startPos;
		var mag = diff.magnitude;
		var dir = diff / mag;
		bool hit = Physics.BoxCast(startPos + boundsOffset,
			bounds/2f,
			dir,
			out var raycastHit,
			Quaternion.identity,
			mag,
			layerMaskObstruction);

		lastHitNormal = raycastHit.normal;
		
		return hit;
	}
	
	private const float shim = .001f;
	
	public bool IsCollidingFromTo(Vector3 startPos, Vector3 endPos, ref Vector3 resolvePos)
	{
		var diff = endPos - startPos;
		var mag = diff.magnitude;
		var dir = diff / mag;
		bool result = Physics.BoxCast(startPos + boundsOffset,
			bounds/2f,
			dir,
			out var raycastHit,
			Quaternion.identity,
			mag,
			layerMaskGround);
		if (result)
			resolvePos = transform.position + dir * (raycastHit.distance - shim);
		return result;
	}

	public bool MoveY(Vector3 move, Vector3 fromPos, ref Vector3 toPos)
	{
		var tryPos = fromPos + move;
		if (!IsCollidingFromTo(fromPos, tryPos, ref toPos))
		{
			toPos = tryPos;
			return true;
		}

		return false;
	}
	
	public bool Move(Vector3 move, Vector3 fromPos, ref Vector3 toPos)
	{
		var tryPos = fromPos + move;
		if (!IsCollidingFromTo(fromPos, tryPos))
		{
			toPos = tryPos;
			return true;
		}

		return false;
	}

	public bool SlideMove(Vector3 move, Vector3 fromPos, ref Vector3 toPos)
	{
		var tryPos = fromPos + move;
		if (!IsCollidingFromTo(fromPos, tryPos))
		{
			toPos = tryPos;
			return true;
		}
		
		if (TryClimb(fromPos, move, ref toPos))
			return true;

		if (TrySlide(fromPos, move, ref toPos))
			return true;

		return false;
	}

	private bool TryClimb(Vector3 fromPos, Vector3 move, ref Vector3 toPos)
	{
		for (var i = 0; i < climbSteps; i++)
		{
			var tryPos = fromPos + move + Vector3.up * (climbHeight * i / climbSteps);
			if (IsCollidingFromTo(fromPos, tryPos)) continue;
			
			toPos = tryPos;
			return true;
		}

		return false;
	}
	

	private bool TrySlide(Vector3 fromPos, Vector3 move, ref Vector3 toPos)
	{
		const int angleBit = 5; // 60
		for (var angle = angleBit; angle < 90; angle += angleBit)
		{
			float p = 1f - (angle / 90f) * .5f;
			
			var moveLeft = Quaternion.Euler(0f, -angle, 0f) * move * p;
			var moveRight = Quaternion.Euler(0f, angle, 0f) * move * p;
			
			var tryPos = fromPos + moveLeft;
			if (!IsCollidingFromTo(fromPos, tryPos))
			{
				toPos = tryPos;
				return true;
			}
			
			tryPos = fromPos + moveRight;
			if (!IsCollidingFromTo(fromPos, tryPos))
			{
				toPos = tryPos;
				return true;
			}
		}

		return false;
	}

	private bool IsGroundAt(Vector3 pos)
	{
		return Physics.Raycast(pos + Vector3.up * checkUpAmount, Vector3.down,
			checkUpAmount + checkDownAmount, layerMaskGround);
	}
}
