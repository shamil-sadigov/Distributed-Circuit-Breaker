﻿using DCB.Client.Shared;

namespace DCB.Client.CriticalLogSaver.Dto;

public class SavedLogResponse
{
    public bool Succeeded { get; set; }
    public LogStorageFailureReason? FailureReason { get; set; }

    public static SavedLogResponse Successful = new()
    {
        Succeeded = true
    };
    
    public static SavedLogResponse Failed(LogStorageFailureReason reason) => new()
    {
        Succeeded = false,
        FailureReason = reason
    };

}