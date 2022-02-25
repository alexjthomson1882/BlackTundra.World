using BlackTundra.World.Pooling;

using System;

using UnityEngine;

namespace BlackTundra.World.Ballistics {

    /// <summary>
    /// Manages a projectile instance.
    /// </summary>
#if UNITY_EDITOR
    [AddComponentMenu("Physics/Ballistics/Projectile")]
#endif
    [RequireComponent(typeof(LineRenderer))]
    [DisallowMultipleComponent]
    public sealed class ProjectileInstance : MonoBehaviour, IObjectPoolable {

        #region variable

        /// <summary>
        /// <see cref="Projectile"/> that the <see cref="ProjectileInstance"/> uses.
        /// </summary>
        [SerializeField]
#if UNITY_EDITOR
        internal
#else
        private
#endif
        Projectile projectile = null;

        [NonSerialized]
        private ObjectPool parentPool = null;

        [NonSerialized]
        private LineRenderer lineRenderer = null;

        [NonSerialized]
        private bool updateLineRenderer = false;

        #endregion

        #region property

        public Vector3 velocity {
            get => projectile._velocity;
            set => projectile._velocity = value;
        }

        #endregion

        #region logic

        #region OnEnable

        private void OnEnable() {
            if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
            projectile.SetStartParameters(transform.position, transform.forward, projectile.kineticEnergy);
            ResetLineRenderer();
        }

        #endregion

        #region Update

        private void Update() {
            if (updateLineRenderer) {
                int pointCount = lineRenderer.positionCount;
                Vector3[] points = new Vector3[pointCount];
                lineRenderer.GetPositions(points);
                int finalIndex = pointCount - 1;
                for (int i = 0; i < finalIndex;) points[i] = points[++i];
                points[finalIndex] = projectile._position;
                lineRenderer.SetPositions(points);
                updateLineRenderer = false;
            }
        }

        #endregion

        #region FixedUpdate

        private void FixedUpdate() {
            float deltaTime = Time.fixedDeltaTime;
            projectile.Simulate(deltaTime);
            UpdateTransform();
            updateLineRenderer = true;
        }

        #endregion

        #region ResetLineRenderer

        private void ResetLineRenderer() {
            int pointCount = lineRenderer.positionCount;
            Vector3[] points = new Vector3[pointCount];
            for (int i = 0; i < pointCount; i++) points[i] = projectile._position;
            lineRenderer.SetPositions(points);
            updateLineRenderer = false;
        }

        #endregion

        #region SetStartParameters

        public void SetStartParameters(in Vector3 position, in Vector3 direction, in float kineticEnergy) {
            projectile.SetStartParameters(position, direction, kineticEnergy);
            UpdateTransform();
        }

        #endregion

        #region UpdateTransform

        private void UpdateTransform() {
            transform.SetPositionAndRotation(
                projectile._position,
                Quaternion.LookRotation(
                    projectile._velocity,
                    Vector3.up
                )
            );
        }

        #endregion

        #region IsAvailable

        public bool IsAvailable(in ObjectPool objectPool) {
            if (objectPool == null) throw new ArgumentNullException(nameof(objectPool));
            return parentPool == null && projectile != null && projectile._lifetime < 0.0f;
        }

        #endregion

        #region OnPoolDispose

        public void OnPoolDispose(in ObjectPool objectPool) {
            if (objectPool == null) throw new ArgumentNullException(nameof(objectPool));
            enabled = false;
            parentPool = null;
            Destroy(gameObject);
        }

        #endregion

        #region OnPoolUse

        public void OnPoolUse(in ObjectPool objectPool) {
            if (objectPool == null) throw new ArgumentNullException(nameof(objectPool));
            if (parentPool != null && parentPool != objectPool) {
                parentPool.ReturnToPool(this);
            }
            parentPool = objectPool;
            enabled = true;
        }

        #endregion

        #region OnPoolRelease

        public void OnPoolRelease(in ObjectPool objectPool) {
            if (objectPool == null) throw new ArgumentNullException(nameof(objectPool));
            if (parentPool == objectPool) {
                parentPool = null;
                enabled = false;
            }
        }

        #endregion

        #endregion

    }

}