// ============================================================================
// PhysicsSystem.cs - Система физики / Physics system
// Обработка коллизий и столкновений
// Collision detection and handling
// ============================================================================

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PurpleLordPlatformer.Entities;

namespace PurpleLordPlatformer.Systems.Physics
{
    public class PhysicsSystem
    {
        private List<GameObject> _objects = new List<GameObject>();
        private List<Collider> _colliders = new List<Collider>();

        public void AddObject(GameObject obj)
        {
            _objects.Add(obj);
        }

        public void RemoveObject(GameObject obj)
        {
            _objects.Remove(obj);
        }

        public void AddCollider(Collider collider)
        {
            _colliders.Add(collider);
        }

        public void RemoveCollider(Collider collider)
        {
            _colliders.Remove(collider);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var collider in _colliders)
            {
                collider.Update(gameTime);
            }
        }

        public CollisionInfo CheckCollision(GameObject obj, Vector2 newPosition)
        {
            var oldPosition = obj.Position;
            obj.Position = newPosition;

            foreach (var collider in _colliders)
            {
                if (collider.CheckCollision(obj))
                {
                    obj.Position = oldPosition;
                    return new CollisionInfo
                    {
                        HasCollision = true,
                        Collider = collider,
                        Normal = GetCollisionNormal(obj, collider)
                    };
                }
            }

            obj.Position = oldPosition;
            return new CollisionInfo { HasCollision = false };
        }

        private Vector2 GetCollisionNormal(GameObject obj, Collider collider)
        {
            Rectangle objRect = obj.Bounds;
            Rectangle colRect = collider.Bounds;

            float overlapX = Math.Min(objRect.Right - colRect.Left, colRect.Right - objRect.Left);
            float overlapY = Math.Min(objRect.Bottom - colRect.Top, colRect.Bottom - objRect.Top);

            if (overlapX < overlapY)
            {
                return objRect.Center.X < colRect.Center.X ? Vector2.UnitLeft : Vector2.UnitRight;
            }
            else
            {
                return objRect.Center.Y < colRect.Center.Y ? Vector2.UnitUp : Vector2.UnitDown;
            }
        }

        public List<GameObject> GetObjectsInArea(Rectangle area)
        {
            var result = new List<GameObject>();
            foreach (var obj in _objects)
            {
                if (area.Intersects(obj.Bounds))
                {
                    result.Add(obj);
                }
            }
            return result;
        }
    }

    public abstract class Collider
    {
        public Rectangle Bounds { get; protected set; }
        public bool IsTrigger { get; set; } = false;
        public string Tag { get; set; } = "";

        public virtual void Update(GameTime gameTime) { }
        public abstract bool CheckCollision(GameObject obj);
    }

    public class BoxCollider : Collider
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public BoxCollider(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            Bounds = new Rectangle(
                (int)(Position.X - Size.X / 2),
                (int)(Position.Y - Size.Y / 2),
                (int)Size.X,
                (int)Size.Y);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateBounds();
        }

        public override bool CheckCollision(GameObject obj)
        {
            return Bounds.Intersects(obj.Bounds);
        }
    }

    public class CollisionInfo
    {
        public bool HasCollision { get; set; }
        public Collider Collider { get; set; }
        public Vector2 Normal { get; set; }
    }
}
