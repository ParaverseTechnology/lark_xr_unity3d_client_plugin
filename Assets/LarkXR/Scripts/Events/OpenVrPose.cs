using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenVrPose {
    /************************************    left hand to right hand  *************************/
    public static readonly Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1, 1, -1));
    /************************************    right hand to left hand  *************************/
    public static readonly Matrix4x4 rTLflipZ = Matrix4x4.Scale(new Vector3(-1, -1, 1));

    public double PoseTimeOffset = 0;
    public Quaternion WorldFromDriverRotation = new Quaternion(0, 0, 0, 1);
    public Vector3 WorldFromDriverTranslation = Vector3.zero;
    public Quaternion DriverFromHeadRotation = new Quaternion(0, 0, 0, 1);
    public Vector3 DriverFromHeadTranslation = Vector3.zero;
    public Vector3 Position = Vector3.zero;
    public Vector3 Velocity = Vector3.zero;
    public Vector3 Acceleration = Vector3.zero;
    public Quaternion Rotation = new Quaternion(0, 0, 0, 1);
    public Vector3 AngularVelocity = Vector3.zero;
    public Vector3 AngularAcceleration = Vector3.zero;

    public OpenVrPose() {}

    public OpenVrPose(Transform transform)
    {
        Matrix4x4 matrixRight = flipZ * transform.localToWorldMatrix * flipZ;
        /// 右手坐标系
        this.Position = new Vector3(transform.localPosition.x, transform.localPosition.y, -transform.localPosition.z);
        this.Rotation = matrixRight.rotation;
    }    
    public OpenVrPose(Vector3 position, Matrix4x4 localToWorldMatrix)
    {
        Matrix4x4 matrixRight = flipZ * localToWorldMatrix * flipZ;
        /// 右手坐标系
        this.Position = new Vector3(position.x, position.y, -position.z);
        this.Rotation = matrixRight.rotation;
    }
}
