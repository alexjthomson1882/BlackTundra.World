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

#endregion

#region logic

        public sealed override bool CanHover(XRBaseInteractable interactable) {
            if (interactable == null) return false;
            WorldItem worldItem = interactable.GetComponent<WorldItem>();
            if (worldItem == null) return false;
            Item item = worldItem.Item;
            return item.HasAnyTag(allowedItemTags) && !item.HasAnyTag(disallowedItemTags) && base.CanHover(interactable);
        }

        public sealed override bool CanSelect(XRBaseInteractable interactable) {
            if (interactable == null) return false;
            WorldItem worldItem = interactable.GetComponent<WorldItem>();
            if (worldItem == null) return false;
            Item item = worldItem.Item;
            XRBaseInteractor interactor = interactable.selectingInteractor;
            return (
                (interactor != null && interactor == this)
                || (Time.time - worldItem.lastReleaseTime) < MaxReleaseTimePassed
                    && item.HasAnyTag(allowedItemTags)
                    && !item.HasAnyTag(disallowedItemTags)
            ) && base.CanSelect(interactable);
        }

        protected sealed override void OnSelectEntered(SelectEnterEventArgs args) {
            base.OnSelectEntered(args);
            XRBaseInteractable interactable = args.interactable;
            WorldItem worldItem = interactable.GetComponent<WorldItem>();
            if (worldItem != null) {
                if (selectedLayer != -1) worldItem.SetLayers(selectedLayer);
                worldItem.OnEnterSnapPoint(this, args);
            }
        }

        protected sealed override void OnSelectExited(SelectExitEventArgs args) {
            base.OnSelectExited(args);
            XRBaseInteractable interactable = args.interactable;
            WorldItem worldItem = interactable.GetComponent<WorldItem>();
            if (worldItem != null) {
                worldItem.EnableCollision();
                worldItem.ResetLayers();
                worldItem.OnExitSnapPoint(this, args);
            }
        }

#endregion

    }

}

#endif