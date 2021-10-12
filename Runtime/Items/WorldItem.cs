using BlackTundra.World.Interaction;

using UnityEngine;

namespace BlackTundra.World.Items {

    /// <summary>
    /// Controls and manages an instance of an <see cref="Item"/> that exists in the world.
    /// </summary>
    public sealed class WorldItem : MonoBehaviour, IInteractable {

        #region variable

        /// <summary>
        /// <see cref="Item"/> associated with the <see cref="WorldItem"/>.
        /// </summary>
        private Item item = null;

        #endregion

        #region property

        #endregion

        #region constructor

        #endregion

        #region logic

        #region InteractStart

        public bool InteractStart(in object sender, in object[] parameters) {
            throw new System.NotImplementedException();
        }

        #endregion

        #region InteractStop

        public bool InteractStop(in object sender, in object[] parameters) {
            throw new System.NotImplementedException();
        }

        #endregion

        #endregion

    }

}