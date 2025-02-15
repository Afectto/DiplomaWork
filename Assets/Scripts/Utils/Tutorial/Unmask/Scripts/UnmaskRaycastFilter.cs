using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIExtensions
{
    /// <summary>
    /// Unmask Raycast Filter.
    /// The ray passes through the unmasked rectangles.
    /// </summary>
    [AddComponentMenu("UI/Unmask/UnmaskRaycastFilter", 2)]
    public class UnmaskRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
    {
        //################################
        // Serialize Members.
        //################################
        [Tooltip("Target unmask components. The ray passes through the unmasked rectangles.")]
        [SerializeField] private List<Unmask> m_TargetUnmasks = new List<Unmask>();

        //################################
        // Public Members.
        //################################
        /// <summary>
        /// Target unmask components. Ray passes through the unmasked rectangles.
        /// </summary>
        public List<Unmask> TargetUnmasks { get { return m_TargetUnmasks; } set { m_TargetUnmasks = value; } }

        public event Action<Unmask> OnClickInTargetUnmasks;
        
        /// <summary>
        /// Given a point and a camera is the raycast valid.
        /// </summary>
        /// <returns>Valid.</returns>
        /// <param name="sp">Screen position.</param>
        /// <param name="eventCamera">Raycast camera.</param>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (!isActiveAndEnabled || m_TargetUnmasks == null || m_TargetUnmasks.Count == 0)
            {
                return true;
            }

            foreach (var unmask in m_TargetUnmasks)
            {
                if (unmask && unmask.isActiveAndEnabled)
                {
                    if (eventCamera)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint((unmask.transform as RectTransform), sp, eventCamera))
                        {
                            OnClickInTargetUnmasks?.Invoke(unmask);
                            return false;
                        }
                    }
                    else
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint((unmask.transform as RectTransform), sp))
                        {
                            OnClickInTargetUnmasks?.Invoke(unmask);
                            return false;
                        }
                    }
                }
            }
            
            return true;
        }

        //################################
        // Private Members.
        //################################

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
        }
    }
}
