////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 zSpace, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

namespace zSpace.Core.Sdk
{
    public enum ZPluginError
    {
        Unknown = -1,
        Ok = 0,
        NotImplemented = 1,
        NotInitialized = 2,
        AlreadyInitialized = 3,
        InvalidParameter = 4,
        InvalidContext = 5,
        InvalidHandle = 6,
        RuntimeIncompatible = 7,
        RuntimeNotFound = 8,
        SymbolNotFound = 9,
        DisplayNotFound = 10,
        DeviceNotFound = 11,
        TargetNotFound = 12,
        CapabilityNotFound = 13,
        BufferTooSmall = 14,
        SyncFailed = 15,
        OperationFailed = 16,
        InvalidAttribute = 17,
    }
}
