using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CollisionHandler2D : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField][Range(0.001f, 0.1f)] private float skinWidth = 0.02f;
    [SerializeField] private float horizontalRaySpacing = 0.25f;
    [SerializeField] private float verticalRaySpacing = 0.25f;
    private int horizontalRayCount;
    private int verticalRayCount;

    [SerializeField] private bool visualizeRays = true;

    private RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        CalculateRaySpacing();
    }

    public void Move(Vector2 moveAmount)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (moveAmount.x != 0)
            HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
            VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);
        Physics2D.SyncTransforms();
    }

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = CastRay(rayOrigin, Vector2.right * directionX, rayLength);
            if (!hit) continue;

            if (visualizeRays)
                Debug.DrawRay(rayOrigin, directionX * rayLength * Vector2.right, Color.red);

            moveAmount.x = (hit.distance - skinWidth) * directionX;
            rayLength = hit.distance;

            collisions.left = directionX == -1;
            collisions.right = directionX == 1;
        }
    }

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);

            RaycastHit2D hit = CastRay(rayOrigin, Vector2.up * directionY, rayLength);

            if (!hit) continue;

            if (visualizeRays)
                Debug.DrawRay(rayOrigin, directionY * rayLength * Vector2.up, Color.red);

            moveAmount.y = (hit.distance - skinWidth) * directionY;
            rayLength = hit.distance;

            collisions.below = directionY == -1;
            collisions.above = directionY == 1;
        }
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(-skinWidth * 2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(-skinWidth * 2);

        horizontalRayCount = Mathf.Max(Mathf.CeilToInt(bounds.size.y / horizontalRaySpacing) + 1, 2);
        verticalRayCount = Mathf.Max(Mathf.CeilToInt(bounds.size.x / verticalRaySpacing) + 1, 2);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = left = right = false;
        }
    }

    public float GetSkinWidth()
    {
        return skinWidth;
    }

    private RaycastHit2D CastRay(Vector2 origin, Vector2 direction, float length)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, length, collisionMask);
        foreach (var hit in hits)
        {
            if (hit.collider == boxCollider) continue;
            if (hit.collider.isTrigger) continue;
            if (hit.collider.gameObject.scene != gameObject.scene) continue;
            
            return hit;
        }
        return default;
    }
}