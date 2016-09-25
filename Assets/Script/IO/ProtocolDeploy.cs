using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;

public class ProtocolDeploy : BaseDeploy
{
    public int id;
    public string rpc;
    public bool encrypt;
    public long testLong;
    public float testFloat;
    public byte testByte;
    public double testDouble;
    public List<int> tests;
    public List<string> tests2;
    public List<bool> tests3;
    public JsonData jsonObject;
    public TestEnum testEnum = TestEnum.A;

    public string toString()
    {
        return "ProtocolDeploy [id=" + id + ", rpc=" + rpc + ", encrypt=" + encrypt + ", testLong=" + testLong
                + ", testFloat=" + testFloat + ", testByte=" + testByte + ", testDouble=" + testDouble + ", tests="
                + tests + ", tests2=" + tests2 + ", tests3=" + tests3 + ", jsonObject=" + jsonObject + "testEnum=" + testEnum + "]";
    }

    public enum TestEnum
    {
        A, B, C, D
    }
}
