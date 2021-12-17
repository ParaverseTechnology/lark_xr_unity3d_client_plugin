using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR
{
public class SampleRTCStatus {
    public static SampleRTCStatus emptyStatus = new SampleRTCStatus();
    private double rtt = 0.0d;
    private double bytesPerSec = 0.0d;
    private long frameDeocdedPreSec = 0;
    private double packageLostRatePerSec = 0.0d;
    public SampleRTCStatus() {}
    public SampleRTCStatus(double rtt, double bytesPerSec, long frameDeocdedPreSec, double packageLostRatePerSec)
    {
        this.rtt = rtt;
        this.bytesPerSec = bytesPerSec;
        this.frameDeocdedPreSec = frameDeocdedPreSec;
        this.packageLostRatePerSec = packageLostRatePerSec;
    }

    public double Rtt
    {
        get
        {
            return rtt;
        }
    }
    public double BytesPerSec
    {
        get
        {
            return bytesPerSec;
        }
    }
    public long FrameDeocdedPreSec
    {
        get
        {
            return frameDeocdedPreSec;
        }
    }
    public double PackageLostRatePerSec
    {
        get
        {
            return packageLostRatePerSec;
        }
    }
}
}
