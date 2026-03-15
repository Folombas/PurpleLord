// ============================================================================
// PhysicsSystem.cs - Система физики и коллизий
// Physics and collision system
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PurpleLord.Entities.Systems
{
    /// <summary>
    /// Система для обработки коллизий между объектами.
    /// System for handling collisions between objects.
    /// </summary>
    public class PhysicsSystem
    {
        private List<GameObject> _objects = new List<GameObject>();
        private List<GameObject> _colliders = new List<GameObject>();

        /// <summary>
        /// Добавить игровой объект в систему.
        /// Add game object to the system.
        /// </summary>
        public void AddObject(GameObject obj)
        {
            if (!_objects.Contains(obj))
                _objects.Add(obj);
        }

        /// <summary>
        /// Добавить коллайдер.
        /// Add collider.
        /// </summary>
        public void AddCollider(GameObject collider)
        {
            if (!_colliders.Contains(collider))
                _colliders.Add(collider);
        }

        /// <summary>
        /// Удалить объект из системы.
        /// Remove object from the system.
        /// </summary>
        public void RemoveObject(GameObject obj)
        {
            _objects.Remove(obj);
            _colliders.Remove(obj);
        }

        /// <summary>
        /// Обновить систему.
        /// Update the system.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Обновляем все объекты
            foreach (var obj in _objects)
            {
                if (obj.IsActive)
                {
                    obj.Update(gameTime);
                }
            }

            // Удаляем неактивные объекты
            _objects.RemoveAll(o => !o.IsActive);
            _colliders.RemoveAll(o => !o.IsActive);
        }

        /// <summary>
        /// Проверить столкновение объекта с коллайдерами.
        /// Check collision of object with colliders.
        /// </summary>
        public CollisionInfo CheckCollision(GameObject obj, Vector2 newPosition)
        {
            // Временно меняем позицию для проверки
            Vector2 originalPosition = obj.Position;
            obj.Position = newPosition;

            foreach (var collider in _colliders)
            {
                if (collider == obj || !collider.IsActive) continue;

                if (obj.Intersects(collider))
                {
                    obj.Position = originalPosition;
                    return new CollisionInfo
                    {
                        HasCollision = true,
                        Collider = collider,
                        Normal = GetCollisionNormal(obj, collider)
                    };
                }
            }

            obj.Position = originalPosition;
            return new CollisionInfo { HasCollision = false };
        }

        /// <summary>
        /// Получить нормаль столкновения.
        /// Get collision normal.
        /// </summary>
        private Vector2 GetCollisionNormal(GameObject a, GameObject b)
        {
            Rectangle aBounds = a.Bounds;
            Rectangle bBounds = b.Bounds;

            float overlapLeft = aBounds.Right - bBounds.Left;
            float overlapRight = bBounds.Right - aBounds.Left;
            float overlapTop = aBounds.Bottom - bBounds.Top;
            float overlapBottom = bBounds.Bottom - aBounds.Top;

            float minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight),
                                        Math.Min(overlapTop, overlapBottom));

            if (minOverlap == overlapLeft) return new Vector2(-1, 0);
            if (minOverlap == overlapRight) return new Vector2(1, 0);
            if (minOverlap == overlapTop) return new Vector2(0, -1);
            if (minOverlap == overlapBottom) return new Vector2(0, 1);

            return Vector2.Zero;
        }

        /// <summary>
        /// Проверить все столкновения между объектами.
        /// Check all collisions between objects.
        /// </summary>
        public List<CollisionPair> GetAllCollisions()
        {
            var collisions = new List<CollisionPair>();

            for (int i = 0; i < _objects.Count; i++)
            {
                for (int j = i + 1; j < _objects.Count; j++)
                {
                    var a = _objects[i];
                    var b = _objects[j];

                    if (!a.IsActive || !b.IsActive) continue;

                    if (a.Intersects(b))
                    {
                        collisions.Add(new CollisionPair { A = a, B = b });
                    }
                }
            }

            return collisions;
        }

        /// <summary>
        /// Получить все объекты в области.
        /// Get all objects in area.
        /// </summary>
        public List<GameObject> GetObjectsInArea(Rectangle area)
        {
            var result = new List<GameObject>();

            foreach (var obj in _objects)
            {
                if (obj.IsActive && area.Intersects(obj.Bounds))
                {
                    result.Add(obj);
                }
            }

            return result;
        }

        /// <summary>
        /// Информация о столкновении.
        /// Collision information.
        /// </summary>
        public class CollisionInfo
        {
            public bool HasCollision { get; set; }
            public GameObject Collider { get; set; }
            public Vector2 Normal { get; set; }
        }

        /// <summary>
        /// Пара столкнувшихся объектов.
        /// Pair of collided objects.
        /// </summary>
        public class CollisionPair
        {
            public GameObject A { get; set; }
            public GameObject B { get; set; }
        }
    }
}
