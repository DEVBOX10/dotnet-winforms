﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Accessibility;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        [DllImport(Libraries.UiaCore, ExactSpelling = true)]
        public static extern HRESULT UiaRaiseAutomationPropertyChangedEvent(Interop.UiaCore.IRawElementProviderSimple pProvider, UIA_PROPERTY_ID id, object? oldValue, object? newValue);
    }
}
