﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms;

public partial class DataGridView
{
    internal class DataGridViewEditingPanelAccessibleObject : ControlAccessibleObject
    {
        private readonly WeakReference<DataGridView> _ownerDataGridView;
        private int[]? _runtimeId;

        public DataGridViewEditingPanelAccessibleObject(DataGridView dataGridView, Panel panel) : base(panel)
        {
            _ownerDataGridView = new(dataGridView);
        }

        internal override Rectangle BoundingRectangle
            => this.TryGetOwnerAs(out Panel? owner) ? owner.AccessibilityObject.Bounds : default;

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            => _ownerDataGridView.TryGetTarget(out var owner)
                ? owner.AccessibilityObject
                : UiaCore.StubFragmentRoot.Instance;

        internal override int[] RuntimeId
            => _runtimeId ??= this.TryGetOwnerAs(out Panel? owner) ? owner.AccessibilityObject.RuntimeId : base.RuntimeId;

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
        {
            if (!_ownerDataGridView.TryGetTarget(out var owner))
            {
                return null;
            }

            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                    DataGridViewCell? currentCell = owner.CurrentCell;
                    if (currentCell is not null && owner.IsCurrentCellInEditMode)
                    {
                        return owner.AccessibilityObject;
                    }

                    break;
                case NavigateDirection.NavigateDirection_FirstChild:
                case NavigateDirection.NavigateDirection_LastChild:
                    return owner.EditingControlAccessibleObject;
            }

            return base.FragmentNavigate(direction);
        }

        public override string? Name => SR.DataGridView_AccEditingPanelAccName;

        internal override void SetFocus()
        {
            if (this.IsOwnerHandleCreated(out Panel? owner) && owner.CanFocus)
            {
                owner.Focus();
            }
        }

        internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyId) =>
            propertyId switch
            {
                UIA_PROPERTY_ID.UIA_AccessKeyPropertyId => this.TryGetOwnerAs(out Panel? owner)
                    ? owner.AccessibilityObject.KeyboardShortcut
                    : null,
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    // If we don't set a default role for the accessible object it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    ? UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId
                    : base.GetPropertyValue(propertyId),
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId
                    => _ownerDataGridView.TryGetTarget(out var owner) && owner.CurrentCell is not null,
                UIA_PROPERTY_ID.UIA_IsContentElementPropertyId => true,
                UIA_PROPERTY_ID.UIA_IsControlElementPropertyId => true,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => _ownerDataGridView.TryGetTarget(out var owner) && owner.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => true,
                UIA_PROPERTY_ID.UIA_ProviderDescriptionPropertyId => SR.DataGridViewEditingPanelUiaProviderDescription,
                _ => base.GetPropertyValue(propertyId)
            };
    }
}
