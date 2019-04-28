using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using ZeroProgress.Common;

namespace ZeroProgress.Common2D
{
    public enum TilemapBoxColliderOption
    {
        FIRST,
        LAST,
        LARGEST,
        SMALLEST,
        AVERAGE
    }

    [RequireComponent(typeof(BoxCollider))]
    public class TilemapBoxCollider : MonoBehaviour
    {
        public TilemapBoxColliderOption SizingRule = TilemapBoxColliderOption.LARGEST;

        public BoxCollider Collider;

        protected virtual void Awake()
        {
            Collider = this.GetComponentIfNull(Collider);
        }

        protected virtual void OnEnable()
        {
            UpdateCollider();
        }

        public void UpdateCollider()
        {
            Bounds colliderBounds = GetTilemapSizeDetails().GetColliderBounds();

            Collider.center = colliderBounds.center;
            Collider.size = colliderBounds.size;
        }

        private TilemapSizeDetails GetTilemapSizeDetails()
        {
            Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

            TilemapSizeDetails details;
            
            switch (SizingRule)
            {
                case TilemapBoxColliderOption.FIRST:
                    details = tilemaps.First().GetSizeDetails();
                    break;
                case TilemapBoxColliderOption.LAST:
                    details = tilemaps.Last().GetSizeDetails();
                    break;
                case TilemapBoxColliderOption.LARGEST:
                    details = tilemaps.Aggregate((x, y) => 
                        x.size.sqrMagnitude > y.size.sqrMagnitude ? x : y).GetSizeDetails();
                    break;
                case TilemapBoxColliderOption.SMALLEST:
                    details = tilemaps.Aggregate((x, y) =>
                        x.size.sqrMagnitude < y.size.sqrMagnitude ? x : y).GetSizeDetails();
                    break;
                case TilemapBoxColliderOption.AVERAGE:
                    details = new TilemapSizeDetails(tilemaps.Select((x) => x.size).Average(), tilemaps.Select((x) => x.cellSize).Average());
                    break;
                default:
                    throw new System.ArgumentException("Invalid sizing rule selected");
            }

            return details;
        }
    }
}