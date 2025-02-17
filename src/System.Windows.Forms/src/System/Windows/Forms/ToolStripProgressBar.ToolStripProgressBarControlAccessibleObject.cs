﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms;

public partial class ToolStripProgressBar
{
    internal class ToolStripProgressBarControlAccessibleObject : ProgressBar.ProgressBarAccessibleObject
    {
        private readonly ToolStripProgressBarControl _ownerToolStripProgressBarControl;

        public ToolStripProgressBarControlAccessibleObject(ToolStripProgressBarControl toolStripProgressBarControl) : base(toolStripProgressBarControl)
        {
            _ownerToolStripProgressBarControl = toolStripProgressBarControl;
        }

        internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
        {
            get
            {
                return _ownerToolStripProgressBarControl.Owner?.Owner?.AccessibilityObject;
            }
        }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
        {
            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                case NavigateDirection.NavigateDirection_PreviousSibling:
                case NavigateDirection.NavigateDirection_NextSibling:
                    return _ownerToolStripProgressBarControl.Owner?.AccessibilityObject.FragmentNavigate(direction);
            }

            return base.FragmentNavigate(direction);
        }

        internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => GetIsOffscreenPropertyValue(_ownerToolStripProgressBarControl.Owner?.Placement, Bounds),
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
