#if USE_XR_TOOLKIT

using BlackTundra.World.Items;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BlackTundra.World.XR {

    /// <summary>
    /// Snap point that allows an XR player to snap items to from their hand.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
#if UNITY_EDITOR
    [AddComponentMenu("XR/Item Snap Point")]
#endif
    public sealed class XRItemSnapPoint : XRSocketInteractor {

        #region constant

        /// <summary>
        /// Max amount of time since an item was released/dropped that an <see cref="XRItemSnapPoint"/> will accept an interaction from.
        /// </summary>
        private const float MaxReleaseTimePassed = 0.1f;

        #endregion

        #region variable

        /// <summary>
        /// Item tags allowed to snap to this snap point.
        /// </summary>
        [SerializeField]
        private string[] allowedItemTags = new string[0];

        [SerializeField]
        private string[] disallowedItemTags = new string[0];

        /// <summary>
        /// Layer to set the selected <see cref="WorldItem"/> to while selected.
        /// </summary>
        /// <remarks>
        /// When set to <c>-1</c>, this feature will be disabled.
        /// </remarks>
        [SerializeField]
        private int selectedLayer = -1;

        /// <summary>
        /// When <c>true</c>, the rotational offset of the item will be ignored when snapped to the snap point.
        /// </summary>
        [SerializeField]
        private bool ignoreItemRotationalOffset = false;

        /// <summary>
        /// Local rotation of the <see cref="XRItemSnapPoint"/>.
        /// </summary>
        private Quaternion localRotation = Quaternion.identity;

        #endregion

        #region logic

        #region CanHover

        public sealed override bool CanHover(IXRHoverInteractable interactable) {
            XRBaseInteractable baseInteractable = interactable as XRBaseInteractable;
            if (interactable == null) return false;
            WorldItem worldItem = baseInteractable.GetComponent<WorldItem>();
            if (worldItem == null) return false;
            Item item = worldItem.Item;
            if (item == null) {
#if UNITY_EDITOR
                Debug.LogWarning($"No item data found for world item `{worldItem.name}`.", this);
#endif
                return false;
            }
            return item.HasAnyTag(allowedItemTags) && !item.HasAnyTag(disallowedItemTags) && base.CanHover(interactable);
        }

        #endregion

        #region CanSelect

        public sealed override bool CanSelect(IXRSelectInteractable interactable) {
            XRBaseInteractable baseInteractable = interactable as XRBaseInteractable;
            if (interactable == null) return false;
            WorldItem worldItem = baseInteractable.GetComponent<WorldItem>();
            if (worldItem == null) return false;
            Item item = worldItem.Item;
            if (item == null) {
#if UNITY_EDITOR
                Debug.LogWarning($"No item data found for world item `{worldItem.name}`.", this);
#endif
                return false;
            }
            IXRSelectInteractor interactor = interactable.GetOldestInteractorSelecting();
            XRBaseInteractor baseInteractor = interactor as XRBaseInteractor;
            return (
                (baseInteractor != null && baseInteractor == this)
                    || (Time.time - worldItem.lastReleaseTime) < MaxReleaseTimePassed && item.HasAnyTag(allowedItemTags) && !item.HasAnyTag(disallowedItemTags)
                )
                && base.CanSelect(interactable);
        }

        #endregion

        #region OnSelectEntered

        protected sealed override void OnSelectEntered(SelectEnterEventArgs args) {
            localRotation = attachTransform.localRotation;
            base.OnSelectEntered(args);
            if (args.interactableObject is Behaviour interactableBehaviour) {
                WorldItem worldItem = interactableBehaviour.GetComponent<WorldItem>();
                if (worldItem != null) {
                    if (ignoreItemRotationalOffset) {
                        attachTransform.localRotation = localRotation * Quaternion.Inverse(worldItem.transform.rotation) * worldItem.XRHoldTransform.rotation;
                    }
                    if (selectedLayer != -1) worldItem.SetLayers(selectedLayer);
                    worldItem.OnEnterSnapPoint(this, args);
                }
            }
        }

        #endregion

        #region OnSelectExited

        protected sealed override void OnSelectExited(SelectExitEventArgs args) {
            attachTransform.localRotation = localRotation;
            base.OnSelectExited(args);
            if (args.interactableObject is Behaviour interactableBehaviour) {
                WorldItem worldItem = interactableBehaviour.GetComponent<WorldItem>();
                if (worldItem != null) {
                    worldItem.EnableCollision();
                    worldItem.ResetLayers();
                    worldItem.OnExitSnapPoint(this, args);
                }
            }
        }

        #endregion

        #endregion

    }

}

#endif